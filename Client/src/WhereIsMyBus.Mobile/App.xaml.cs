using Microsoft.Extensions.DependencyInjection;
using WhereIsMyBus.Mobile.Views;

namespace WhereIsMyBus.Mobile;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        return new Window(
            new AnimatedWelcomePage(
                () => _services.GetRequiredService<AppShell>()));
    }
}