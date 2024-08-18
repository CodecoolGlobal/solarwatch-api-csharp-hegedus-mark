using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarWatch.Data.Models;
using SolarWatch.Data.Repositories;
using SolarWatch.Data;

namespace SolarWatch.Test
{
    public class CityRepositoryTests
    {
        private SolarWatchDbContext _dbContext;
        private CityRepository _cityRepository;
        private ILogger<CityRepository> _logger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<SolarWatchDbContext>()
                .UseInMemoryDatabase(databaseName: "SolarWatchTestDb")
                .Options;
            
            _dbContext = new SolarWatchDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            _logger = Substitute.For<ILogger<CityRepository>>();
            _cityRepository = new CityRepository(_dbContext, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void Add_City_ShouldAddCityToDatabase()
        {
            // Arrange
            var city = new City { Name = "TestCity", Country = "TestCountry", Latitude = 10, Longitude = 20 };

            // Act
            _cityRepository.Add(city);

            // Assert
            var cities = _cityRepository.GetAll().ToList();
            Assert.That(cities.Count(), Is.EqualTo(1));
            Assert.That(cities.First().Name, Is.EqualTo("TestCity"));
        }

        [Test]
        public void GetByName_CityExists_ShouldReturnCity()
        {
            // Arrange
            var city = new City { Name = "TestCity", Country = "TestCountry", Latitude = 10, Longitude = 20 };
            _cityRepository.Add(city);

            // Act
            var result = _cityRepository.GetByName("TestCity").ToList();

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("TestCity"));
        }

        [Test]
        public void Delete_City_ShouldRemoveCityFromDatabase()
        {
            // Arrange
            var city = new City { Name = "TestCity", Country = "TestCountry", Latitude = 10, Longitude = 20 };
            _cityRepository.Add(city);

            // Act
            _cityRepository.Delete(city);

            // Assert
            var cities = _cityRepository.GetAll();
            Assert.That(cities.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Update_City_ShouldUpdateCityInDatabase()
        {
            // Arrange
            var city = new City { Name = "TestCity", Country = "TestCountry", Latitude = 10, Longitude = 20 };
            _cityRepository.Add(city);
            
            city.Name = "UpdatedCity";

            // Act
            _cityRepository.Update(city);

            // Assert
            var updatedCity = _cityRepository.GetByName("UpdatedCity").FirstOrDefault();
            Assert.That(updatedCity, Is.Not.Null);
            Assert.That(updatedCity.Name, Is.EqualTo("UpdatedCity"));
        }
    }
}