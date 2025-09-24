using System;
using ElectronicsStoreMVC.Models;
using Microsoft.AspNetCore.Identity;


namespace ElectronicsStoreMVC.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<ApplicationUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("User Manager or Role Manager is null => exit");
                return;
            }

            //Check if admin role exists
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role doesnt exist and hence will be created");
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            //Check if seller role exists
            exists = await roleManager.RoleExistsAsync("seller");
            if (!exists)
            {
                Console.WriteLine("Seller role doesnt exist and hence will be created");
                await roleManager.CreateAsync(new IdentityRole("seller"));
            }

            //Check if client role exists
            exists = await roleManager.RoleExistsAsync("client");
            if (!exists)
            {
                Console.WriteLine("Client role doesnt exist and hence will be created");
                await roleManager.CreateAsync(new IdentityRole("client"));
            }

            //Check if atleast one admin user exists
            var adminUsers = await userManager.GetUsersInRoleAsync("admin");
            if (adminUsers.Any())
            {
                Console.WriteLine("Admin user already exists=> exit");
                return;
            }

            //If no admins , Create an admin
            var user = new ApplicationUser()
            {
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "Admin@Admin.com",
                Email = "Admin@Admin.com",
                CreatedAt = DateTime.Now,
            };
            string initialPassword = "admin123";

            var result = await userManager.CreateAsync(user, initialPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "admin");
                Console.WriteLine("Admin created succesfully..Please update the intial password!");
                Console.WriteLine("Email:" + user.Email);
                Console.WriteLine("Password:" + initialPassword);
            }

        }
    }
}
