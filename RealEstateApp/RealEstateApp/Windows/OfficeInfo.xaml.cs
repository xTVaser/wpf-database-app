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
    /// Interaction logic for OfficeInfo.xaml
    /// </summary>
    public partial class OfficeInfo : Window {

        private OfficeItem item;

        public OfficeInfo(OfficeItem item) {

            InitializeComponent();
            this.item = item;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            // Set address field
            officeAddress.Content = item.Address;

            // Set office contact information
            officePhoneNumber.Content = item.PhoneNumber;
            officeFaxNumber.Content = item.FaxNumber;
            officeEmail.Content = item.Email;

            // Set broker information and employee listing
            using (var context = new Model()) {

                // Broker information
                SqlParameter officeIDParam = new SqlParameter("id", item.ID);
                SqlParameter brokerNameParam = new SqlParameter("broker", item.BrokerUsername);
                Object[] parameters = new object[] { officeIDParam, brokerNameParam };

                Employee broker = context.Employees.SqlQuery("SELECT * FROM Employee WHERE username = @broker AND office_id = @id", parameters).FirstOrDefault<Employee>();

                brokerName.Content = broker.first_name + " " + broker.last_name;
                brokerEmail.Content = broker.email;

                // employee listing
                var employees = context.Employees.SqlQuery("SELECT * FROM Employee WHERE office_id = @id", new SqlParameter("id", item.ID)).ToList<Employee>();

                foreach (Employee employee in employees) {

                    EmployeeItem newItem = new EmployeeItem(employee.employee_type);
                    newItem.FirstName = employee.first_name;
                    newItem.LastName = employee.last_name;
                    newItem.Email = employee.email;

                    employeeListView.Items.Add(newItem);
                }
            }
        }
    }
}
