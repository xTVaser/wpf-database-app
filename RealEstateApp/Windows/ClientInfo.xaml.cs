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
    /// Interaction logic for ClientInfo.xaml
    /// </summary>
    public partial class ClientInfo : Window {

        private ClientItem item;

        public ClientInfo(ClientItem item) {

            InitializeComponent();
            this.item = item;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Fill in easy information
            clientName.Content = item.FirstName + " " + item.LastName;
            phoneNumber.Content = item.PhoneNumber;
            email.Content = item.Email;

            // Fill in assigned agent ID
            if (item.agentId != null) {

                using (var context = new Model()) {

                    var queryResult = context.Agents.SqlQuery("SELECT * FROM Agent WHERE id = @id", new SqlParameter("id", item.agentId)).FirstOrDefault<Agent>();

                    assignedAgentName.Content = queryResult.Employee.first_name + " " + queryResult.Employee.last_name;
                }
            }
            else
                assignedAgentName.Content = "No Assigned Agent";

            // Fill any offers they may have
            using (var context = new Model()) {

                var queryResults = context.Offers.SqlQuery("SELECT * FROM Offer WHERE client_id = @client", new SqlParameter("client", item.id)).ToList<Offer>();

                foreach (Offer o in queryResults) {

                    OfferItem newItem = new OfferItem(o.date_offered);
                    newItem.Address = HelperFunctions.AddressToString(o.Listing.StreetAddress);
                    newItem.AmountDecimal = o.amount;

                    offerGridView.Items.Add(newItem);
                }
            }
        }
    }
}
