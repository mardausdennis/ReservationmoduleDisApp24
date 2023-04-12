using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui;

namespace DisApp24;


public partial class App : Application
{
	public App(AppShell appShell)
	{
		InitializeComponent();

        MainPage = appShell;
	}
}
