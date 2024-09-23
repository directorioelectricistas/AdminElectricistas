using DirectorioElectricistas.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;

namespace DirectorioElectricistas.Repositories
{
    public class SpecialistCollection : ISpecialistCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();
        private IMongoCollection<Specialist> Collection;

        public SpecialistCollection() 
        {
            Collection = _repository.db.GetCollection<Specialist>("Specialists");    
        }

        public void DeleteSpecialist(string id)
        {
            var filter = Builders<Specialist>.Filter.Eq(s => s.Id, new ObjectId(id));
            Collection.DeleteOneAsync(filter);
        }

        public List<Specialist> GetAllSpecialists()
        {
            
            var query = Collection.
                Find(new BsonDocument()).ToListAsync();



            return query.Result;

            
        }

        public List<Specialist> GetAllSpecialistsStartingWith(string name)
        {

            var filter = Builders<Specialist>.Filter.Regex("Place", new BsonRegularExpression($"^{name}", "i"));
            var query = Collection.Find(filter).ToListAsync();

            return query.Result;
        }

        public Specialist GetSpecialistById(string id)
        {
            var specialist = Collection.Find(
                new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;

            return specialist;
        }

        public void InsertSpecialist(Specialist specialist)
        {
            Collection.InsertOneAsync(specialist);
        }

        public void UpdateSpecialist(Specialist specialist)
        {
            var filter = Builders<Specialist>
                .Filter
                .Eq(s => s.Id, specialist.Id);
            Collection.ReplaceOneAsync(filter, specialist);
        }

        public Specialist GetSpecialistByCardId(string cardId)
        {
            var filter = Builders<Specialist>.Filter.Eq(s => s.CardId, cardId);
            return Collection.Find(filter).FirstOrDefault();
        }

        public Specialist GetSpecialistByCardIdAndState(string cardId, string state)
        {
            var filter = Builders<Specialist>.Filter.And(
                Builders<Specialist>.Filter.Eq(s => s.CardId, cardId),
                Builders<Specialist>.Filter.Eq(s => s.State, state)
            );
            return Collection.Find(filter).FirstOrDefault();
        }


    }
}
