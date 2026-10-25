using Microsoft.Maui.Controls;
using Client.ViewModels;

namespace Client.Pages;

// Partial class backing the XAML ChatPage. UI elements are defined in ChatPage.xaml.
public partial class ChatPage : ContentPage
{
    public ChatPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await ((ChatViewModel)BindingContext).InitializeAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Chat unavailable", ex.Message, "OK");
        }
    }
}
