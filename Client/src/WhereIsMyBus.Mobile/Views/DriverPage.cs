using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using WhereIsMyBus.Domain.Entities;
using WhereIsMyBus.Mobile.Core.Interfaces;
using WhereIsMyBus.Mobile.Services;
using WhereIsMyBus.Mobile.Views;

public sealed class DriverPage : ContentPage
{
    BusApiClient _obj;
    private readonly IDriverDashboardService _dashboardService;
    private bool _signingIn; 
    // ───────── Palette (light / dark) ─────────
    private static readonly Color Brand = Color.FromArgb("#0866F5");

    private static readonly Color PageLight = Color.FromArgb("#F3F5F9");
    private static readonly Color PageDark = Color.FromArgb("#0B1020");
    private static readonly Color SurfaceLight = Color.FromArgb("#FFFFFF");
    private static readonly Color SurfaceDark = Color.FromArgb("#161D33");
    private static readonly Color FieldLight = Color.FromArgb("#F3F5F9");
    private static readonly Color FieldDark = Color.FromArgb("#0F1529");
    private static readonly Color LineLight = Color.FromArgb("#E3E7EF");
    private static readonly Color LineDark = Color.FromArgb("#2A3352");
    private static readonly Color InkLight = Color.FromArgb("#0E1730");
    private static readonly Color InkDark = Color.FromArgb("#F2F5FF");
    private static readonly Color MutedLight = Color.FromArgb("#5B6785");
    private static readonly Color MutedDark = Color.FromArgb("#9AA6C4");
    private static readonly Color PlaceholderLight = Color.FromArgb("#9AA3B8");
    private static readonly Color PlaceholderDark = Color.FromArgb("#6C7796");

    private static readonly Color ErrorLight = Color.FromArgb("#B42318");
    private static readonly Color ErrorDark = Color.FromArgb("#FDA29B");
    private static readonly Color ErrorTintLight = Color.FromArgb("#FEF3F2");
    private static readonly Color ErrorTintDark = Color.FromArgb("#3A1717");
    private static readonly Color ErrorLineLight = Color.FromArgb("#FECDCA");
    private static readonly Color ErrorLineDark = Color.FromArgb("#7A2E2A");

    private static readonly Color WarnLight = Color.FromArgb("#9A4B00");
    private static readonly Color WarnDark = Color.FromArgb("#FFC46B");
    private static readonly Color WarnTintLight = Color.FromArgb("#FFF7E8");
    private static readonly Color WarnTintDark = Color.FromArgb("#2E2210");
    private static readonly Color WarnLineLight = Color.FromArgb("#FADCA8");
    private static readonly Color WarnLineDark = Color.FromArgb("#5C4316");

    private static readonly Color NoticeLight = Color.FromArgb("#F0FDF4");
    private static readonly Color NoticeDark = Color.FromArgb("#10261B");
    private static readonly Color NoticeLineLight = Color.FromArgb("#BBF7D0");
    private static readonly Color NoticeLineDark = Color.FromArgb("#1F4D35");
    private static readonly Color NoticeTextLight = Color.FromArgb("#166534");
    private static readonly Color NoticeTextDark = Color.FromArgb("#86EFAC");

    // ───────── Theme plumbing ─────────
    // Each action re-applies one themed value. All of them run when the theme changes.
    private readonly List<Action<bool>> themeActions = new();
    private bool messageIsError;

    private static bool IsDarkTheme =>
        Application.Current?.RequestedTheme == AppTheme.Dark;

