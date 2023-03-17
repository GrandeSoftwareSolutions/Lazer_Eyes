/*
 * LazerEyes AlertNotifications.xaml.cs
 * Logic related to alert notifications.
 * Course- IST440
 * Author- Cameron Grande
 * Date Developed- 9/26/22
 * Last Changed- 12/02/22
 * Rev: 5
 */

using Lazer_Eyes.Resources.Languages;

namespace Lazer_Eyes;

public partial class AlertNotifications : ContentPage
{
    public Settings SettingsObj;

    /// <summary>
    /// Constructor- loads page and initial settings.
    /// </summary>
    public AlertNotifications()
	{
		InitializeComponent();
        SettingsObj = Settings.Get();
        DefaultSwitch.IsToggled = SettingsObj.GetNotificationSettingsDefault();
        
#if IOS
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior()
        {
            TextOverride = ApplicationResource.Back,
        });
#endif 

        if (DefaultSwitch.IsToggled)
        {
            setAllToDefault();
        } 
         else
        {
            AuditoryNotificationsSwitch.IsToggled = SettingsObj.GetAuditoryDefault();
            AuditoryAlertsVolumeSlider.Value = SettingsObj.GetVolume();
            TactileNotificationsSwitch.IsToggled = SettingsObj.GetTactileSettingsDefault();
            LowLightNotificationsSwitch.IsToggled = SettingsObj.GetLowLightToggle();
            LowLightIntensitySlider.Value = SettingsObj.GetLowLightIntensity();
        }
    }

    /// <summary>
    /// Sets all settings related to alerts to their default values.
    /// </summary>
    void setAllToDefault()
    {
        AuditoryNotificationsSwitch.IsToggled = Settings.AuditorySettingsDefault;
        AuditoryNotificationsSwitch.IsEnabled = false;
        SettingsObj.SetAuditoryDefault(Settings.AuditorySettingsDefault);
        setAuditoryNotificationsToDefault();

        TactileNotificationsSwitch.IsToggled = Settings.TactileSettingsDefault;
        TactileNotificationsSwitch.IsEnabled = false;
        SettingsObj.SetTactileSettingsDefault(Settings.TactileSettingsDefault);

        LowLightNotificationsSwitch.IsToggled = Settings.LowLightToggleDefault;
        LowLightNotificationsSwitch.IsEnabled = false;
        SettingsObj.SetLowLightToggle(Settings.LowLightToggleDefault);
        LowLightIntensitySlider.IsEnabled = false;
        LowLightIntensitySlider.Value = Settings.LowLightIntensityDefault;
    }

    /// <summary>
    /// Logic hit when toggleing default alert notifications switch.
    /// Turns defaults on or off.
    /// </summary>
    void DefaultAlertNotificationsToggled(object sender, ToggledEventArgs e)
    {
        DefaultSwitch.IsToggled = e.Value;
        SettingsObj.SetNotificationSettingsDefault(e.Value);
        if (e.Value)
        {
            setAllToDefault();
        }
        else
        {
            turnOffAuditoryNotificationsDefault();
            AuditoryNotificationsSwitch.IsEnabled = true;

            TactileNotificationsSwitch.IsEnabled = true;

            LowLightNotificationsSwitch.IsEnabled = true;
            LowLightIntensitySlider.IsEnabled = true;
        }
    }

    /// <summary>
    /// Logic hit when toggleing default auditory notifications switch.
    /// Turns defaults on or off.
    /// </summary>
    void AuditorySwitchToggled(object sender, ToggledEventArgs e)
    {
        AuditoryNotificationsSwitch.IsToggled = e.Value;
        SettingsObj.SetAuditoryDefault(e.Value);
        if (!e.Value)
        {
            setAuditoryNotificationsToDefault();
        } 
         else
        {
            turnOffAuditoryNotificationsDefault();
        }
    }
    /// <summary>
    /// Restores all auditory notifications to default settings.
    /// </summary>
    void setAuditoryNotificationsToDefault()
    {
        AuditoryAlertsVolumeSlider.Value = Settings.VolumeDefault;
        AuditoryAlertsVolumeSlider.IsEnabled = false;
        SettingsObj.SetVolume(Settings.VolumeDefault);
    }

    /// <summary>
    /// Re-enables user modification of auditory settings.
    /// </summary>
    void turnOffAuditoryNotificationsDefault()
    {
        AuditoryAlertsVolumeSlider.IsEnabled = true;
    }

    /// <summary>
    /// Logic hit when alerts volume slider changes. Changes value locally and internally.
    /// </summary>
    void AuditoryAlertsVolumeSliderChanged(object sender, ValueChangedEventArgs args)
    {
        SettingsObj.SetVolume(args.NewValue);
    }


    
    /// <summary>
    /// Logic hit when tactile switch is toggled. Enables/disables tactile features.
    /// </summary>
    void TactileSwitchToggled(object sender, ToggledEventArgs e)
    {
        TactileNotificationsSwitch.IsToggled = e.Value;
        SettingsObj.SetTactileSettingsDefault(e.Value);
    }


   

 


    /// <summary>
    /// Logic hit when log light switch is toggled. Turned Low light features on or off.
    /// </summary>

    void LowLightNotificationsSwitchToggled(object sender, ToggledEventArgs e)
    {
        LowLightNotificationsSwitch.IsToggled = e.Value;
        SettingsObj.SetLowLightToggle(e.Value);
        if (!e.Value)
        {
            LowLightIntensitySlider.Value = Settings.LowLightIntensityDefault;
            LowLightIntensitySlider.IsEnabled = false;
        } else
        {
            LowLightIntensitySlider.IsEnabled = true;
        }
    }

    /// <summary>
    /// Logic hit when low light slider changes. Changes value locally and internally.
    /// </summary>
    void LowLightIntensitySliderChanged(object sender, ValueChangedEventArgs args)
    {
        SettingsObj.SetLowLightIntensity(args.NewValue);
    }

    /// <summary>
    /// Will exit current settings page and return to calling location.
    /// </summary>
    private void ReturnHome(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}