using FileStorage.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb
{
    public class GridFsRepository
    {
        IMongoDatabase database; // база данных
        public IGridFSBucket gridFS { get; set; }   // файловое хранилище

        public GridFsRepository(string connectionString)
        {
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            database = client.GetDatabase(connection.DatabaseName);
            // получаем доступ к файловому хранилищу
            gridFS = new GridFSBucket(database);
        }

        public async void UploadFileAsync(StoredFileModel file)
        {
            await gridFS.UploadFromBytesAsync(file.Name, file.Content);
        }

        public void DownLoadFileAsync()
        {

        }
    }
}
