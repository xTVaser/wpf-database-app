using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

// TODO: implement

namespace RealEstateApp {
    /// <summary>
    /// Interaction logic for ListingInfo.xaml
    /// </summary>
    public partial class ListingInfo : Window {

        private ListingItem item;

        public ListingInfo(ListingItem item) {

            InitializeComponent();
            this.item = item;
        }

        private void AddFeature(object sender, RoutedEventArgs e) {

        }

        private void AddOffer(object sender, RoutedEventArgs e) {

        }

        private void RemoveListing(object sender, RoutedEventArgs e) {

        }

        private void ContactSeller(object sender, RoutedEventArgs e) {

        }
    }
}
