using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SARecorder.Drawing
{
    public class PathDrawer : IDrawer
    {
        private bool drawingActivated = false;
        private Path currentPath = new Path();
        private PathGeometry currentGeometry = new PathGeometry();

        public UIElement Awake(Point start)
        {
            drawingActivated = true;

            currentGeometry = new PathGeometry();
            currentGeometry.Figures.Add(new PathFigure());
            currentGeometry.Figures[0].StartPoint = start;

            currentPath = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Data = currentGeometry
            };

            return currentPath;
        }

        public void Cancel(Point last)
        {
            drawingActivated = false;
            if (currentGeometry.IsFrozen == false)
            {
                currentGeometry.Freeze();
            }
            currentPath.Data = currentGeometry;
        }

        public void Draw(Point current)
        {
            if (drawingActivated)
            {
                currentGeometry.Figures[0].Segments.Add(new LineSegment(current, true));
            }
        }

        public void Finish(Point end)
        {
            drawingActivated = false;
            if (currentGeometry.IsFrozen == false)
            {
                currentGeometry.Freeze();
            }
        }
    }
}
