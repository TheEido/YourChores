using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourChores.ApiModels
{
    //In case that we don't have a response
    public class ApiResponse
    {
        public bool Success => Errors == null || Errors.Count == 0;
        public List<string> Errors { get; set; }

        public void AddError(string error)
        {
            if (Errors == null)
            {
                Errors = new List<string>();
            }
            Errors.Add(error);
        }
    }


    //In case that we have a response
    public class ApiResponse<T> : ApiResponse
        where T: new()
    {
        
        public T Response { get; set; }

        
    }
}
