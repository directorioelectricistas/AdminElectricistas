using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DirectorioElectricistas.Models
{
    public class Users
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }

}
