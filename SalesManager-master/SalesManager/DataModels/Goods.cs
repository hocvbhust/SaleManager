using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesManager.DataModels
{
    class Goods
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public Int32 Price { get; set; }
        public string Description { get; set; }
        public Int32 TotalPrice { get; set; }
        public string PriceText { get; set; }
        public string TotalPriceText { get; set; }
        public string GoodsType { get; set; }
        public DateTime InputDate { get; set; }
        public string DVT { get; set; }
        public void setTotalPrice()
        {
            TotalPrice = Count * Price;
            TotalPriceText = String.Format("{0:0,0}", TotalPrice);
        }
        public void setPriceText()
        {
            PriceText = String.Format("{0:0,0}", Price);
        }
        public Goods()
        {

        }
        public Goods(string Code, string Name, Int32 Price, int Count)
        {
            this.Code = Code;
            this.Name = Name;
            this.Price = Price;
            this.Count = Count;
            this.TotalPrice = Count * Price;
            PriceText = String.Format("{0:0,0}", Price);
            TotalPriceText = String.Format("{0:0,0}", TotalPrice);
        }

        public Goods(string ID, string Code, string Name, DateTime inputDate, string goodsType, Int32 Price, int Count)
        {
            this.ID = ID;
            this.Code = Code;
            this.Name = Name;
            this.InputDate = inputDate;
            this.GoodsType = goodsType;
            this.Count = Count;
            this.Price = Price;
            this.TotalPrice = Count * Price;
            PriceText = String.Format("{0:0,0}", Price);
            TotalPriceText = String.Format("{0:0,0}", TotalPrice);
        }
    }
}
