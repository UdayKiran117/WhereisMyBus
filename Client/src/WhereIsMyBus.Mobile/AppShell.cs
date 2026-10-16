using WhereIsMyBus.Mobile.Views;

namespace WhereIsMyBus.Mobile;

public sealed class AppShell : Shell
{
    public AppShell(SearchPage search)
    {
        FlyoutBehavior = FlyoutBehavior.Disabled;
        Title = "Where Is My Bus";

        // Passenger page only.
        Items.Add(new ShellContent
        {
            Title = "Find bus",
            Route = "passenger",
            Content = search
        });
    }
}