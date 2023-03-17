using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazer_Eyes.Platforms.iOS
{
    /// <summary>
    /// Processes light intensity in lumens and reports on lighting quality
    /// </summary>
    public static class LightDetector
    {
        /// <summary>
        /// Singleton object containing app settings
        /// </summary>
        private static Settings SettingsObj = Settings.Get();
        
        /// <summary>
        /// Checks if current ambient light intensity in lumens satisifes 
        /// light quality threshold defined in the settings
        /// </summary>
        /// <param name="ambientLightIntensity">The current ARFrame's light intensity</param>
        public static void CheckLightQuality(float ambientLightIntensity)
        {
            // System.Diagnostics.Debug.WriteLine($"Current Ambient Light Intensity: {ambientLightIntensity}");
            if (ambientLightIntensity < SettingsObj.GetLowLightIntensity())
            {
                LidarUtils.IsLightingPoor = true;
            }
            else
            {
                LidarUtils.IsLightingPoor = false;
            }
        }
    }
}
