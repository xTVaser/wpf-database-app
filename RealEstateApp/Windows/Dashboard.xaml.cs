﻿using System;
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
using System.ComponentModel;

namespace RealEstateApp {

    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window {

        private Employee user;
        private Agent agent;
        private string accountType;
        private bool isBroker;
        private Int32? agentID;

        public Dashboard(Employee user, bool isBroker, Int32 agentID) {

            InitializeComponent();
            this.user = user;
            this.isBroker = isBroker;
            this.agentID = agentID;

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
                newEmployeeBtn.Visibility = Visibility.Collapsed;
            }

            // Else, agent
            else {

                // Get agent object
                using (var context = new Model()) {

                    agent = context.Agents.SqlQuery("SELECT * FROM Agent WHERE employee_username = @username AND employee_office_id = @oid",
                        new SqlParameter("username", user.username),
                        new SqlParameter("oid", user.office_id)).FirstOrDefault<Agent>();
                }

                // If not the broker, dont need to manage employees
                if (isBroker is false) {
                    employeeTab.Visibility = Visibility.Collapsed;
                    newEmployeeBtn.Visibility = Visibility.Collapsed;
                }
                else
                    FillEmployeeTab(employeeGridView);

                // Both agent types can make listings and view their balances
                newListingBtn.Visibility = Visibility.Collapsed;
                FillBalanceTab();
            }
            
            // Everyone can see these tabs and content within
            FillListingTab(listingGridView);
            FillOfficeTab(officeGridView);
            FillClientTab(clientGridView);

            // Set the status text at the top
            accountStatus.Content += user.first_name + " " + user.last_name;
            accessStatus.Content += accountType;
        }

        /// <summary>
        /// Populates the listing tab with ListingItems
        /// </summary>
        /// <param name="list">The listview that contains the items</param>
        private void FillListingTab(ListView list) {
            
            // Fill tab with all listings
            using ( var context = new Model()) {

                var queryResult = context.Listings.SqlQuery("SELECT * FROM Listing").ToList();

                foreach (Listing l in queryResult) {

                    ListingItem newItem = new ListingItem(l.year_built, l.date_listed);
                    newItem.id = l.id;
                    newItem.Address = HelperFunctions.AddressToString(l.StreetAddress);
                    newItem.Bedrooms = l.num_bedrooms;
                    newItem.Bathrooms = l.num_bathrooms;
                    newItem.Stories = l.num_stories;
                    newItem.AskingPriceDecimal = l.asking_price;

                    newItem.originalItem = l;

                    list.Items.Add(newItem);
                }
            }
        }

        private void newListingBtn_Click(object sender, RoutedEventArgs e) {

            // Open new listing window
            NewListing newListingWindow = new NewListing(listingGridView);
            newListingWindow.Show();
        }

