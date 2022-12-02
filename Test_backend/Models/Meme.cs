using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Test_backend.Models
{
    public class Meme
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Original { get; set; }

        public string Thumbnail { get; set; }

    }
}