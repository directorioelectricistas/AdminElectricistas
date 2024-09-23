using DirectorioElectricistas.Models;

namespace DirectorioElectricistas.Repositories
{
    public interface IUsersCollection
    {
        void InsertUsers(Users user);
        void UpdateUsers(Users user);
        void DeleteUsers(string user);

        List<Users> GetAllUsers();


        Users GetUserById(string id);
       

    }
}
