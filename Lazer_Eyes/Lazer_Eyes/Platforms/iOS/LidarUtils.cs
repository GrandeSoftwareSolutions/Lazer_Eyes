using System;
using Lazer_Eyes.Platforms.iOS;

namespace Lazer_Eyes
{ 
    /// <summary>
    /// Interfaces with ARKit to use LiDAR and object detection
    /// </summary>
    public static partial class LidarUtils
    {
        static bool isStarted = false;

        /// <summary>
        /// Manages ARkit functionality
        /// </summary>
        static ARKitManager arkitManager;

        /// <summary>
        /// Initialize obstacle detection for IOS with ARKit
        /// </summary>
        public static partial void GetAlerts()
        {
            if (!isStarted)
            {
                isStarted = true;
                arkitManager = new ARKitManager();
            }
        }
    }
}