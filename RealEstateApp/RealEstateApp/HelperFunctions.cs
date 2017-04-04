using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RealEstateApp.EntityModels;

namespace RealEstateApp {

    public static class HelperFunctions {

        public static string AddressToString(StreetAddress addr) {

            return addr.address_num.ToString() + " " 
                + addr.street + " " 
                + addr.street_type + ", " 
                + addr.city + ", " 
                + addr.province_short + ", " 
                + addr.postal_code;
        }

        public static Boolean NullOrEmpty(params string[] strings) {

            // Check if any of the strings is null or empty
            return strings.Any(s => s == null || s.Equals(""));
        }

        public static Boolean StringNumeric(string s) {

            foreach(char c in s) {

                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }
}
