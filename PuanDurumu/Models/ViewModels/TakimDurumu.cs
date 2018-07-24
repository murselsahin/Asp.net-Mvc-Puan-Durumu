using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PuanDurumu.Models.ViewModels
{
    public class TakimDurumu
    {
        public int Takim_Id { get; set; }
        public string TakimAdi { get; set; }
        public int OynananMac { get; set; }
        public int Galibiyet { get; set; }
        public int Beraberlik { get; set; }
        public int Maglubiyet { get; set; }
        public int AtilanGol { get; set; }
        public int YenilenGol { get; set; }
        public int Averaj { get; set; }
        public int Puan { get; set; }
    }
}