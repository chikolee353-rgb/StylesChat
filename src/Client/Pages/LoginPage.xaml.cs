using Microsoft.Maui.Controls;
using Client.Services;
using System.Net.Http.Json;

namespace Client.Pages;

public class LoginPage : ContentPage
{
    private readonly Entry _username;
    private readonly Entry _password;
    private readonly Button _loginButton;

    public LoginPage()
    {
        Title = "Sign in";
        _username = new Entry { Placeholder = "Username" };
        _password = new Entry { Placeholder = "Password", IsPassword = true };
        _loginButton = new Button { Text = "Login" };
        _loginButton.Clicked += OnLogin;

        Content = new StackLayout
        {
            Padding = 20,
            VerticalOptions = LayoutOptions.Center,
            Children = { _username, _password, _loginButton }
        };
    }

    private async void OnLogin(object? sender, EventArgs e)
    {
        var username = _username.Text?.Trim();
        var password = _password.Text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Provide username and password", "OK");
            return;
        }

        try
        {
            // Call server login endpoint
            var client = new HttpClient { BaseAddress = new Uri("https://localhost:5001") };
            var payload = new { Username = username, Password = password };
            var resp = await client.PostAsJsonAsync("api/auth/login", payload);
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Login failed", resp.ReasonPhrase, "OK");
                return;
            }

            var obj = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (obj != null)
            {
                // store token in preferences for demo purposes
                Preferences.Set("accessToken", obj.AccessToken);
                // navigate to chat
                await Navigation.PushAsync(new ChatPage());
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }
}
