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
using System.IO;

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
        private DrawingMode currentMode = DrawingMode.FreeDraw;
        public DrawingMode CurrentMode
        {
            get { return currentMode; }
            set
            {
                drawingFunctions[currentMode].Cancel(lastPoint);
                currentMode = value;
            }
        }
        private Point lastPoint = new Point();

        public BasicPainter()
        {
            InitializeComponent();
        }

        public void ClearCanvas()
        {
            CanvasBoard.Children.Clear();
        }

        public void SaveImage(string fileName, string folderName = null)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)CanvasBoard.ActualWidth, (int)CanvasBoard.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(CanvasBoard);

            string folderPath = Environment.CurrentDirectory + @$"\Answers\";
            if (folderName != null)
            {
                folderPath += @$"{folderName}\";
            }
            string filePath = $"{folderPath}{fileName}.png";
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (di.Exists == false)
            {
                di.Create();
            }
                        
            using Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            encoder.Save(stream);
        }

        private void WhiteBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(CanvasBoard);
            UIElement element = drawingFunctions[currentMode].Awake(lastPoint);
            CanvasBoard.Children.Add(element);
        }

        private void WhiteBoard_MouseMove(object sender, MouseEventArgs e)
        {
            lastPoint = e.GetPosition(CanvasBoard);
            drawingFunctions[currentMode].Draw(lastPoint);
        }

        private void WhiteBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            lastPoint = e.GetPosition(CanvasBoard);
            drawingFunctions[currentMode].Finish(lastPoint);
        }

        private void WhiteBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            lastPoint = e.GetPosition(CanvasBoard);
            drawingFunctions[currentMode].Cancel(lastPoint);
        }
    }
}
