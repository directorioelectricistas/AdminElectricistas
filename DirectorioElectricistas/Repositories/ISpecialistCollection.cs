using DirectorioElectricistas.Models;

namespace DirectorioElectricistas.Repositories
{
    public interface ISpecialistCollection
    {
        void InsertSpecialist(Specialist specialist);
        void UpdateSpecialist(Specialist specialist);
        void DeleteSpecialist(string specialist);

        List<Specialist> GetAllSpecialists();

        List<Specialist> GetAllSpecialistsStartingWith(string name);
        Specialist GetSpecialistById(string id);
        Specialist GetSpecialistByCardId(string id);

        Specialist GetSpecialistByCardIdAndState(string cardId, string state);
        
    }
}
