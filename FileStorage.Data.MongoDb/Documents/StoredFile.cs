using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Data.MongoDb.Documents
{
    public class StoredFile
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Ext { get; set; }

        public string MimeType { get; set; }

        public byte[] Content { get; set; }

        public string Md5 { get; set; }

        public string Sha1 { get; set; }

        public string Owner { get; set; }

        public long Size { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateUploaded { get; set; } = DateTime.Now;

        public bool IsDuplicateMd5 { get; set; }

        public bool IsDuplicateSha1 { get; set; }

        public bool IsDeleted { get; set; }
    }
}
