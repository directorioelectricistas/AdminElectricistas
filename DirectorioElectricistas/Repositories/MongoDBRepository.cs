using MongoDB.Driver;

namespace DirectorioElectricistas.Repositories
{
    public class MongoDBRepository
    {
        public MongoClient client;

        public IMongoDatabase db;

        public MongoDBRepository()
        {
            client = new MongoClient("mongodb+srv://directorioelectricistas:ugXgrCQgxBFXJU17@electricistas.yagbp.mongodb.net/?retryWrites=true&w=majority&appName=Electricistas");

            db = client.GetDatabase("Directory");
            
        }

    }
}
