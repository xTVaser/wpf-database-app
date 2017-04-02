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
using System.Windows.Navigation;
using System.Windows.Shapes;

using RealEstateApp.EntityModels;
using System.Data.SqlClient;

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// When window is loaded, populate the office ids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomePage_Loaded(object sender, RoutedEventArgs e) {

            using (var context = new Model()) {
                
                // Query the DB
                var queryResults = context.Database.SqlQuery<int>("SELECT id from office").ToList<int>();

                // Take all of the IDs and convert them to strings
                List<string> idList = new List<string>();
                foreach (int i in queryResults)
                    idList.Add(i.ToString());

                if (idList.Count == 0)
                    idList.Add("No Offices Found");

                // Display in combobox
                officeList.ItemsSource = idList;
                officeList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Login to the system, test credentials at the moment they are stored in plaintext
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login(object sender, RoutedEventArgs e) {

            // Get the fields
            string officeIDText = officeList.SelectedValue.ToString();
            string username = usernameField.Text;
            string password = passwordField.Password;

            // Check that fields are filled out
            if (officeIDText.Equals("No Offices Found") || officeIDText == null || officeIDText.Equals("") ||
                usernameField.Text.Equals("") || passwordField.Password.Equals("")) {
                MessageBox.Show("Please fill out the remaining fields", "Empty Fields");
                return;
            }

            int officeID = int.Parse(officeIDText);
            Employee authedUser = null;

            // Check credentials
            using (var context = new Model()) {

                // Get a single query
                SqlParameter usernameParam = new SqlParameter("username", username);
                SqlParameter officeIdParam = new SqlParameter("id", officeID);
                Object[] parameters = new object[] { usernameParam, officeIdParam };
                var queryResult = context.employees.SqlQuery("SELECT * FROM employee WHERE username = @username AND office_id = @id", parameters).FirstOrDefault<Employee>();

                // If no result, then there is no user with those credentials
                if (queryResult == null)
                    MessageBox.Show("User not found", "Failed Login");

                // If only the password was wrong, ask if they'd like to change password
                else if (!queryResult.password.Equals(password)) {
                    if (MessageBox.Show("Incorrect password, would you like to change it?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                        ForgotPassword newWindow = new ForgotPassword(queryResult, "Forgot Password");
                        newWindow.Show();
                    }
                    else {
                        // Do nothing
                    }
                }
                // Otherwise, valid login, continue
                else
                    authedUser = queryResult;
            }

            // Clear fields

            // If we have a signed in user, then open the next window
            if (authedUser != null) {

                // If this is the first time signing in, then display the password reset field
                if (authedUser.first_login is true) {

                    ForgotPassword newWindow = new ForgotPassword(authedUser, "Change Default Password");
                    newWindow.Show();
                }
                else {
                    Dashboard newWindow = new Dashboard(authedUser);
                    newWindow.Show();
                }
            }
        }
    }
}
