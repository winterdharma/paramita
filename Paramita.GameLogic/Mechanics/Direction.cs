﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Paramita.GameLogic.Mechanics
{
    public enum Compass {
        North = 0,
        Northeast,
        East,
        Southeast,
        South,
        Southwest,
        West,
        Northwest,
        None
    }

    public static class Direction
    {
        #region Fields
        private static Point[] _direction = new Point[9] {
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1 , 1),
            new Point(-1, 0),
            new Point(-1, -1),
            new Point(0, 0) };

        private static List<Point> _cardinalPoints = new List<Point>()
            { North, East, South, West };

        private static List<Point> _eightDirections = new List<Point>()
        { North, Northeast, East, Southeast, South, Southwest, West, Northwest };
        #endregion

        #region Directions
        public static List<Point> EightCompassPoints
        {
            get { return _eightDirections; }
        }

        public static List<Point> CardinalPoints
        {
            get { return _cardinalPoints; }
        }
        public static Point North
        {
            get { return _direction[0]; }
        }
        public static Point Northeast
        {
            get { return _direction[1]; }
        }
        public static Point East
        {
            get { return _direction[2]; }
        }
        public static Point Southeast
        {
            get { return _direction[3]; }
        }
        public static Point South
        {
            get { return _direction[4]; }
        }
        public static Point Southwest
        {
            get { return _direction[5]; }
        }
        public static Point West
        {
            get { return _direction[6]; }
        }
        public static Point Northwest
        {
            get { return _direction[7]; }
        }
        #endregion

        public static Point GetPoint(Compass c)
        {
            return _direction[(int)c];
        }

        public static Compass GetDirection(Point p)
        {
            var direction = Compass.None;

            if (p == North)
                direction = Compass.North;
            else if (p == South)
                direction = Compass.South;
            else if (p == East)
                direction = Compass.East;
            else if (p == West)
                direction = Compass.West;
            else if (p == Northeast)
                direction = Compass.Northeast;
            else if (p == Northwest)
                direction = Compass.Northwest;
            else if (p == Southeast)
                direction = Compass.Southeast;
            else if (p == Southwest)
                direction = Compass.Southwest;

            return direction;
        }
    }
}
