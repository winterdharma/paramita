﻿using Microsoft.Xna.Framework;

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
        private static Point[] direction = new Point[9] {
            new Point(0, -1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1),
            new Point(0, 1),
            new Point(-1 , 1),
            new Point(-1, 0),
            new Point(-1, -1),
            new Point(0, 0) };

        public static Point GetPoint(Compass c)
        {
            return direction[(int)c];
        }
    }
}
