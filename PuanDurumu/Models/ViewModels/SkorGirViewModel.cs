using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PuanDurumu.Models.ViewModels
{
    public class SkorGirViewModel
    {
        //Bindlenecekler
        [Required]
        public int EvSahibi_Id { get; set; }

        [Required]
        public int Deplasman_Id { get; set; }

        public Maclar Maclar { get; set; }

        public Takimlar Takim { get; set; }

        //Görüntülenecekler

        public SelectList TakimlarData { get; set; }

    }
}