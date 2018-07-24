using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuanDurumu.Models
{
    [Table("Maclar")]
    public class Maclar
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EvSkor { get; set; }

        public int DeplasmanSkor { get; set; }

        
        public DateTime Tarih { get; set; }

        public virtual Takimlar EvSahibiTakimi { get; set; }

        public virtual Takimlar DeplasmanTakimi { get; set; }

        public Maclar()
        {
            Tarih = DateTime.Now;
        }
        
        

    }
}