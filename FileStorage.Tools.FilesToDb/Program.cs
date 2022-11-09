using FileStorage;
using FileStorage.Models;
using FileStorage.Data.MongoDb;
using FileStorage.Data.MongoDb.Documents;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Athn.Tools.FilesToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            string con = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            FileRepository repository = new FileRepository(new DatabaseSettings() { ConnectionString = con });
            CreateIndexes(repository).GetAwaiter();
            //var path = @"d:\FileStorage\";
            var path = @"d:\Dollvilla\Files";
            new FileUploader(path, repository, new ConsoleLogger()).Upload().GetAwaiter();

            Console.ReadLine();
        }

        private static async Task CreateIndexes(FileRepository repository)
        {
            var storedFilesBuilder = Builders<StoredFile>.IndexKeys;
            var indexMd5Model = new CreateIndexModel<StoredFile>(storedFilesBuilder.Ascending(x => x.Md5));
            await repository.StoredFiles.Indexes.CreateOneAsync(indexMd5Model);
            //var indexSha1Model = new CreateIndexModel<StoredFile>(storedFilesBuilder.Ascending(x => x.Sha1));
            //await repository.StoredFiles.Indexes.CreateOneAsync(indexSha1Model);
        }

        public class DatabaseSettings : IDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }
    }
}
