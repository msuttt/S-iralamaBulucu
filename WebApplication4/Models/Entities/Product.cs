using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication4.Models.Entities
{
    public class Product
    {
        public int Id { get; set; }
       
        public string Url { get; set; }
      
        public string ProductName { get; set; }
        //[Required(ErrorMessage = "Boş geçemezsiniz"),MaxLength(9),MinLength(9)]
        public string Code { get; set; }

    }
}