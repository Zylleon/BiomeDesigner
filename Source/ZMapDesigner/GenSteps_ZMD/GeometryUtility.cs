using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.BaseGen;
using Verse;


namespace ZMapDesigner.GenSteps_ZMD
{
    public static class GeometryUtility
    {

        /// <summary>
        /// Draws a solid ellipse on the provided map and returns the tiles in that ellipse
        /// </summary>
        /// <param name="focus1"></param>
        /// <param name="focus2"></param>
        /// <param name="totalDist"></param>
        /// <param name="map">Total distance between both foci and any point on the perimeter of the ellipse</param>
        /// <returns></returns>
        public static List<IntVec3> MakeEllipse(IntVec3 focus1, IntVec3 focus2, int totalDist, Map map)
        {
            List<IntVec3> ellipse = new List<IntVec3>();

            foreach (IntVec3 current in map.AllCells)
            {
                if (GeometryUtility.DistanceBetweenPoints(focus1, current) + GeometryUtility.DistanceBetweenPoints(focus2, current) < totalDist)
                {
                    ellipse.Add(current);
                }
            }
            return ellipse;
        }

        /// <summary>
        /// Distance between 2 points as the crow flies
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static float DistanceBetweenPoints(IntVec3 point1, IntVec3 point2)
        {
            float dist = 0;
            double xDist = Math.Pow(point1.x - point2.x, 2);
            double zDist = Math.Pow(point1.z - point2.z, 2);
            dist = (float)Math.Sqrt(xDist + zDist);
            return dist;
        }


        public static List<IntVec3> MakeCircleAround(IntVec3 center, float radius, Map map)
        {
            List<IntVec3> circle = new List<IntVec3>();

            foreach (IntVec3 current in map.AllCells)
            {
                if (GeometryUtility.DistanceBetweenPoints(center, current) <= radius)
                {
                    circle.Add(current);
                }
            }
            return circle;
        }

    }
}
