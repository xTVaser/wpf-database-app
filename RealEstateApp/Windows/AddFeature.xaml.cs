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
    /// Interaction logic for AddFeature.xaml
    /// </summary>
    public partial class AddFeature : Window {

        private int listingId;
        private ListView featureList;

        public AddFeature(int listingId, ListView featureList) {

            InitializeComponent();
            this.listingId = listingId;
            this.featureList = featureList;
        }

        private void SubmitFeature(object sender, RoutedEventArgs e) {

            // Get the items from the form
            string header = headerField.Text;
            string description = descriptionField.Text;

            if (HelperFunctions.NullOrEmpty(header, description)) {
                MessageBox.Show("Fields are empty, fill them in and try again", "Empty Fields");
                return;
            }

            using (var context = new Model()) {

                SqlParameter idParam = new SqlParameter("id", listingId);
                SqlParameter headerParam = new SqlParameter("header", header);
                SqlParameter descriptionParam = new SqlParameter("description", description);

                Object[] parameters = { idParam, headerParam, descriptionParam };

                context.Database.ExecuteSqlCommand("INSERT INTO Feature (listing_id, heading, body) VALUES (@id, @header, @description)", parameters);
            }

            FeatureItem newItem = new FeatureItem(header, description);
            featureList.Items.Add(newItem);
            Close();
        }
    }
}
