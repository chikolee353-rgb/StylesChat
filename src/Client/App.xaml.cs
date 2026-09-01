using Microsoft.Maui.Controls;

namespace Client;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Show login first; if token present go to chat
        var token = Preferences.Get("accessToken", string.Empty);
        if (!string.IsNullOrEmpty(token))
        {
            MainPage = new NavigationPage(new Pages.ChatPage());
        }
        else
        {
            MainPage = new NavigationPage(new Pages.LoginPage());
        }
    }
}
