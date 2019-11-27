using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPP_LIULI.Models
{
    public class TodayProductRate
    {
        public string ProductName { get; set; }
        public double ProductCount { get; set; }
        public double ProductRate { get; set; }
    }

    public class EveryDayProducttion
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class TeamProduction
    {
        public string Team { get; set; }
        public double ProductRate { get; set; }
        public double ScrapRate { get; set; }
    }

    public class CustomerSpread
    {
        public string CustomerName { get; set; }
        public double Rate { get; set; }
        public double ScrapRate { get; set; }
    }

    public class FurtureXisMonth
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class CustomerPriceRate
    {
        public string CustomerName { get; set; }
        public double CustomerPriceCount { get; set; }
        public double Rate { get; set; }
    }

    public class MonthProduction
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class MonthSand
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }
}