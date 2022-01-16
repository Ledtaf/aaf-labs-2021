using System;
using System.Collections.Generic;
using System.Linq;
using RBush;

namespace PointClass
{
    public class Point : ISpatialData, IComparable<Point>, IEquatable<Point>
    {
        private readonly Envelope _envelope;

        public Point(double minX, double minY, double maxX, double maxY)
        {
            _envelope = new Envelope(
                minX: minX,
                minY: minY,
                maxX: maxX,
                maxY: maxY);
        }

        public Point(double min, double max) //FIX IT
        {
            _envelope = new Envelope(min, max);
        }

        public override string ToString()
        {
            return $"[{_envelope.MinX},{_envelope.MaxX}]";
        }
        public ref readonly Envelope Envelope => ref _envelope;

        public int CompareTo(Point other)
        {
            if (this.Envelope.MinX != other.Envelope.MinX)
                return this.Envelope.MinX.CompareTo(other.Envelope.MinX);
            if (this.Envelope.MinY != other.Envelope.MinY)
                return this.Envelope.MinY.CompareTo(other.Envelope.MinY);
            if (this.Envelope.MaxX != other.Envelope.MaxX)
                return this.Envelope.MaxX.CompareTo(other.Envelope.MaxX);
            if (this.Envelope.MaxY != other.Envelope.MaxY)
                return this.Envelope.MaxY.CompareTo(other.Envelope.MaxY);
            return 0;
        }

        public bool Equals(Point other) =>
            this._envelope == other._envelope;
    }
}