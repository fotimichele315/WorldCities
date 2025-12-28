using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;
using System.Security;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime;


namespace WorldCities.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // prevents non development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not Allowed");

            var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");

            using var stream = System.IO.File.OpenRead(path);
            using var excelpackage = new ExcelPackage(stream);

            // get the first worksheet
            var worksheet = excelpackage.Workbook.Worksheets[0];

            // Define how many rows we want to process
            var nEndRow = worksheet.Dimension.End.Row;

            // initialize the record counter
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            // create a lookup dictionary
            // containing all the countries alredy existing
            // into the database (it will be empty on first run)
            var countriesByName =  new Dictionary<string, Country>();

            try
            {
                  countriesByName = _context.Countries.
                       AsNoTracking()
                       .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            } catch (Exception ex)
            {

var d = "";          
            }

            // iterate throgh all rows, skipping the first one
            for (int nrow = 2; nrow < nEndRow; nrow++)
            {
                var row = worksheet.Cells[
                    nrow, 1, nrow, worksheet.Dimension.End.Column];
                var countryName = row[nrow, 5].GetValue<string>();
                var iso2 = row[nrow, 6].GetValue<string>();
                var iso3 = row[nrow, 7].GetValue<string>();

                //skip the country if it alredy exists in the database
                if (countriesByName.ContainsKey(countryName))
                    continue;

                // create the Country enity and fill it with xlsx data
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3
                };

                // add the new country  to the db context
                await _context.Countries.AddAsync(country);

                // store in our lookup to retrive its Id later on
                countriesByName.Add(countryName, country);

                //increment the counter 
                numberOfCountriesAdded++;
            }
            // save all the countries into the database
            if (numberOfCountriesAdded > 0)
                await _context.SaveChangesAsync();


            // create a lookup dictionary
            // containing all the cities alredy existing
            // into the database (it will be empty on first run)
            var cities = _context.Cities.
                AsNoTracking()
                .ToDictionary(x => (
                Name: x.Name,
                Lat: x.Lat,
                Lon: x.Lon,
                CountryId: x.CountryId,
                Population: x.Population

                ));

            // iterate throgh all rows, skipping the first one
            for (int nrow = 2; nrow < nEndRow; nrow++)
            {
                var row = worksheet.Cells[
                    nrow, 1, nrow, worksheet.Dimension.End.Column];
                var name = row[nrow, 1].GetValue<string>();
                var lat = row[nrow, 3].GetValue<decimal>();
                var lon = row[nrow, 4].GetValue<decimal>();
                var countryName = row[nrow, 5].GetValue<string>();
                var population = row[nrow, 10].GetValue<int>();

                // retrive country id by countryname
                var countryId = countriesByName[countryName].Id;

                //skip the city if it alredy exists in the database
                if (cities.ContainsKey((
                    Name: name,
                    Lat: lat,
                    Lon: lon,
                    CountryId: countryId,
                    Population: population
                    )))
                    continue;

                // create the City enity and fill it with xlsx data
                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId,
                    Population = population
                };

                // add the new city  to the db context
                await _context.Cities.AddAsync(city);


                //increment the counter 
                numberOfCitiesAdded++;
            }
            // save all the cities into the database
            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }

    }
}
