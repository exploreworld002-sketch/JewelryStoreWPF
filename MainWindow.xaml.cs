using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace JewelryStoreWPF
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer animationTimer;
        private DispatcherTimer particleTimer;
        private DispatcherTimer clockTimer;
        private List<Ellipse> particles;
        private Random random = new Random();
        private bool isAnimationEnabled = true;
        private int animationFrameSkip = 0;

        // Use ObservableCollection for better performance with data binding
        private ObservableCollection<Sale> recentSales;
        private List<InventoryItem> lowStockItems;

        // Cache frequently used resources
        private Style cachedDarkGlassStyle;
        private Style cachedLightGlassStyle;
        private Style cachedDarkNavStyle;
        private Style cachedLightNavStyle;
        private Style cachedDarkTextStyle;
        private Style cachedLightTextStyle;
        private Style cachedDarkListStyle;
        private Style cachedLightListStyle;
        private Style cachedDarkListItemStyle;
        private Style cachedLightListItemStyle;
        private Style cachedDarkHeaderStyle;
        private Style cachedLightHeaderStyle;

        public MainWindow()
        {
            
            // Initialize data first
            InitializeData();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Close();

            InitializeComponent();
            CacheStyles(); // Cache styles immediately after InitializeComponent
            LoadRecentSales();
            StartOptimizedAnimations();
            CreateOptimizedParticles();
            DrawOptimizedChart();
            ApplyOptimizedTheme();
        }

        private void InitializeData()
        {
            // Initialize collections
            recentSales = new ObservableCollection<Sale>
            {
                new Sale { CustomerName = "Sarah Johnson", Item = "Diamond Ring", Amount = 15000, Date = DateTime.Now.AddDays(-1) },
                new Sale { CustomerName = "Mike Brown", Item = "Gold Necklace", Amount = 8500, Date = DateTime.Now.AddDays(-2) },
                new Sale { CustomerName = "Emma Davis", Item = "Pearl Earrings", Amount = 3200, Date = DateTime.Now.AddDays(-3) }
            };

            lowStockItems = new List<InventoryItem>
            {
                new InventoryItem { Name = "Gold Rings", Stock = 5, MinStock = 10 },
                new InventoryItem { Name = "Diamond Earrings", Stock = 3, MinStock = 8 },
                new InventoryItem { Name = "Silver Chains", Stock = 7, MinStock = 15 }
            };
        }

        private void CacheStyles()
        {
            try
            {
                // Cache all styles to avoid repeated resource lookups
                cachedDarkGlassStyle = FindResource("GlassPanelDark") as Style;
                cachedLightGlassStyle = FindResource("GlassPanelLight") as Style;
                cachedDarkNavStyle = FindResource("NavButtonDark") as Style;
                cachedLightNavStyle = FindResource("NavButtonLight") as Style;
                cachedDarkTextStyle = FindResource("TextBlockDark") as Style;
                cachedLightTextStyle = FindResource("TextBlockLight") as Style;
                cachedDarkListStyle = FindResource("ListViewDark") as Style;
                cachedLightListStyle = FindResource("ListViewLight") as Style;
                cachedDarkListItemStyle = FindResource("ListViewItemDark") as Style;
                cachedLightListItemStyle = FindResource("ListViewItemLight") as Style;
                cachedDarkHeaderStyle = FindResource("GridViewHeaderDark") as Style;
                cachedLightHeaderStyle = FindResource("GridViewHeaderLight") as Style;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Style caching error: {ex.Message}");
            }
        }

        private void LoadRecentSales()
        {
            // Update date time once
            if (DateTimeText != null)
                DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");

            // Bind data efficiently
            if (SalesListView != null)
                SalesListView.ItemsSource = recentSales;

            // Create low stock alerts efficiently
            CreateOptimizedLowStockAlerts();

            // Start optimized clock timer
            clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            // Update only if the control exists and is visible
            if (DateTimeText?.IsVisible == true)
            {
                DateTimeText.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - HH:mm:ss");
            }
        }

        private void CreateOptimizedLowStockAlerts()
        {
            if (AlertsPanel == null) return;

            AlertsPanel.Children.Clear();

            // Use BeginInit/EndInit for batch updates
            AlertsPanel.BeginInit();

            try
            {
                foreach (var item in lowStockItems)
                {
                    var alertBorder = CreateAlertBorder(item);
                    AlertsPanel.Children.Add(alertBorder);
                }
            }
            finally
            {
                AlertsPanel.EndInit();
            }

            // Stagger animations to reduce initial load
            for (int i = 0; i < AlertsPanel.Children.Count; i++)
            {
                var border = AlertsPanel.Children[i] as Border;
                if (border != null)
                {
                    StartDelayedFadeIn(border, i * 100);
                }
            }
        }

        private Border CreateAlertBorder(InventoryItem item)
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
                Opacity = 0 // Start invisible for animation
            };

            var grid = new Grid();
            grid.BeginInit();

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
                Cursor = Cursors.Hand,
                Template = CreateRoundedButtonTemplate() // Create template once
            };

            grid.Children.Add(nameText);
            grid.Children.Add(stockText);
            grid.Children.Add(reorderButton);
            grid.EndInit();

            alertBorder.Child = grid;
            return alertBorder;
        }

        private void StartDelayedFadeIn(Border border, int delay)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(delay)
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                border.BeginAnimation(OpacityProperty, fadeIn);
            };
            timer.Start();
        }

        private ControlTemplate CreateRoundedButtonTemplate()
        {
            // Cache template creation
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

        private void DrawOptimizedChart()
        {
            if (ChartCanvas == null) return;

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

            // Batch create all chart elements
            var elementsToAdd = new List<UIElement>();

            for (int i = 0; i < salesData.Length; i++)
            {
                var barHeight = (salesData[i] / maxValue) * chartHeight;
                var x = startX + (i * barSpacing);
                var y = chartHeight - barHeight + 20;

                // Create bar with optimized gradient
                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = 0,
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
                elementsToAdd.Add(bar);

                // Create labels
                var dayLabel = new TextBlock
                {
                    Text = days[i],
                    Foreground = App.IsDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Canvas.SetLeft(dayLabel, x + barWidth / 2 - 15);
                Canvas.SetTop(dayLabel, chartHeight + 30);
                elementsToAdd.Add(dayLabel);

                var valueLabel = new TextBlock
                {
                    Text = $"₹{salesData[i]:N0}K",
                    Foreground = App.IsDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51)),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Opacity = 0
                };

                Canvas.SetLeft(valueLabel, x + barWidth / 2 - 20);
                Canvas.SetTop(valueLabel, y - 20);
                elementsToAdd.Add(valueLabel);
            }

            // Add all elements at once
            foreach (var element in elementsToAdd)
            {
                ChartCanvas.Children.Add(element);
            }

            // Start animations after all elements are added
            StartChartAnimations(salesData, chartHeight);
        }

        private void StartChartAnimations(double[] salesData, double chartHeight)
        {
            if (!isAnimationEnabled) return;

            var maxValue = 200.0;
            var bars = new List<Rectangle>();
            var valueLabels = new List<TextBlock>();

            // Collect bars and value labels
            for (int i = 0; i < ChartCanvas.Children.Count; i++)
            {
                if (ChartCanvas.Children[i] is Rectangle rect)
                    bars.Add(rect);
                else if (ChartCanvas.Children[i] is TextBlock tb && tb.Opacity == 0)
                    valueLabels.Add(tb);
            }

            // Animate bars with reduced complexity
            for (int i = 0; i < Math.Min(bars.Count, salesData.Length); i++)
            {
                var bar = bars[i];
                var barHeight = (salesData[i] / maxValue) * chartHeight;
                var currentTop = Canvas.GetTop(bar);

                // Simplified animation
                var heightAnimation = new DoubleAnimation(0, barHeight, TimeSpan.FromMilliseconds(400 + i * 50));
                var positionAnimation = new DoubleAnimation(currentTop, currentTop - barHeight, TimeSpan.FromMilliseconds(400 + i * 50));

                bar.BeginAnimation(Rectangle.HeightProperty, heightAnimation);
                bar.BeginAnimation(Canvas.TopProperty, positionAnimation);

                // Animate corresponding value label
                if (i < valueLabels.Count)
                {
                    var labelAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
                    {
                        BeginTime = TimeSpan.FromMilliseconds(400 + i * 50)
                    };
                    valueLabels[i].BeginAnimation(OpacityProperty, labelAnimation);
                }
            }
        }

        private void CreateOptimizedParticles()
        {
            particles = new List<Ellipse>();

            // Reduce particle count for better performance
            var particleCount = Math.Min(20, (int)(this.ActualWidth * this.ActualHeight / 50000));

            for (int i = 0; i < particleCount; i++)
            {
                var particle = new Ellipse
                {
                    Width = random.Next(2, 4),
                    Height = random.Next(2, 4),
                    Fill = new SolidColorBrush(Color.FromArgb((byte)random.Next(20, 60), 255, 255, 255)),
                    Opacity = random.NextDouble() * 0.4 + 0.1
                };

                Canvas.SetLeft(particle, random.Next(0, Math.Max(1, (int)this.ActualWidth)));
                Canvas.SetTop(particle, random.Next(0, Math.Max(1, (int)this.ActualHeight)));

                ParticleCanvas.Children.Add(particle);
                particles.Add(particle);
            }
        }

        private void StartOptimizedAnimations()
        {
            // Optimized particle animation with frame skipping
            particleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // Increased interval
            };
            particleTimer.Tick += OptimizedParticleTimer_Tick;
            particleTimer.Start();

            // Reduced frequency animation timer
            animationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Increased interval
            };
            animationTimer.Tick += (s, e) =>
            {
                if (isAnimationEnabled)
                    DrawOptimizedChart();
            };
            animationTimer.Start();

            // Delayed entrance animations
            Dispatcher.BeginInvoke(new Action(StartEntranceAnimations), DispatcherPriority.Background);
        }

        private void OptimizedParticleTimer_Tick(object sender, EventArgs e)
        {
            // Frame skipping for performance
            animationFrameSkip++;
            if (animationFrameSkip % 2 != 0) return; // Skip every other frame

            if (!isAnimationEnabled || particles == null) return;

            // Update only visible particles
            var visibleBounds = new Rect(0, 0, ActualWidth, ActualHeight);

            foreach (var particle in particles)
            {
                var currentX = Canvas.GetLeft(particle);
                var currentY = Canvas.GetTop(particle);

                // Skip particles outside visible area
                if (currentX < -10 || currentX > visibleBounds.Width + 10 ||
                    currentY < -10 || currentY > visibleBounds.Height + 10)
                    continue;

                var newX = currentX + (random.NextDouble() - 0.5) * 1.5;
                var newY = currentY + (random.NextDouble() - 0.5) * 1.5;

                // Wrap around screen
                if (newX < 0) newX = visibleBounds.Width;
                if (newX > visibleBounds.Width) newX = 0;
                if (newY < 0) newY = visibleBounds.Height;
                if (newY > visibleBounds.Height) newY = 0;

                Canvas.SetLeft(particle, newX);
                Canvas.SetTop(particle, newY);

                // Reduced opacity changes
                if (random.NextDouble() < 0.005)
                {
                    particle.Opacity = random.NextDouble() * 0.4 + 0.1;
                }
            }
        }

        private void StartEntranceAnimations()
        {
            try
            {
                if (!isAnimationEnabled) return;

                AnimateStatsCards();
                AnimateWelcomeSection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation error: {ex.Message}");
            }
        }

        private void AnimateStatsCards()
        {
            if (StatsGrid?.Children == null) return;

            for (int i = 0; i < StatsGrid.Children.Count; i++)
            {
                if (StatsGrid.Children[i] is Border card)
                {
                    card.Opacity = 0;
                    card.RenderTransform = new TranslateTransform(0, 30); // Reduced distance

                    var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300)) // Faster
                    {
                        BeginTime = TimeSpan.FromMilliseconds(i * 100) // Reduced delay
                    };
                    card.BeginAnimation(OpacityProperty, fadeIn);

                    var slideIn = new DoubleAnimation(30, 0, TimeSpan.FromMilliseconds(300))
                    {
                        BeginTime = TimeSpan.FromMilliseconds(i * 100)
                    };
                    card.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);
                }
            }
        }

        private void AnimateWelcomeSection()
        {
            try
            {
                if (WelcomeBorder == null) return;

                WelcomeBorder.Opacity = 0;
                WelcomeBorder.RenderTransform = new TranslateTransform(-50, 0); // Reduced distance

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(400)); // Faster
                WelcomeBorder.BeginAnimation(OpacityProperty, fadeIn);

                var slideIn = new DoubleAnimation(-50, 0, TimeSpan.FromMilliseconds(400));
                WelcomeBorder.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
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
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200)); // Faster
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
                App.IsDarkMode = !App.IsDarkMode;
                ApplyOptimizedTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme toggle error: {ex.Message}");
            }
        }

        private void ApplyOptimizedTheme()
        {
            try
            {
                ThemeToggleButton.Content = App.IsDarkMode ? "🌙" : "☀️";

                // Use cached gradients
                RadialGradientBrush gradientBrush = CreateOptimizedGradient();
                MainGrid.Background = gradientBrush;

                // Apply cached styles efficiently
                ApplyThemedStyles();

                // Simplified theme animation
                var themeAnimation = new DoubleAnimation(0.9, 1.0, TimeSpan.FromMilliseconds(200));
                MainGrid.BeginAnimation(OpacityProperty, themeAnimation);

                // Redraw chart on background thread
                Dispatcher.BeginInvoke(new Action(DrawOptimizedChart), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply theme error: {ex.Message}");
            }
        }

        private RadialGradientBrush CreateOptimizedGradient()
        {
            if (App.IsDarkMode)
            {
                //manually making minimized button white  on dark theme
                btnMinimized.Foreground = new SolidColorBrush(Colors.White);

                return new RadialGradientBrush
                {
                    GradientOrigin = new Point(0.3, 0.3),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(15, 15, 35), 0),
                        new GradientStop(Color.FromRgb(26, 26, 46), 0.5),
                        new GradientStop(Color.FromRgb(22, 33, 62), 1)
                    }
                };
            }
            else
            {
                //manually making minimized button dark black on light theme
                btnMinimized.Foreground = new SolidColorBrush(Colors.Black);

                return new RadialGradientBrush
                {
                    GradientOrigin = new Point(0.3, 0.3),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(245, 247, 250), 0),
                        new GradientStop(Color.FromRgb(235, 240, 245), 0.5),
                        new GradientStop(Color.FromRgb(225, 230, 240), 1)
                    }
                };
            }
        }

        private void ApplyThemedStyles()
        {
            try
            {
                var glassStyle = App.IsDarkMode ? cachedDarkGlassStyle : cachedLightGlassStyle;
                var navStyle = App.IsDarkMode ? cachedDarkNavStyle : cachedLightNavStyle;
                var textStyle = App.IsDarkMode ? cachedDarkTextStyle : cachedLightTextStyle;
                var listStyle = App.IsDarkMode ? cachedDarkListStyle : cachedLightListStyle;
                var listItemStyle = App.IsDarkMode ? cachedDarkListItemStyle : cachedLightListItemStyle;

                // Apply styles using cached references
                if (glassStyle != null)
                {
                    SidebarBorder.Style = glassStyle;
                    ChartBorder.Style = glassStyle;
                    QuickActionsBorder.Style = glassStyle;
                    SalesBorder.Style = glassStyle;
                    AlertsBorder.Style = glassStyle;
                }

                if (navStyle != null)
                {
                    DashboardBtn.Style = navStyle;
                    CustomersBtn.Style = navStyle;
                    InventoryBtn.Style = navStyle;
                    SalesBtn.Style = navStyle;

                    PurchaseBtn.Style = navStyle;
                    AnalyticsBtn.Style = navStyle;
                    ReportsBtn.Style = navStyle;
                    SettingsBtn.Style = navStyle;
                }

                if (textStyle != null)
                {
                    TitleIcon.Style = textStyle;
                    TitleText.Style = textStyle;
                    ChartTitle.Style = textStyle;
                    QuickActionsTitle.Style = textStyle;
                    SalesTitle.Style = textStyle;
                }

                if (listStyle != null)
                {
                    SalesListView.Style = listStyle;
                }

                // FIXED: Apply ListView item style for proper selection visibility
                if (listItemStyle != null)
                {
                    SalesListView.ItemContainerStyle = listItemStyle;
                }

                // FIXED: Apply header styles for better appearance
                ApplyGridViewHeaderStyles();

                ThemeToggleButton.Foreground = App.IsDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Apply themed styles error: {ex.Message}");
            }
        }

        private void ApplyGridViewHeaderStyles()
        {
            try
            {
                var headerStyle = App.IsDarkMode ? cachedDarkHeaderStyle : cachedLightHeaderStyle;

                if (headerStyle != null && SalesListView?.View is GridView gridView)
                {
                    foreach (GridViewColumn column in gridView.Columns)
                    {
                        column.HeaderContainerStyle = headerStyle;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Header style application error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Clean up timers
            animationTimer?.Stop();
            particleTimer?.Stop();
            clockTimer?.Stop();

            // Clear collections
            particles?.Clear();
            recentSales?.Clear();

            base.OnClosed(e);
        }

        // Performance toggle method (you can call this if needed)
        public void ToggleAnimations(bool enable)
        {
            isAnimationEnabled = enable;
            if (!enable)
            {
                particleTimer?.Stop();
                animationTimer?.Stop();
            }
            else
            {
                particleTimer?.Start();
                animationTimer?.Start();
            }
        }
    }

    // Data models remain the same
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