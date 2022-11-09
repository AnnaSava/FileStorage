using FileStorage.Data.MongoDb.Documents;
using FileStorage.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb
{
    public class FileRepository : IFileRepository
    {
        IMongoDatabase database; // база данных

        public FileRepository(IDatabaseSettings settings)
        {
            var connection = new MongoUrlBuilder(settings.ConnectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(settings.ConnectionString);
            // получаем доступ к самой базе данных
            database = client.GetDatabase(connection.DatabaseName);
        }

        public IMongoCollection<StoredFile> StoredFiles
        {
            get { return database.GetCollection<StoredFile>("StoredFiles"); }
        }

        public async Task<string> UploadFileAsync(StoredFileModel file)
        {
            var _file = new StoredFile()
            {
                Name = file.Name,
                Content = file.Content,
                MimeType = file.MimeType,
                Md5 = file.Md5,
                Sha1 = file.Sha1,
                Ext = file.Ext,
                IsDuplicateMd5 = file.IsDuplicateMd5,
                IsDuplicateSha1 = file.IsDuplicateSha1,
                Size = file.Size
            };

            await StoredFiles.InsertOneAsync(_file);

            return _file.Id;
        }

        public async Task<StoredFileModel> DownLoadFileAsync(string id)
        {
            var found = await StoredFiles
                .FindAsync(m => m.Id == id);
            //.FindAsync(x => true);
            return found.ToList().Select(m => new StoredFileModel()
            {
                Id = m.Id,
                Content = m.Content,
                DateUploaded = m.DateUploaded,
                IsDeleted = m.IsDeleted,
                Md5 = m.Md5,
                MimeType = m.MimeType,
                Name = m.Name,
                Owner = m.Owner,
                Sha1 = m.Sha1,
                Size = m.Size
            }).FirstOrDefault();
        }

        public async Task UpdateManySha1Async(IEnumerable<StoredFileModel> files)
        {
            foreach (var file in files)
            {
                var cursor = await StoredFiles.FindAsync(m => m.Id == file.Id);
                var fileToUpdate = cursor.FirstOrDefault();
                fileToUpdate.Sha1 = file.Sha1;
                var replaceOneResult = await StoredFiles.ReplaceOneAsync(m => m.Id == file.Id, fileToUpdate);
            }
        }

        public async Task<bool> FindAnyByMd5(string md5hash)
        {
            var found = await StoredFiles.FindAsync(m => m.Md5 == md5hash);
            return found.Any();
        }

        public async Task<IEnumerable<StoredFileModel>> FindAllByMd5(string md5hash)
        {
            var found = await StoredFiles.FindAsync(m => m.Md5 == md5hash);
            return found.ToList().Select(m => new StoredFileModel()
            {
                Id = m.Id,
                Content = m.Content,
                DateUploaded = m.DateUploaded,
                IsDeleted = m.IsDeleted,
                Md5 = m.Md5,
                MimeType = m.MimeType,
                Name = m.Name,
                Owner = m.Owner,
                Sha1 = m.Sha1,
                Size = m.Size
            });
        }

        public async Task<bool> FindAnyBySha1(string sha1hash)
        {
            var found = await StoredFiles.FindAsync(m => m.Sha1 == sha1hash);
            return found.Any();
        }
    }
}
