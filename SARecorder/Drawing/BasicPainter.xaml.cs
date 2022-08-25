using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SARecorder.Drawing
{
    public enum DrawingMode
    {
        None,
        Line,
        FreeDraw,
        Rectangle,
        Circle
    }

    /// <summary>
    /// BasicPainter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BasicPainter : UserControl
    {
        private Dictionary<DrawingMode, IDrawer> drawingFunctions = new Dictionary<DrawingMode, IDrawer>()
        {
            { DrawingMode.Line, new LineDrawer() },
            { DrawingMode.FreeDraw, new PathDrawer() }
        };
        public DrawingMode CurrentMode = DrawingMode.FreeDraw;

        public BasicPainter()
        {
            InitializeComponent();
        }

        private void WhiteBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point startPoint = e.GetPosition(WhiteBoard);
            UIElement element = drawingFunctions[CurrentMode].Awake(startPoint);
            WhiteBoard.Children.Add(element);
        }

        private void WhiteBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(WhiteBoard);
            drawingFunctions[CurrentMode].Draw(point);
        }

        private void WhiteBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            drawingFunctions[CurrentMode].Finish(e.GetPosition(WhiteBoard));
        }

        private void WhiteBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(WhiteBoard);
            drawingFunctions[CurrentMode].Cancel(point);
        }
    }
}
