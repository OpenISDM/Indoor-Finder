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

namespace IndoorFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EditView _editView = new EditView();

        public MainWindow()
        {
            InitializeComponent();
            this.contentControl.Content = _editView;
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = _editView;
        }
    }

    public class Beacon : Image
    {
        public string GUID { get; }
        public string Coordinate_X { get; }
        public string Coordinate_Y { get; }

        public Beacon()
        {
            
        }
    }
}
