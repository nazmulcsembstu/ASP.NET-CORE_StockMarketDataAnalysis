using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWeb.Models
{
    public class StockMarketClass
    {
        [Key]
        public int id { get; set; }
        public string date { get; set; }
        public string trade_code { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string open { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
    }
}
