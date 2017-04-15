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

// TODO Update list when completed, extract that logic into a method

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for NewEmployee.xaml
    /// </summary>
    public partial class NewEmployee : Window {

        bool isNewEmployee = true;
        EmployeeItem item;

        public NewEmployee(EmployeeItem item, bool isNewEmployee) {

            InitializeComponent();

            this.item = item;
            this.isNewEmployee = isNewEmployee; // true = making a new employee, not editing
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // If we are editing an employee change the title as well as fill in all of the fields
            if (isNewEmployee is false) {

                Title = "Edit Employee";
                // Disable the username field
                usernameField.IsReadOnly = true;
                adminBox.IsEnabled = false;
                agentBox.IsEnabled = false;

                // Fill the fields
                usernameField.Text = item.Username;
                emailField.Text = item.Email;
                firstNameField.Text = item.FirstName;
                lastNameField.Text = item.LastName;
                securityQuestionField.Text = item.securityQuestion;
                securityAnswerField.Text = item.securityAnswer;

                if (item.Type.Equals("S")) {
                    salaryField.Text = item.Salary.ToString();

                    // Disable agent fields
                    phoneNumberField.IsEnabled = false;
                    commissionField.IsEnabled = false;
                    brokerShareField.IsEnabled = false;
                }
                else {
                    phoneNumberField.Text = item.PhoneNumber; // maybe need to pass to helper function
                    commissionField.Text = item.Commission.ToString();
                    if (item.BrokerShare != null)
                        brokerShareField.Text = item.BrokerShare.ToString();

                    // Disable administrator fields
                    salaryField.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Main logic for when the user submits the form, small difference if they are editing an existing employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitForm(object sender, RoutedEventArgs e) {

            string username = usernameField.Text;
            string email = emailField.Text;
            string firstName = firstNameField.Text;
            string lastName = lastNameField.Text;

            string salaryText = salaryField.Text;
            string phoneNumber = phoneNumberField.Text;
            string commissionText = commissionField.Text;
            string brokerShareText = brokerShareField.Text;

            string securityQuestion = securityQuestionField.Text;
            string securityAnswer = securityAnswerField.Text;

            // Check options that every employee needs.
            if (HelperFunctions.NullOrEmpty(username, email, firstName, lastName, 
                securityQuestion, securityAnswer) is true) {
                MessageBox.Show("Fields are left empty!", "Empty Fields");
                return;
            }
            else if ((bool)adminBox.IsChecked && HelperFunctions.NullOrEmpty(salaryText) is true) {
                MessageBox.Show("Fields are left empty!", "Empty Fields");
                return;
            }
            else if ((bool)agentBox.IsChecked && HelperFunctions.NullOrEmpty(phoneNumber, commissionText, brokerShareText) is true) {
                MessageBox.Show("Fields are left empty!", "Empty Fields");
                return;
            }
            else if ((bool)adminBox.IsChecked is false && (bool)agentBox.IsChecked is false) {
                MessageBox.Show("Fields are left empty!", "Empty Fields");
                return;
            }

            // generate a random password
            string randPassword = HelperFunctions.randString(16);

            // First we have to make the employee object and then we are done
            using (var context = new Model()) {

                SqlParameter usernameParam = new SqlParameter("username", username);
                SqlParameter officeIdParam = new SqlParameter("oid", item.OfficeID);
                SqlParameter emailParam = new SqlParameter("email", email);
                SqlParameter passParam = new SqlParameter("pass", randPassword);
                SqlParameter fnameParam = new SqlParameter("fname", firstName);
                SqlParameter lnameParam = new SqlParameter("lname", lastName);
                string type = ((bool)adminBox.IsChecked) ? "S" : "A";
                SqlParameter typeParam = new SqlParameter("type", type);
                SqlParameter questionParam = new SqlParameter("question", securityQuestion);
                SqlParameter answerParam = new SqlParameter("answer", securityAnswer);
                SqlParameter firstLoginParam = new SqlParameter("firstLogin", 1);

                Object[] parameters = { usernameParam, officeIdParam, passParam, emailParam, fnameParam, lnameParam,
                                        typeParam, questionParam, answerParam, firstLoginParam};

                try {

                    if (isNewEmployee) {
                        context.Database.ExecuteSqlCommand("INSERT INTO Employee (username, office_id, password, email, first_name, last_name, employee_type, " +
                                                            "security_question, security_answer, first_login) VALUES (@username, @oid, @pass, @email, @fname, @lname, " +
                                                            "@type, @question, @answer, @firstLogin)", parameters);
                        randPasswordField.Text = randPassword;
                    }
                    else {
                        context.Database.ExecuteSqlCommand("UPDATE Employee SET email = @email, first_name = @fname, last_name = @lname, " +
                                                            "security_question = @question, security_answer = @answer WHERE username = @username AND office_id = @oid", parameters);
                    }
                }
                catch (Exception ex) {

                    // When editing, the username is not allowed to be changed and an update would be used.
                    MessageBox.Show("Username already defined for that office", "Incorrect Fields");
                    return;
                }

            }

            // The only database error that can happen at this point is a duplicate username/officeid
            // Next we need to make the record in the agent or administrator table
            if ((bool)adminBox.IsChecked) {

                using (var context = new Model()) {

                    decimal salary;
                    bool parseAttempt = Decimal.TryParse(salaryText, out salary);

                    if (parseAttempt is false || salary < 0) {
                        MessageBox.Show("Entered an Incorrect Salary", "Incorrect Fields");
                        return;
                    }

                    SqlParameter usernameParam = new SqlParameter("username", username);
                    SqlParameter officeIdParam = new SqlParameter("oid", item.OfficeID);
                    SqlParameter salaryParam = new SqlParameter("salary", salary);
                    Object[] parameters = { usernameParam, officeIdParam, salaryParam };

                    if (isNewEmployee) {
                        context.Database.ExecuteSqlCommand("INSERT INTO Administrator VALUES (@username, @oid, @salary)", parameters);
                    }
                    else {
                        context.Database.ExecuteSqlCommand("UPDATE Administrator SET salary = @salary WHERE employee_username = @username AND employee_office_id = @oid", parameters);
                    }

                    Close();
                }
            }

            else {
                using (var context = new Model()) {

                    float commission;
                    float brokerShare;
                    bool parseAttemptOne = float.TryParse(commissionText, out commission);
                    bool parseAttemptTwo = float.TryParse(brokerShareText, out brokerShare);

                    if (parseAttemptOne is false || parseAttemptTwo is false || commission < 0 || brokerShare < 0 || commission > 100 || brokerShare > 100 ||
                        phoneNumber.Length > 10 || HelperFunctions.StringNumeric(phoneNumber) is false) {
                        MessageBox.Show("Agent related fields are incorrect", "Incorrect Fields");
                        return;
                    }

                    SqlParameter usernameParam = new SqlParameter("username", username);
                    SqlParameter officeIdParam = new SqlParameter("oid", item.OfficeID);
                    SqlParameter phoneParam = new SqlParameter("phone", HelperFunctions.PhoneNumberToString(phoneNumber));
                    SqlParameter balanceParam = new SqlParameter("bal", 0);
                    SqlParameter commissionParam = new SqlParameter("commission", commission);
                    SqlParameter shareParam = new SqlParameter("share", brokerShare);
                    Object[] parameters = { usernameParam, officeIdParam, phoneParam, balanceParam, commissionParam, shareParam };

                    if (isNewEmployee) {
                        context.Database.ExecuteSqlCommand("INSERT INTO Agent VALUES (@username, @oid, @phone, @bal, @comission, @share)", parameters);
                    }
                    else {
                        context.Database.ExecuteSqlCommand("UPDATE Agent SET phone_number = @phone, commission_percentage = @commission, broker_share = @share" +
                                                            "WHERE employee_username = @username AND employee_office_id = @oid", parameters);
                    }
                    Close();
                }
            }

            
        }

        private void adminBox_Checked(object sender, RoutedEventArgs e) {
            
            // Enable administrator fields
            salaryField.IsEnabled = true;

            // Disable agent fields
            phoneNumberField.IsEnabled = false;
            commissionField.IsEnabled = false;
            brokerShareField.IsEnabled = false;

            clearSpecificFields();
            
        }

        private void agentBox_Checked(object sender, RoutedEventArgs e) {
            
            // Enable agent fields
            phoneNumberField.IsEnabled = true;
            commissionField.IsEnabled = true;
            brokerShareField.IsEnabled = true;

            // Disable administrator fields
            salaryField.IsEnabled = false;

            clearSpecificFields();
        }

        private void clearSpecificFields() {

            salaryField.Clear();

            phoneNumberField.Clear();
            commissionField.Clear();
            brokerShareField.Clear();
        }

        
    }
}
