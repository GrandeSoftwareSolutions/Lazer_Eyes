/*
 * LazerEyes Settings.cs
 * Singleton associated with settings
 * Course- IST440
 * Author- Cameron Grande, Kseniia Gromova
 * Date Developed- 10/18/22
 * Last Changed- 12/02/22
 * Rev: 5
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lazer_Eyes
{
    public sealed class Settings
    {
        //fields and attributes 
        private static Settings         s_settings;
        private static readonly object  s_padlock = new();

        private bool    _alertSettingsDefault;
        private bool    _auditoryDefault;
        private double  _distanceThreshold;
        private int     _distanceUnit;
        private bool    _lowLightToggle;
        private double  _lowLightIntensity;
        private bool    _notificationSettingsDefault;
        private bool    _measureDistanceInStrides;
        private double  _strideLength;
        private bool    _tactileSettingsDefault;
        private double  _volume;

        public const bool      AlertSettingsDefault =           true;
        public const bool      AuditoryDefault =                true;
        public const bool      AuditorySettingsDefault =        true;
        public const double    DistanceThresholdDefault =       5.0;
        public const int       DistanceUnitDefault =            (int) Units.Meters;
        public const bool      LowLightToggleDefault =          true;
        public const double    LowLightIntensityDefault  =      1000.0;
        public const bool      NotificationSettingsDefault =    true;
        public const bool      MeasureDistanceInStrides =      false;
        public const double    StrideLengthDefault =            2.0;
        public const bool      TactileSettingsDefault =         true;
        public const double    VolumeDefault =                  0.50;

        public enum Units
        {
            Meters,
            Feet,
            Steps
        }

        /// <summary>
        /// Constructor- will check/load if settings already exist, if not will load defaults.
        /// Will create the global settings object singleton. This cannot be called directly and can only be 
        /// accessed through Settings.get();
        /// </summary>
        private Settings()
        {
            if (!Preferences.ContainsKey("alertSettingsDefault"))
            {
                Preferences.Set("alertSettingsDefault", AlertSettingsDefault);
                Preferences.Set("distanceUnit", DistanceUnitDefault);
                Preferences.Set("distanceThreshold", DistanceThresholdDefault);
                Preferences.Set("distanceMeasuredInStrides", MeasureDistanceInStrides);
                Preferences.Set("strideLength", StrideLengthDefault);
                Preferences.Set("notificationSettingsDefault", NotificationSettingsDefault);
                Preferences.Set("auditoryDefault", AuditoryDefault);
                Preferences.Set("volume", VolumeDefault);
                Preferences.Set("tactileSettingsDefault", TactileSettingsDefault);
                Preferences.Set("lowLightToggle", LowLightToggleDefault);
                Preferences.Set("lowLightIntensity", LowLightIntensityDefault);
            }
            _alertSettingsDefault = Preferences.Get("alertSettingsDefault", AlertSettingsDefault);
            _distanceUnit = Preferences.Get("distanceUnit", DistanceUnitDefault);           
            _distanceThreshold = Preferences.Get("distanceThreshold", DistanceThresholdDefault);
            _measureDistanceInStrides = Preferences.Get("measureDistanceInStrides", MeasureDistanceInStrides);
            _strideLength = Preferences.Get("strideLength", StrideLengthDefault);
            _notificationSettingsDefault = Preferences.Get("notificationSettingsDefault", NotificationSettingsDefault);
            _auditoryDefault = Preferences.Get("auditoryDefault", AuditoryDefault);
            _volume = Preferences.Get("volume", VolumeDefault);
            _tactileSettingsDefault = Preferences.Get("tactileSettingsDefault", TactileSettingsDefault);
            _lowLightToggle = Preferences.Get("lowLightToggle", LowLightToggleDefault);
            _lowLightIntensity = Preferences.Get("lowLightIntensity", LowLightIntensityDefault);
        }

        /// <summary>
        /// Get- singleton implementation that will get settings instance in thread-safe manor
        /// Use this method to access settings object.
        /// </summary>
        public static Settings Get()
        {
            lock (s_padlock)
            {
                if (s_settings == null)
                {
                    s_settings = new Settings();
                }
                return s_settings;
            }
        }

        //setters and getters below
        /// <summary>
        /// Provides Alert Settings Default
        /// </summary>
        public bool GetAlertSettingsDefault()
        {
            return _alertSettingsDefault;
        }

        /// <summary>
        /// Sets Alert Settings Default
        /// </summary>
        public void SetAlertSettingsDefault(bool alertSettingsDefault)
        {
            this._alertSettingsDefault = alertSettingsDefault;
            Preferences.Set("alertSettingsDefault", alertSettingsDefault);
        }
        /// <summary>
        /// Gets Distance Unit
        /// </summary>
        public int GetDistanceUnit()
        {
            return _distanceUnit;
        }
        /// <summary>
        /// Sets Distance Unit
        /// </summary>
        public void SetDistanceUnit(int distanceUnit)
        {
            this._distanceUnit = distanceUnit;
            Preferences.Set("distanceUnit", distanceUnit);
        }
        /// <summary>
        /// Gets Distance Threshold
        /// </summary>
        public double GetDistanceThreshold()
        {
            return _distanceThreshold;
        }
        /// <summary>
        /// Sets Distance Threshold
        /// </summary>
        public void SetDistanceThreshold(double distanceThreshold)
        {
            this._distanceThreshold = distanceThreshold;
            Preferences.Set("distanceThreshold", distanceThreshold);
        }
        public bool GetMeasureDistanceInStrides()
        {
            return _measureDistanceInStrides;
        }
        public void SetMeasureDistanceInStrides(bool measuredInStrides)
        {
            this._measureDistanceInStrides = measuredInStrides;
            Preferences.Set("measureDistanceInStrides", measuredInStrides);
        }
        /// <summary>
        /// Gets Stride Length
        /// </summary>
        public double GetStrideLength()
        {
            return _strideLength;
        }
        /// <summary>
        /// Sets Stride Length
        /// </summary>
        public void SetStrideLength(double strideLength)
        {
            this._strideLength = strideLength;
            Preferences.Set("strideLength", strideLength);
        }
        /// <summary>
        /// Gets Notification Settings Default
        /// </summary>
        public bool GetNotificationSettingsDefault()
        {
            return _notificationSettingsDefault;
        }
        /// <summary>
        /// Sets Notification Settings Default
        /// </summary>
        public void SetNotificationSettingsDefault(bool notificationSettingsDefault)
        {
            this._notificationSettingsDefault = notificationSettingsDefault;
            Preferences.Set("notificationSettingsDefault", notificationSettingsDefault);
        }
        /// <summary>
        /// Gets Auditory Default
        /// </summary>
        public bool GetAuditoryDefault()
        {
            return _auditoryDefault;
        }
        /// <summary>
        /// Sets Auditory Default
        /// </summary>
        public void SetAuditoryDefault(bool auditoryDefault)
        {
            this._auditoryDefault = auditoryDefault;
            Preferences.Set("auditoryDefault", auditoryDefault);
        }
        /// <summary>
        /// Gets Volume
        /// </summary>
        public double GetVolume()
        {
            return _volume;
        }
        /// <summary>
        /// Sets Volume
        /// </summary>
        public void SetVolume(double volume)
        {
            this._volume = volume;
            Preferences.Set("volume", volume);
        }
        /// <summary>
        /// Gets Tactile Settings Default
        /// </summary>
        public bool GetTactileSettingsDefault()
        {
            return _tactileSettingsDefault;
        }
        /// <summary>
        /// Sets Tactile Settings Default
        /// </summary>
        public void SetTactileSettingsDefault(bool tactileSettingsDefault)
        {
            this._tactileSettingsDefault = tactileSettingsDefault;
            Preferences.Set("tactileSettingsDefault", tactileSettingsDefault);
        }
        /// <summary>
        /// Gets Low Light Toggle
        /// </summary>
        public bool GetLowLightToggle()
        {
            return _lowLightToggle;
        }
        /// <summary>
        /// Sets Low Light Toggle
        /// </summary>
        public void SetLowLightToggle(bool lowLightToggle)
        {
            this._lowLightToggle = lowLightToggle;
            Preferences.Set("lowLightToggle", lowLightToggle);
        }
        /// <summary>
        /// Gets Low Light Intensity
        /// </summary>
        public double GetLowLightIntensity()
        {
            return _lowLightIntensity;
        }
        /// <summary>
        /// Sets Low Light Intensity
        /// </summary>
        public void SetLowLightIntensity(double lowLightIntensity)
        {
            this._lowLightIntensity = lowLightIntensity;
            Preferences.Set("lowLightIntensity", lowLightIntensity);
        }
    }
}
