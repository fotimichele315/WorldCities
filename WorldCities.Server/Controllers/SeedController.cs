using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime;
using System.Security;
using System.Security.Principal;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;
using Microsoft.AspNetCore.Authorization;


namespace WorldCities.Server.Controllers
{
    [Authorize(Roles = "Admimistrator")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // prevents non development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not Allowed");

            var path = System.IO.Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx");

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

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUser()
        {
            // Setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they don't exist yet)
            if(await _roleManager.FindByNameAsync(role_RegisteredUser) == null) 
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

            if(await _roleManager.FindByNameAsync(role_Administrator) == null) 
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

            // create a list to track  the newly added users
            var addedUserList = new List<ApplicationUser>();

            // check if the admin user already exists
            var email_Admin = "admin@email.com";

            if(await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser Account
                var user_admin = new ApplicationUser()
                {
                    Email = email_Admin,
                    UserName = email_Admin
                };


             
var config = _configuration["DefaultPassword:Administrator"]!;
                // insert the admin user into the db
                await _userManager.CreateAsync(user_admin, config);

                //assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_admin, role_RegisteredUser); 
                await _userManager.AddToRoleAsync(user_admin, role_Administrator);

                //confirm the the email and remove Lockout
                user_admin.EmailConfirmed = true;
                user_admin.LockoutEnabled = false;

                //add the user the added user list
                addedUserList.Add(user_admin);
            }

            //check if the standard user already exists
            // check if the admin user already exists
            var email_User = "user@email.com";

            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // create a new standard ApplicationUser Account
                var user_User = new ApplicationUser()
                {
                    Email = email_User,
                    UserName = email_User
                };
var config = _configuration["DefaultPassword:RegisteredUser"]!;
                // insert the admin user into the db
                await _userManager.CreateAsync(user_User, config);

                //assign the "RegisteredUser" roles
                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                //confirm the the email and remove Lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                //add the user the added user list
                addedUserList.Add(user_User);
            }


            // if we added at least one user, persist the changes into the db
            if(addedUserList.Count>0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
                {
                    Count= addedUserList.Count,
                    Users= addedUserList
                });
        }
        }
}
