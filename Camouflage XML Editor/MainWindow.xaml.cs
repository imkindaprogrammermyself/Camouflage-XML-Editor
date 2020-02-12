using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using ColorPickerWPF;
using ColorPickerWPF.Code;

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
                    SetSchemeColor(num, window.picker.Color);
                }
            }
            else if (cmd == "previous")
            {
                dictCurrentsPreviousDefaults[num][0].Fill = dictCurrentsPreviousDefaults[num][1].Fill;
                SetSchemeColor(num, ((SolidColorBrush)dictCurrentsPreviousDefaults[num][0].Fill).Color);

            }
            else if (cmd == "default")
            {
                dictCurrentsPreviousDefaults[num][0].Fill = dictCurrentsPreviousDefaults[num][2].Fill;
                SetSchemeColor(num, ((SolidColorBrush)dictCurrentsPreviousDefaults[num][0].Fill).Color);
            }
            SetRectangleFillGradient(RectSchemeDisplay,
                    GetLinearGradientBrush(
                    ((SolidColorBrush)RectCurrent0.Fill).Color,
                    ((SolidColorBrush)RectCurrent1.Fill).Color,
                    ((SolidColorBrush)RectCurrent2.Fill).Color,
                    ((SolidColorBrush)RectCurrent3.Fill).Color
                    ));
        }

        private void SetSchemeColor(string num, Color color)
        {
            switch (num)
            {
                case "0":
                    {
                        scheme.Black = color;
                        break;
                    }
                case "1":
                    {
                        scheme.Red = color;
                        break;
                    }
                case "2":
                    {
                        scheme.Green = color;
                        break;
                    }
                case "3":
                    {
                        scheme.Blue = color;
                        break;
                    }
                case "ui":
                    {
                        scheme.Ui = color;
                        break;
                    }
            }
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
                loader = new Loader();
                if (loader.Load(ofd.FileName))
                {
                    ships = new Ships(loader.ShipGroup);
                    camos = new Camouflages(loader.Camouflage, loader.ShipGroup);
                    schemes = new Schemes(loader.ColorScheme);
                    EnableTabGrids(true);
                    UpdateCbShips();
                    miSave.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Invalid xml file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ClearAll();
                    miSave.IsEnabled = false;
                }
            }
        }

        private void EnableGrid(bool en, Grid grid = null, List<Grid> grids = null)
        {
            if (grid != null)
            {
                grid.IsEnabled = en;
            }
            else
            {
                if (grids != null)
                {
                    grids.ForEach(g => g.IsEnabled = en);
                }
            }
        }

        private void SetRectanglesFill(List<Rectangle> rectangles, Color color)
        {
            rectangles.ForEach(r => r.Fill = new SolidColorBrush(color));
        }

        private void SetRectangleFillColor(Rectangle rectangle, Color color)
        {
            rectangle.Fill = new SolidColorBrush(color);
        }

        private void SetRectangleFillGradient(Rectangle rectangle, LinearGradientBrush lgb)
        {
            rectangle.Fill = lgb;
        }

        private void SaveCamouflageFile(object sender, RoutedEventArgs e)
        {
            
        }

        private void CloseProgram(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateCbShips()
        {
            CbShip.ItemsSource = ships.Names;
            CbShip.SelectedIndex = 0;
        }

        private void CbShip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (string)((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                SetRectanglesFill(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]).ToList(), Color.FromArgb(0, 0, 0, 0));
                Binding listBinding = new Binding() { Source = camos.AssociatedWith(selectedItem) };
                CbCamouflage.DisplayMemberPath = "Value";
                CbCamouflage.SelectedValuePath = "Key";
                CbCamouflage.SetBinding(ItemsControl.ItemsSourceProperty, listBinding);
                CbCamouflage.SelectedIndex = 0;
            }
        }

        private void CbCamouflage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedItem != null)
            {
                var kvp = (KeyValuePair<int, string>)((ComboBox)sender).SelectedItem;
                if (!camos.AssociatedColorScheme(kvp.Key).Any())
                {
                    EnableGrid(false, grids: colorGrids);
                    CbScheme.ItemsSource = null;
                    CbScheme.IsEnabled = false;
                    ClearColors();
                }
                else
                {
                    EnableGrid(true, grids: colorGrids);
                    CbScheme.ItemsSource = camos.AssociatedColorScheme(kvp.Key);
                    CbScheme.SelectedIndex = 0;
                    CbScheme.IsEnabled = true;
                }
                LbTexture.ItemsSource = camos.AssociatedTextures(kvp.Key);
            }
        }

        private void CbScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (string)((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                SetRectanglesFill(dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]).ToList(), Color.FromArgb(0, 0, 0, 0));
                scheme = schemes.GetScheme(selectedItem);
                SetRectangleFillGradient(RectSchemeDisplay,
                    GetLinearGradientBrush(scheme.Black, scheme.Red, scheme.Green, scheme.Blue));
                SetRectangleFillColor(RectCurrent0, scheme.Black);
                SetRectangleFillColor(RectCurrent1, scheme.Red);
                SetRectangleFillColor(RectCurrent2, scheme.Green);
                SetRectangleFillColor(RectCurrent3, scheme.Blue);
                SetRectangleFillColor(RectDefault0, scheme.DefaultBlack);
                SetRectangleFillColor(RectDefault1, scheme.DefaultRed);
                SetRectangleFillColor(RectDefault2, scheme.DefaultGreen);
                SetRectangleFillColor(RectDefault3, scheme.DefaultBlue);
            }
            if (scheme.Ui != Color.FromArgb(0, 0, 0, 0))
            {
                SetRectangleFillColor(RectCurrentUi, scheme.Ui);
                SetRectangleFillColor(RectDefaultUi, scheme.DefaultUi);
                EnableGrid(true, grid: GridColorUi);
            }
            else
            {
                SetRectangleFillColor(RectCurrentUi, Color.FromArgb(0, 0, 0, 0));
                SetRectangleFillColor(RectDefaultUi, Color.FromArgb(0, 0, 0, 0));
                EnableGrid(false, grid: GridColorUi);
            }
        }

        private void ClearColors()
        {
            var c = dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[0]).ToList();
            var p = dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[1]).ToList();
            var d = dictCurrentsPreviousDefaults.Select(kvp => kvp.Value[2]).ToList();
            SetRectanglesFill(c, Color.FromArgb(0, 0, 0, 0));
            SetRectanglesFill(p, Color.FromArgb(0, 0, 0, 0));
            SetRectanglesFill(d, Color.FromArgb(0, 0, 0, 0));
            SetRectangleFillGradient(RectSchemeDisplay,
            GetLinearGradientBrush(
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0)
                ));
        }

        private void ClearAll()
        {
            cbs.ForEach(cb => cb.ItemsSource = null);
            LbTexture.ItemsSource = null;
            ClearColors();
            EnableTabGrids(false);
        }

        private LinearGradientBrush GetLinearGradientBrush(Color color0, Color color1, Color color2, Color color3)
        {
            var gsc = new GradientStopCollection
            {
                new GradientStop(color0, 0.25),
                new GradientStop(color1, 0.25),
                new GradientStop(color1, 0.50),
                new GradientStop(color2, 0.50),
                new GradientStop(color2, 0.75),
                new GradientStop(color3, 0.75)
            };
            return new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = gsc
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
