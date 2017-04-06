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

// TODO: implement

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
    }
}
