using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YourChores.ApiModels
{
    public class ChangeNameApiModel
    {
        public class Request
        {
            [MaxLength(50)]
            [Required]
            public string FirstName { get; set; }

            [MaxLength(50)]
            [Required]
            public string LastName { get; set; }
        }
    }
}
