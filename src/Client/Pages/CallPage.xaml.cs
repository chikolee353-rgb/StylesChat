using Microsoft.Maui.Controls;

namespace Client.Pages;

public class CallPage : ContentPage
{
    public CallPage()
    {
        Title = "Call";
        Content = new StackLayout
        {
            Children =
            {
                new Label { Text = "Call UI placeholder - integrate WebRTC or a third-party SDK here" }
            }
        };
    }
}
