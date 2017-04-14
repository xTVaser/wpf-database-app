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
    /// Interaction logic for AddOffer.xaml
    /// </summary>
    public partial class AddOffer : Window {

        private int listingId;
        private List<int> clientIDs = new List<int>();

        public AddOffer(int listingId) {

            InitializeComponent();
            this.listingId = listingId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            using (var context = new Model()) {

                var clients = context.Clients.SqlQuery("SELECT * FROM Client").ToList<Client>();

                List<string> clientList = new List<string>();
                foreach (Client c in clients) {

                    clientList.Add(c.id + ": " + c.first_name + " " + c.last_name);
                    clientIDs.Add(c.id);
                }

                if (clientList.Count == 0)
                    clientList.Add("No Clients Found");
                else
                    clientList.Insert(0, "");

                // Display in the combo box
                clientBox.ItemsSource = clientList;
            }
        }

        private void AddNewOffer(object sender, RoutedEventArgs e) {

            string amountText = amountField.Text;

            // Check client is valid first
            if (clientIDs.Count == 0 || clientBox.SelectedIndex == 0 || HelperFunctions.NullOrEmpty(amountText)) {
                MessageBox.Show("Fields are empty, fill them in and try again", "Empty Fields");
                return;
            }

            // Check if the amount was parseable
            decimal amount;
            bool success = Decimal.TryParse(amountText, out amount);

            if (success is false) {
                MessageBox.Show("Not a valid amount", "Empty Fields");
                return;
            }

            // Otherwise we are good to insert
            using (var context = new Model()) {

                // First check to see if there are any offers for this listing by the same client
                var checkQuery = context.Offers.SqlQuery("SELECT * FROM Offer WHERE client_id = @cid AND listing_id = @lid",
                    new SqlParameter("cid", clientIDs.ElementAt(clientBox.SelectedIndex)), new SqlParameter("lid", listingId)).ToList<Offer>();

                // If there is a result, than we will update the field instead of inserting
                if (checkQuery.Count > 0) {

                    SqlParameter amountParam = new SqlParameter("amount", amount);
                    SqlParameter dateParam = new SqlParameter("today", DateTime.Today);
                    SqlParameter clientParam = new SqlParameter("cid", clientIDs.ElementAt(clientBox.SelectedIndex-1)); // TODO check other uses of combobox to see if i made a similar mistake
                    SqlParameter listingParam = new SqlParameter("lid", listingId);

                    Object[] parameters = { amountParam, dateParam, clientParam, listingParam };

                    context.Database.ExecuteSqlCommand("UPDATE Offer SET amount = @amount, date_offered = @today " +
                                                        "WHERE client_id = @cid AND listing_id = @lid", parameters);
                }
                // Otherwise, just insert
                else {

                    SqlParameter amountParam = new SqlParameter("amount", amount);
                    SqlParameter dateParam = new SqlParameter("today", DateTime.Today);
                    SqlParameter clientParam = new SqlParameter("cid", clientIDs.ElementAt(clientBox.SelectedIndex-1));
                    SqlParameter listingParam = new SqlParameter("lid", listingId);

                    Object[] parameters = { amountParam, dateParam, clientParam, listingParam };

                    context.Database.ExecuteSqlCommand("INSERT INTO Offer (client_id, listing_id, amount, date_offered) VALUES (@cid, @lid, @amount, @today)", parameters);
                }

                Close();

            }
        }
    }
}
