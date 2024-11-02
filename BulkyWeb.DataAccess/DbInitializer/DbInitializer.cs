using Azure.Identity;
using Bulky.Models;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitiliazer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityUser> roleManager,
            ApplcationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public void Initialize()
        {

            // create migrations if not created
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)){
                    _db.Database.Migrate();
                }
            }
            catch (Exception exception)
            {
            }
            // create roles if there ARE not ccreated
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                // if users are not create we will need to create new.
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@petermc.com",
                    Email = "admin@petermc.com",
                    Name = "Peter Mcgibney",
                    PhoneNumber = "1234567",
                    StreetAddress = "123 street",
                    State = "New York",
                    PostalCode = "11113",
                    City = "Astoria",
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@petermc.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }


            return;
        }
    }
}