using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class ClientItem {

        public int id { get; set; } 
        public int? agentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ClientType { get; set; }

        public ClientItem(string type) {

            if (type.Equals("B"))
                ClientType = "Buyer";
            else if (type.Equals("S"))
                ClientType = "Seller";
            else
                ClientType = "Buyer/Seller";
        }
    }
}
