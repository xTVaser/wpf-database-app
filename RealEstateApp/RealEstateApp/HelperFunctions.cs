using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RealEstateApp.EntityModels;
using System.Data.SqlClient;
using System.Windows;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace RealEstateApp {

    public static class HelperFunctions {

        public static string AddressToString(StreetAddress addr) {

            return addr.address_num.ToString() + " " 
                + addr.street + " " 
                + addr.street_type + ", " 
                + addr.city + ", " 
                + addr.province_short + ", " 
                + addr.postal_code;
        }

        public static string PhoneNumberToString(string number) {

            string firstSegment = number.Substring(0, 3);
            string secondSegment = number.Substring(3, 3);
            string thirdSegment = number.Substring(6, 4);

            return "(" + firstSegment + ")-" + secondSegment + "-" + thirdSegment;
        }

        public static Boolean NullOrEmpty(params string[] strings) {

            // Check if any of the strings is null or empty
            return strings.Any(s => s == null || s.Equals(""));
        }

        public static Boolean IntegersBelowZero(params int[] ints) {

            // Check if any ints are below zero
            return ints.Any(i => i <= 0);
        }

        public static Boolean StringNumeric(string s) {

            foreach (char c in s) {

                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        public static Boolean CheckPostalCode(string s) {

            // Get rid of empty space
            s = s.Replace(" ", "");

            var matches = Regex.Match(s, @"^[ABCEGHJKLMNPRSTVXYabceghjklmnprstvxy]{1}\d{1}[A-Za-z]{1}\d{1}[A-Za-z]{1}\d{1}$");
            return matches.Success;
        }

        public static Int32 AddNewClient(string fname, string lname, string clientType, string phoneNumber, string email) {

            // If any of the fields are empty, then stop
            if (HelperFunctions.NullOrEmpty(fname, lname, clientType, phoneNumber, email)) {

                MessageBox.Show("Please fill out the remaining fields", "Empty Fields");
                return -1;
            }

            using (var context = new Model()) {

                SqlParameter fnameParam = new SqlParameter("fname", fname);
                SqlParameter lnameParam = new SqlParameter("lname", lname);

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
                    return -1;
                }
                SqlParameter phoneParam = new SqlParameter("phonenumber", phoneNumber);

                // Validate emails
                try {
                    email = new MailAddress(email).Address;
                }
                catch (FormatException) {

                    MessageBox.Show("Invalid Email", "Invalid Fields");
                    return -1;
                }

                SqlParameter emailParam = new SqlParameter("email", email);
                Object[] parameters = new object[] { fnameParam, lnameParam, clientParam, phoneParam, emailParam };
                var queryResult = context.Database.ExecuteSqlCommand("INSERT INTO client (client_type, first_name, last_name, phone_number, email)" +
                                                                       "VALUES (@clienttype, @fname, @lname, @phonenumber, @email)", parameters);

                var result = context.Clients.SqlQuery("SELECT * FROM client WHERE ID = IDENT_CURRENT('client')").FirstOrDefault<Client>();
                
                return result.id;
            }
        }
    }
}
