using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SARecorder.Drawing
{
    public class PathDrawer : IDrawer
    {
        private bool drawingActivated = false;
        private Path currentPath = new Path();
        private StreamGeometry lineData = new StreamGeometry();
        private StreamGeometryContext lineContext = null;

        public UIElement Awake(Point start)
        {
            drawingActivated = true;

            lineData = new StreamGeometry();
            currentPath = new Path()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };

            lineContext = lineData.Open();
            lineContext.BeginFigure(start, false, false);

            return currentPath;
        }

        public void Cancel(Point last)
        {
            drawingActivated = false;

            currentPath.Data = lineData;
            if (lineContext != null)
            {
                lineContext.Close();
                lineContext = null;
            }
            currentPath.Data.Freeze();
        }

        public void Draw(Point current)
        {
            if (drawingActivated)
            {
                if (lineContext != null)
                {
                    lineContext.LineTo(current, true, true);

                    Geometry currentData = lineData.GetFlattenedPathGeometry();
                    currentData.Freeze();
                    currentPath.Data = currentData;
                }
            }
        }

        public void Finish(Point end)
        {
            drawingActivated = false;

            currentPath.Data = lineData;
            if (lineContext != null)
            {
                lineContext.Close();
                lineContext = null;
            }
            currentPath.Data.Freeze();
        }
    }
}
