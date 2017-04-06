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
using System.Data;

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for NewListing.xaml
    /// </summary>
    public partial class NewListing : Window {

        private bool clientSelected = false;
        private List<int> clientIDs = new List<int>();
        private string imageFilePath = null;

        public NewListing() {

            InitializeComponent();
        }

        /// <summary>
        /// When the window is loaded, the client list must be populated 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // populate client list text field
            using (var context = new Model()) {

                // Query the DB
                var queryResults = context.Database.SqlQuery<Client>("SELECT * from client").ToList<Client>();
                
                // Add all of the clients into the list so they can be queried later on
                foreach (Client c in queryResults) {

                    clientIDs.Add(c.id);
                    existingClientList.Items.Add(c.first_name + " " + c.last_name);
                }

                // If we have no existing clients, disable the field
                if (clientIDs.Count == 0)
                    existingClientList.IsEnabled = false;
            }
        }

        /// <summary>
        /// If the user selects an existing client, disable and clear the manual input fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void existingClientList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            if (existingClientList.SelectedItem == null)
                return;
            else if(existingClientList.SelectedIndex == 0) {
                // Turn fields on
                clientFirstName.IsEnabled = true;
                clientLastName.IsEnabled = true;
                clientPhoneNumber.IsEnabled = true;
                clientEmail.IsEnabled = true;
                clientSelected = false;
            }
            else {
                // Turn fields off
                clientFirstName.IsEnabled = false;
                clientLastName.IsEnabled = false;
                clientPhoneNumber.IsEnabled = false;
                clientEmail.IsEnabled = false;
                clientSelected = true;
            }
        }

        private bool changedYearBuilt = false;

        private void yearBuiltField_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {

            changedYearBuilt = true;
        }

        private void SubmitListing(object sender, RoutedEventArgs e) {

            Int32 sellerID = 0;

            // If not existing client, have to make the client to get its ID, a seller client at this point
            if (clientSelected is false) {

                string fname = clientFirstName.Text;
                string lname = clientLastName.Text;
                string phone = clientPhoneNumber.Text;
                string email = clientEmail.Text;

                //need the ID of the last inserted row
                sellerID = HelperFunctions.AddNewClient(fname, lname, "Seller", phone, email);

                if (sellerID == -1)
                    return;
            }
            // otherwise, we have to just find the client's ID
            else 
                sellerID = clientIDs.ElementAt(existingClientList.SelectedIndex);

            // Next we have to make a new address
            int streetNum = -1;
            bool parseResult = int.TryParse(streetNumberField.Text, out streetNum);

            if (streetNum < 0) {
                MessageBox.Show("Street Address cannot be below 0", "Incorrect Fields");
                return;
            }

            string streetName = streetNameField.Text;
            string streetType = ((ComboBoxItem)streetTypeBox.SelectedItem).Content.ToString();
            string city = cityField.Text;
            string province = ((ComboBoxItem)provinceBox.SelectedItem).Content.ToString();
            string postalcode = postalCodeField.Text.ToString();

            // Check fields to make sure they arent null
            if (HelperFunctions.NullOrEmpty(streetName, streetType, province, city) 
                || HelperFunctions.CheckPostalCode(postalcode) is false 
                || parseResult is false) {

                MessageBox.Show("Address related fields have resulted in an error", "Incorrect Fields");
                return;
            }

            // If not, we can make a new address and store it's ID as well.
            SqlParameter streetNumParam = new SqlParameter("num", streetNum);
            SqlParameter streetNameParam = new SqlParameter("name", streetNum);
            SqlParameter streetTypeParam = new SqlParameter("type", streetType);
            SqlParameter cityParam = new SqlParameter("city", city);
            SqlParameter provinceParam = new SqlParameter("province", province);
            SqlParameter postalCodeParam = new SqlParameter("postalcode", postalcode);

            Object[] parameters = new object[] { streetNumParam, streetNameParam, streetTypeParam, cityParam, provinceParam, postalCodeParam };
            Int32 addressID = 0;
            using (var context = new Model()) {

                context.Database.ExecuteSqlCommand("INSERT INTO street_address (address_num, street, street_type, city, province_short, postal_code)" +
                                                   "VALUES (@num, @name, @type, @city, @province, @postalcode)", parameters);

                // Get the foreign key ID
                var result = context.StreetAddresses.SqlQuery("SELECT * FROM street_address WHERE ID = IDENT_CURRENT('street_address')").FirstOrDefault<StreetAddress>();
                addressID = result.id;
            }

            // Finally, we can make the listing with the left over information
            List<bool> parsedAttempts = new List<bool>();
            byte numBedrooms = 0;
            byte numBathrooms = 0;
            byte numStories = 0;
            double squareFootage = -1;
            double lotSize = -1;

            parsedAttempts.Add(byte.TryParse(numberBedroomsField.Text, out numBedrooms));
            parsedAttempts.Add(byte.TryParse(numberBathroomsField.Text, out numBathrooms));
            parsedAttempts.Add(byte.TryParse(numberStoriesField.Text, out numStories));

            double.TryParse(squareFootageField.Text, out squareFootage);
            double.TryParse(lotSizeField.Text, out lotSize);

            // If anything goes wrong in the parsing
            if (HelperFunctions.IntegersBelowZero(numBedrooms, numBathrooms, numStories) 
                || parsedAttempts.Any(a => a is false)) {
                
                if (squareFootageField.Text.Equals("") is false && squareFootage < 0) {
                    MessageBox.Show("House information fields are incorrect", "Incorrect Fields");
                    return;
                }
                if (lotSizeField.Text.Equals("") is false && lotSize < 0) {
                    MessageBox.Show("House information fields are incorrect", "Incorrect Fields");
                    return;
                }

                MessageBox.Show("House information fields are incorrect", "Incorrect Fields");
                return;
            }

            // squarefootage and lotsize information is optional so must be handled differently
            if (squareFootage > 0)
                if (lotSize > 0) { }

            bool hasGarage = (bool)hasGarageBox.IsChecked;
            DateTime yearBuilt = (DateTime)yearBuiltField.SelectedDate;
            Decimal askingPrice = -1;
            if(Decimal.TryParse(askingPriceField.Text, out askingPrice) is false || askingPrice < 0) {
                MessageBox.Show("Asking price was entered incorrectly", "Incorrect Fields");
                return;
            }
            // TODO dates are not validated

            DateTime dateListed = DateTime.Today;

            // TODO display picture here

            // Insert into database
            using (var context = new Model()) {

                SqlParameter addressFK = new SqlParameter("addrFK", addressID);
                SqlParameter sellerFK = new SqlParameter("sellerFK", sellerID);
                SqlParameter askingPriceParam = new SqlParameter("askingprice", askingPrice);
                SqlParameter numBedParam = new SqlParameter("bed", numBedrooms);
                SqlParameter numBathParam = new SqlParameter("bath", numBathrooms);
                SqlParameter numStoriesParam = new SqlParameter("stories", numStories);
                SqlParameter hasGarageParam = new SqlParameter("garage", hasGarage);
                SqlParameter yearBuiltParam = new SqlParameter("yearbuilt", SqlDbType.DateTime); //optional
                yearBuiltParam.Value = yearBuilt;
                SqlParameter sqFootageParam = new SqlParameter("sqfootage", squareFootage); //optional
                SqlParameter lotSizeParam = new SqlParameter("lotsize", lotSize); //optional
                // Display picture // optional
                SqlParameter timestamp = new SqlParameter("timestamp", SqlDbType.DateTime);
                timestamp.Value = dateListed;


                yearBuiltParam.IsNullable = true;
                sqFootageParam.IsNullable = true;
                lotSizeParam.IsNullable = true;

                if (changedYearBuilt is false)
                    yearBuiltParam.Value = DBNull.Value;
                if (squareFootage <= 0)
                    sqFootageParam.Value = DBNull.Value;
                if (lotSize <= 0)
                    lotSizeParam.Value = DBNull.Value;

                parameters = new object[] { addressFK, sellerFK, askingPriceParam, numBedParam, numBathParam, numStoriesParam,
                    hasGarageParam, yearBuiltParam, sqFootageParam, lotSizeParam, timestamp };

                context.Database.ExecuteSqlCommand("INSERT INTO listing (address_id, seller_id, asking_price, num_bedrooms, num_bathrooms, " +
                                                   "num_stories, has_garage, year_built, square_footage, lot_size, date_listed)" +
                                                   "VALUES (@addrFK, @sellerFK, @askingprice, @bed, @bath," +
                                                   "@stories, @garage, @yearbuilt, @sqfootage, @lotsize, @timestamp)", parameters);

                Close();
            }
        }

        /// <summary>
        /// Dialog for browsing for an image to be stored in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileBrowser(object sender, RoutedEventArgs e) {

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "Image Files (*.png, *.jpg, *.bmp, *.jpeg)|*.png;*.jpg;*.bmp;*.jpeg";

            bool? result = fileDialog.ShowDialog();

            if (result is true) {

                imageFilePath = fileDialog.FileName;
                // TODO: actually store the image http://stackoverflow.com/questions/21601858/insert-picturebox-image-into-sql-server-database
                // might need to filter the images if they arent all supported
            }

        }

        
    }
}
