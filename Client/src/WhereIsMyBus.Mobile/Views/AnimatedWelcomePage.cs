using Microsoft.Maui.Graphics;

namespace WhereIsMyBus.Mobile.Views;

public sealed class AnimatedWelcomePage : ContentPage
{
    private const string AnimationName = "WelcomeAnimation";

    private readonly Func<Page> _createNextPage;
    private readonly GraphicsView _art;
    private readonly WelcomeDrawing _drawing = new();

    private bool _running;
    private bool _finished;

    public AnimatedWelcomePage(Func<Page> createNextPage)
    {
        _createNextPage = createNextPage;

        BackgroundColor = Color.FromArgb("#08172E");

        Shell.SetNavBarIsVisible(this, false);
        NavigationPage.SetHasNavigationBar(this, false);

        _art = new GraphicsView
        {
            Drawable = _drawing,
            InputTransparent = true,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        var skipButton = new Button
        {
            Text = "Skip intro",
            TextColor = Color.FromArgb("#C2D2EB"),
            BackgroundColor = Colors.Transparent,
            FontSize = 14,
            MinimumHeightRequest = 48,
            HorizontalOptions = LayoutOptions.End,
            Margin = new Thickness(16, 16, 16, 0)
        };

        skipButton.Clicked += (_, _) => OpenApp();

        var footer = new Label
        {
            Text = "YOUR JOURNEY, MADE SIMPLE",
            TextColor = Color.FromArgb("#8196B8"),
            FontSize = 10,
            CharacterSpacing = 2,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(16, 12, 16, 28)
        };

        var layout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        layout.Add(skipButton, 0, 0);
        layout.Add(_art, 0, 1);
        layout.Add(footer, 0, 2);

        Content = layout;

        // Start only when the drawing surface has a usable size.
        _art.SizeChanged += (_, _) => StartAnimation();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StartAnimation();
    }

    private void StartAnimation()
    {
        if (_running || _finished ||
            _art.Width <= 0 || _art.Height <= 0 ||
            Window is null)
        {
            return;
        }

        _running = true;

        var animation = new Animation(
            value =>
            {
                _drawing.Progress = (float)value;
                _art.Invalidate();
            },
            start: 0,
            end: 1);

        animation.Commit(
            this,
            AnimationName,
            rate: 16,
            length: 5600,
            easing: Easing.Linear,
            finished: (_, cancelled) =>
            {
                _running = false;

                if (!cancelled)
                    OpenApp();
            });
    }

    private void OpenApp()
    {
        if (_finished || Window is null)
            return;

        var window = Window;

        // Create the destination before marking the intro complete.
        var nextPage = _createNextPage();

        _finished = true;
        this.AbortAnimation(AnimationName);

        // Replace the intro so Back does not return to it.
        window.Page = nextPage;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        this.AbortAnimation(AnimationName);
        _running = false;
    }

    private sealed class WelcomeDrawing : IDrawable
    {
        public float Progress { get; set; }

        private static readonly Color Navy =
            Color.FromArgb("#08172E");

        private static readonly Color Blue =
            Color.FromArgb("#3986FF");

        private static readonly Color Mint =
            Color.FromArgb("#60E5C0");

        private static float Segment(float value, float start, float end) =>
            Math.Clamp((value - start) / (end - start), 0f, 1f);

        private static float Smooth(float value) =>
            value * value * (3f - 2f * value);

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Navy;
            canvas.FillRectangle(dirtyRect);

            // Fixed design coordinates scaled to fit the available screen.
            var scale = Math.Min(
                dirtyRect.Width / 360f,
                dirtyRect.Height / 500f);

            if (scale <= 0)
                return;

            canvas.SaveState();

            canvas.Translate(
                (dirtyRect.Width - 360f * scale) / 2f,
                (dirtyRect.Height - 500f * scale) / 2f);

            canvas.Scale(scale, scale);

            DrawBackground(canvas);

            var sceneOpacity =
                1f - Segment(Progress, 0.65f, 0.76f);

            if (sceneOpacity > 0)
            {
                canvas.SaveState();
                canvas.Alpha = sceneOpacity;
                DrawScene(canvas);
                canvas.RestoreState();
            }

            var logoProgress =
                Segment(Progress, 0.72f, 0.9f);

            if (logoProgress > 0)
            {
                canvas.SaveState();
                canvas.Alpha = logoProgress;

                var logoScale = 0.88f + 0.12f * Smooth(logoProgress);

                canvas.Translate(180, 245);
                canvas.Scale(logoScale, logoScale);
                canvas.Translate(-180, -245);

                DrawLogo(canvas);

                canvas.RestoreState();
            }

            // Subtle progress indicator.
            canvas.FillColor = Color.FromArgb("#20314C");
            canvas.FillRoundedRectangle(140, 456, 80, 3, 2);

            canvas.FillColor = Mint;
            canvas.FillRoundedRectangle(
                140, 456, 80 * Progress, 3, 2);

            canvas.RestoreState();
        }

        private static void DrawBackground(ICanvas canvas)
        {
            canvas.FillColor = Color.FromArgb("#102441");
            canvas.FillCircle(180, 225, 140);

            canvas.StrokeColor = Color.FromArgb("#193351");
            canvas.StrokeSize = 1;
            canvas.DrawCircle(180, 225, 160);

            canvas.FillColor = Mint;
            canvas.FillCircle(48, 153, 3);

            canvas.FillColor = Blue;
            canvas.FillCircle(306, 111, 4);
            canvas.FillCircle(307, 337, 3);
        }

        private void DrawScene(ICanvas canvas)
        {
            // Restrict the moving illustration to its own scene.
            canvas.SaveState();
            canvas.ClipRectangle(10, 105, 340, 245);

            // City silhouettes.
            canvas.FillColor = Color.FromArgb("#19304E");
            canvas.FillRoundedRectangle(22, 207, 42, 98, 5);
            canvas.FillRoundedRectangle(76, 175, 53, 130, 5);
            canvas.FillRoundedRectangle(253, 191, 44, 114, 5);
            canvas.FillRoundedRectangle(310, 222, 34, 83, 5);

            canvas.FillColor = Color.FromArgb("#294363");

            for (var row = 0; row < 4; row++)
            {
                canvas.FillRoundedRectangle(
                    87, 189 + row * 23, 9, 10, 2);

                canvas.FillRoundedRectangle(
                    108, 189 + row * 23, 9, 10, 2);
            }

            // Road.
            canvas.FillColor = Color.FromArgb("#233A58");
            canvas.FillRoundedRectangle(10, 310, 340, 3, 2);

            canvas.FillColor = Color.FromArgb("#405777");

            for (var x = 25; x < 340; x += 48)
                canvas.FillRoundedRectangle(x, 329, 24, 2, 1);

            // Bus slides into the stop, then drives away.
            var arrival = Smooth(Segment(Progress, 0.22f, 0.43f));
            var departure = Smooth(Segment(Progress, 0.60f, 0.72f));

            var busX = -205f + 335f * arrival + 300f * departure;

            if (Progress >= 0.2f)
            {
                canvas.SaveState();
                canvas.Translate(busX, 226);

                DrawBus(canvas);

                // Open the door while the passenger boards.
                if (Progress >= 0.43f && Progress < 0.59f)
                {
                    canvas.FillColor = Navy;
                    canvas.FillRoundedRectangle(30, 18, 24, 51, 4);
                }

                canvas.RestoreState();
            }

            // Person moves toward the bus door.
            var boarding = Smooth(Segment(Progress, 0.45f, 0.57f));
            var personX = 82f + 88f * boarding;
            var personOpacity =
                1f - Segment(Progress, 0.53f, 0.59f);

            canvas.SaveState();
            canvas.Alpha = personOpacity;
            canvas.Translate(personX, 239 - 9 * boarding);

            DrawPerson(canvas, boarding > 0 && boarding < 1);

            canvas.RestoreState();

            // Search bubble above the phone.
            if (Progress < 0.45f)
            {
                canvas.SaveState();

                canvas.Alpha =
                    Smooth(Segment(Progress, 0f, 0.1f));

                canvas.FillColor = Colors.White;
                canvas.FillRoundedRectangle(43, 135, 157, 47, 15);

                canvas.FontColor = Color.FromArgb("#14375F");
                canvas.FontSize = 13;

                canvas.DrawString(
                    Progress < 0.23f
                        ? "Finding my bus..."
                        : "My bus is here!",
                    43, 135, 157, 47,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Center);

                canvas.RestoreState();
            }

            canvas.RestoreState();

            canvas.FontColor = Colors.White;
            canvas.FontSize = 25;

            canvas.DrawString(
                Progress < 0.23f
                    ? "Find your bus."
                    : Progress < 0.45f
                        ? "See it arrive."
                        : "Start your journey.",
                10, 365, 340, 44,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);

            canvas.FontColor = Color.FromArgb("#A6BCD9");
            canvas.FontSize = 14;

            canvas.DrawString(
                "Less waiting. More moving.",
                10, 410, 340, 28,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }

        private void DrawPerson(ICanvas canvas, bool walking)
        {
            var stride = walking
                ? MathF.Sin(Progress * 150f) * 6f
                : 0f;

            // Head.
            canvas.FillColor = Color.FromArgb("#F1BB94");
            canvas.FillCircle(0, 0, 10);

            // Hair.
            canvas.FillColor = Color.FromArgb("#172234");
            canvas.FillRoundedRectangle(-10, -11, 20, 8, 4);

            // Shirt.
            canvas.FillColor = Mint;
            canvas.FillRoundedRectangle(-11, 13, 23, 31, 8);

            // Legs.
            canvas.StrokeSize = 7;
            canvas.StrokeColor = Color.FromArgb("#B6C9E6");
            canvas.DrawLine(-5, 43, -8 + stride, 65);
            canvas.DrawLine(6, 43, 10 - stride, 65);

            // Shoes.
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 5;
            canvas.DrawLine(-10 + stride, 68, -1 + stride, 68);
            canvas.DrawLine(7 - stride, 68, 17 - stride, 68);

            // Arm holding the phone.
            canvas.StrokeColor = Color.FromArgb("#F1BB94");
            canvas.StrokeSize = 6;
            canvas.DrawLine(9, 22, 22, 30);
            canvas.DrawLine(22, 30, 29, 17);

            // Phone.
            canvas.FillColor = Color.FromArgb("#071326");
            canvas.FillRoundedRectangle(24, 2, 13, 22, 4);

            canvas.FillColor = Blue;
            canvas.FillRoundedRectangle(27, 5, 7, 13, 2);
        }

        private static void DrawBus(ICanvas canvas)
        {
            canvas.FillColor = Blue;
            canvas.FillRoundedRectangle(0, 0, 190, 72, 14);

            canvas.FillColor = Color.FromArgb("#C7E4FF");

            for (var i = 0; i < 4; i++)
            {
                canvas.FillRoundedRectangle(
                    65 + i * 28, 12, 22, 25, 5);
            }

            // Door.
            canvas.FillColor = Color.FromArgb("#14375F");
            canvas.FillRoundedRectangle(30, 12, 24, 57, 5);

            canvas.FillColor = Color.FromArgb("#B7DDFF");
            canvas.FillRoundedRectangle(34, 17, 16, 27, 3);

            // Accent stripe.
            canvas.FillColor = Mint;
            canvas.FillRoundedRectangle(65, 46, 104, 4, 2);

            // Wheels.
            foreach (var x in new[] { 24f, 157f })
            {
                canvas.FillColor = Color.FromArgb("#061021");
                canvas.FillCircle(x, 72, 12);

                canvas.FillColor = Color.FromArgb("#A9BDD9");
                canvas.FillCircle(x, 72, 5);
            }

            // Headlight.
            canvas.FillColor = Color.FromArgb("#FFF0B5");
            canvas.FillRoundedRectangle(179, 49, 8, 10, 3);
        }

        private static void DrawLogo(ICanvas canvas)
        {
            // Custom location-pin mark.
            var pin = new PathF();

            pin.MoveTo(180, 278);
            pin.CurveTo(159, 252, 124, 222, 124, 189);
            pin.CurveTo(124, 158, 149, 134, 180, 134);
            pin.CurveTo(211, 134, 236, 158, 236, 189);
            pin.CurveTo(236, 222, 201, 252, 180, 278);
            pin.Close();

            canvas.FillColor = Blue;
            canvas.FillPath(pin);

            // Front-facing bus inside the pin.
            canvas.FillColor = Colors.White;
            canvas.FillRoundedRectangle(153, 162, 54, 61, 12);

            canvas.FillColor = Navy;
            canvas.FillRoundedRectangle(159, 174, 42, 23, 5);

            canvas.FillColor = Mint;
            canvas.FillRoundedRectangle(169, 166, 22, 4, 2);

            canvas.FillColor = Blue;
            canvas.FillCircle(164, 208, 4);
            canvas.FillCircle(196, 208, 4);

            canvas.FillColor = Colors.White;
            canvas.FillRoundedRectangle(158, 218, 9, 11, 3);
            canvas.FillRoundedRectangle(193, 218, 9, 11, 3);

            // Brand name.
            canvas.FontColor = Colors.White;
            canvas.FontSize = 29;

            canvas.DrawString(
                "Where Is My Bus",
                10, 307, 340, 48,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);

            canvas.FontColor = Color.FromArgb("#ADC2DF");
            canvas.FontSize = 15;

            canvas.DrawString(
                "Your bus. Your journey.",
                10, 361, 340, 30,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);
        }
    }
}