    // ───────── Controls ─────────
    private readonly Entry driverId;
    private readonly Entry password;
    private readonly Border messageBox;
    private readonly Label messageLabel;
    HttpClient _client;
    public DriverPage(HttpClient client)
    {
        _obj = new BusApiClient(client);
        _client = client;
        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
        ThemeProperty(this, VisualElement.BackgroundColorProperty, PageLight, PageDark);

        driverId = new Entry
        {
            Placeholder = "Enter your driver mobile no",
            Keyboard = Keyboard.Plain,
            ReturnType = ReturnType.Next,
            ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
            IsSpellCheckEnabled = false,
            IsTextPredictionEnabled = false,
            FontSize = 17,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetDescription(driverId, "Driver ID");
        ThemeEntry(driverId);

        password = new Entry
        {
            Placeholder = "Enter your password",
            IsPassword = true,
            Keyboard = Keyboard.Text,
            ReturnType = ReturnType.Go,
            IsSpellCheckEnabled = false,
            IsTextPredictionEnabled = false,
            FontSize = 17,
            BackgroundColor = Colors.Transparent,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetDescription(password, "Password");
        ThemeEntry(password);

        var passwordToggle = new Button
        {
            Text = "Show",
            BackgroundColor = Colors.Transparent,
            TextColor = Brand,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            Padding = new Thickness(10, 0),
            MinimumHeightRequest = 44,
            VerticalOptions = LayoutOptions.Center
        };
        SemanticProperties.SetDescription(passwordToggle, "Show or hide password");
        passwordToggle.Clicked += (_, _) =>
        {
            password.IsPassword = !password.IsPassword;
            passwordToggle.Text = password.IsPassword ? "Show" : "Hide";
        };

        var passwordGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };
        passwordGrid.Add(password);
        passwordGrid.Add(passwordToggle, 1, 0);

        // Enter on the ID field moves to the password; Enter on the password signs in.
        driverId.Completed += (_, _) => password.Focus();

        messageLabel = new Label
        {
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            LineBreakMode = LineBreakMode.WordWrap
        };

        messageBox = new Border
        {
            IsVisible = false,
            StrokeThickness = 1,
            Padding = new Thickness(14, 12),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = messageLabel
        };
        themeActions.Add(_ => ApplyMessageTone());

        var signIn = new Button
        {
            Text = "Sign in",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            BackgroundColor = Brand,
            CornerRadius = 14,
            MinimumHeightRequest = 54,
            Margin = new Thickness(0, 4, 0, 0)
        };
        SemanticProperties.SetDescription(signIn, "Sign in");
        signIn.Clicked += OnSignInClicked;
        password.Completed += OnSignInClicked;

        var driverShell = CreateFieldShell(driverId);
        var passwordShell = CreateFieldShell(passwordGrid);

        var hintRow = new HorizontalStackLayout
        {
            Spacing = 8,
            Children =
            {
                new Label
                {
                    Text = "🔒",
                    FontSize = 14,
                    VerticalOptions = LayoutOptions.Center
                },
                CreateMutedLabel("Your details are protected and only used to sign you in.", 13)
            }
        };

        var card = new Border
        {
            Padding = new Thickness(20),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 22 },
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Color.FromArgb("#0E1730")),
                Opacity = 0.08f,
                Radius = 18,
                Offset = new Point(0, 6)
            },
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children =
                {
                    CreateFieldLabel("Driver ID"),
                    driverShell,
                    CreateFieldLabel("Password"),
                    passwordShell,
                    hintRow,
                    messageBox,
                    signIn
                }
            }
        };
        ThemeProperty(card, Border.StrokeProperty, LineLight, LineDark);
        ThemeProperty(card, VisualElement.BackgroundColorProperty, SurfaceLight, SurfaceDark);

        var noticeLabel = new Label
        {
            Text = "Need access? Ask your transport administrator to create a driver account.",
            FontSize = 14,
            LineBreakMode = LineBreakMode.WordWrap
        };
        ThemeProperty(noticeLabel, Label.TextColorProperty, NoticeTextLight, NoticeTextDark);

        var notice = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(16, 14),
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Content = noticeLabel
        };
        ThemeProperty(notice, Border.StrokeProperty, NoticeLineLight, NoticeLineDark);
        ThemeProperty(notice, VisualElement.BackgroundColorProperty, NoticeLight, NoticeDark);

        // Hero header with gradient and rounded bottom edge
        // Full-width image with title overlay
        var headerGrid = new Grid
        {
            HeightRequest = 280,
            HorizontalOptions = LayoutOptions.Fill,
            IsClippedToBounds = true
        };

        // Background image
        headerGrid.Add(new Image
        {
            Source = "driver_steering.png",
            Aspect = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        });

        // Dark gradient makes the title readable
        headerGrid.Add(new BoxView
        {
            InputTransparent = true,
            Background = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops =
        {
            new GradientStop(Color.FromArgb("#D90A1630"), 0f),
            new GradientStop(Color.FromArgb("#800A1630"), 0.35f),
            new GradientStop(Color.FromArgb("#000A1630"), 0.75f)
        }
            }
        });

        // Title and gold underline
        headerGrid.Add(new VerticalStackLayout
        {
            Margin = new Thickness(20, 28, 20, 0),
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start,
            InputTransparent = true,
            Children =
    {
        new Label
        {
            Text = "Driver Login",
            TextColor = Colors.White,
            FontSize = 32,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 0.5,
            HorizontalTextAlignment = TextAlignment.Center,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.Black),
                Offset = new Point(0, 3),
                Radius = 10,
                Opacity = 0.9f
            }
        },

        new Border
        {
            WidthRequest = 64,
            HeightRequest = 4,
            Padding = 0,
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb("#FFD166"),
            HorizontalOptions = LayoutOptions.Center,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 2
            },
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(
                    Color.FromArgb("#FFD166")),
                Offset = new Point(0, 1),
                Radius = 8,
                Opacity = 0.65f
            }
        }
    }
        });

        var header = new Border
        {
            Padding = 0,
            Margin = 0,
            StrokeThickness = 0,
            BackgroundColor = Color.FromArgb("#0A1F4D"),
            HorizontalOptions = LayoutOptions.Fill,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(0, 0, 32, 32)
            },
            Content = headerGrid
        };

        // Body column: overlaps the header, capped in width for tablets and landscape
        var body = new VerticalStackLayout
        {
            Padding = new Thickness(16, 0, 16, 32),
            Spacing = 16,
            Margin = new Thickness(0, -24, 0, 0),
            MaximumWidthRequest = 480,
            HorizontalOptions = LayoutOptions.Center,
            Children = { card, notice }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 0,
                Children = { header, body }
            }
        };

        // Application.Current can be null during construction, so subscribe in OnAppearing
        ApplyTheme();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyTheme();

        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged -= OnRequestedThemeChanged;
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) => ApplyTheme();

    // ───────── Theme helpers ─────────

    private void ApplyTheme()
    {
        bool dark = IsDarkTheme;
        foreach (var action in themeActions)
            action(dark);
    }

    private void ThemeProperty(BindableObject target, BindableProperty property, Color light, Color dark)
    {
        themeActions.Add(isDark => target.SetValue(property, isDark ? dark : light));
        target.SetValue(property, IsDarkTheme ? dark : light);
    }

    private void ThemeEntry(Entry entry)
    {
        ThemeProperty(entry, Entry.TextColorProperty, InkLight, InkDark);
        ThemeProperty(entry, Entry.PlaceholderColorProperty, PlaceholderLight, PlaceholderDark);
    }

    private void ApplyMessageTone()
    {
        bool dark = IsDarkTheme;

        if (messageIsError)
        {
            messageLabel.TextColor = dark ? ErrorDark : ErrorLight;
            messageBox.BackgroundColor = dark ? ErrorTintDark : ErrorTintLight;
            messageBox.Stroke = dark ? ErrorLineDark : ErrorLineLight;
        }
        else
        {
            messageLabel.TextColor = dark ? WarnDark : WarnLight;
            messageBox.BackgroundColor = dark ? WarnTintDark : WarnTintLight;
            messageBox.Stroke = dark ? WarnLineDark : WarnLineLight;
        }
    }

    // ───────── Layout helpers ─────────

    private Border CreateFieldShell(View content)
    {
        var shell = new Border
        {
            StrokeThickness = 1,
            Padding = new Thickness(14, 0),
            MinimumHeightRequest = 54,
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Content = content
        };
        ThemeProperty(shell, Border.StrokeProperty, LineLight, LineDark);
        ThemeProperty(shell, VisualElement.BackgroundColorProperty, FieldLight, FieldDark);
        return shell;
    }

    private Label CreateFieldLabel(string text)
    {
        var label = new Label
        {
            Text = text,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(2, 0, 0, 0)
        };
        ThemeProperty(label, Label.TextColorProperty, MutedLight, MutedDark);
        return label;
    }

    private Label CreateMutedLabel(string text, double size)
    {
        var label = new Label
        {
            Text = text,
            FontSize = size,
            LineBreakMode = LineBreakMode.WordWrap,
            VerticalOptions = LayoutOptions.Center
        };
        ThemeProperty(label, Label.TextColorProperty, MutedLight, MutedDark);
        return label;
    }

    // ───────── Behavior ─────────

    private void ShowMessage(string text, bool isError)
    {
        messageLabel.Text = text;
        messageIsError = isError;
        ApplyMessageTone();
        messageBox.IsVisible = true;
    }

    private async void OnSignInClicked(object? sender, EventArgs e)
    {
        if (_signingIn)
            return;

        var id = driverId.Text?.Trim();
        var enteredPassword = password.Text;

        if (string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(enteredPassword))
        {
            ShowMessage(
                "Enter your mobile number and password to continue.",
                isError: true);

            if (string.IsNullOrWhiteSpace(id))
                driverId.Focus();
            else
                password.Focus();

            return;
        }

        _signingIn = true;

        try
        {
            var request = new DriverDetails
            {
                DriverMobile = id,
                DriverAlternateMobile = id,
                SecondDriverMobile = id,
                SecondDriverAlternateMobile = id,
                Password = enteredPassword
            };

            var response = await _obj.GetDriverInformationAsync(request);

            if (response is null)
            {
                ShowMessage(
                    "Invalid mobile number or password.",
                    isError: true);
                return;
            }

            //var mobile = ("9876543210");
            var mobile = id;

            var isPrimaryDriver =
                mobile == (response.DriverMobile) ||
                mobile == (response.DriverAlternateMobile);

            var isSecondDriver =
                mobile == (response.SecondDriverMobile) ||
                mobile == (response.SecondDriverAlternateMobile);

            if (!isPrimaryDriver && !isSecondDriver)
            {
                ShowMessage(
                    "The returned driver account does not match your mobile number.",
                    isError: true);
                return;
            }

            var name = isPrimaryDriver ? response.DriverName : response.SecondDriverName;

            password.Text = "";

            await Navigation.PushAsync(new DriverDashboardPage(_dashboardService, _client, name ?? "Driver", id));
        }
        catch (UnauthorizedAccessException)
        {
            ShowMessage(
                "Invalid mobile number or password.",
                isError: true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);

            ShowMessage(
                "Unable to sign in. Please try again.",
                isError: true);
        }
        finally
        {
            _signingIn = false;
        }
    }

}