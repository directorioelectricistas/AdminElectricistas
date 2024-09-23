using DirectorioElectricistas.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DirectorioElectricistas.Repositories
{
    public class UsersCollection : IUsersCollection
    {

        internal MongoDBRepository _repository = new MongoDBRepository();

        private IMongoCollection<Users> Collection;

        public UsersCollection() 
        {
            Collection = _repository.db.GetCollection<Users>("Users");        
        }

        public void DeleteUsers(string id)
        {
            var filter = Builders<Users>.Filter.Eq(s => s.Id, new ObjectId(id));
            Collection.DeleteOneAsync(filter);
        }

        public List<Users> GetAllUsers()
        {
            var query = Collection.
                Find(new BsonDocument()).ToListAsync();

            return query.Result;
        }

        public Users GetUserById(string id)
        {
            var user = Collection.Find(
                 new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;

            return user;
        }

        public void InsertUsers(Users user)
        {
            Collection.InsertOneAsync(user);
        }

        public void UpdateUsers(Users user)
        {
            var filter = Builders<Users>
                .Filter
                .Eq(s => s.Id, user.Id);
            Collection.ReplaceOneAsync(filter, user);
        }
    }
}
