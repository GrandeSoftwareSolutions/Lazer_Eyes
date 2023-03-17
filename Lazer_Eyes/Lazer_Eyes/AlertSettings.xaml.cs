/*
 * LazerEyes AlertSettings.xaml.cs
 * Code responsible for handling _SettingsObj related to alerts
 * Course- IST440
 * Author- Cameron Grande
 * Date Developed- 9/26/22
 * Last Changed- 12/02/22
 * Rev: 5
 */

using Lazer_Eyes.Resources.Languages;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace Lazer_Eyes;

public partial class AlertSettings : ContentPage
{
    public Settings SettingsObj;

    /// <summary>
    /// Constructor- loads page and initial settings.
    /// </summary>
    public AlertSettings()
	{
		InitializeComponent();
        InitializeUnitsPicker();
        SettingsObj = Settings.Get();
        DefaultSwitch.IsToggled = SettingsObj.GetAlertSettingsDefault();
#if IOS
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior()
        {
            TextOverride = ApplicationResource.Back,

        });
#endif
        if (DefaultSwitch.IsToggled)
        {
            setAllSettingsToDefault();
        }
        else
        {
            ThresholdSlider.Value = SettingsObj.GetDistanceThreshold();
            UnitsPicker.SelectedIndex = SettingsObj.GetDistanceUnit();
            StrideSwitch.IsToggled = SettingsObj.GetMeasureDistanceInStrides();
        }

        if (!StrideSwitch.IsToggled)
        {
            StrideSlider.IsEnabled = false;
            StrideEntry.IsEnabled = false;
            StrideSlider.Value = 0;
        }
        else
        {
            StrideSlider.IsEnabled = true;
            StrideEntry.IsEnabled = true;
            StrideSlider.Value = SettingsObj.GetStrideLength();
        }
    }

    /// <summary>
    /// Sets the initial units picker value.
    /// </summary>
    void InitializeUnitsPicker()
    {
        foreach (Settings.Units unit in (Settings.Units[])Enum.GetValues(typeof(Settings.Units)))
        {
            switch (unit.ToString())
            {
                case "Meters":
                    UnitsPicker.Items.Add(ApplicationResource.Meters);
                    break;
                case "Feet":
                    UnitsPicker.Items.Add(ApplicationResource.Feet);
                    break;
            }
        }
    }

    /// <summary>
    /// Logic hit when Alert Settings switch is toggled. Enables/ disables default settings.
    /// </summary>
    void DefaultAlertSettingsToggled(object sender, ToggledEventArgs e)
    {
        DefaultSwitch.IsToggled = e.Value;
        SettingsObj.SetAlertSettingsDefault(e.Value);
        if (e.Value)
        {
            setAllSettingsToDefault();
        }
        else
        {
            UnitsPicker.IsEnabled = true;
            ThresholdSlider.IsEnabled = true;
            ThresholdEntry.IsEnabled = true;
            StrideSwitch.IsEnabled = true;
        }
    }

    /// <summary>
    /// Sets all settings to default values.
    /// </summary>
    void setAllSettingsToDefault()
    {
        UnitsPicker.SelectedIndex = Settings.DistanceUnitDefault;
        UnitsPicker.IsEnabled = false;
        SettingsObj.SetDistanceUnit(Settings.DistanceUnitDefault);

        ThresholdSlider.Value = Settings.DistanceThresholdDefault;
        ThresholdSlider.IsEnabled = false;
        ThresholdEntry.IsEnabled = false;
        SettingsObj.SetDistanceThreshold(Settings.DistanceThresholdDefault);

        StrideSwitch.IsEnabled = false;
        StrideSwitch.IsToggled = false;
        StrideSlider.Value = 0;
        StrideEntry.IsEnabled = false;
        StrideSlider.IsEnabled = false;
        SettingsObj.SetStrideLength(0);
    }

    /// <summary>
    /// Logic on change of units picket. Sets value locally and internally.
    /// </summary>
    void UnitsPickerIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;
        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            UnitsPicker.SelectedIndex = selectedIndex;
            SettingsObj.SetDistanceUnit(selectedIndex);
            if (!StrideSwitch.IsToggled)
            {
                StrideSlider.IsEnabled = false;
                StrideSlider.Value = 0;
            }
            else
            {
                StrideSlider.IsEnabled = true;
                StrideEntry.IsEnabled = true;
                StrideSlider.Value = SettingsObj.GetStrideLength();
            }
        }
    }

    /// <summary>
    /// Logic on change of distance threshold slider. Changes value locally and internally.
    /// </summary>
    void DistanceThresholdSliderChanged(object sender, ValueChangedEventArgs args)
    {
        SettingsObj.SetDistanceThreshold(args.NewValue);
    }

    /// <summary>
    /// Logic on change of stride slider. Changes value locally and internally.
    /// </summary>
    void StrideSliderChanged(object sender, ValueChangedEventArgs args)
    {
        SettingsObj.SetStrideLength(args.NewValue);
    }

    void StrideSwitchToggled(object sender, ToggledEventArgs e)
    {
        Console.WriteLine(e.Value);
        SettingsObj.SetMeasureDistanceInStrides(e.Value);
        StrideSwitch.IsToggled = e.Value;
        if (e.Value)
        {
            StrideSlider.IsEnabled = true;
            StrideEntry.IsEnabled = true;
        }
        else
        {
            StrideSlider.IsEnabled = false;
            StrideEntry.IsEnabled = false;
        }
    }

    /// <summary>
    /// Will exit current settings page and return to calling location.
    /// </summary>
    private void ReturnHome(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

}