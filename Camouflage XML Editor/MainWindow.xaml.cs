using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CamouflageXmlEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Loader loader;
        private Ships ships;
        private Camouflages camos;
        private Schemes schemes;
        private ColorScheme scheme;
        private List<Grid> colorGrids;
        private List<ComboBox> cbs;
        private Dictionary<string, Rectangle[]> dictCurrentsPreviousDefaults;
        public MainWindow()
        {
            InitializeComponent();
            InitializeControlLists();
            EnableTabGrids(false);
            RegisterButtonEvents();
            RegisterSourceChangeEvent();
        }

        private void InitializeControlLists()
        {
            colorGrids = new List<Grid>
            {
                GridColor0,
                GridColor1,
                GridColor2,
                GridColor3,
                GridColorUi
            };
            cbs = new List<ComboBox>
            {
                CbShip,
                CbCamouflage,
                CbScheme
            };
            dictCurrentsPreviousDefaults = new Dictionary<string, Rectangle[]>
            {
                { "0", new Rectangle[]{RectCurrent0,RectPrevious0,RectDefault0} },
                { "1", new Rectangle[]{RectCurrent1,RectPrevious1,RectDefault1} },
                { "2", new Rectangle[]{RectCurrent2,RectPrevious2,RectDefault2} },
                { "3", new Rectangle[]{RectCurrent3,RectPrevious3,RectDefault3} },
                { "ui", new Rectangle[]{RectCurrentUi,RectPreviousUi,RectDefaultUi} }
            };
        }

        private void AutoSelectZero(object sender, EventArgs e)
        {
            var s = (ComboBox)sender;
            s.SelectedIndex = 0;
        }

        private void RegisterSourceChangeEvent()
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ComboBox));
            if (dpd != null)
            {
                dpd.AddValueChanged(CbShip, AutoSelectZero);
                dpd.AddValueChanged(CbCamouflage, AutoSelectZero);
                dpd.AddValueChanged(CbScheme, AutoSelectZero);
            }
        }

        private void ButtonEventHandler(object sender, RoutedEventArgs e)
        {
            string[] tag = ((Button)sender).Tag.ToString().Split('_');
            string cmd = tag[0];
            string num = tag[1];
            if (cmd == "current")
            {
                var a = ((SolidColorBrush)dictCurrentsPreviousDefaults[num][0].Fill).Color;
                ColorPickerWindow window = new ColorPickerWindow();
                window.picker.SetColor(a);
                if (window.ShowDialog() == true)
                {
                    dictCurrentsPreviousDefaults[num][1].Fill = new SolidColorBrush(a);
                    dictCurrentsPreviousDefaults[num][0].Fill = new SolidColorBrush(window.picker.Color);
                    Utilities.SetSchemeColor(scheme, num, window.picker.Color);
                }
            }
            else if (cmd == "previous")
            {
                dictCurrentsPreviousDefaults[num][0].Fill = dictCurrentsPreviousDefaults[num][1].Fill;
                Utilities.SetSchemeColor(scheme, num, ((SolidColorBrush)dictCurrentsPreviousDefaults[num][0].Fill).Color);

            }
            else if (cmd == "default")
            {
                dictCurrentsPreviousDefaults[num][0].Fill = dictCurrentsPreviousDefaults[num][2].Fill;
                Utilities.SetSchemeColor(scheme, num, ((SolidColorBrush)dictCurrentsPreviousDefaults[num][0].Fill).Color);
            }
            Utilities.SetRectangleFillGradient(RectSchemeDisplay,
                    Utilities.GetLinearGradientBrush(
                    ((SolidColorBrush)RectCurrent0.Fill).Color,
                    ((SolidColorBrush)RectCurrent1.Fill).Color,
                    ((SolidColorBrush)RectCurrent2.Fill).Color,
                    ((SolidColorBrush)RectCurrent3.Fill).Color
                    ));
        }
        private void RegisterButtonEvents()
        {
            colorGrids.ForEach(cg =>
            {
                cg.Children.OfType<Button>().ToList().ForEach(btn => btn.Click += ButtonEventHandler);
            });
        }

        private void EnableTabGrids(bool en)
        {
            colorGrids.ForEach(tg => tg.IsEnabled = en);
            GridCamouflage.IsEnabled = en;
        }

        private void OpenCamouflageFile(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "camouflages | *.xml"
            };
            if (ofd.ShowDialog() == true)
            {
                ClearAll();
                loader = new Loader();
                if (loader.Load(ofd.FileName))
                {
                    ships = new Ships(loader.ShipGroup);
                    camos = new Camouflages(loader.Camouflage, loader.ShipGroup);
                    schemes = new Schemes(loader.ColorScheme);
                    EnableTabGrids(true);
                    CbShip.ItemsSource = ships.Names;
                    miSave.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Invalid xml file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    miSave.IsEnabled = false;
                }
            }
        }

        private void SaveCamouflageFile(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "camouflages | *.xml",
                FileName = "camouflages.xml"
            };
            if (sfd.ShowDialog() == true)
            {

                if (loader.Save(sfd.FileName))
                {
                    MessageBox.Show("File saved.", "File saved.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error saving the file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CbShip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (string)((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                Utilities.SetRectangleFillColor(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]).ToList(), Color.FromArgb(0, 0, 0, 0));
                Binding listBinding = new Binding() { Source = camos.AssociatedWith(selectedItem) };
                CbCamouflage.DisplayMemberPath = "Value.Name";
                CbCamouflage.SelectedValuePath = "Key";
                CbCamouflage.SetBinding(ItemsControl.ItemsSourceProperty, listBinding);
            }
        }

        private void CbCamouflage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem != null)
            {
                var kvp = (KeyValuePair<int, Camouflage>)((ComboBox)sender).SelectedItem;
                if (!camos.HasColorScheme(kvp.Key))
                {
                    Utilities.EnableGrid(false, colorGrids);
                    CbScheme.ItemsSource = null;
                    CbScheme.IsEnabled = false;
                    ClearColors();
                }
                else
                {
                    Utilities.EnableGrid(true, colorGrids);
                    var s = schemes.GetAssociatedSchemeKeyed(camos.AssociatedColorScheme(kvp.Key));
                    var listBinding = new Binding() { Source = s };
                    CbScheme.DisplayMemberPath = "Value.Name";
                    CbScheme.SelectedValuePath = "Key";
                    CbScheme.SetBinding(ItemsControl.ItemsSourceProperty, listBinding);
                    CbScheme.IsEnabled = true;
                }
                LbTexture.ItemsSource = camos.AssociatedTextures(kvp.Key);
            }
        }

        private void CbScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = ((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                scheme = ((KeyValuePair<int, ColorScheme>)((ComboBox)sender).SelectedItem).Value;
                Utilities.SetRectangleFillColor(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]).ToList(), Color.FromArgb(0, 0, 0, 0));
                Utilities.SetRectangleFillGradient(RectSchemeDisplay,
                    Utilities.GetLinearGradientBrush(scheme.Black, scheme.Red, scheme.Green, scheme.Blue));
                Utilities.SetRectangleFillColor(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[0]).ToArray(), scheme.AllColors);
                Utilities.SetRectangleFillColor(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[2]).ToArray(), scheme.AllDefaultColors);
                if (scheme.Ui != Utilities.Transparent)
                {
                    Utilities.EnableGrid(true, GridColorUi);
                }
                else
                {
                    Utilities.SetRectangleFillColor(RectCurrentUi, Utilities.Transparent);
                    Utilities.SetRectangleFillColor(RectDefaultUi, Utilities.Transparent);
                    Utilities.EnableGrid(false, GridColorUi);
                }
            }
        }

        private void ClearColors()
        {
            var c = dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[0])
                .Concat(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]))
                .Concat(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[2]))
                .ToList();
            
            Utilities.SetRectangleFillColor(c, Utilities.Transparent);
            Utilities.SetRectangleFillGradient(RectSchemeDisplay, Utilities.ClearLinearGradientBrush);
        }

        private void ClearAll()
        {
            cbs.ForEach(cb => cb.ItemsSource = null);
            LbTexture.ItemsSource = null;
            ClearColors();
            EnableTabGrids(false);
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }
    }
}
