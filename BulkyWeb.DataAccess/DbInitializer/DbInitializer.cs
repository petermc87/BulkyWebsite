using Azure.Identity;
using Bulky.Models;
using Bulky.Utility;
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

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }
        public void Initialize()

        {
            RemoveSeedData();
            SeedData();
        }
        public void RemoveSeedData()
        {
            foreach(var role in _db.Roles.ToList())
            {
                _roleManager.DeleteAsync(role).GetAwaiter().GetResult();
            }
            foreach(var user in _db.Users.ToList())
            {
                _userManager.DeleteAsync(user).GetAwaiter().GetResult();
            }
            _db.SaveChanges();
        }
        public void SeedData()
        {
            // create roles if there ARE not ccreated
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                var adminUser = new ApplicationUser
                {
                    UserName = "admin@petermc.com",
                    Email = "admin@petermc.com",
                    Name = "Peter Mcgibney",
                    PhoneNumber = "1234567",
                    StreetAddress = "123 street",
                    State = "New York",
                    PostalCode = "11113",
                    City = "Astoria",
                    EmailConfirmed = true,
                };

                var createResult = _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();

                if (createResult.Succeeded)
                {
                    adminUser.EmailConfirmed = true;
                    _db.SaveChanges();
                    _userManager.AddToRoleAsync(adminUser, SD.Role_Admin).GetAwaiter().GetResult();
                }
            }

        }
    }
}