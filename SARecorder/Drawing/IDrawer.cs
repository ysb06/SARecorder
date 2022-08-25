using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SARecorder.Drawing
{
    public interface IDrawer
    {
        UIElement Awake(Point start);
        void Draw(Point current);
        void Finish(Point end);
        void Cancel(Point last);
    }
}
