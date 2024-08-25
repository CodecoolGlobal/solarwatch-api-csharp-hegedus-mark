using Microsoft.Extensions.Logging;
using SolarWatch.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace SolarWatch.Data.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly SolarWatchDbContext _dbContext;
        private readonly ILogger<CityRepository> _logger;

        public CityRepository(SolarWatchDbContext dbContext, ILogger<CityRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        
        public IEnumerable<City> GetAll()
        {
            _logger.LogInformation("Getting all cities from the database.");
            var cities = _dbContext.Cities.ToList();
            _logger.LogInformation("Retrieved {CityCount} cities from the database.", cities.Count);
            return cities;
        }

        public IEnumerable<City> GetByName(string name)
        {
            _logger.LogInformation("Getting cities with name containing: {CityName}", name);
            var cities = _dbContext.Cities
                .Where(c => c.Name.ToLower().Contains(name.ToLower()))
                .ToList();
            _logger.LogInformation("Retrieved {CityCount} cities with name containing: {CityName}", cities.Count, name);
            return cities;
        }

        public City? GetById(int id)
        {
            _logger.LogInformation("Getting cities with id: {CityId}", id);
            var city = _dbContext.Cities.FirstOrDefault(c => c.CityId == id);
            _logger.LogInformation("Retrieved city with id: {CityId}", city.CityId);
            return city;
        }

        public void Add(City city)
        {
            _logger.LogInformation("Adding city: {CityName}", city.Name);
            _dbContext.Cities.Add(city);
            _dbContext.SaveChanges();
            _logger.LogInformation("City added: {CityName}", city.Name);
        }

        public void Delete(City city)
        {
            _logger.LogInformation("Deleting city: {CityName}", city.Name);
            _dbContext.Cities.Remove(city);
            _dbContext.SaveChanges();
            _logger.LogInformation("City deleted: {CityName}", city.Name);
        }

        public void Update(City city)
        {
            _logger.LogInformation("Updating city: {CityName}", city.Name);
            _dbContext.Cities.Update(city);
            _dbContext.SaveChanges();
            _logger.LogInformation("City updated: {CityName}", city.Name);
        }
    }
}
