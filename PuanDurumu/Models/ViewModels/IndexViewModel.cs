using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuanDurumu.Models.ViewModels
{
    public class IndexViewModel
    {
        public List<TakimDurumu> ListTakimDurumu { get; set; }
        
    }

    public class TakimDetay
    {
        public string Tarih { get; set; }

        public string Sonuc { get; set; }
    }
}