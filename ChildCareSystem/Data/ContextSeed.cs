using ChildCareSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ChildCareSystemUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Role.Customer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Role.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Role.Staff.ToString()));
        }
    }
}
