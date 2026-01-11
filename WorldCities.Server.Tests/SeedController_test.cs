using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Moq;
using WorldCities.Server.Data;
using WorldCities.Server.Data.Models;
using WorldCities.Server.Controllers;



namespace WorldCities.Server.Tests
{
    public class SeedController_test
    {
        ///<Summary>
        /// Test the CreateDefaultUser() method
        /// </Summary>

        [Fact]
        public async Task CreateDefaultUser()
        {
            // Arrange
            // create the option instances required by the ApplicationDbContext

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "WorldCities").Options;


            // Create a IWebHost environment mock instance
            var mockEnv = Mock.Of<IWebHostEnvironment>();

            // Create a IConfiguration mock instance
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.SetupGet(x => x[It.Is<string>(s=> s == "DefaultPassword:RegisteredUser")]).Returns("MockP$$word");
            mockConfiguration.SetupGet(x => x[It.Is<string>(s=> s == "DefaultPassword:Administrator")]).Returns("MockP$$word");

            // Create a ApplicationDbContext instance using the in-memory Db
            using var context = new ApplicationDbContext(options);

            // create a rolemanager instance
            var roleManager = IdentityHelper.GetRoleManger(new RoleStore<IdentityRole>(context));


            // create a rolemanager instance
            var userManager = IdentityHelper.GetUserManger(new UserStore<ApplicationUser>(context));

            // create a SeedController instance
            var controller = new SeedController(context, roleManager, userManager, mockEnv, mockConfiguration.Object);

            // define the variables for the user we want to test
            ApplicationUser? user_Admin = null;
            ApplicationUser? user_User = null;
            ApplicationUser? user_NotExisting = null;

            //Act
            // execute the SeddController's CreateDefaultUser
            // method to create the default users (and roles)
            await controller.CreateDefaultUser();

            // retrive the users
            user_Admin = await userManager.FindByEmailAsync("admin@email.com");
            user_User = await userManager.FindByEmailAsync("user@email.com");
            user_NotExisting = await userManager.FindByEmailAsync("notExisting@email.com");

            // Assert
            Assert.NotNull(user_Admin);
            Assert.NotNull(user_User);
            Assert.Null(user_NotExisting);




        }



    }
}
