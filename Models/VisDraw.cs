﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Geo.Models.Db
{
    public class VisDraw
    {
        DrawingGroup dg = new();
        public void DrawLine(double x1, double y1, double x2, double y2, Brush br, double width) =>
             dg.Children.Add(new GeometryDrawing(null, new Pen(br, width) { DashCap = PenLineCap.Round }, new LineGeometry(new(x1, y1), new(x2, y2))));

        public void DrawText(string text, double x, double y, Brush br, double size = 1) =>
            dg.Children.Add(new GeometryDrawing(Brushes.Black, null, new FormattedText(text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Arial"), size, Brushes.Black, 1).BuildGeometry(new(x, y))));

        public void DrawCircle(double x, double y, double r, Brush br) =>
            dg.Children.Add(new GeometryDrawing(br, null, new EllipseGeometry(new(x, y), r, r)));

        public void DrawPoly(ICollection<Point> points, Brush br, double width, bool isClosed)
        {
            if (points is null || points.Count < 1) return;
            dg.Children.Add(new GeometryDrawing(null, new Pen(br, width) { LineJoin = PenLineJoin.Round, DashCap = PenLineCap.Round }, new PathGeometry(new PathFigure[] {
                new PathFigure(
                    points.First(),
                    points.Skip(1).Select(p => new LineSegment(p, true)), isClosed)
            })));
        }

        public DrawingImage Render(int offset = 3, int grid = 10, bool drawAxies = false)
        {
            double minx = (int)dg.Bounds.Left - offset, miny = (int)dg.Bounds.Top - offset, maxx = (int)dg.Bounds.Right - offset, maxy = (int)dg.Bounds.Bottom + offset;
            var chilndren = dg.Children.ToArray();
            dg.Children.Clear();

            // Отрисовываем линии и числа по оси Y
            for (double j = miny + grid - miny % grid; j < maxy; j += grid)
            {
                DrawLine(minx, j, maxx, j, Brushes.Gray, 0.1);
                DrawText($"{j}", 0, j, Brushes.Black, 1); // Рисуем числа на оси Y
            }

            // Отрисовываем линии и числа по оси X
            for (double i = minx + grid - minx % grid; i < maxx; i += grid)
            {
                DrawLine(i, miny, i, maxy, Brushes.Gray, 0.1);
                DrawText($"{i}", i, 0, Brushes.Black, 1); // Рисуем числа на оси X
            }

            // Отрисовываем черные линии для главных осей X и Y
            if ((miny < 0 && maxy > 0) || drawAxies)
                DrawLine(minx, 0, maxx, 0, Brushes.Black, 0.3);

            if ((minx < 0 && maxx > 0) || drawAxies)
                DrawLine(0, miny, 0, maxy, Brushes.Black, 0.3);

            foreach (var p in chilndren)
                dg.Children.Add(p);

            return new DrawingImage(dg);
        }



    }
}
