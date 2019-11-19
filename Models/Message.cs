using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Demo.Models
{
    public class Message
    {
        [Key]
        public int MessageId {get;set;}
        public string Content {get;set;}
        public int UserId{get;set;}
        public User Creator{get;set;}
    }
}