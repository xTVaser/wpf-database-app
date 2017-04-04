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
    /// Interaction logic for ForgotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window {

        private Employee user;
        private string windowTitle;

        /// <summary>
        /// Constructs Forgot Password window
        /// </summary>
        /// <param name="user">The information on the user that attempted to login or needs to change their password from the default</param>
        /// <param name="windowTitle">Window title that can be changed depending on context</param>
        public ForgotPassword(Employee user, String windowTitle) {

            InitializeComponent();
            this.user = user;
            this.windowTitle = windowTitle;
        }

        /// <summary>
        /// Events for when the forgot password window is completely loaded up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Set window title as well as the security question
            this.Title = windowTitle;
            securityQuestion.Content = user.security_question + "?";
        }

        /// <summary>
        /// Handler for changing the actual password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePassword(object sender, RoutedEventArgs e) {

            // Get the fields
            string newPasswordText = newPassword.Password;
            string verifyPasswordText = verifyPassword.Password;
            string securityAnswerText = securityAnswer.Text;

            string outputMessage = "";

            // Testing conditions
            if (newPasswordText.Equals(verifyPasswordText) is false)
                outputMessage += "Password's do not match.\n";
            if (securityAnswerText.ToLower().Equals(user.security_answer.ToLower()) is false)
                outputMessage += "Incorrect security answer.\n";
            if (newPasswordText.Equals("") || verifyPasswordText.Equals("") || securityAnswerText.Equals(""))
                outputMessage += "Fields are empty.\n";

            // If any problem, stop
            if (outputMessage.Equals("") is false) {
                MessageBox.Show(outputMessage, "Error");

                ClearFields();
                return;
            }

            // Otherwise change the password
            using (var context = new Model()) {

                SqlParameter newpass = new SqlParameter("newpass", newPasswordText);
                SqlParameter username = new SqlParameter("username", user.username);
                SqlParameter oid = new SqlParameter("oid", user.office_id);

                Object[] parameters = new object[] { newpass, username, oid };

                context.Database.ExecuteSqlCommand("UPDATE employee SET password = @newpass, first_login = 0 WHERE username = @username AND office_id = @oid", parameters);
                MessageBox.Show("Password changed successfully, relogin with your new credentials", "Success");
            }
            this.Close();
        }

        private void ClearFields() {

            newPassword.Clear();
            verifyPassword.Clear();
            securityAnswer.Clear();
        }
    }
}
