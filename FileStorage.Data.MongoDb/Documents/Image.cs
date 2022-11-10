using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb.Documents
{
    public class Image
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PreviewId { get; set; } // Инфа дублируется с айтемом из Files

        public List<ImageFile> Files { get; set; }
    }
}
