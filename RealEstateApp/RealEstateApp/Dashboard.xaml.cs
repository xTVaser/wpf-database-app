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
                employeeTab.Visibility = Visibility.Collapsed;

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
                //FillTab(employeeTab);

                


            //FillTab(officeTab);
            //FillTab(clientTab);

            // Set the title at the top
            statusLabel.Content = "Logged in as: " + user.first_name + " " + user.last_name + " Access Level: " + accountType;
            
        }

        /// <summary>
        /// Populates the listing tab with ListingItems
        /// </summary>
        /// <param name="list">The listview that contains the items</param>
        private void FillListingTab(ListView list) {
            
            // Fill tab with all listings
            using ( var context = new Model()) {

                var queryResult = context.listings.SqlQuery("SELECT * FROM listing").ToList();

                foreach (Listing l in queryResult) {

                    ListingItem newItem = new ListingItem();
                    newItem.Address = HelperFunctions.AddressToString(l.street_address);
                    newItem.Bedrooms = l.num_bedrooms;
                    newItem.Bathrooms = l.num_bathrooms;
                    newItem.Stories = l.num_stories;
                    newItem.YearBuilt = (DateTime) l.year_built;
                    newItem.AskingPrice = l.asking_price;
                    newItem.DateListed = l.date_listed;

                    list.Items.Add(newItem);
                }
            }
        }

        private void newListingBtn_Click(object sender, RoutedEventArgs e) {

            // Open new listing window
            NewListing newListingWindow = new NewListing();
            newListingWindow.Show();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as ListingItem;
        }

        // Fill client page

        private void newClientBtn_Click(object sender, RoutedEventArgs e) {

            // Open client window
            AddClient addClientWindow = new AddClient();
            addClientWindow.Show();
        }
    }
}
