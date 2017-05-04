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
using System.Drawing;
using System.IO;

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for NewListing.xaml
    /// </summary>
    public partial class NewListing : Window {

        private bool clientSelected = false;
        private bool clientTypeChanged = false;
        private List<int> clientIDs = new List<int>();
        private string imageFilePath = null;
        private byte[] selectedImageData = null;
        private ListView list;

        public NewListing(ListView list) {

            InitializeComponent();
            this.list = list;
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
                var queryResults = context.Database.SqlQuery<Client>("SELECT * FROM Client").ToList<Client>();
                
                // Add all of the clients into the list so they can be queried later on
                foreach (Client c in queryResults) {

                    clientIDs.Add(c.id);
                    existingClientList.Items.Add(c.first_name + " " + c.last_name);
                }

                // If we have no existing clients, disable the field
                if (clientIDs.Count == 0)
                    existingClientList.IsEnabled = false;
            }

            // Limit Date Ranges
            yearBuiltField.DisplayDateEnd = DateTime.Today;
            yearBuiltField.DisplayDateStart = DateTime.Today.AddYears(-250); // Subtract 250 years for the first ever possbily built house
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
                sellerID = HelperFunctions.AddNewClient(null, fname, lname, "Seller", phone, email);

                if (sellerID == -1) {
                    MessageBox.Show("Unable to create client", "Incorrect Fields");
                    return;
                }
            }
            // otherwise, we have to just find the client's ID
            else {

                sellerID = clientIDs.ElementAt(existingClientList.SelectedIndex - 1);

                // If the client is only a buyer atm, upgrade to Both
                using (var context = new Model()) {

                    var seller = context.Clients.SqlQuery("SELECT * FROM Client WHERE id = @id", new SqlParameter("id", sellerID)).FirstOrDefault<Client>();

                    if (seller.client_type.Equals("B")) {
                        context.Database.ExecuteSqlCommand("UPDATE Client SET client_type = 'E' WHERE id = @id", new SqlParameter("id", sellerID));
                        clientTypeChanged = true;
                    }
                }
            }


            // Next we have to make a new address
            int streetNum = -1;
            bool parseResult = int.TryParse(streetNumberField.Text, out streetNum);

            if (streetNum < 0) {
                MessageBox.Show("Street Address cannot be below 0", "Incorrect Fields");
                // Get rid of the client if existing
                RemoveClient(sellerID);
                return;
            }

            string streetName = streetNameField.Text;
            string city = cityField.Text;
            string postalcode = postalCodeField.Text.ToString();

            // Check fields to make sure they arent null
            if (HelperFunctions.NullOrEmpty(streetName, city)  || streetTypeBox.SelectedIndex == -1 || provinceBox.SelectedIndex == -1
                || HelperFunctions.CheckPostalCode(postalcode) is false 
                || parseResult is false) {

                MessageBox.Show("Address related fields have resulted in an error", "Incorrect Fields");
                // Get rid of the client if existing
                RemoveClient(sellerID);
                return;
            }

            string streetType = ((ComboBoxItem)streetTypeBox.SelectedItem).Content.ToString();
            string province = ((ComboBoxItem)provinceBox.SelectedItem).Content.ToString();

            // If not, we can make a new address and store it's ID as well.
            SqlParameter streetNumParam = new SqlParameter("num", streetNum);
            SqlParameter streetNameParam = new SqlParameter("name", streetName);
            SqlParameter streetTypeParam = new SqlParameter("type", streetType);
            SqlParameter cityParam = new SqlParameter("city", city);
            SqlParameter provinceParam = new SqlParameter("province", province);
            SqlParameter postalCodeParam = new SqlParameter("postalcode", postalcode);

            Object[] parameters = new object[] { streetNumParam, streetNameParam, streetTypeParam, cityParam, provinceParam, postalCodeParam };
            Int32 addressID = 0;
            using (var context = new Model()) {

                context.Database.ExecuteSqlCommand("INSERT INTO StreetAddress (address_num, street, street_type, city, province_short, postal_code)" +
                                                   "VALUES (@num, @name, @type, @city, @province, @postalcode)", parameters);

                // Get the foreign key ID
                var result = context.StreetAddresses.SqlQuery("SELECT * FROM StreetAddress WHERE ID = IDENT_CURRENT('StreetAddress')").FirstOrDefault<StreetAddress>();
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
                    // Get rid of the client if existing
                    RemoveClient(sellerID);
                    return;
                }
                if (lotSizeField.Text.Equals("") is false && lotSize < 0) {
                    MessageBox.Show("House information fields are incorrect", "Incorrect Fields");
                    // Get rid of the client if existing
                    RemoveClient(sellerID);
                    return;
                }

                MessageBox.Show("House information fields are incorrect", "Incorrect Fields");
                // Get rid of the client if existing
                RemoveClient(sellerID);
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
                // Get rid of the client if existing
                RemoveClient(sellerID);
                return;
            }

            DateTime dateListed = DateTime.Today;
            
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
                SqlParameter sqFootageParam = new SqlParameter("sqfootage", SqlDbType.Real); //optional
                sqFootageParam.Value = squareFootage;
                SqlParameter lotSizeParam = new SqlParameter("lotsize", SqlDbType.Real); //optional
                lotSizeParam.Value = lotSize;
                SqlParameter pictureParam = new SqlParameter("picture", SqlDbType.Image); //optional
                pictureParam.Value = selectedImageData; 
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
                if (selectedImageData == null)
                    pictureParam.Value = DBNull.Value;

                parameters = new object[] { addressFK, sellerFK, askingPriceParam, numBedParam, numBathParam, numStoriesParam,
                    hasGarageParam, yearBuiltParam, sqFootageParam, lotSizeParam, pictureParam, timestamp };

                try {
                    context.Database.ExecuteSqlCommand("INSERT INTO Listing (address_id, seller_id, asking_price, num_bedrooms, num_bathrooms, " +
                                                       "num_stories, has_garage, year_built, square_footage, lot_size, display_picture, date_listed)" +
                                                       "VALUES (@addrFK, @sellerFK, @askingprice, @bed, @bath," +
                                                       "@stories, @garage, @yearbuilt, @sqfootage, @lotsize, @picture, @timestamp)", parameters);
                }
                catch (Exception ex) {
                    MessageBox.Show("Could not create listing", "Incorrect Fields");

                    // Get rid of the client if existing
                    RemoveClient(sellerID);
                    return;
                }

                // Get the record we just added
                var result = context.Listings.SqlQuery("SELECT * FROM Listing WHERE ID = IDENT_CURRENT('listing')").FirstOrDefault<Listing>();

                // Construct the list item
                ListingItem newItem = new ListingItem(yearBuilt, dateListed);
                newItem.id = result.id;
                newItem.Address = HelperFunctions.AddressToString(result.StreetAddress);
                newItem.Bedrooms = result.num_bedrooms;
                newItem.Bathrooms = result.num_bathrooms;
                newItem.Stories = result.num_stories;
                newItem.AskingPriceDecimal = result.asking_price;
                newItem.originalItem = result;

                // Add it to the listview
                list.Items.Add(newItem);
            
                Close();
            }
        }

        private void RemoveClient(int sellerID) {

            using (var context = new Model()) {
                // Get rid of the client if existing
                if (clientSelected is false)
                    context.Database.ExecuteSqlCommand("DELETE FROM Client WHERE id = @id", new SqlParameter("id", sellerID));

                // If existing client
                else if (clientTypeChanged is true) {
                    context.Database.ExecuteSqlCommand("UPDATE Client SET client_type = 'B' WHERE id = @id", new SqlParameter("id", sellerID));
                    clientTypeChanged = false;
                }
            }
        }

        /// <summary>
        /// Dialog for browsing for an image to be stored in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFileBrowser(object sender, RoutedEventArgs e) {

            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.Filter = "Image Files (*.jpg, *.jpeg)|*.jpg;*.jpeg";

            bool? result = fileDialog.ShowDialog();

            if (result is true) {

                imageFilePath = fileDialog.FileName;

                // Make the image holder
                System.Drawing.Image selectedImage = System.Drawing.Image.FromFile(imageFilePath);
                // Convert to byte array to store in DB
                using (MemoryStream ms = new MemoryStream()) {

                    selectedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    selectedImageData = ms.ToArray();
                }
            }
        }
    }
}
