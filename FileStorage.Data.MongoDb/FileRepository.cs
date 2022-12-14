using FileStorage.Data.MongoDb.Documents;
using FileStorage.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

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

        public async Task<IEnumerable<string>> GetStoredFileIds(int page, int count = 20)
        {
            var result = await GetStoredFileIds0(page, count);
            return result;
        }

        private async Task<IEnumerable<string>> GetStoredFileIds1(int page, int count = 20)
        {
            var countFacet = AggregateFacet.Create("count",
               PipelineDefinition<StoredFile, AggregateCountResult>.Create(new[]
               {
                    PipelineStageDefinitionBuilder.Count<StoredFile>()
               }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<StoredFile, StoredFile>.Create(new[]
                {
                PipelineStageDefinitionBuilder.Sort(Builders<StoredFile>.Sort.Ascending(x => x.Id)),
                PipelineStageDefinitionBuilder.Skip<StoredFile>((page - 1) * count),
                PipelineStageDefinitionBuilder.Limit<StoredFile>(count),
                }));

            var filter = Builders<StoredFile>.Filter.Empty;

            var t = StoredFiles.Indexes.List().ToList();

            foreach (var t0 in t)
            {

            }

            var aggregation = await StoredFiles.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var _count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count ?? 0;

            var totalPages = (int)_count / count;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<StoredFile>();

            return data.Select(m => m.Id.ToString());
        }

        private async Task<IEnumerable<string>> GetStoredFileIds0(int page, int count = 20)
        {
            var topLevelProjection = Builders<StoredFile>.Projection
                //.Include(u => u.Id)
                .Exclude(m => m.Content)
                .Exclude(m => m.Md5)
                .Exclude(m => m.Sha1);

            var filter = Builders<StoredFile>.Filter.Eq(x => x.IsDeleted, false);

            try
            {
                //var t = StoredFiles.CountDocuments(filter);

                //var aggregateFluent = await StoredFiles.Aggregate()
                //    .Match(filter)
                //    .Project(m => new StoredFile { Id = m.Id })
                //    .SortBy(m => m.Id)
                //    .Skip((page - 1) * count)
                //    .Limit(count)
                //    .ToListAsync();

                var query = StoredFiles
                    .AsQueryable()                    
                    .OrderByDescending(u => u.Id)
                    .Skip((page - 1) * count)
                    .Take(count)
                    .Select(u => new { u.Id });

                var q = query.GetExecutionModel();

                var linqTopLevelResults = await query.ToListAsync();

                // https://stackoverflow.com/questions/29682371/how-is-an-iasynccursor-used-for-iteration-with-the-mongodb-c-sharp-driver
                //var found = await StoredFiles.FindAsync(filter);
                //found.ForEachAsync

                //var found = await StoredFiles.Find(filter)
                //    .Project(topLevelProjection)
                //    .SortBy(m => m.Id)
                //    .Skip((page - 1) * count)
                //    .Limit(count)
                //    .ToListAsync();

                return linqTopLevelResults.Select(m => m.Id.ToString());
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
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
