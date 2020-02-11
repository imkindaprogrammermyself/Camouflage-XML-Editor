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
        private List<Rectangle> rectDisplays;
        public MainWindow()
        {
            InitializeComponent();
            InitializeControlLists();
            RegisterDisplayEvents();
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
            rectDisplays = new List<Rectangle>
            {
                RectDisplay0,
                RectDisplay1,
                RectDisplay2,
                RectDisplay3,
            };
        }

        private void EnableTabGrids(bool en)
        {
            tabGrids.ForEach(tg => tg.IsEnabled = en);
        }

        private void RegisterDisplayEvents()
        {
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
                loader = new Loader(ofd.FileName);
                ships = new Ships(loader.ShipGroup);
                camos = new Camouflages(loader.Camouflage, loader.ShipGroup);
                schemes = new Schemes(loader.ColorScheme);
                EnableTabGrids(true);
                UpdateCbShips();
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

        private void SetRectanglesFill(List<Rectangle> rectangles, Brush fill)
        {
            rectangles.ForEach(r => r.Fill = fill);
        }

        private void SetRectangleFillColor(Rectangle rectangle, Color color)
        {
            rectangle.Fill = new SolidColorBrush(color);
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
                if (camos.AssociatedColorScheme(kvp.Key).Count() == 0)
                {
                    SetRectanglesFill(rectDefaults, new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
                    SetRectanglesFill(rectCurrents, new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
                    SetRectanglesFill(rectPrevious, new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
                    SetRectanglesFill(rectDisplays, new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)));
                    EnableGrid(false, grids: colorGrids);
                    CbScheme.ItemsSource = null;
                }
                else
                {
                    EnableGrid(true, grids: colorGrids);
                    CbScheme.ItemsSource = camos.AssociatedColorScheme(kvp.Key);
                    CbScheme.SelectedIndex = 0;
                }
            }
        }

        private void CbScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (string)((ComboBox)sender).SelectedItem;
            if (selectedItem != null)
            {
                scheme = schemes.GetScheme(selectedItem);
                SetRectangleFillColor(RectDisplay0, scheme.Black);
                SetRectangleFillColor(RectDisplay1, scheme.Red);
                SetRectangleFillColor(RectDisplay2, scheme.Green);
                SetRectangleFillColor(RectDisplay3, scheme.Blue);
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
    }
}
