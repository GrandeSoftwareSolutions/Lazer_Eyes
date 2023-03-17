/*
 * LazerEyes Settings.xaml.cs
 * Settings overview page, loads specific settings pages
 * Course- IST440
 * Author- Cameron Grande
 * Date Developed- 9/26/22
 * Last Changed- 9/26/22
 * Rev: 1
 */
using Lazer_Eyes.Resources.Languages;

namespace Lazer_Eyes;

public partial class SettingsMenu : ContentPage
{
	public SettingsMenu()
	{
		InitializeComponent();
#if IOS
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior()
        {
            TextOverride = ApplicationResource.Back,

        });
#endif
    }

    private void AlertNotifications(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AlertNotifications());
    }

    private void AlertSettings(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AlertSettings());
    }
    private void Help(object sender, EventArgs e)
    {
        Navigation.PushAsync(new Help());
    }

    private void ReturnHome(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}