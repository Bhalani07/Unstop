﻿using System.Net;

namespace Unstop.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>(); 
        }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages { get; set; }

        public object Result { get; set; }

        public Pagination PaginationData { get; set; }
    }
}
