using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourChores.ApiModels
{
    public class ChangePasswordApiModel
    {
        public class Request
        {
            [Required]
            public string OldPassword { get; set; }

            [Required]
            public string NewPassword { get; set; }
        }
    }
}
