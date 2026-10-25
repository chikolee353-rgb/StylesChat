using Microsoft.Maui.Controls;
using Client.Services;
using System.Net.Http.Json;

namespace Client.Pages;

public class LoginPage : ContentPage
{
    private readonly Entry _username;
    private readonly Entry _password;
    private readonly Button _loginButton;
    private readonly Button _registerButton;

    public LoginPage()
    {
        Title = "Sign in";
        _username = new Entry { Placeholder = "Username" };
        _password = new Entry { Placeholder = "Password", IsPassword = true };
        _loginButton = new Button { Text = "Login" };
        _registerButton = new Button { Text = "Create account" };
        _loginButton.Clicked += (_, _) => AuthenticateAsync(isRegistration: false);
        _registerButton.Clicked += (_, _) => AuthenticateAsync(isRegistration: true);

        Content = new StackLayout
        {
            Padding = 20,
            VerticalOptions = LayoutOptions.Center,
            Children = { _username, _password, _loginButton, _registerButton }
        };
    }

    private async void AuthenticateAsync(bool isRegistration)
    {
        var username = _username.Text?.Trim();
        var password = _password.Text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlertAsync("Error", "Provide username and password", "OK");
            return;
        }

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri(ApiConfiguration.BaseUrl) };
            var endpoint = isRegistration ? "api/auth/register" : "api/auth/login";
            using var resp = await client.PostAsJsonAsync(endpoint, new { Username = username, Password = password });
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadFromJsonAsync<ErrorResponse>();
                await DisplayAlertAsync(isRegistration ? "Registration failed" : "Login failed",
                    error?.Error ?? resp.ReasonPhrase ?? "The request failed.", "OK");
                return;
            }

            var obj = await resp.Content.ReadFromJsonAsync<LoginResponse>();
            if (obj != null)
            {
                Preferences.Set("accessToken", obj.AccessToken);
                await Shell.Current.GoToAsync(nameof(ChatPage));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}
