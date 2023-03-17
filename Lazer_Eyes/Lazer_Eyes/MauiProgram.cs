/*
 * LazerEyes MauiProgram.cs
 * Generated code to launch MauiApp
 * Course- IST440
 * Author- Cameron Grande, Iliza Nazeraj
 * Date Developed- 9/26/22
 * Last Changed- 10/22/22
 * Rev: 1
 */

using Plugin.Maui.Audio;

namespace Lazer_Eyes;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("telerikfontexamples.ttf", "TelerikFont");
            })
            .ConfigureMauiHandlers(handlers => {
#if IOS
                handlers.AddHandler(typeof(Shell), typeof(CustomShellRenderer));
#endif
            });
		builder.Services.AddSingleton(AudioManager.Current);
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddLocalization();

		return builder.Build();
	}
}