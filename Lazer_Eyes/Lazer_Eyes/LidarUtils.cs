using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



namespace Lazer_Eyes
{
    /// <summary>
    /// Provides access to platform specific frameworks for using LiDAR
    /// </summary>
    public static partial class LidarUtils
    {
        /// <summary>
        /// The highest priority obstacle to reported to the user
        /// </summary>
        public static Obstacle CurrentObstacle;

        /// <summary>
        /// 
        /// </summary>
        public static partial void GetAlerts();

        /// <summary>
        /// Holds data for a detected obstacle
        /// </summary>
        public struct Obstacle
        {
            /// <summary>
            /// Name of obstacle
            /// </summary>
            public string? ObstacleName;
            /// <summary>
            /// Distance obstacle is from the user's camera
            /// </summary>
            public double? Distance;
        }

        /// <summary>
        /// Flag when lighting condition is not adequate
        /// </summary>
        public static bool IsLightingPoor = false;

    }

}
