using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace JewelryStoreWPF
{
    public partial class LoginWindow : Window
    {
        private bool isDarkMode = true;
        private bool isPasswordVisible = false;

        // Login credentials
        private const string VALID_USERNAME = "admin";
        private const string VALID_PASSWORD = "admin";

        public LoginWindow()
        {
            InitializeComponent();

            // Set focus to username textbox after window loads
            this.Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UsernameTextBox.Focus();
                }), DispatcherPriority.ApplicationIdle);
            };

            // Add key handlers for Enter key
            UsernameTextBox.KeyDown += InputKeyDown;
            PasswordBox.KeyDown += InputKeyDown;
            PasswordTextBox.KeyDown += InputKeyDown;

            // Sync password fields
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            PasswordTextBox.TextChanged += PasswordTextBox_TextChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!isPasswordVisible && PasswordBox.Password != PasswordTextBox.Text)
                {
                    PasswordTextBox.Text = PasswordBox.Password;
                    System.Diagnostics.Debug.WriteLine($"PasswordBox changed: {PasswordBox.Password.Length} chars");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PasswordBox_PasswordChanged error: {ex.Message}");
            }
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (isPasswordVisible && PasswordTextBox.Text != PasswordBox.Password)
                {
                    PasswordBox.Password = PasswordTextBox.Text;
                    System.Diagnostics.Debug.WriteLine($"PasswordTextBox changed: {PasswordTextBox.Text.Length} chars");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PasswordTextBox_TextChanged error: {ex.Message}");
            }
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
            try
            {
                string username = UsernameTextBox.Text.Trim();
                string password = isPasswordVisible ? PasswordTextBox.Text : PasswordBox.Password;

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
                    // Show error with simple animation
                    ShowError("Invalid username or password!");

                    // Simple shake animation
                    ShakeLoginForm();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoginButton_Click error: {ex.Message}");
                ShowError("An error occurred during login. Please try again.");
            }
        }

        private void EyeToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isPasswordVisible = !isPasswordVisible;

                if (isPasswordVisible)
                {
                    // Show password as plain text
                    PasswordTextBox.Text = PasswordBox.Password;
                    PasswordBox.Visibility = Visibility.Collapsed;
                    PasswordTextBox.Visibility = Visibility.Visible;
                    EyeToggleButton.Content = "🙈"; // Eye closed - password is visible

                    // Set focus and cursor position
                    PasswordTextBox.Focus();
                    PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;
                }
                else
                {
                    // Hide password (show dots)
                    PasswordBox.Password = PasswordTextBox.Text;
                    PasswordTextBox.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Visible;
                    EyeToggleButton.Content = "👁"; // Eye open - password is hidden

                    // Set focus to password box
                    PasswordBox.Focus();
                }

                // Apply correct theme style to ensure visibility
                ApplyEyeButtonTheme();

                System.Diagnostics.Debug.WriteLine($"Password visibility toggled to: {isPasswordVisible}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EyeToggle_Click error: {ex.Message}");
            }
        }

        private void ApplyEyeButtonTheme()
        {
            try
            {
                Style eyeStyle = isDarkMode ?
                    FindResource("EyeToggleButton") as Style :
                    FindResource("EyeToggleButtonLight") as Style;

                if (eyeStyle != null)
                {
                    EyeToggleButton.Style = eyeStyle;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ApplyEyeButtonTheme error: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            try
            {
                ErrorText.Text = message;
                ErrorBorder.Visibility = Visibility.Visible;
                ErrorBorder.Opacity = 0;

                // Simple fade in animation
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                ErrorBorder.BeginAnimation(OpacityProperty, fadeIn);

                // Auto-hide after 3 seconds
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(3)
                };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    HideError();
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowError error: {ex.Message}");
            }
        }

        private void HideError()
        {
            try
            {
                if (ErrorBorder.Visibility == Visibility.Visible)
                {
                    var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                    fadeOut.Completed += (s, e) => ErrorBorder.Visibility = Visibility.Collapsed;
                    ErrorBorder.BeginAnimation(OpacityProperty, fadeOut);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HideError error: {ex.Message}");
            }
        }

        private void ShakeLoginForm()
        {
            try
            {
                var shakeTransform = new TranslateTransform();
                LoginBorder.RenderTransform = shakeTransform;

                // Simple shake animation
                var shakeAnimation = new DoubleAnimation(0, -8, TimeSpan.FromMilliseconds(50))
                {
                    AutoReverse = true,
                    RepeatBehavior = new RepeatBehavior(3)
                };

                shakeTransform.BeginAnimation(TranslateTransform.XProperty, shakeAnimation);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShakeLoginForm error: {ex.Message}");
            }
        }

        private void ShowSuccessAndRedirect()
        {
            try
            {
                // Show success notification
                SuccessNotification.Visibility = Visibility.Visible;
                SuccessNotification.Opacity = 0;

                var successFadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                SuccessNotification.BeginAnimation(OpacityProperty, successFadeIn);

                // Disable login form
                LoginButton.IsEnabled = false;
                UsernameTextBox.IsEnabled = false;
                PasswordBox.IsEnabled = false;
                PasswordTextBox.IsEnabled = false;
                EyeToggleButton.IsEnabled = false;

                // Redirect after 0 second
                var redirectTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(0)
                };
                redirectTimer.Tick += (s, e) =>
                {
                    redirectTimer.Stop();

                    // Simple fade out
                    var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
                    fadeOut.Completed += (sender, args) =>
                    {
                        try
                        {
                            // Open main dashboard (assuming MainWindow exists)
                            var mainWindow = new MainWindow();
                            Application.Current.MainWindow = mainWindow;
                            mainWindow.Show();

                            // Close login window
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Redirect error: {ex.Message}");
                            // If MainWindow doesn't exist, just close the login
                            this.Close();
                        }
                    };
                    this.BeginAnimation(OpacityProperty, fadeOut);
                };
                redirectTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowSuccessAndRedirect error: {ex.Message}");
            }
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

                // Create simple gradient brushes for background
                LinearGradientBrush gradientBrush;

                if (isDarkMode)
                {
                    // Apply dark theme
                    gradientBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
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
                    gradientBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
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

                // Simple theme change animation
                var themeAnimation = new DoubleAnimation(0.9, 1.0, TimeSpan.FromMilliseconds(200));
                MainGrid.BeginAnimation(OpacityProperty, themeAnimation);

                System.Diagnostics.Debug.WriteLine($"Theme changed to: {(isDarkMode ? "Dark" : "Light")}");
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
                // Apply dark theme styles
                var resources = new[]
                {
                    new { Element = (FrameworkElement)LoginBorder, StyleKey = "GlassPanelDark" },
                    new { Element = (FrameworkElement)UsernameTextBox, StyleKey = "ModernTextBoxDark" },
                    new { Element = (FrameworkElement)PasswordTextBox, StyleKey = "ModernTextBoxDark" },
                    new { Element = (FrameworkElement)PasswordBox, StyleKey = "ModernPasswordBoxDark" },
                    new { Element = (FrameworkElement)EyeToggleButton, StyleKey = "EyeToggleButton" }
                };

                foreach (var item in resources)
                {
                    var style = FindResource(item.StyleKey) as Style;
                    if (style != null && item.Element != null)
                    {
                        item.Element.Style = style;
                    }
                }

                // Apply text styles
                var textElements = new FrameworkElement[]
                {
                    TitleIcon, TitleText, BrandTitle, BrandSubtitle, BrandDescription,
                    LoginTitle, LoginSubtitle, UsernameLabel, PasswordLabel,
                    FooterText1, FooterText2
                };

                var darkTextStyle = FindResource("TextBlockDark") as Style;
                if (darkTextStyle != null)
                {
                    foreach (var element in textElements)
                    {
                        if (element != null)
                        {
                            element.Style = darkTextStyle;
                        }
                    }
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
                // Apply light theme styles
                var resources = new[]
                {
                    new { Element = (FrameworkElement)LoginBorder, StyleKey = "GlassPanelLight" },
                    new { Element = (FrameworkElement)UsernameTextBox, StyleKey = "ModernTextBoxLight" },
                    new { Element = (FrameworkElement)PasswordTextBox, StyleKey = "ModernTextBoxLight" },
                    new { Element = (FrameworkElement)PasswordBox, StyleKey = "ModernPasswordBoxLight" },
                    new { Element = (FrameworkElement)EyeToggleButton, StyleKey = "EyeToggleButtonLight" }
                };

                foreach (var item in resources)
                {
                    var style = FindResource(item.StyleKey) as Style;
                    if (style != null && item.Element != null)
                    {
                        item.Element.Style = style;
                    }
                }

                // Apply text styles
                var textElements = new FrameworkElement[]
                {
                    TitleIcon, TitleText, BrandTitle, BrandSubtitle, BrandDescription,
                    LoginTitle, LoginSubtitle, UsernameLabel, PasswordLabel,
                    FooterText1, FooterText2
                };

                var lightTextStyle = FindResource("TextBlockLight") as Style;
                if (lightTextStyle != null)
                {
                    foreach (var element in textElements)
                    {
                        if (element != null)
                        {
                            element.Style = lightTextStyle;
                        }
                    }
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
            try
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TitleBar_MouseLeftButtonDown error: {ex.Message}");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (s, args) => Application.Current.Shutdown();
            this.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void EyeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isPasswordVisible = !isPasswordVisible;

                if (isPasswordVisible)
                {
                    PasswordTextBox.Text = PasswordBox.Password;
                    PasswordBox.Visibility = Visibility.Collapsed;
                    PasswordTextBox.Visibility = Visibility.Visible;
                    EyeToggleButton.Content = "🙈";

                    // Force style
                    PasswordTextBox.FontFamily = new FontFamily("Segoe UI");
                    PasswordTextBox.FontWeight = FontWeights.Bold;

                    PasswordTextBox.Focus();
                    PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;
                }
                else
                {
                    PasswordBox.Password = PasswordTextBox.Text;
                    PasswordTextBox.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Visible;
                    EyeToggleButton.Content = "👁";

                    // Force style
                    PasswordBox.FontFamily = new FontFamily("Segoe UI");
                    PasswordBox.FontWeight = FontWeights.Bold;

                    PasswordBox.Focus();
                }


                // Apply correct theme style to ensure visibility
                ApplyEyeButtonTheme();

                System.Diagnostics.Debug.WriteLine($"Password visibility toggled to: {isPasswordVisible}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"EyeToggle_Click error: {ex.Message}");
            }
        }
    }
}