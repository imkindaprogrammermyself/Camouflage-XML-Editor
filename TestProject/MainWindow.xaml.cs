using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TestProject
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
        private List<Grid> tabGrids;
        private List<Grid> colorGrids;
        private List<Rectangle> rectCurrents;
        private List<Rectangle> rectPrevious;
        private List<Rectangle> rectDefaults;
        private List<ComboBox> cbs;
        public MainWindow()
        {
            InitializeComponent();
            InitializeControlLists();
            EnableTabGrids(false);
        }

        private void InitializeControlLists()
        {
            tabGrids = new List<Grid>
            {
                GridCamouflage,
                GridColor0,
                GridColor1,
                GridColor2,
                GridColor3,
                GridColorUi
            };
            colorGrids = new List<Grid>
            {
                GridColor0,
                GridColor1,
                GridColor2,
                GridColor3,
                GridColorUi
            };
            rectCurrents = new List<Rectangle>
            {
                RectCurrent0,
                RectCurrent1,
                RectCurrent2,
                RectCurrent3,
                RectCurrentUi
            };
            rectPrevious = new List<Rectangle>
            {
                RectPrevious0,
                RectPrevious1,
                RectPrevious2,
                RectPrevious3,
                RectPreviousUi
            };
            rectDefaults = new List<Rectangle>
            {
                RectDefault0,
                RectDefault1,
                RectDefault2,
                RectDefault3,
                RectDefaultUi
            };
            cbs = new List<ComboBox>
            {
                CbShip,
                CbCamouflage,
                CbScheme
            };
        }

        private void EnableTabGrids(bool en)
        {
            tabGrids.ForEach(tg => tg.IsEnabled = en);
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

        private void ClearAll()
        {
            cbs.ForEach(cb => cb.ItemsSource = null);
            LbTexture.ItemsSource = null;
            SetRectanglesFill(rectCurrents, Color.FromArgb(0, 0, 0, 0));
            SetRectanglesFill(rectPrevious, Color.FromArgb(0, 0, 0, 0));
            SetRectanglesFill(rectDefaults, Color.FromArgb(0, 0, 0, 0));
            SetRectangleFillGradient(RectSchemeDisplay,
                GetLinearGradientBrush(Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0)));
            EnableTabGrids(false);
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
            EnableGrid(false, GridCamouflage);
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
                    SetRectanglesFill(rectDefaults, Color.FromArgb(0, 0, 0, 0));
                    SetRectanglesFill(rectCurrents, Color.FromArgb(0, 0, 0, 0));
                    SetRectanglesFill(rectPrevious, Color.FromArgb(0, 0, 0, 0));
                    EnableGrid(false, grids: colorGrids);
                    CbScheme.ItemsSource = null;
                    CbScheme.IsEnabled = false;
                    SetRectangleFillGradient(RectSchemeDisplay,
                    GetLinearGradientBrush(Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0),
                    Color.FromArgb(0, 0, 0, 0)));
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
    }
}
