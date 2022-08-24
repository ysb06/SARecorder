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

namespace SARecorder.SAGAT
{
    /// <summary>
    /// TouchPainter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TouchPainter : UserControl
    {
        private bool DrawingActivated = false;

        public TouchPainter()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