        /// <summary>
        /// Will open a window with additional information on the listing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as ListingItem;
            ListingInfo newWindow = new ListingInfo(item, listingGridView, user);
            newWindow.Show();
        }

        /// <summary>
        /// Populates the office tab with OfficeItems
        /// </summary>
        /// <param name="list">The listview that contains the items</param>
        private void FillOfficeTab(ListView list) {

            // Fill tab with all offices information
            using (var context = new Model()) {

                var queryResult = context.Offices.SqlQuery("SELECT * FROM Office").ToList();

                foreach (Office o in queryResult) {

                    OfficeItem newItem = new OfficeItem();
                    newItem.ID = o.id;
                    newItem.Address = HelperFunctions.AddressToString(o.StreetAddress);
                    newItem.PhoneNumber = HelperFunctions.PhoneNumberToString(o.phone_number);
                    newItem.FaxNumber = HelperFunctions.PhoneNumberToString(o.fax_number);
                    newItem.Email = o.email;

                    newItem.BrokerUsername = o.broker_username;

                    list.Items.Add(newItem);
                }
            }
        }

        private void officeGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as OfficeItem;
            OfficeInfo newWindow = new OfficeInfo(item);
            newWindow.Show();
        }
        
        private void FillClientTab(ListView list) {

            // Fill tab with all offices information
            using (var context = new Model()) {

                List<Client> queryResult = new List<Client>();

                // If the user is not an administrator then only display their clients
                if (agentID != -1) {
                    SqlParameter idParam = new SqlParameter("id", agentID);
                    queryResult = context.Clients.SqlQuery("SELECT * FROM Client WHERE assigned_agent = @id", idParam).ToList<Client>();
                }
                else
                    queryResult = context.Clients.SqlQuery("SELECT * FROM Client").ToList();

                foreach (Client c in queryResult) {

                    ClientItem newItem = new ClientItem(c.client_type);
                    newItem.FirstName = c.first_name;
                    newItem.LastName = c.last_name;
                    newItem.PhoneNumber = HelperFunctions.PhoneNumberToString(c.phone_number);
                    newItem.Email = c.email;

                    newItem.id = c.id;
                    newItem.agentId = c.assigned_agent;

                    list.Items.Add(newItem);
                }
            }
        }

        private void newClientBtn_Click(object sender, RoutedEventArgs e) {
            
            // Open client window
            AddClient addClientWindow = new AddClient(clientGridView, agent);
            addClientWindow.Show();
        }

        private void clientGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as ClientItem;
            ClientInfo newWindow = new ClientInfo(item);
            newWindow.Show();
        }

        private void FillEmployeeTab(ListView list) {

            // Get all of the employees and fill the list
            using (var context = new Model()) {

                // Only get the employees under the current brokers authority
                SqlParameter idParam = new SqlParameter("id", user.office_id);

                // employee listing
                var employees = context.Employees.SqlQuery("SELECT * FROM Employee WHERE office_id = @id", idParam).ToList<Employee>();

                foreach (Employee employee in employees) {

                    if (employee.fired != null || employee.username.Equals(user.username))
                        continue;

                    EmployeeItem newItem = new EmployeeItem(employee.employee_type);
                    newItem.FirstName = employee.first_name;
                    newItem.LastName = employee.last_name;
                    newItem.Email = employee.email;
                    newItem.Username = employee.username;
                    newItem.OfficeID = employee.office_id;
                    newItem.securityQuestion = employee.security_question;
                    newItem.securityAnswer = employee.security_answer;

                    if (employee.Administrator != null || employee.employee_type.Equals("S"))
                        newItem.Salary = employee.Administrator.salary;
                    else if (employee.Agent != null) {
                        newItem.PhoneNumber = employee.Agent.phone_number;
                        newItem.Commission = employee.Agent.commission_percentage;
                        newItem.BrokerShare = employee.Agent.broker_share;
                    }

                    list.Items.Add(newItem);
                }
            }
        }

        /// <summary>
        /// Populates balance tab with current balance, commission and broker share stats, and a history of all commissions paid in
        /// </summary>
        private void FillBalanceTab() {

            // Set the simple label fields
            currentBalanceField.Content += agent.commission_balance.ToString("#.00");
            commissionShareField.Content += agent.commission_percentage + "%";
            brokerShareField.Content += agent.broker_share + "%";

            // Populate the commissions
            using (var context = new Model()) {

                var commissions = context.Commissions.SqlQuery("SELECT * FROM Commission WHERE payable_to = @id", new SqlParameter("id", agent.id)).ToList<Commission>();

                foreach (Commission c in commissions) {

                    CommissionItem newItem = new CommissionItem();
                    newItem.Amount = c.amount;
                    newItem.Reason = c.reason;
                    newItem.Date = c.date_payed_out;

                    commissionGridView.Items.Add(newItem);
                }
            }
        }

        /// <summary>
        /// Editing the employee involves just opening the same dialog for creating a new employee, but it is already populated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_editEmployee_Click(object sender, RoutedEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as EmployeeItem;
            NewEmployee newWindow = new NewEmployee(item, false, employeeGridView); // false = we are editing an employee, not creating a new one
            newWindow.Show();
        }

        /// <summary>
        /// Firing an employee just opens a dialog box that depending on the response will perform the cascading deletion
        /// If it is an agent, we do not delete the account, just turn it inactive, otherwise if it is an admin, just delete it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_fireEmployee_Click(object sender, RoutedEventArgs e) {

            var item = ((FrameworkElement)e.OriginalSource).DataContext as EmployeeItem;

            // Confirm the firing of an employee
            if (MessageBox.Show("Are you sure you want to fire this employee?", "Removal Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                using (var context = new Model()) {

                    // An administrator can just simple be removed from the system
                    if (item.Type.Equals("S")) {

                        context.Database.ExecuteSqlCommand("DELETE FROM Employee WHERE username = @username AND office_id = @id",
                            new SqlParameter("username", item.Username),
                            new SqlParameter("id", item.OfficeID));
                    }
                    // Else it's an agent, we cant delete it because monetary records are involved so we will just make the agent unable to login
                    else {

                        context.Database.ExecuteSqlCommand("UPDATE Employee SET fired = 1 WHERE username = @username AND office_id = @id",
                            new SqlParameter("username", item.Username),
                            new SqlParameter("id", item.OfficeID));
                    }

                    // In any event, update the employee list
                    employeeGridView.Items.Remove(item);
                    ICollectionView view = CollectionViewSource.GetDefaultView(employeeGridView.Items);
                    view.Refresh();
                }
            }
        }

        /// <summary>
        /// When the user clicks on the create new employee window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newEmployeeBtn_Click(object sender, RoutedEventArgs e) {

            EmployeeItem newItem = new EmployeeItem(user.employee_type);
            newItem.FirstName = user.first_name;
            newItem.LastName = user.last_name;
            newItem.Email = user.email;
            newItem.Username = user.username;
            newItem.OfficeID = user.office_id;

            NewEmployee newWindow = new NewEmployee(newItem, true, employeeGridView);
            newWindow.Show();
        }
    }
}
