using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourChores.ApiModels
{
    public class LoginApiModel
    {
        public class Request
        {
            public string UserNameOrEmail { get; set; }
            public string Password { get; set; }
        }

        public class Response
        {
            public string Token { get; set; }
        }
    }
}
