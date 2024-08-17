using SolarWatch.Data.Models;

namespace SolarWatch.Data.Repositories;

public interface ICityRepository
{
    IEnumerable<City> GetAll();
    IEnumerable<City> GetByName(string name);

    void Add(City city);
    void Delete(City city);
    void Update(City city);
}