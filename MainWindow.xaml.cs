using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const int gridSize = 30;

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAxes();
            TransormCoordinateSystem();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawAxes();
            TransormCoordinateSystem();

        }

        private void DrawAxes()
        {
            axisCanvas.Children.Clear();

            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            Line xAxis = new Line();
            Line yAxis = new Line();


            xAxis.Stroke = Brushes.Black;
            yAxis.Stroke = Brushes.Black;

            xAxis.StrokeThickness = 2;
            yAxis.StrokeThickness = 2;

            xAxis.X1 = 0;
            xAxis.Y1 = height / 2;
            xAxis.X2 = width;
            xAxis.Y2 = height / 2;

            yAxis.X1 = width / 2;
            yAxis.Y1 = 0;
            yAxis.X2 = width / 2;
            yAxis.Y2 = height;

            axisCanvas.Children.Add(xAxis);
            axisCanvas.Children.Add(yAxis);

            GenerateAxisNumbers();

            DrawGrid();
        }

        private void DrawGrid()
        {
            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            for (double i = -height/2; i <= 0; i += gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = 0,
                    X2 = width,
                    Y1 = i+height,
                    Y2 = i+height
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = height / 2; i >= 0; i -= gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = 0,
                    X2 = width,
                    Y1 = i,
                    Y2 = i
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = -width/2; i <= 0; i += gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = i+width,
                    X2 = i+width,
                    Y1 = 0,
                    Y2 = height
                };

                axisCanvas.Children.Add(line);
            }

            for (double i = width / 2; i >= 0; i -= gridSize)
            {
                Line line = new Line()
                {
                    Opacity = 0.3,
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.5,
                    X1 = i,
                    X2 = i,
                    Y1 = 0,
                    Y2 = height
                };

                axisCanvas.Children.Add(line);
            }
        }

        private void GenerateAxisNumbers()
        {
            double width = axisCanvas.ActualWidth;
            double height = axisCanvas.ActualHeight;

            double quantOfX = width / gridSize;
            double quantOfY = height / gridSize;

            for (int i = (int)(-(quantOfX / 2)); i <= (quantOfX / 2); ++i)
            {
                TextBlock xText = new TextBlock()
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness((i + (quantOfX / 2)) * gridSize, height / 2, 0, 0)
                };
                axisCanvas.Children.Add(xText);
            }

            for (int i = (int)((quantOfX / 2)); i >= (-quantOfX / 2); --i)
            {
                TextBlock yText = new TextBlock()
                {
                    Text = i.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(width / 2, (-i + (quantOfY / 2)) * gridSize, 0, 0)
                };
                axisCanvas.Children.Add(yText);
            }
        }

        private void TransormCoordinateSystem()
        {
            double centerX = axisCanvas.ActualWidth / 2;
            double centerY = axisCanvas.ActualHeight / 2;

            TranslateTransform transform = new TranslateTransform(centerX, centerY);
            canvas.RenderTransform = transform;
        }

        public Point[] CalculateSquareVertices(Point topLeft, Point bottomLeft)
        {
            var sideVector = new Point(bottomLeft.X - topLeft.X, bottomLeft.Y - topLeft.Y);

            var flippedVector = new Point(-sideVector.Y, sideVector.X);

            var topRight = new Point(topLeft.X - flippedVector.X, topLeft.Y - flippedVector.Y);
            var bottomRight = new Point(bottomLeft.X - flippedVector.X, bottomLeft.Y - flippedVector.Y);


            return new Point[] {topLeft, bottomLeft, bottomRight, topRight};
        }

        public bool IsHexDigit(char c)
        {
            bool isHexDigit = (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

            return isHexDigit;
        }

        public bool IsHexCorrect(string hex)
        {
            if(string.IsNullOrEmpty(hex)) return false;
            if (hex[0] != '#') return false;
            if (hex.Length != 7) return false;

            hex = hex.TrimStart('#');

            foreach (char c in hex)
                if (!IsHexDigit(c))
                    return false;
            return true;
        }

        public SolidColorBrush? getColorFromHex(string hex)
        {

            if (string.IsNullOrEmpty(hex))
                return null;

            if (!IsHexCorrect(hex))
            {
                MessageBox.Show("Check whether the hex is correct!");
                return null;
            }

            Color color = (Color)ColorConverter.ConvertFromString(hex);
            SolidColorBrush brush = new SolidColorBrush(color);

            return brush;
        }

        public Ellipse getExcircle(Point topLeft, Point bottomRight)
        {
            double diameter = Math.Sqrt((topLeft.X - bottomRight.X) * (topLeft.X - bottomRight.X) + (topLeft.Y - bottomRight.Y) * (topLeft.Y - bottomRight.Y));

            Ellipse ex = new Ellipse()
            {
                Width = diameter,
                Height = diameter,
                Stroke = Brushes.Black,
                Opacity = 0.6,
            };

            return ex;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {

            if (!(double.TryParse(xTopLeftTextBox.Text, out double xTopLeft) &&
               double.TryParse(yTopLeftTextBox.Text, out double yTopLeft) &&
               double.TryParse(xBottomLeftTextBox.Text, out double xBottomLeft) &&
               double.TryParse(yBottomLeftTextBox.Text, out double yBottomLeft)))
            {
                MessageBox.Show("Please, enter correct values in coordinates textboxes!");
                return;
            }
            SolidColorBrush squareBrush = getColorFromHex(SquareColorTextBox.Text);
            SolidColorBrush circleBrush = getColorFromHex(CircleColorTextBox.Text);

            if (squareBrush == null)
                squareBrush = Brushes.Black;
            if (circleBrush == null)
                circleBrush = Brushes.White;

            xTopLeft *= gridSize;
            yTopLeft *= -gridSize;
            xBottomLeft *= gridSize;
            yBottomLeft *= -gridSize;

            double angle = Math.Atan2(yBottomLeft - yTopLeft, xBottomLeft - xTopLeft);

            Point topLeft = new Point(xTopLeft, yTopLeft);
            Point bottomLeft = new Point(xBottomLeft, yBottomLeft);

            Point[] points = CalculateSquareVertices(topLeft, bottomLeft);
            
            Polygon square = new Polygon
            {
                Points = new PointCollection(points),
                Fill = squareBrush,
                Stroke = Brushes.Black,
                Opacity = 0.75
            };

            Ellipse excircle = getExcircle(points[0], points[2]);

            double newX = (points[0].X + points[2].X) / 2;
            double newY = (points[0].Y + points[2].Y) / 2;

            TranslateTransform translateTransform = new TranslateTransform(newX - (excircle.Width / 2), newY - (excircle.Height / 2));

            excircle.RenderTransform = translateTransform;
            excircle.Fill = circleBrush;

            canvas.Children.Add(square);
            canvas.Children.Add(excircle);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
        }

    }
}
