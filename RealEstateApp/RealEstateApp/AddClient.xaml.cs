using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Net.Mail;

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for AddClient.xaml
    /// </summary>
    public partial class AddClient : Window {

        public AddClient() {
            InitializeComponent();
        }

        private void AddNewClient(object sender, RoutedEventArgs e) {

            string firstName = firstNameField.Text;
            string lastName = lastNameField.Text;
            string clientType = clientTypeField.SelectedValue.ToString();
            string phoneNumber = phoneNumberField.Text;
            string email = emailField.Text;

            // If any of the fields are empty, then stop
            if (HelperFunctions.NullOrEmpty(firstName, lastName, clientType, phoneNumber, email)) {

                MessageBox.Show("Please fill out the remaining fields", "Empty Fields");
                return;
            }

            using (var context = new Model()) {

                SqlParameter fnameParam = new SqlParameter("fname", firstName);
                SqlParameter lnameParam = new SqlParameter("lname", lastName);

                if (clientType.Equals("Buyer"))
                    clientType = "B";
                else
                    clientType = "S";
                SqlParameter clientParam = new SqlParameter("clienttype", clientType);

                // Correct phone number
                phoneNumber = phoneNumber.Replace("-", "");
                if (phoneNumber.Count() == 11 && HelperFunctions.StringNumeric(phoneNumber))
                    phoneNumber = phoneNumber.Remove(0, 1);
                else if (HelperFunctions.StringNumeric(phoneNumber) is false) {
                    MessageBox.Show("Invalid Phone Number", "Invalid Fields");
                    return;
                }
                SqlParameter phoneParam = new SqlParameter("phonenumber", phoneNumber);

                // Validate emails
                try {
                    email = new MailAddress(email).Address;
                }
                catch(FormatException) {

                    MessageBox.Show("Invalid Email", "Invalid Fields");
                    return;
                }

                SqlParameter emailParam = new SqlParameter("email", email);
                Object[] parameters = new object[] { fnameParam, lnameParam, clientParam, phoneParam, emailParam };
                var queryResult = context.Database.ExecuteSqlCommand("INSERT INTO client (client_type, first_name, last_name, phone_number, email)" +
                                                                     "VALUES (@clienttype, @fname, @lname, @phonenumber, @email)", parameters);
                // Close Window after submission
                Close();
            }

        }
    }
}
