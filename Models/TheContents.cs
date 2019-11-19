using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Demo.Models
{
    public class TheContents
    {
        public string translated{get;set;}
        public string text{get;set;}
        public string translation{get;set;}
    }
}