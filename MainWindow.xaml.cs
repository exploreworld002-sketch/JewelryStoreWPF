using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JewelryStoreWPF
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer animationTimer;
        private DispatcherTimer particleTimer;
        private List<Ellipse> particles;
        private Random random = new Random();
        private bool isDarkMode = true;

        // Sample data
        private List<Sale> recentSales = new List<Sale>
{
    new Sale { CustomerName = "Sarah Johnson", Item = "Diamond Ring", Amount = 15000, Date = DateTime.Now.AddDays(-1) },
    new Sale { CustomerName = "Mike Brown", Item = "Gold Necklace", Amount = 8500, Date = DateTime.Now.AddDays(-2) },
    new Sale { CustomerName = "Emma Davis", Item = "Pearl Earrings", Amount = 3200, Date = DateTime.Now.AddDays(-3) },
    new Sale { CustomerName = "John Wilson", Item = "Silver Bracelet", Amount = 1800, Date = DateTime.Now.AddDays(-4) },
    new Sale { CustomerName = "Lisa Anderson", Item = "Emerald Necklace", Amount = 12000, Date = DateTime.Now.AddDays(-5) },

    new Sale { CustomerName = "David Thompson", Item = "Sapphire Ring", Amount = 6700, Date = DateTime.Now.AddDays(-6) },
    new Sale { CustomerName = "Amy Scott", Item = "Ruby Pendant", Amount = 9200, Date = DateTime.Now.AddDays(-7) },
    new Sale { CustomerName = "Chris Evans", Item = "Gold Chain", Amount = 4100, Date = DateTime.Now.AddDays(-8) },
    new Sale { CustomerName = "Nancy Moore", Item = "Platinum Bracelet", Amount = 13400, Date = DateTime.Now.AddDays(-9) },
    new Sale { CustomerName = "Kevin Taylor", Item = "Diamond Earrings", Amount = 7800, Date = DateTime.Now.AddDays(-10) },

    new Sale { CustomerName = "Olivia White", Item = "Pearl Necklace", Amount = 6300, Date = DateTime.Now.AddDays(-11) },
    new Sale { CustomerName = "Jason Martin", Item = "Gold Bangles", Amount = 4700, Date = DateTime.Now.AddDays(-12) },
    new Sale { CustomerName = "Sophia Garcia", Item = "Diamond Bracelet", Amount = 15800, Date = DateTime.Now.AddDays(-13) },
    new Sale { CustomerName = "Brian Clark", Item = "Sapphire Earrings", Amount = 6900, Date = DateTime.Now.AddDays(-14) },
    new Sale { CustomerName = "Grace Hall", Item = "Ruby Ring", Amount = 9900, Date = DateTime.Now.AddDays(-15) },

    new Sale { CustomerName = "Daniel Lewis", Item = "Gold Pendant", Amount = 5600, Date = DateTime.Now.AddDays(-16) },
    new Sale { CustomerName = "Chloe Young", Item = "Emerald Earrings", Amount = 8300, Date = DateTime.Now.AddDays(-17) },
    new Sale { CustomerName = "Ethan King", Item = "Silver Chain", Amount = 2900, Date = DateTime.Now.AddDays(-18) },
    new Sale { CustomerName = "Lily Wright", Item = "Platinum Necklace", Amount = 14100, Date = DateTime.Now.AddDays(-19) },
    new Sale { CustomerName = "Logan Lopez", Item = "Gold Bracelet", Amount = 3600, Date = DateTime.Now.AddDays(-20) },

    new Sale { CustomerName = "Ava Hill", Item = "Diamond Pendant", Amount = 11800, Date = DateTime.Now.AddDays(-21) },
    new Sale { CustomerName = "Matthew Green", Item = "Ruby Necklace", Amount = 9700, Date = DateTime.Now.AddDays(-22) },
    new Sale { CustomerName = "Isabella Adams", Item = "Pearl Ring", Amount = 4100, Date = DateTime.Now.AddDays(-23) },
    new Sale { CustomerName = "Lucas Nelson", Item = "Silver Earrings", Amount = 2700, Date = DateTime.Now.AddDays(-24) },
    new Sale { CustomerName = "Zoe Carter", Item = "Platinum Ring", Amount = 10200, Date = DateTime.Now.AddDays(-25) },

    new Sale { CustomerName = "Henry Mitchell", Item = "Gold Earrings", Amount = 5100, Date = DateTime.Now.AddDays(-26) },
    new Sale { CustomerName = "Ella Perez", Item = "Diamond Chain", Amount = 14500, Date = DateTime.Now.AddDays(-27) },
    new Sale { CustomerName = "Sebastian Roberts", Item = "Emerald Ring", Amount = 7600, Date = DateTime.Now.AddDays(-28) },
    new Sale { CustomerName = "Mia Turner", Item = "Ruby Bracelet", Amount = 6200, Date = DateTime.Now.AddDays(-29) },
    new Sale { CustomerName = "Jack Phillips", Item = "Sapphire Necklace", Amount = 8800, Date = DateTime.Now.AddDays(-30) },

    new Sale { CustomerName = "Emily Campbell", Item = "Gold Ring", Amount = 5300, Date = DateTime.Now.AddDays(-31) },
    new Sale { CustomerName = "Anthony Parker", Item = "Pearl Pendant", Amount = 3500, Date = DateTime.Now.AddDays(-32) },
    new Sale { CustomerName = "Aria Evans", Item = "Diamond Earrings", Amount = 9800, Date = DateTime.Now.AddDays(-33) },
    new Sale { CustomerName = "Nathan Edwards", Item = "Silver Necklace", Amount = 3100, Date = DateTime.Now.AddDays(-34) },
    new Sale { CustomerName = "Scarlett Collins", Item = "Gold Bracelet", Amount = 3900, Date = DateTime.Now.AddDays(-35) },

    new Sale { CustomerName = "Gabriel Stewart", Item = "Ruby Earrings", Amount = 7400, Date = DateTime.Now.AddDays(-36) },
    new Sale { CustomerName = "Victoria Sanchez", Item = "Diamond Necklace", Amount = 16200, Date = DateTime.Now.AddDays(-37) },
    new Sale { CustomerName = "Samuel Morris", Item = "Sapphire Ring", Amount = 6100, Date = DateTime.Now.AddDays(-38) },
    new Sale { CustomerName = "Luna Rogers", Item = "Pearl Earrings", Amount = 4200, Date = DateTime.Now.AddDays(-39) },
    new Sale { CustomerName = "Owen Reed", Item = "Platinum Bracelet", Amount = 12300, Date = DateTime.Now.AddDays(-40) },

    new Sale { CustomerName = "Penelope Cook", Item = "Emerald Pendant", Amount = 8400, Date = DateTime.Now.AddDays(-41) },
    new Sale { CustomerName = "Dylan Morgan", Item = "Gold Chain", Amount = 4600, Date = DateTime.Now.AddDays(-42) },
    new Sale { CustomerName = "Nora Bell", Item = "Diamond Ring", Amount = 13300, Date = DateTime.Now.AddDays(-43) },
    new Sale { CustomerName = "Leo Murphy", Item = "Ruby Necklace", Amount = 7200, Date = DateTime.Now.AddDays(-44) },
    new Sale { CustomerName = "Avery Bailey", Item = "Silver Bracelet", Amount = 2900, Date = DateTime.Now.AddDays(-45) },

    new Sale { CustomerName = "Grayson Rivera", Item = "Gold Pendant", Amount = 5400, Date = DateTime.Now.AddDays(-46) },
    new Sale { CustomerName = "Hazel Cooper", Item = "Pearl Ring", Amount = 3600, Date = DateTime.Now.AddDays(-47) },
    new Sale { CustomerName = "Elijah Richardson", Item = "Sapphire Earrings", Amount = 8800, Date = DateTime.Now.AddDays(-48) },
    new Sale { CustomerName = "Layla Cox", Item = "Diamond Bracelet", Amount = 10900, Date = DateTime.Now.AddDays(-49) },
    new Sale { CustomerName = "Carter Howard", Item = "Emerald Necklace", Amount = 9500, Date = DateTime.Now.AddDays(-50) }
};


        private List<InventoryItem> lowStockItems = new List<InventoryItem>
        {
            new InventoryItem { Name = "Gold Rings", Stock = 5, MinStock = 10 },
            new InventoryItem { Name = "Diamond Earrings", Stock = 3, MinStock = 8 },
            new InventoryItem { Name = "Silver Chains", Stock = 7, MinStock = 15 }
        };

        public MainWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Close();

            InitializeComponent();
            InitializeData();
            StartAnimations();
            CreateParticles();
            DrawChart();
        }

        private void InitializeData()
        {
            // Update date time
            DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");

            // Load recent sales
            SalesListView.ItemsSource = recentSales;

            // Create low stock alerts
            CreateLowStockAlerts();

            // Start real-time clock
            var clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            clockTimer.Tick += (s, e) =>
            {
                DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");
            };
            clockTimer.Start();
        }

        private void CreateLowStockAlerts()
        {
            AlertsPanel.Children.Clear();

            foreach (var item in lowStockItems)
            {
                var alertBorder = new Border
                {
                    CornerRadius = new CornerRadius(12),
                    Margin = new Thickness(0, 0, 0, 15),
                    Height = 80,
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(255, 107, 107), 0),
                            new GradientStop(Color.FromRgb(238, 90, 82), 1)
                        }
                    },
                    Opacity = 0.9
                };

                var grid = new Grid();

                var nameText = new TextBlock
                {
                    Text = item.Name,
                    FontSize = 14,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(15, 15, 0, 0)
                };

                var stockText = new TextBlock
                {
                    Text = $"Stock: {item.Stock} (Min: {item.MinStock})",
                    FontSize = 12,
                    Foreground = Brushes.White,
                    Opacity = 0.8,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(15, 35, 0, 0)
                };

                var reorderButton = new Button
                {
                    Content = "🔄 Reorder",
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(Color.FromRgb(67, 233, 123), 0),
                            new GradientStop(Color.FromRgb(56, 249, 215), 1)
                        }
                    },
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    FontSize = 11,
                    FontWeight = FontWeights.Bold,
                    Width = 80,
                    Height = 30,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 15, 0),
                    Cursor = Cursors.Hand
                };

                // Add rounded corners to button
                reorderButton.Template = CreateRoundedButtonTemplate();

                grid.Children.Add(nameText);
                grid.Children.Add(stockText);
                grid.Children.Add(reorderButton);
                alertBorder.Child = grid;

                AlertsPanel.Children.Add(alertBorder);

                // Add fade-in animation
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
                alertBorder.BeginAnimation(OpacityProperty, fadeIn);
            }
        }

        private ControlTemplate CreateRoundedButtonTemplate()
        {
            var template = new ControlTemplate(typeof(Button));

            var border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            border.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));

            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            border.AppendChild(contentPresenter);
            template.VisualTree = border;

            return template;
        }

        private void DrawChart()
        {
            ChartCanvas.Children.Clear();

            var salesData = new double[] { 80, 120, 90, 150, 110, 180, 160 };
            var days = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            var colors = new Color[]
            {
                Color.FromRgb(102, 126, 234),
                Color.FromRgb(240, 147, 251),
                Color.FromRgb(79, 172, 254),
                Color.FromRgb(67, 233, 123),
                Color.FromRgb(250, 112, 154),
                Color.FromRgb(168, 237, 234),
                Color.FromRgb(255, 154, 158)
            };

            var maxValue = 200.0;
            var barWidth = 40;
            var barSpacing = 60;
            var chartHeight = 200;
            var startX = 40;

            for (int i = 0; i < salesData.Length; i++)
            {
                var barHeight = (salesData[i] / maxValue) * chartHeight;
                var x = startX + (i * barSpacing);
                var y = chartHeight - barHeight + 20;

                // Create animated bar
                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = 0, // Start with 0 height for animation
                    Fill = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(0, 1),
                        GradientStops = new GradientStopCollection
                        {
                            new GradientStop(colors[i], 0),
                            new GradientStop(Color.FromArgb(100, colors[i].R, colors[i].G, colors[i].B), 1)
                        }
                    },
                    RadiusX = 8,
                    RadiusY = 8
                };

                Canvas.SetLeft(bar, x);
                Canvas.SetTop(bar, y + barHeight);
                ChartCanvas.Children.Add(bar);

                // Animate bar height
                var heightAnimation = new DoubleAnimation(0, barHeight, TimeSpan.FromMilliseconds(800 + i * 100))
                {
                    EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
                };
                bar.BeginAnimation(Rectangle.HeightProperty, heightAnimation);

                // Animate bar position
                var positionAnimation = new DoubleAnimation(y + barHeight, y, TimeSpan.FromMilliseconds(800 + i * 100))
                {
                    EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
                };
                bar.BeginAnimation(Canvas.TopProperty, positionAnimation);

                // Add day label
                var dayLabel = new TextBlock
                {
                    Text = days[i],
                    Foreground = isDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Canvas.SetLeft(dayLabel, x + barWidth / 2 - 15);
                Canvas.SetTop(dayLabel, chartHeight + 30);
                ChartCanvas.Children.Add(dayLabel);

                // Add value label on top of bar
                var valueLabel = new TextBlock
                {
                    Text = $"₹{salesData[i]:N0}K",
                    Foreground = isDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Opacity = 0
                };

                Canvas.SetLeft(valueLabel, x + barWidth / 2 - 20);
                Canvas.SetTop(valueLabel, y - 20);
                ChartCanvas.Children.Add(valueLabel);

                // Animate value label
                var labelAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
                {
                    BeginTime = TimeSpan.FromMilliseconds(800 + i * 100)
                };
                valueLabel.BeginAnimation(OpacityProperty, labelAnimation);
            }
        }

        private void CreateParticles()
        {
            particles = new List<Ellipse>();

            for (int i = 0; i < 30; i++)
            {
                var particle = new Ellipse
                {
                    Width = random.Next(2, 6),
                    Height = random.Next(2, 6),
                    Fill = new SolidColorBrush(Color.FromArgb((byte)random.Next(20, 80), 255, 255, 255)),
                    Opacity = random.NextDouble() * 0.6 + 0.2
                };

                Canvas.SetLeft(particle, random.Next(0, (int)this.Width));
                Canvas.SetTop(particle, random.Next(0, (int)this.Height));

                particles.Add(particle);
            }
        }

        private void StartAnimations()
        {
            // Particle animation
            particleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };
            particleTimer.Tick += (s, e) =>
            {
                foreach (var particle in particles)
                {
                    var currentX = Canvas.GetLeft(particle);
                    var currentY = Canvas.GetTop(particle);

                    var newX = currentX + (random.NextDouble() - 0.5) * 2;
                    var newY = currentY + (random.NextDouble() - 0.5) * 2;

                    // Wrap around screen
                    if (newX < 0) newX = this.ActualWidth;
                    if (newX > this.ActualWidth) newX = 0;
                    if (newY < 0) newY = this.ActualHeight;
                    if (newY > this.ActualHeight) newY = 0;

                    Canvas.SetLeft(particle, newX);
                    Canvas.SetTop(particle, newY);

                    // Random opacity changes
                    if (random.NextDouble() < 0.01)
                    {
                        var opacityAnimation = new DoubleAnimation(
                            particle.Opacity,
                            random.NextDouble() * 0.6 + 0.2,
                            TimeSpan.FromMilliseconds(1000));
                        particle.BeginAnimation(OpacityProperty, opacityAnimation);
                    }
                }
            };
            particleTimer.Start();

            // General animation timer
            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            animationTimer.Tick += (s, e) =>
            {
                // Redraw chart with slight variations
                DrawChart();
            };
            animationTimer.Start();

            // Start entrance animations
            StartEntranceAnimations();
        }

        private void StartEntranceAnimations()
        {
            try
            {
                // Animate the stats cards
                AnimateStatsCards();

                // Animate the welcome section
                AnimateWelcomeSection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
        }

        private void AnimateStatsCards()
        {
            var statsGrid = StatsGrid;
            if (statsGrid != null)
            {
                for (int i = 0; i < statsGrid.Children.Count; i++)
                {
                    var card = statsGrid.Children[i] as Border;
                    if (card != null)
                    {
                        // Set initial state
                        card.Opacity = 0;
                        card.RenderTransform = new TranslateTransform(0, 50);

                        // Animate opacity
                        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(600))
                        {
                            BeginTime = TimeSpan.FromMilliseconds(i * 200)
                        };
                        card.BeginAnimation(OpacityProperty, fadeIn);

                        // Animate slide up
                        var slideIn = new DoubleAnimation(50, 0, TimeSpan.FromMilliseconds(600))
                        {
                            BeginTime = TimeSpan.FromMilliseconds(i * 200),
                            EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut }
                        };
                        card.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
                    }
                }
            }
        }

        private void AnimateWelcomeSection()
        {
            try
            {
                var welcomeBorder = WelcomeBorder;
                if (welcomeBorder != null)
                {
                    welcomeBorder.Opacity = 0;
                    welcomeBorder.RenderTransform = new TranslateTransform(-100, 0);

                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(800));
                    welcomeBorder.BeginAnimation(OpacityProperty, fadeIn);

                    var slideIn = new DoubleAnimation(-100, 0, TimeSpan.FromMilliseconds(800))
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                    };
                    welcomeBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Welcome animation error: {ex.Message}");
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
            // Animate window close
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, args) => this.Close();
            this.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
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

                    // Apply dark theme styles
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

                    // Apply light theme styles
                    ApplyLightThemeStyles();
                }

                // Apply the background
                MainGrid.Background = gradientBrush;

                // Redraw chart with updated colors
                DrawChart();

                // Animate the theme change
                var themeAnimation = new DoubleAnimation(0.8, 1.0, TimeSpan.FromMilliseconds(300))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };
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
                // Update glassmorphism panels
                var darkGlassStyle = FindResource("GlassPanelDark") as Style;
                var darkNavStyle = FindResource("NavButtonDark") as Style;
                var darkTextStyle = FindResource("TextBlockDark") as Style;
                var darkListStyle = FindResource("ListViewDark") as Style;

                if (darkGlassStyle != null)
                {
                    SidebarBorder.Style = darkGlassStyle;
                    ChartBorder.Style = darkGlassStyle;
                    QuickActionsBorder.Style = darkGlassStyle;
                    SalesBorder.Style = darkGlassStyle;
                    AlertsBorder.Style = darkGlassStyle;
                }

                if (darkNavStyle != null)
                {
                    DashboardBtn.Style = darkNavStyle;
                    CustomersBtn.Style = darkNavStyle;
                    InventoryBtn.Style = darkNavStyle;
                    SalesBtn.Style = darkNavStyle;
                    PurchaseBtn.Style = darkNavStyle;
                    AnalyticsBtn.Style = darkNavStyle;
                    ReportsBtn.Style = darkNavStyle;
                    SettingsBtn.Style = darkNavStyle;
                }

                if (darkTextStyle != null)
                {
                    TitleIcon.Style = darkTextStyle;
                    TitleText.Style = darkTextStyle;
                    ChartTitle.Style = darkTextStyle;
                    QuickActionsTitle.Style = darkTextStyle;
                    SalesTitle.Style = darkTextStyle;
                }

                if (darkListStyle != null)
                {
                    SalesListView.Style = darkListStyle;
                }

                // Update button colors
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
                // Update glassmorphism panels
                var lightGlassStyle = FindResource("GlassPanelLight") as Style;
                var lightNavStyle = FindResource("NavButtonLight") as Style;
                var lightTextStyle = FindResource("TextBlockLight") as Style;
                var lightListStyle = FindResource("ListViewLight") as Style;

                if (lightGlassStyle != null)
                {
                    SidebarBorder.Style = lightGlassStyle;
                    ChartBorder.Style = lightGlassStyle;
                    QuickActionsBorder.Style = lightGlassStyle;
                    SalesBorder.Style = lightGlassStyle;
                    AlertsBorder.Style = lightGlassStyle;
                }

                if (lightNavStyle != null)
                {
                    DashboardBtn.Style = lightNavStyle;
                    CustomersBtn.Style = lightNavStyle;
                    InventoryBtn.Style = lightNavStyle;
                    SalesBtn.Style = lightNavStyle;
                    PurchaseBtn.Style = lightNavStyle;
                    AnalyticsBtn.Style = lightNavStyle;
                    ReportsBtn.Style = lightNavStyle;
                    SettingsBtn.Style = lightNavStyle;
                }

                if (lightTextStyle != null)
                {
                    TitleIcon.Style = lightTextStyle;
                    TitleText.Style = lightTextStyle;
                    ChartTitle.Style = lightTextStyle;
                    QuickActionsTitle.Style = lightTextStyle;
                    SalesTitle.Style = lightTextStyle;
                }

                if (lightListStyle != null)
                {
                    SalesListView.Style = lightListStyle;
                }

                // Update button colors
                ThemeToggleButton.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply light theme styles error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            animationTimer?.Stop();
            particleTimer?.Stop();
            base.OnClosed(e);
        }
    }

    // Data models
    public class Sale
    {
        public string CustomerName { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }

    public class InventoryItem
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
    }
}