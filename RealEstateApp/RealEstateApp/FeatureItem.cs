using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp {

    public class FeatureItem {

        public String Header { get; set; }
        public String Description { get; set; }

        public FeatureItem(string header, string description) {

            Header = header;
            Description = description;
        }
    }
}
