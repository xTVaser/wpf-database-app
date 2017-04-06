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

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window {

        private Employee user;
        private string accountType;
        private bool isBroker;

        public Dashboard(Employee user, bool isBroker) {

            InitializeComponent();
            this.user = user;
            this.isBroker = isBroker;

            if (isBroker)
                accountType = "Broker";
            else if (user.employee_type.Equals("S"))
                accountType = "Administrative Staff";
            else
                accountType = "Agent";
        }

        /// <summary>
        /// Setup dashboard window on load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Hide tabs that aren't needed for the particular user
            // If Administrator, do not need to see balance or employee tabs.
            if (user.employee_type.Equals("S")) {
                balanceTab.Visibility = Visibility.Collapsed;

                newClientBtn.Visibility = Visibility.Collapsed;
                FillListingTab(listingGridView);
            }
            // Else, agent
            else {
                newListingBtn.Visibility = Visibility.Collapsed;
                //FillTab(balanceTab);
            }

            // If not the broker, dont need to manage employees
            if (isBroker is false) {
                employeeTab.Visibility = Visibility.Collapsed;
                newEmployeeBtn.Visibility = Visibility.Collapsed;
            }
            else
                Console.WriteLine("stub");
            //FillTab(employeeTab);
            
            // Everyone can see these tabs and content within
            FillOfficeTab(officeGridView);
            FillClientTab(clientGridView);

            // Set the status text at the top
            accountStatus.Content += user.first_name + " " + user.last_name;
            accessStatus.Content += accountType;
        }

        //Button Methods

        private void newListingBtn_Click(object sender, RoutedEventArgs e) {

            // Open new listing window
            NewListing newListingWindow = new NewListing();
            newListingWindow.Show();
        }

        private void newClientBtn_Click(object sender, RoutedEventArgs e) {

            // Open client window
            AddClient addClientWindow = new AddClient();
            addClientWindow.Show();
        }

        /// <summary>
        /// Populates the listing tab with ListingItems
        /// </summary>
        /// <param name="list">The listview that contains the items</param>
        private void FillListingTab(ListView list) {
            
            // Fill tab with all listings
            using ( var context = new Model()) {

                var queryResult = context.Listings.SqlQuery("SELECT * FROM Listing").ToList();

                foreach (Listing l in queryResult) {

                    ListingItem newItem = new ListingItem(l.year_built, l.date_listed);
                    newItem.id = l.id;
                    newItem.Address = HelperFunctions.AddressToString(l.StreetAddress);
                    newItem.Bedrooms = l.num_bedrooms;
                    newItem.Bathrooms = l.num_bathrooms;
                    newItem.Stories = l.num_stories;
                    newItem.AskingPriceDecimal = l.asking_price;

                    list.Items.Add(newItem);
                }
            }
        }

        private void listGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as ListingItem;
        }

        private void FillOfficeTab(ListView list) {

            // Fill tab with all offices information
            using (var context = new Model()) {

                var queryResult = context.Offices.SqlQuery("SELECT * FROM Office").ToList();

                foreach (Office o in queryResult) {

                    OfficeItem newItem = new OfficeItem();
                    newItem.ID = o.id;
                    newItem.Address = HelperFunctions.AddressToString(o.StreetAddress);
                    newItem.PhoneNumber = HelperFunctions.PhoneNumberToString(o.phone_number);
                    newItem.FaxNumber = HelperFunctions.PhoneNumberToString(o.fax_number);
                    newItem.Email = o.email;

                    newItem.BrokerUsername = o.broker_username;

                    list.Items.Add(newItem);
                }
            }
        }

        private void officeGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }

        // TODO: agents will only see their clients, right now it shows all
        private void FillClientTab(ListView list) {

            // Fill tab with all offices information
            using (var context = new Model()) {

                var queryResult = context.Clients.SqlQuery("SELECT * FROM Client").ToList();

                foreach (Client c in queryResult) {

                    ClientItem newItem = new ClientItem(c.client_type);
                    newItem.FirstName = c.first_name;
                    newItem.LastName = c.last_name;
                    newItem.PhoneNumber = HelperFunctions.PhoneNumberToString(c.phone_number);
                    newItem.Email = c.email;

                    newItem.id = c.id;
                    newItem.agentId = c.assigned_agent;

                    list.Items.Add(newItem);
                }
            }
        }

        private void clientGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

        }
    }
}
