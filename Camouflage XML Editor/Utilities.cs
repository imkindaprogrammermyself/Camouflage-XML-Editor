using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CamouflageXmlEditor
{
    static class Utilities
    {
        public static LinearGradientBrush GetLinearGradientBrush(Color color0, Color color1, Color color2, Color color3)
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
        public static void SetRectangleFillGradient(Rectangle rectangle, LinearGradientBrush lgb)
        {
            rectangle.Fill = lgb;
        }
        public static void SetRectangleFillColor(List<Rectangle> rectangles, Color color)
        {
            rectangles.ForEach(r => r.Fill = new SolidColorBrush(color));
        }
        public static void SetRectangleFillColor(Rectangle rectangle, Color color)
        {
            rectangle.Fill = new SolidColorBrush(color);
        }
        public static void SetRectangleFillColor(Rectangle[] rectangle, Color[] color)
        {
            foreach (var (item, index) in rectangle.WithIndex())
            {
                item.Fill = new SolidColorBrush(color[index]);
            }
        }
        public static void EnableGrid(bool en, Grid grid)
        {
            grid.IsEnabled = en;
        }
        public static void EnableGrid(bool en, List<Grid> grids)
        {
            grids.ForEach(g => g.IsEnabled = en);
        }
        public static void SetSchemeColor(ColorScheme scheme, string num, Color color)
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
        public static LinearGradientBrush ClearLinearGradientBrush
        {
            get => GetLinearGradientBrush(
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0),
                Color.FromArgb(0, 0, 0, 0)
                );
        }
        public static Color Transparent
        {
            get => Color.FromArgb(0, 0, 0, 0);
        }
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
            => self.Select((item, index) => (item, index));
    }
}
