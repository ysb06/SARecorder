using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SARecorder.Drawing
{
    class LineDrawer : IDrawer
    {
        private bool drawingActivated = false;
        private Line currentLine;

        public UIElement Awake(Point start)
        {
            drawingActivated = true;

            currentLine = new Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                X1 = start.X,
                Y1 = start.Y
            };

            return currentLine;
        }

        public void Cancel(Point last)
        {
            drawingActivated = false;
        }

        public void Draw(Point current)
        {
            if (drawingActivated)
            {
                currentLine.X2 = current.X;
                currentLine.Y2 = current.Y;
            }
        }

        public void Finish(Point end)
        {
            drawingActivated = false;
        }
    }
}
