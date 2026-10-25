namespace STYLES_CHAT
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Client.Pages.ChatPage), typeof(Client.Pages.ChatPage));
        }
    }
}
