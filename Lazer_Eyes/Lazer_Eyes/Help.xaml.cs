using Lazer_Eyes.Resources.Languages;

namespace Lazer_Eyes;

public partial class Help : ContentPage
{
	public Help()
	{
		InitializeComponent();
#if IOS
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior()
        {
            TextOverride = ApplicationResource.Back,
           
        });
#endif
    }
    private void ReturnHome(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

}