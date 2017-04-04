using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class ListingItem {

        // Listing data should have:
        // street address information
        // number of bedrooms
        // number of bathrooms
        // number of stories
        // year built
        // asking price
        // date listed
        public string Address { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Stories { get; set; }
        public DateTime YearBuilt { get; set; }
        public decimal AskingPrice { get; set; }
        public DateTime DateListed { get; set; }
        
    }
}
