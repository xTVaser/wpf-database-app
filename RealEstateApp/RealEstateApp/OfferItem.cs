using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class OfferItem {

        public string Address { get; set; }
        public decimal AmountDecimal { get; set; }
        public string Amount {
            get {
                return String.Format("{0:C}", AmountDecimal);
            }
        }
        public string DateOffered { get; set; }

        public OfferItem(DateTime dateOffered) {

            DateOffered = dateOffered.ToShortDateString();
        }
    }
}
