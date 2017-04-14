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

using RealEstateApp.EntityModels;
using System.Data.SqlClient;

namespace RealEstateApp {
    /// <summary>
    /// Interaction logic for ListingInfo.xaml
    /// </summary>
    public partial class ListingInfo : Window {

        private ListingItem item;
        private Listing originalItem;

        public ListingInfo(ListingItem item) {

            InitializeComponent();
            this.item = item;
            originalItem = item.originalItem;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Add the ID number to the label at the top
            listingIDLabel.Content += item.id.ToString();

            // Add the address information
            listingAddress.Text = item.Address;

            // Add all of the direct features
            featureGridView.Items.Add(new FeatureItem("Asking Price", item.AskingPrice));
            featureGridView.Items.Add(new FeatureItem("Date Listed", item.DateListed));
            featureGridView.Items.Add(new FeatureItem("Date Built", item.YearBuilt));

            if (originalItem.square_footage != null)
                featureGridView.Items.Add(new FeatureItem("Square Footage", originalItem.square_footage.ToString()));
            if (originalItem.lot_size != null)
                featureGridView.Items.Add(new FeatureItem("Lot Size", originalItem.lot_size.ToString()));

            featureGridView.Items.Add(new FeatureItem("Number of Bedrooms", item.Bedrooms.ToString()));
            featureGridView.Items.Add(new FeatureItem("Number of Bathrooms", item.Bathrooms.ToString()));
            featureGridView.Items.Add(new FeatureItem("Number of Stories", item.Stories.ToString()));
            featureGridView.Items.Add(new FeatureItem("Has Garage?", originalItem.has_garage.ToString()));

            // Add the addition features
            using (var context = new Model()) {

                var features = context.Features.SqlQuery("SELECT * FROM Feature WHERE listing_id = @id", new SqlParameter("id", item.id)).ToList<Feature>();

                foreach (Feature f in features)
                    featureGridView.Items.Add(new FeatureItem(f.heading, f.body));
            }

            // TODO Display picture here
        }

        /// <summary>
        /// When the add feature button is clicked, we open a popup to create a new feature, 
        /// pass id of listing, as well as feature list so it can be dynamically updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFeature(object sender, RoutedEventArgs e) {

            AddFeature newWindow = new AddFeature(item.id, featureGridView);
            newWindow.Show();
        }

        /// <summary>
        /// Add offer button is clicked, another popup in order to add an offer, we send the ID of the current listing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddOffer(object sender, RoutedEventArgs e) {

            AddOffer newWindow = new AddOffer(item.id);
            newWindow.Show();
        }

        /// <summary>
        /// Listings are removed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseListing(object sender, RoutedEventArgs e) {





        }

        /// <summary>
        /// Display client info screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContactSeller(object sender, RoutedEventArgs e) {

            // Construct the client object
            using (var context = new Model()) {

                var client = context.Clients.SqlQuery("SELECT * FROM Client WHERE id = @id", new SqlParameter("id", originalItem.seller_id)).FirstOrDefault<Client>();

                ClientItem newItem = new ClientItem(client.client_type);
                newItem.id = client.id;
                newItem.agentId = newItem.agentId;
                newItem.Email = client.email;
                newItem.FirstName = client.first_name;
                newItem.LastName = client.last_name;
                newItem.PhoneNumber = HelperFunctions.PhoneNumberToString(client.phone_number);

                // Open the window
                ClientInfo newWindow = new ClientInfo(newItem);
                newWindow.Show();
            }

        }
    }
}
