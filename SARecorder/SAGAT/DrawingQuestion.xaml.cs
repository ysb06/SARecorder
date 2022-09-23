using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SARecorder.SAGAT
{
    /// <summary>
    /// DrawingQuestion.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DrawingQuestion : UserControl
    {
        [Category("SAGAT"), Description("Text for the question")]
        public string MainTitle
        {
            get => QuestionText.Text.ToString();
            set => QuestionText.Text = value;
        }

        [Category("SAGAT"), Description("Text for the question")]
        public string SubTitle
        {
            get => SubText.Text.ToString();
            set => SubText.Text = value;
        }

        [Category("Brushes"), Description("Canvas background")]
        public Brush CanvasBackground
        {
            get => WhiteBoard.CanvasBoard.Background;
            set => WhiteBoard.CanvasBoard.Background = value;
        }

        public DrawingQuestion()
        {
            InitializeComponent();
        }

        public void SaveAsFile(string name, string prefix)
        {
            WhiteBoard.SaveImage($"{prefix}_{Name}", name);
        }

        private void LineModeButton_Click(object sender, RoutedEventArgs e)
        {
            WhiteBoard.CurrentMode = Drawing.DrawingMode.Line;
        }

        private void FreeModeButton_Click(object sender, RoutedEventArgs e)
        {
            WhiteBoard.CurrentMode = Drawing.DrawingMode.FreeDraw;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            WhiteBoard.ClearCanvas();
        }
    }
}
