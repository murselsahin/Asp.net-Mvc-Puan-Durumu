using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PuanDurumu.Models
{
    [Table("Takimlar")]
    public class Takimlar
    {
        [Key , DatabaseGenerated(DatabaseGeneratedOption.Identity) ]
        public int Id { get; set; }

        [Required , StringLength(100)]
        public string Adi { get; set; }

        [InverseProperty("EvSahibiTakimi")]
        public virtual IEnumerable<Maclar> EvSahibi { get; set; }

        [InverseProperty("DeplasmanTakimi")]
        public virtual IEnumerable<Maclar> Deplasman { get; set; }

    }
}