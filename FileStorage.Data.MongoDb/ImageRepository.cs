using FileStorage.Data.MongoDb.Documents;
using FileStorage.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb
{
    public class ImageRepository : IImageRepository
    {
        IMongoDatabase database; // база данных

        public ImageRepository(IDatabaseSettings settings)
        {
            var connection = new MongoUrlBuilder(settings.ConnectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(settings.ConnectionString);
            // получаем доступ к самой базе данных
            database = client.GetDatabase(connection.DatabaseName);
        }

        public IMongoCollection<Image> Images
        {
            get { return database.GetCollection<Image>("Images"); }
        }

        public async Task<string> Create(ImageModel model)
        {
            // TODO use AutoMapper
            var image = new Image
            {
                Id = ObjectId.GenerateNewId().ToString(),
                PreviewId = model.PreviewId,
                Files = model.Files.Select(m => new ImageFile
                {
                    FileId = m.FileId,
                    Height = m.Height,
                    Type = m.Type,
                    Width = m.Width
                })
                .ToList()
            };

            await Images.InsertOneAsync(image);

            return image.Id;
        }

        // TODO async???
        public IEnumerable<ImageModel> GetImages(int page, int count)
        {
            var images = Images.AsQueryable<Image>()
                .OrderBy(x => x.Id)
                .Skip((page - 1) * count)
                .Take(count).ToList();

            return images.Select(m => new ImageModel
            {
                Id = m.Id,
                PreviewId = m.PreviewId,
                Files = m.Files.Select(f => new ImageFileModel
                {
                    FileId = f.FileId,
                    Type = f.Type,
                    Height = f.Height,
                    Width = f.Width
                }).ToList()
            });
        }
    }
}
