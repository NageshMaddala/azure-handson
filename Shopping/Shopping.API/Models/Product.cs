using MongoDB.Bson.Serialization.Attributes;

namespace Shopping.API.Models
{
    public class Product
    {
        /// <summary>
        /// This represents that it as the unique id for the json object
        /// </summary>
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageFile { get; set; }
        public decimal Price { get; set; }
    }
}
