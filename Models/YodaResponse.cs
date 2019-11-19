using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Demo.Models
{
        public class YodaResponse
        {
            public ResponseSuccess success{get;set;}
            public TheContents contents{get;set;}


            
        }
}