using System;
using System.Collections.Generic;
using System.Windows;

namespace TCP.App.Models.Electronics;

public class PathSegment
{
    public double Length { get; set; }
    public double StartDistance { get; set; }
    
    // For Line
    public Point Start { get; set; }
    public Point End { get; set; }
    
    // For Bezier
    public bool IsBezier { get; set; }
    public Point Control { get; set; }

    public Point GetPoint(double t)
    {
        if (!IsBezier)
        {
            return new Point(
                Start.X + (End.X - Start.X) * t,
                Start.Y + (End.Y - Start.Y) * t
            );
        }
        else
        {
            double u = 1 - t;
            double tt = t * t;
            double uu = u * u;
            
            double x = uu * Start.X + 2 * u * t * Control.X + tt * End.X;
            double y = uu * Start.Y + 2 * u * t * Control.Y + tt * End.Y;
            return new Point(x, y);
        }
    }

    public Vector GetTangent(double t)
    {
        if (!IsBezier)
        {
            var v = End - Start;
            v.Normalize();
            return v;
        }
        else
        {
            // Derivative of quadratic bezier: 2(1-t)(P1-P0) + 2t(P2-P1)
            var p1_p0 = Control - Start;
            var p2_p1 = End - Control;
            
            double dx = 2 * (1 - t) * p1_p0.X + 2 * t * p2_p1.X;
            double dy = 2 * (1 - t) * p1_p0.Y + 2 * t * p2_p1.Y;
            
            var v = new Vector(dx, dy);
            v.Normalize();
            return v;
        }
    }
}

public static class PathMath
{
    public static List<PathSegment> GenerateSegments(IEnumerable<TrackNode> nodes, double defaultRadius)
    {
        var segments = new List<PathSegment>();
        var points = new List<Point>();
        foreach (var n in nodes) points.Add(new Point(n.X, n.Y));
        
        if (points.Count < 2) return segments;

        Point currentPoint = points[0];

        for (int i = 1; i < points.Count - 1; i++)
        {
            var prev = points[i - 1];
            var curr = points[i];
            var next = points[i + 1];

            Vector v1 = prev - curr;
            Vector v2 = next - curr;
            
            double len1 = v1.Length;
            double len2 = v2.Length;

            if (len1 < 1.0 || len2 < 1.0)
            {
                continue;
            }

            double r = Math.Min(defaultRadius, Math.Min(len1 / 2.1, len2 / 2.1));

            v1.Normalize();
            v2.Normalize();

            var pStart = curr + v1 * r;
            var pEnd = curr + v2 * r;

            // Line from current to pStart
            var lineSeg = new PathSegment
            {
                IsBezier = false,
                Start = currentPoint,
                End = pStart,
                Length = (pStart - currentPoint).Length
            };
            segments.Add(lineSeg);

            // Bezier from pStart to pEnd with control curr
            var bezSeg = new PathSegment
            {
                IsBezier = true,
                Start = pStart,
                Control = curr,
                End = pEnd,
                Length = EstimateBezierLength(pStart, curr, pEnd)
            };
            segments.Add(bezSeg);

            currentPoint = pEnd;
        }

        // Final line
        var last = points[points.Count - 1];
        var finalSeg = new PathSegment
        {
            IsBezier = false,
            Start = currentPoint,
            End = last,
            Length = (last - currentPoint).Length
        };
        segments.Add(finalSeg);

        // Calculate StartDistances
        double currentDist = 0;
        foreach (var seg in segments)
        {
            seg.StartDistance = currentDist;
            currentDist += seg.Length;
        }

        return segments;
    }

    private static double EstimateBezierLength(Point p0, Point p1, Point p2)
    {
        double len = 0;
        Point prev = p0;
        int steps = 10;
        for (int i = 1; i <= steps; i++)
        {
            double t = (double)i / steps;
            double u = 1 - t;
            double x = u * u * p0.X + 2 * u * t * p1.X + t * t * p2.X;
            double y = u * u * p0.Y + 2 * u * t * p1.Y + t * t * p2.Y;
            var pt = new Point(x, y);
            len += (pt - prev).Length;
            prev = pt;
        }
        return len;
    }
}
