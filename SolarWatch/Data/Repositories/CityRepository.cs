using SolarWatch.Data.Models;

namespace SolarWatch.Data.Repositories;

public class CityRepository : ICityRepository
{
    private readonly SolarWatchDbContext _dbContext;

    public CityRepository(SolarWatchDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IEnumerable<City> GetAll()
    {
        return _dbContext.Cities.ToList();
    }

    public IEnumerable<City> GetByName(string name)
    {
        return _dbContext.Cities.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToList();
    }

    public void Add(City city)
    {
        _dbContext.Cities.Add(city);
        _dbContext.SaveChanges();
    }

    public void Delete(City city)
    {
        _dbContext.Cities.Remove(city);
        _dbContext.SaveChanges();
    }

    public void Update(City city)
    {
        _dbContext.Cities.Update(city);
        _dbContext.SaveChanges();
    }
}