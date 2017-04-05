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

            Int32 result = HelperFunctions.AddNewClient(firstName, lastName, clientType, phoneNumber, email);

            if (result != -1)
                Close();
        }
    }
}
