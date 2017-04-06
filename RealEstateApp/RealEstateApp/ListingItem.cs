using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class ListingItem {

        public int id { get; set; }
        public string Address { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Stories { get; set; }
        public string YearBuilt { get; set;}
        public decimal AskingPriceDecimal { get; set; }
        public string AskingPrice {
            get {
                return String.Format("{0:C}", AskingPriceDecimal);
            }
        }
        public string DateListed { get; set; }

        public ListingItem(DateTime? yearBuilt, DateTime dateListed) {

            if (yearBuilt != null)
                YearBuilt = ((DateTime)yearBuilt).ToShortDateString();
            else
                YearBuilt = "N/A";
            DateListed = dateListed.ToShortDateString();
        }
        
    }
}
