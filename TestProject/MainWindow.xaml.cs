using Microsoft.Win32;
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

namespace TestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            GridColorMenu.Children.OfType<Rectangle>().ToList().ForEach(rec =>
            {
                rec.MouseLeftButtonUp += delegate
                {
                    tabControlMenu.SelectedIndex = int.Parse(rec.Tag.ToString());
                };
            });
        }

        private void OpenCamouflageFile(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Camouflage | camouflages.xml"
            };
            if (ofd.ShowDialog() == true)
            {
                Console.WriteLine(ofd.FileName);
            }
        }

        private void SaveCamouflageFile(object sender, RoutedEventArgs e)
        {

        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
