using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb.Documents
{
    public class ImageFile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId { get; set; }

        public string Type { get; set; } // Web, FullSize, Thumb etc.

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
