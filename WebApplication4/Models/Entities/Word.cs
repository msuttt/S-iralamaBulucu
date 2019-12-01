using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication4.Models.Entities
{
    public class Word
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        //[Required(ErrorMessage ="Boş Geçilemez"),MaxLength(50)]
        public string TheWord { get; set; }
        public DateTime? AddingTime { get; set; }
        public int? GeneralSort { get; set; }
        public int? Page { get; set; }
        public int? SortOfPage { get; set; }
       
        
    }
}