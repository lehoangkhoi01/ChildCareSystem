using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChildCareSystem.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ChildCareSystemUser class
    public class ChildCareSystemUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }

        
    }
}
