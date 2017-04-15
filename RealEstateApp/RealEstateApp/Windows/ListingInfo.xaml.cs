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
using System.IO;

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
            
            // Set Display picture
            if (originalItem.display_picture != null) {
                
                using (MemoryStream ms = new MemoryStream(originalItem.display_picture)) {
                    
                    // Read in the Byte Array
                    ms.Position = 0;
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = ms;
                    img.EndInit();

                    // Display in window
                    listingImage.Source = img;
                }
            }
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

            // First, find the highest offer on the house, if there are no offers skip the next step
            using (var context = new Model()) {

                var offers = context.Offers.SqlQuery("SELECT * FROM Offer WHERE listing_id = @id ORDER BY amount DESC", 
                    new SqlParameter("id", item.id)).ToList<Offer>();

                // If no offers, skip the payment calculations
                if (offers.Count > 0) {

                    decimal highestAmount = offers.ElementAt(0).amount;

                    // Check to see if the seller has an agent
                    var seller = context.Clients.SqlQuery("SELECT * FROM Client WHERE id = @id", new SqlParameter("id", originalItem.seller_id)).FirstOrDefault<Client>();
                    
                    if (seller.assigned_agent != null) {

                        var agent = context.Agents.SqlQuery("SELECT * FROM Agent WHERE id = @id", new SqlParameter("id", seller.assigned_agent)).FirstOrDefault<Agent>();

                        // Next calculate what the agent will get as a commission and what the broker will get 
                        decimal agentAmount = (decimal)((float)highestAmount * (agent.commission_percentage / 100));
                        // Next calculate the amount that the broker gets from the agent's commission
                        decimal brokerAmount = (decimal)((float)agentAmount * (agent.broker_share / 100));

                        // Distribute payments
                        // Insert the two commissions into the database
                        context.Database.ExecuteSqlCommand("INSERT INTO Commission VALUES (@id, @amount, @reason, @date)",
                            new SqlParameter("id", agent.id),
                            new SqlParameter("amount", agentAmount),
                            new SqlParameter("reason", "Commission for Closing of Listing: " + item.Address),
                            new SqlParameter("date", DateTime.Now));

                        // Deposit commission into balance // TODO make sure this auto updates as well
                        context.Database.ExecuteSqlCommand("UPDATE Agent SET commission_balance = @amount WHERE id = @id",
                            new SqlParameter("amount", agentAmount + agent.commission_balance),
                            new SqlParameter("id", agent.id));

                        // Get the broker's information
                        var broker = context.Agents.SqlQuery("SELECT * FROM Agent WHERE employee_username = @brokername AND employee_office_id = @id",
                            new SqlParameter("brokername", agent.Employee.Office.broker_username),
                            new SqlParameter("id", agent.employee_office_id)).FirstOrDefault<Agent>();

                        // if broker, skip second payment which would be a double commission records
                        if (agent.id != broker.id) {
                            // Get the broker's ID
                            context.Database.ExecuteSqlCommand("INSERT INTO Commission VALUES (@id, @amount, @reason, @date)",
                                new SqlParameter("id", broker.id),
                                new SqlParameter("amount", brokerAmount),
                                new SqlParameter("reason", "Commission for Closing of Listing: " + item.Address),
                                new SqlParameter("date", DateTime.Now));

                            // Deposit commission into balance // TODO make sure this auto updates as well
                            context.Database.ExecuteSqlCommand("UPDATE Agent SET commission_balance = @amount WHERE id = @id",
                                new SqlParameter("amount", brokerAmount + broker.commission_balance),
                                new SqlParameter("id", broker.id));
                        }
                    }
                }

                // TODO this
                // Next we need to start deleting everything
                // First manually delete seller if the seller does not have any other houses for sale or purchasing
                // Cascading deletion for Offers / Address / Features > Listing itself
                // update client type if needed and if not deleted
            }
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
