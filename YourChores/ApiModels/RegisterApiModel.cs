using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourChores.ApiModels
{
    public class RegisterApiModel
    {
        public class Request
        {
            [Required]
            public string UserName { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public class Response
        {
            public string UserName { get; set; }

            public string Email { get; set; }

            public IEnumerable<IdentityError> Errors { get; internal set; }
        }
    }
}
