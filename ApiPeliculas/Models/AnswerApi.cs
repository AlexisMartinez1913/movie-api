﻿using System.Net;

namespace ApiPeliculas.Models

{
    public class AnswerApi
    {
        public AnswerApi()
        {
            ErrorMessages = new List<string>();
        }

        public HttpStatusCode StatusCode {get; set;}
        public bool IsSuccess { get; set;} = true;
        public List<string> ErrorMessages { get; set;}
        public object Result { get; set;}
    }
}
