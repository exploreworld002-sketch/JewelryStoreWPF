using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JewelryStoreWPF
{
    public partial class LoginWindow : Window
    {
        private DispatcherTimer particleTimer;
        private DispatcherTimer logoAnimationTimer;
        private List<Ellipse> particles;
        private Random random = new Random();
        private bool isDarkMode = true;

        // Login credentials
        private const string VALID_USERNAME = "admin";
        private const string VALID_PASSWORD = "admin";

        public LoginWindow()
        {
            InitializeComponent();
            InitializeAnimations();
            CreateParticles();
            StartEntranceAnimations();

            // Set focus to username textbox
            UsernameTextBox.Focus();

            // Add key handlers for Enter key
            UsernameTextBox.KeyDown += InputKeyDown;
            PasswordBox.KeyDown += InputKeyDown;
        }

        private void InitializeAnimations()
        {
            // Logo pulsing animation
            logoAnimationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            logoAnimationTimer.Tick += (s, e) => AnimateLogo();
            logoAnimationTimer.Start();
        }

        private void CreateParticles()
        {
            particles = new List<Ellipse>();

            for (int i = 0; i < 25; i++)
            {
                var particle = new Ellipse
                {
                    Width = random.Next(2, 5),
                    Height = random.Next(2, 5),
                    Fill = new SolidColorBrush(Color.FromArgb((byte)random.Next(30, 100), 255, 255, 255)),
                    Opacity = random.NextDouble() * 0.7 + 0.3
                };

                Canvas.SetLeft(particle, random.Next(0, (int)this.Width));
                Canvas.SetTop(particle, random.Next(0, (int)this.Height));

                ParticleCanvas.Children.Add(particle);
                particles.Add(particle);
            }

            // Start particle animation
            particleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            particleTimer.Tick += (s, e) => AnimateParticles();
            particleTimer.Start();
        }

        private void AnimateParticles()
        {
            foreach (var particle in particles)
            {
                var currentX = Canvas.GetLeft(particle);
                var currentY = Canvas.GetTop(particle);

                var newX = currentX + (random.NextDouble() - 0.5) * 3;
                var newY = currentY + (random.NextDouble() - 0.5) * 3;

                // Wrap around screen
                if (newX < 0) newX = this.ActualWidth;
                if (newX > this.ActualWidth) newX = 0;
                if (newY < 0) newY = this.ActualHeight;
                if (newY > this.ActualHeight) newY = 0;

                Canvas.SetLeft(particle, newX);
                Canvas.SetTop(particle, newY);

                // Random opacity changes
                if (random.NextDouble() < 0.02)
                {
                    var opacityAnimation = new DoubleAnimation(
                        particle.Opacity,
                        random.NextDouble() * 0.7 + 0.3,
                        TimeSpan.FromMilliseconds(1500));
                    particle.BeginAnimation(OpacityProperty, opacityAnimation);
                }
            }
        }

        private void AnimateLogo()
        {
            // Pulsing glow effect
            var glowAnimation = new DoubleAnimation(0.6, 1.0, TimeSpan.FromMilliseconds(1500))
            {
                AutoReverse = true,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            LogoBorder.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, glowAnimation);

            // Subtle scale animation
            var scaleTransform = new ScaleTransform(1.0, 1.0, 100, 100);
            LogoBorder.RenderTransform = scaleTransform;

            var scaleAnimation = new DoubleAnimation(1.0, 1.05, TimeSpan.FromMilliseconds(1500))
            {
                AutoReverse = true,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        private void StartEntranceAnimations()
        {
            // Hide elements initially
            LoginBorder.Opacity = 0;
            LoginBorder.RenderTransform = new TranslateTransform(0, 100);

            FeaturesPanel.Opacity = 0;
            FeaturesPanel.RenderTransform = new TranslateTransform(-50, 0);

            StatsPreview.Opacity = 0;
            StatsPreview.RenderTransform = new TranslateTransform(50, 0);

            LogoBorder.Opacity = 0;
            LogoBorder.RenderTransform = new ScaleTransform(0.5, 0.5, 100, 100);

            // Animate logo entrance
            var logoFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(1000))
            {
                BeginTime = TimeSpan.FromMilliseconds(200),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };
            LogoBorder.BeginAnimation(OpacityProperty, logoFadeIn);

            var logoScaleIn = new DoubleAnimation(0.5, 1.0, TimeSpan.FromMilliseconds(1000))
            {
                BeginTime = TimeSpan.FromMilliseconds(200),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
            };
            LogoBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, logoScaleIn);
            LogoBorder.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, logoScaleIn);

            // Animate login form entrance
            var formFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(600)
            };
            LoginBorder.BeginAnimation(OpacityProperty, formFadeIn);

            var formSlideIn = new DoubleAnimation(100, 0, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(600),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            LoginBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty, formSlideIn);

            // Animate features panel
            var featuresFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(1000)
            };
            FeaturesPanel.BeginAnimation(OpacityProperty, featuresFadeIn);

            var featuresSlideIn = new DoubleAnimation(-50, 0, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            FeaturesPanel.RenderTransform.BeginAnimation(TranslateTransform.XProperty, featuresSlideIn);

            // Animate stats preview
            var statsFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(1200)
            };
            StatsPreview.BeginAnimation(OpacityProperty, statsFadeIn);

            var statsSlideIn = new DoubleAnimation(50, 0, TimeSpan.FromMilliseconds(800))
            {
                BeginTime = TimeSpan.FromMilliseconds(1200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            StatsPreview.RenderTransform.BeginAnimation(TranslateTransform.XProperty, statsSlideIn);
        }

        private void InputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(null, null);
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Hide any existing error
            HideError();

            // Validate credentials
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter both username and password!");
                return;
            }

            if (username == VALID_USERNAME && password == VALID_PASSWORD)
            {
                // Success! Show success notification and redirect
                ShowSuccessAndRedirect();
            }
            else
            {
                // Show error with animation
                ShowError("Invalid username or password!");

                // Shake animation for login form
                ShakeLoginForm();
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
            ErrorBorder.Opacity = 0;

            // Animate error appearance
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
            ErrorBorder.BeginAnimation(OpacityProperty, fadeIn);

            // Auto-hide after 4 seconds
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                HideError();
            };
            timer.Start();
        }

        private void HideError()
        {
            if (ErrorBorder.Visibility == Visibility.Visible)
            {
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                fadeOut.Completed += (s, e) => ErrorBorder.Visibility = Visibility.Collapsed;
                ErrorBorder.BeginAnimation(OpacityProperty, fadeOut);
            }
        }

        private void ShakeLoginForm()
        {
            var shakeTransform = new TranslateTransform();
            LoginBorder.RenderTransform = shakeTransform;

            var shakeAnimation = new DoubleAnimationUsingKeyFrames();
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromMilliseconds(0)));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-10, TimeSpan.FromMilliseconds(100)));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(10, TimeSpan.FromMilliseconds(200)));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(-5, TimeSpan.FromMilliseconds(300)));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(5, TimeSpan.FromMilliseconds(400)));
            shakeAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromMilliseconds(500)));

            shakeTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);
        }

        private void ShowSuccessAndRedirect()
        {
            // Show success notification
            SuccessNotification.Visibility = Visibility.Visible;
            SuccessNotification.Opacity = 0;

            var successFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400));
            SuccessNotification.BeginAnimation(OpacityProperty, successFadeIn);

            // Disable login form
            LoginButton.IsEnabled = false;
            UsernameTextBox.IsEnabled = false;
            PasswordBox.IsEnabled = false;

            // Redirect after 2 seconds
            var redirectTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            redirectTimer.Tick += (s, e) =>
            {
                redirectTimer.Stop();

                // Fade out login window
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
                fadeOut.Completed += (sender, args) =>
                {
                    // Open main dashboard
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    // Close login window
                    //         this.Close();
                };
                this.BeginAnimation(OpacityProperty, fadeOut);
            };
            redirectTimer.Start();
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isDarkMode = !isDarkMode;
                ApplyTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme toggle error: {ex.Message}");
            }
        }

        private void ApplyTheme()
        {
            try
            {
                // Update theme toggle button
                ThemeToggleButton.Content = isDarkMode ? "🌙" : "☀️";

                // Create gradient brushes for background
                RadialGradientBrush gradientBrush;

                if (isDarkMode)
                {
                    // Apply dark theme
                    gradientBrush = new RadialGradientBrush
                    {
                        GradientOrigin = new Point(0.3, 0.3),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(15, 15, 35), 0),
                            new GradientStop(Color.FromRgb(26, 26, 46), 0.5),
                            new GradientStop(Color.FromRgb(22, 33, 62), 1)
                        }
                    };

                    ApplyDarkThemeStyles();
                }
                else
                {
                    // Apply light theme
                    gradientBrush = new RadialGradientBrush
                    {
                        GradientOrigin = new Point(0.3, 0.3),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(245, 247, 250), 0),
                            new GradientStop(Color.FromRgb(235, 240, 245), 0.5),
                            new GradientStop(Color.FromRgb(225, 230, 240), 1)
                        }
                    };

                    ApplyLightThemeStyles();
                }

                // Apply the background
                MainGrid.Background = gradientBrush;

                // Animate the theme change
                var themeAnimation = new DoubleAnimation(0.8, 1.0, TimeSpan.FromMilliseconds(400))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
                MainGrid.BeginAnimation(OpacityProperty, themeAnimation);

                System.Diagnostics.Debug.WriteLine($"Login Theme changed to: {(isDarkMode ? "Dark" : "Light")}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply theme error: {ex.Message}");
            }
        }

        private void ApplyDarkThemeStyles()
        {
            try
            {
                var darkGlassStyle = FindResource("GlassPanelDark") as Style;
                var darkTextStyle = FindResource("TextBlockDark") as Style;
                var darkTextBoxStyle = FindResource("ModernTextBoxDark") as Style;
                var darkPasswordStyle = FindResource("ModernPasswordBoxDark") as Style;

                if (darkGlassStyle != null)
                {
                    LoginBorder.Style = darkGlassStyle;
                }

                if (darkTextStyle != null)
                {
                    TitleIcon.Style = darkTextStyle;
                    TitleText.Style = darkTextStyle;
                    BrandTitle.Style = darkTextStyle;
                    BrandSubtitle.Style = darkTextStyle;
                    BrandDescription.Style = darkTextStyle;
                    LoginTitle.Style = darkTextStyle;
                    LoginSubtitle.Style = darkTextStyle;
                    UsernameLabel.Style = darkTextStyle;
                    PasswordLabel.Style = darkTextStyle;
                    FooterText1.Style = darkTextStyle;
                    FooterText2.Style = darkTextStyle;
                }

                if (darkTextBoxStyle != null)
                {
                    UsernameTextBox.Style = darkTextBoxStyle;
                }

                if (darkPasswordStyle != null)
                {
                    PasswordBox.Style = darkPasswordStyle;
                }

                ThemeToggleButton.Foreground = Brushes.White;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply dark theme styles error: {ex.Message}");
            }
        }

        private void ApplyLightThemeStyles()
        {
            try
            {
                var lightGlassStyle = FindResource("GlassPanelLight") as Style;
                var lightTextStyle = FindResource("TextBlockLight") as Style;
                var lightTextBoxStyle = FindResource("ModernTextBoxLight") as Style;
                var lightPasswordStyle = FindResource("ModernPasswordBoxLight") as Style;

                if (lightGlassStyle != null)
                {
                    LoginBorder.Style = lightGlassStyle;
                }

                if (lightTextStyle != null)
                {
                    TitleIcon.Style = lightTextStyle;
                    TitleText.Style = lightTextStyle;
                    BrandTitle.Style = lightTextStyle;
                    BrandSubtitle.Style = lightTextStyle;
                    BrandDescription.Style = lightTextStyle;
                    LoginTitle.Style = lightTextStyle;
                    LoginSubtitle.Style = lightTextStyle;
                    UsernameLabel.Style = lightTextStyle;
                    PasswordLabel.Style = lightTextStyle;
                    FooterText1.Style = lightTextStyle;
                    FooterText2.Style = lightTextStyle;
                }

                if (lightTextBoxStyle != null)
                {
                    UsernameTextBox.Style = lightTextBoxStyle;
                }

                if (lightPasswordStyle != null)
                {
                    PasswordBox.Style = lightPasswordStyle;
                }

                ThemeToggleButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply light theme styles error: {ex.Message}");
            }
        }

        // Window control events
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));
            fadeOut.Completed += (s, args) => Application.Current.Shutdown();
            this.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        protected override void OnClosed(EventArgs e)
        {
            particleTimer?.Stop();
            logoAnimationTimer?.Stop();
            base.OnClosed(e);
        }
    }
}