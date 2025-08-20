using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JewelryStoreWPF
{
    public partial class InventoryPage : Window, INotifyPropertyChanged
    {
        // Collections for data
        public ObservableCollection<Inventory> Inventorys { get; set; }
        public ObservableCollection<Inventory> FilteredItems { get; set; }

        // Properties for statistics
        private int _totalItems = 0;
        private int _activeItems = 0;
        private int _lowStockItems = 0;
        private decimal _totalValue = 0;

        public int TotalItems
        {
            get => _totalItems;
            set { _totalItems = value; OnPropertyChanged(nameof(TotalItems)); }
        }

        public int ActiveItems
        {
            get => _activeItems;
            set { _activeItems = value; OnPropertyChanged(nameof(ActiveItems)); }
        }

        public int LowStockItems
        {
            get => _lowStockItems;
            set { _lowStockItems = value; OnPropertyChanged(nameof(LowStockItems)); }
        }

        public decimal TotalValue
        {
            get => _totalValue;
            set { _totalValue = value; OnPropertyChanged(nameof(TotalValue)); }
        }

        // Current editing item
        private Inventory _currentEditingItem = null;

        // Animation components
        private DispatcherTimer _particleTimer;
        private List<Ellipse> _particles = new List<Ellipse>();
        private Random _random = new Random();

        public InventoryPage()
        {
            InitializeComponent();
            InitializeData();
            InitializeUI();
            InitializeParticleAnimation();
            ApplyTheme(App.IsDarkMode);
        }

        #region Initialization

        private void InitializeData()
        {
            Inventorys = new ObservableCollection<Inventory>();
            FilteredItems = new ObservableCollection<Inventory>();

            // Load sample data with premium jewelry items
            LoadSampleData();

            InventoryListView.ItemsSource = FilteredItems;
            DataContext = this;
        }

        private void LoadSampleData()
        {
            var sampleItems = new List<Inventory>
            {
                new Inventory
                {
                    Id = 1,
                    ProductName = "Diamond Solitaire Engagement Ring",
                    Category = "Rings",
                    SKU = "DSR001",
                    Quantity = 8,
                    MinStock = 3,
                    PurchasePrice = 75000,
                    SellingPrice = 125000,
                    Status = "Active",
                    MetalType = "18K White Gold",
                    Weight = 4.2m,
                    Description = "Exquisite 1.5 carat diamond solitaire ring in 18K white gold setting. GIA certified diamond with excellent cut, color H, clarity VS1."
                },
                new Inventory
                {
                    Id = 2,
                    ProductName = "Pearl Drop Necklace",
                    Category = "Necklaces",
                    SKU = "PDN002",
                    Quantity = 2,
                    MinStock = 5,
                    PurchasePrice = 35000,
                    SellingPrice = 55000,
                    Status = "Low Stock",
                    MetalType = "14K Yellow Gold",
                    Weight = 12.5m,
                    Description = "Elegant Akoya pearl drop necklace with 14K yellow gold chain. Features 7-8mm lustrous pearls."
                },
                new Inventory
                {
                    Id = 3,
                    ProductName = "Sapphire Stud Earrings",
                    Category = "Earrings",
                    SKU = "SSE003",
                    Quantity = 0,
                    MinStock = 4,
                    PurchasePrice = 25000,
                    SellingPrice = 42000,
                    Status = "Out of Stock",
                    MetalType = "Platinum",
                    Weight = 2.8m,
                    Description = "Ceylon sapphire stud earrings set in platinum. 1 carat total weight with matching blue sapphires."
                },
                new Inventory
                {
                    Id = 4,
                    ProductName = "Tennis Bracelet",
                    Category = "Bracelets",
                    SKU = "TB004",
                    Quantity = 12,
                    MinStock = 6,
                    PurchasePrice = 95000,
                    SellingPrice = 165000,
                    Status = "Active",
                    MetalType = "18K White Gold",
                    Weight = 18.7m,
                    Description = "Classic diamond tennis bracelet featuring 3 carats of round brilliant diamonds in 18K white gold."
                },
                new Inventory
                {
                    Id = 5,
                    ProductName = "Emerald Pendant",
                    Category = "Pendants",
                    SKU = "EP005",
                    Quantity = 6,
                    MinStock = 3,
                    PurchasePrice = 45000,
                    SellingPrice = 78000,
                    Status = "Active",
                    MetalType = "14K Rose Gold",
                    Weight = 5.3m,
                    Description = "Colombian emerald pendant in 14K rose gold with diamond halo. 2.5 carat emerald-cut emerald."
                },
                new Inventory
                {
                    Id = 6,
                    ProductName = "Vintage Watch Collection",
                    Category = "Watches",
                    SKU = "VWC006",
                    Quantity = 1,
                    MinStock = 2,
                    PurchasePrice = 150000,
                    SellingPrice = 225000,
                    Status = "Low Stock",
                    MetalType = "18K Yellow Gold",
                    Weight = 85.2m,
                    Description = "Luxury Swiss automatic watch with 18K gold case and leather strap. Limited edition timepiece."
                }
            };

            foreach (var item in sampleItems)
            {
                Inventorys.Add(item);
                FilteredItems.Add(item);
            }

            UpdateStatistics();
        }

        private void InitializeUI()
        {
            // Initialize category combo boxes
            var categories = new List<string>
            {
                "Rings", "Necklaces", "Earrings", "Bracelets", "Watches", "Pendants"
            };
            cmbCategory.ItemsSource = categories;
            cmbFilterCategory.ItemsSource = new List<string> { "All Categories" }.Concat(categories).ToList();
            cmbFilterCategory.SelectedIndex = 0;

            // Initialize metal types
            var metalTypes = new List<string>
            {
                "18K Yellow Gold", "18K White Gold", "18K Rose Gold",
                "14K Yellow Gold", "14K White Gold", "14K Rose Gold",
                "Platinum", "Sterling Silver", "Titanium"
            };
            cmbMetalType.ItemsSource = metalTypes;

            // Initialize status combo boxes
            var statuses = new List<string> { "Active", "Low Stock", "Out of Stock", "Inactive" };
            cmbFilterStatus.ItemsSource = new List<string> { "All Statuses" }.Concat(statuses).ToList();
            cmbFilterStatus.SelectedIndex = 0;
        }

        private void InitializeParticleAnimation()
        {
            _particleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };
            _particleTimer.Tick += ParticleTimer_Tick;
            _particleTimer.Start();

            // Create initial particles
            for (int i = 0; i < 25; i++)
            {
                CreateParticle();
            }
        }

        #endregion

        #region Particle Animation

        private void CreateParticle()
        {
            var particle = new Ellipse
            {
                Width = _random.Next(2, 5),
                Height = _random.Next(2, 5),
                Fill = new SolidColorBrush(Color.FromArgb((byte)_random.Next(30, 80), 102, 126, 234)),
                Opacity = _random.NextDouble() * 0.6 + 0.2
            };

            Canvas.SetLeft(particle, _random.NextDouble() * ActualWidth);
            Canvas.SetTop(particle, _random.NextDouble() * ActualHeight);

            ParticleCanvas.Children.Add(particle);
            _particles.Add(particle);
        }

        private void ParticleTimer_Tick(object sender, EventArgs e)
        {
            foreach (var particle in _particles.ToList())
            {
                var currentX = Canvas.GetLeft(particle);
                var currentY = Canvas.GetTop(particle);

                // Gentle floating motion
                var newX = currentX + (_random.NextDouble() - 0.5) * 2;
                var newY = currentY + (_random.NextDouble() - 0.5) * 2;

                // Wrap around screen
                if (newX < -10) newX = ActualWidth + 10;
                if (newX > ActualWidth + 10) newX = -10;
                if (newY < -10) newY = ActualHeight + 10;
                if (newY > ActualHeight + 10) newY = -10;

                Canvas.SetLeft(particle, newX);
                Canvas.SetTop(particle, newY);

                // Subtle opacity changes
                if (_random.NextDouble() < 0.01)
                {
                    particle.Opacity = _random.NextDouble() * 0.6 + 0.2;
                }
            }
        }

        #endregion

        #region Event Handlers

        // Title bar events
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, args) => Close();
            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            App.IsDarkMode = !App.IsDarkMode;
            ApplyTheme(App.IsDarkMode);
        }

        // Navigation
        private void DashboardBtn_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            Close();
        }

        // Form events
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                var newItem = CreateInventoryFromForm();
                newItem.Id = Inventorys.Count > 0 ? Inventorys.Max(x => x.Id) + 1 : 1;

                Inventorys.Add(newItem);
                FilteredItems.Add(newItem);

                ClearForm();
                UpdateStatistics();
                ShowSuccessMessage("Item saved successfully!");
            }
        }

        private void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEditingItem != null && ValidateForm())
            {
                UpdateInventoryFromForm(_currentEditingItem);
                ClearForm();
                UpdateStatistics();
                ApplyFilters();
                ShowSuccessMessage("Item updated successfully!");
            }
        }

        private void ClearForm_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as Inventory;
            if (item != null)
            {
                LoadItemToForm(item);
            }
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button?.Tag as Inventory;

            if (item != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{item.ProductName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Inventorys.Remove(item);
                    FilteredItems.Remove(item);
                    UpdateStatistics();
                    ShowSuccessMessage("Item deleted successfully!");
                }
            }
        }

        // Search and filter events
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            ApplyFilters();
        }

        private void InventoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed
        }

        #endregion

        #region Helper Methods

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                ShowValidationError("Product name is required.", txtProductName);
                return false;
            }

            if (cmbCategory.SelectedItem == null)
            {
                ShowValidationError("Please select a category.", cmbCategory);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSKU.Text))
            {
                ShowValidationError("SKU code is required.", txtSKU);
                return false;
            }

            // Check for duplicate SKU
            if (_currentEditingItem == null || _currentEditingItem.SKU != txtSKU.Text.Trim().ToUpper())
            {
                if (Inventorys.Any(x => x.SKU == txtSKU.Text.Trim().ToUpper()))
                {
                    ShowValidationError("SKU code already exists. Please use a different code.", txtSKU);
                    return false;
                }
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                ShowValidationError("Please enter a valid quantity.", txtQuantity);
                return false;
            }

            if (!decimal.TryParse(txtPurchasePrice.Text, out decimal purchasePrice) || purchasePrice <= 0)
            {
                ShowValidationError("Please enter a valid purchase price.", txtPurchasePrice);
                return false;
            }

            if (!decimal.TryParse(txtSellingPrice.Text, out decimal sellingPrice) || sellingPrice <= 0)
            {
                ShowValidationError("Please enter a valid selling price.", txtSellingPrice);
                return false;
            }

            if (!int.TryParse(txtMinStock.Text, out int minStock) || minStock < 0)
            {
                ShowValidationError("Please enter a valid minimum stock level.", txtMinStock);
                return false;
            }

            return true;
        }

        private Inventory CreateInventoryFromForm()
        {
            decimal.TryParse(txtWeight.Text, out decimal weight);

            return new Inventory
            {
                ProductName = txtProductName.Text.Trim(),
                Category = cmbCategory.SelectedItem.ToString(),
                SKU = txtSKU.Text.Trim().ToUpper(),
                Quantity = int.Parse(txtQuantity.Text),
                MinStock = int.Parse(txtMinStock.Text),
                PurchasePrice = decimal.Parse(txtPurchasePrice.Text),
                SellingPrice = decimal.Parse(txtSellingPrice.Text),
                Status = DetermineStatus(int.Parse(txtQuantity.Text), int.Parse(txtMinStock.Text)),
                MetalType = cmbMetalType.SelectedItem?.ToString() ?? "",
                Weight = weight,
                Description = txtDescription.Text.Trim()
            };
        }

        private void UpdateInventoryFromForm(Inventory item)
        {
            item.ProductName = txtProductName.Text.Trim();
            item.Category = cmbCategory.SelectedItem.ToString();
            item.SKU = txtSKU.Text.Trim().ToUpper();
            item.Quantity = int.Parse(txtQuantity.Text);
            item.MinStock = int.Parse(txtMinStock.Text);
            item.PurchasePrice = decimal.Parse(txtPurchasePrice.Text);
            item.SellingPrice = decimal.Parse(txtSellingPrice.Text);
            item.Status = DetermineStatus(int.Parse(txtQuantity.Text), int.Parse(txtMinStock.Text));
            item.MetalType = cmbMetalType.SelectedItem?.ToString() ?? "";
            decimal.TryParse(txtWeight.Text, out decimal weight);
            item.Weight = weight;
            item.Description = txtDescription.Text.Trim();
        }

        private string DetermineStatus(int quantity, int minStock)
        {
            if (quantity == 0)
                return "Out of Stock";
            else if (quantity <= minStock)
                return "Low Stock";
            else
                return "Active";
        }

        private void LoadItemToForm(Inventory item)
        {
            _currentEditingItem = item;

            txtProductName.Text = item.ProductName;
            cmbCategory.SelectedItem = item.Category;
            txtSKU.Text = item.SKU;
            txtQuantity.Text = item.Quantity.ToString();
            txtMinStock.Text = item.MinStock.ToString();
            txtPurchasePrice.Text = item.PurchasePrice.ToString();
            txtSellingPrice.Text = item.SellingPrice.ToString();
            cmbMetalType.SelectedItem = item.MetalType;
            txtWeight.Text = item.Weight.ToString();
            txtDescription.Text = item.Description;

            // Update form title and buttons with animation
            FormTitle.Text = "✏️ Edit Item";
            btnSave.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Visible;

            // Animate form border
            var scaleTransform = new ScaleTransform(1.02, 1.02);
            FormBorder.RenderTransform = scaleTransform;
            var scaleAnimation = new DoubleAnimation(1.02, 1.0, TimeSpan.FromMilliseconds(200));
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        private void ClearForm()
        {
            _currentEditingItem = null;

            txtProductName.Clear();
            cmbCategory.SelectedIndex = -1;
            txtSKU.Clear();
            txtQuantity.Clear();
            txtMinStock.Clear();
            txtPurchasePrice.Clear();
            txtSellingPrice.Clear();
            cmbMetalType.SelectedIndex = -1;
            txtWeight.Clear();
            txtDescription.Clear();

            // Reset form title and buttons
            FormTitle.Text = "➕ Add New Item";
            btnSave.Visibility = Visibility.Visible;
            btnUpdate.Visibility = Visibility.Collapsed;
        }

        private void ApplyFilters()
        {
            var searchTerm = txtSearch.Text?.ToLower() ?? "";
            var categoryFilter = cmbFilterCategory.SelectedItem?.ToString();
            var statusFilter = cmbFilterStatus.SelectedItem?.ToString();

            var filtered = Inventorys.Where(item =>
            {
                bool matchesSearch = string.IsNullOrEmpty(searchTerm) ||
                    item.ProductName.ToLower().Contains(searchTerm) ||
                    item.SKU.ToLower().Contains(searchTerm) ||
                    item.Category.ToLower().Contains(searchTerm) ||
                    item.MetalType.ToLower().Contains(searchTerm);

                bool matchesCategory = categoryFilter == "All Categories" || item.Category == categoryFilter;
                bool matchesStatus = statusFilter == "All Statuses" || item.Status == statusFilter;

                return matchesSearch && matchesCategory && matchesStatus;
            });

            FilteredItems.Clear();
            foreach (var item in filtered)
            {
                FilteredItems.Add(item);
            }

            // Show/hide empty state
            EmptyStatePanel.Visibility = FilteredItems.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateStatistics()
        {
            TotalItems = Inventorys.Count;
            ActiveItems = Inventorys.Count(x => x.Status == "Active");
            LowStockItems = Inventorys.Count(x => x.Status == "Low Stock" || x.Status == "Out of Stock");
            TotalValue = Inventorys.Sum(x => x.SellingPrice * x.Quantity);

            // Update UI text blocks with animations
            AnimateStatUpdate(TotalItemsText, $"Total: {TotalItems}");
            AnimateStatUpdate(ActiveItemsText, $"Active: {ActiveItems}");
            AnimateStatUpdate(LowStockText, $"Low Stock: {LowStockItems}");
            AnimateStatUpdate(TotalValueText, $"₹{TotalValue:N0}");
        }

        private void AnimateStatUpdate(TextBlock textBlock, string newText)
        {
            var fadeOut = new DoubleAnimation(1, 0.3, TimeSpan.FromMilliseconds(150));
            fadeOut.Completed += (s, e) =>
            {
                textBlock.Text = newText;
                var fadeIn = new DoubleAnimation(0.3, 1, TimeSpan.FromMilliseconds(150));
                textBlock.BeginAnimation(OpacityProperty, fadeIn);
            };
            textBlock.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void ShowValidationError(string message, FrameworkElement focusElement)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            focusElement?.Focus();

            // Add red border animation for visual feedback
            var originalBrush = focusElement.GetValue(Control.BorderBrushProperty);
            focusElement.SetValue(Control.BorderBrushProperty, Brushes.Red);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) =>
            {
                focusElement.SetValue(Control.BorderBrushProperty, originalBrush);
                timer.Stop();
            };
            timer.Start();
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Theme Management

        private void ApplyTheme(bool isDarkMode)
        {
            ThemeToggleButton.Content = isDarkMode ? "🌙" : "☀️";

            // Update background gradient
            if (isDarkMode)
            {
                MainGrid.Background = new RadialGradientBrush
                {
                    GradientOrigin = new Point(0.3, 0.3),
                    RadiusX = 0.8,
                    RadiusY = 0.8,
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(15, 15, 35), 0),
                        new GradientStop(Color.FromRgb(26, 26, 46), 0.5),
                        new GradientStop(Color.FromRgb(22, 33, 62), 1)
                    }
                };
                btnMinimized.Foreground = Brushes.White;
            }
            else
            {
                MainGrid.Background = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1),
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromRgb(240, 242, 255), 0),
                        new GradientStop(Color.FromRgb(250, 252, 255), 0.5),
                        new GradientStop(Color.FromRgb(245, 247, 250), 1)
                    }
                };
                btnMinimized.Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }

            // Apply themed styles
            ApplyThemedStyles(isDarkMode);

            // Animate theme transition
            var themeAnimation = new DoubleAnimation(0.95, 1.0, TimeSpan.FromMilliseconds(300));
            MainGrid.BeginAnimation(OpacityProperty, themeAnimation);
        }

        private void ApplyThemedStyles(bool isDarkMode)
        {
            try
            {
                var glassStyle = isDarkMode ?
                    FindResource("GlassPanelDark") as Style :
                    FindResource("GlassPanelLight") as Style;

                var navStyle = isDarkMode ?
                    FindResource("NavButtonDark") as Style :
                    FindResource("NavButtonLight") as Style;

                var textStyle = isDarkMode ?
                    FindResource("TextBlockDark") as Style :
                    FindResource("TextBlockLight") as Style;

                var textBoxStyle = isDarkMode ?
                    FindResource("ModernTextBoxDark") as Style :
                    FindResource("ModernTextBoxLight") as Style;

                var comboBoxStyle = isDarkMode ?
                    FindResource("ModernComboBoxDark") as Style :
                    FindResource("ModernComboBoxLight") as Style;

                var listStyle = isDarkMode ?
                    FindResource("ListViewDark") as Style :
                    FindResource("ListViewLight") as Style;

                var listItemStyle = isDarkMode ?
                    FindResource("ListViewItemDark") as Style :
                    FindResource("ListViewItemLight") as Style;

                var headerStyle = isDarkMode ?
                    FindResource("GridViewHeaderDark") as Style :
                    FindResource("GridViewHeaderLight") as Style;

                // Apply styles
                if (glassStyle != null)
                {
                    SidebarBorder.Style = glassStyle;
                    FormBorder.Style = glassStyle;
                    SearchBorder.Style = glassStyle;
                    InventoryBorder.Style = glassStyle;
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
                }

                // Apply form control styles
                if (textBoxStyle != null)
                {
                    txtProductName.Style = textBoxStyle;
                    txtSKU.Style = textBoxStyle;
                    txtQuantity.Style = textBoxStyle;
                    txtPurchasePrice.Style = textBoxStyle;
                    txtSellingPrice.Style = textBoxStyle;
                    txtMinStock.Style = textBoxStyle;
                    txtWeight.Style = textBoxStyle;
                    txtDescription.Style = textBoxStyle;
                    txtSearch.Style = textBoxStyle;
                }

                if (comboBoxStyle != null)
                {
                    cmbCategory.Style = comboBoxStyle;
                    cmbMetalType.Style = comboBoxStyle;
                    cmbFilterCategory.Style = comboBoxStyle;
                    cmbFilterStatus.Style = comboBoxStyle;
                }

                if (listStyle != null)
                {
                    InventoryListView.Style = listStyle;
                }

                if (listItemStyle != null)
                {
                    InventoryListView.ItemContainerStyle = listItemStyle;
                }

                // Apply header styles to GridView columns
                if (headerStyle != null && InventoryListView.View is GridView gridView)
                {
                    foreach (GridViewColumn column in gridView.Columns)
                    {
                        column.HeaderContainerStyle = headerStyle;
                    }
                }

                // Apply text styles to all labels
                ApplyTextStyles(textStyle);

                ThemeToggleButton.Foreground = isDarkMode ? Brushes.White : new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Theme application error: {ex.Message}");
            }
        }

        private void ApplyTextStyles(Style textStyle)
        {
            if (textStyle == null) return;

            // Find all TextBlocks and apply the style
            var textBlocks = new[]
            {
                TitleIcon, TitleText
            };

            foreach (var textBlock in textBlocks)
            {
                if (textBlock != null)
                {
                    textBlock.Style = textStyle;
                }
            }

            // Apply to labels in the form (these need to be found by traversing the visual tree)
            ApplyStyleToChildren<TextBlock>(FormBorder, textStyle);
            ApplyStyleToChildren<TextBlock>(SearchBorder, textStyle);
            ApplyStyleToChildren<TextBlock>(InventoryBorder, textStyle);
        }

        private void ApplyStyleToChildren<T>(DependencyObject parent, Style style) where T : FrameworkElement
        {
            if (parent == null) return;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T element && element.Name != "TotalItemsText" &&
                    element.Name != "ActiveItemsText" && element.Name != "LowStockText" &&
                    element.Name != "TotalValueText")
                {
                    element.Style = style;
                }

                ApplyStyleToChildren<T>(child, style);
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Cleanup

        protected override void OnClosed(EventArgs e)
        {
            // Clean up timers and resources
            _particleTimer?.Stop();
            _particles?.Clear();
            Inventorys?.Clear();
            FilteredItems?.Clear();

            base.OnClosed(e);
        }

        #endregion
    }

    #region Data Models

    // Enhanced Inventory Item Model
    public class Inventory : INotifyPropertyChanged
    {
        private int _id;
        private string _productName;
        private string _category;
        private string _sku;
        private int _quantity;
        private int _minStock;
        private decimal _purchasePrice;
        private decimal _sellingPrice;
        private string _status;
        private string _metalType;
        private decimal _weight;
        private string _description;

        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(nameof(Id)); }
        }

        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(nameof(ProductName)); }
        }

        public string Category
        {
            get => _category;
            set { _category = value; OnPropertyChanged(nameof(Category)); }
        }

        public string SKU
        {
            get => _sku;
            set { _sku = value; OnPropertyChanged(nameof(SKU)); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(nameof(Quantity)); }
        }

        public int MinStock
        {
            get => _minStock;
            set { _minStock = value; OnPropertyChanged(nameof(MinStock)); }
        }

        public decimal PurchasePrice
        {
            get => _purchasePrice;
            set { _purchasePrice = value; OnPropertyChanged(nameof(PurchasePrice)); }
        }

        public decimal SellingPrice
        {
            get => _sellingPrice;
            set { _sellingPrice = value; OnPropertyChanged(nameof(SellingPrice)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public string MetalType
        {
            get => _metalType;
            set { _metalType = value; OnPropertyChanged(nameof(MetalType)); }
        }

        public decimal Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(nameof(Weight)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        // Calculated properties
        public decimal ProfitMargin => SellingPrice > 0 ? ((SellingPrice - PurchasePrice) / SellingPrice) * 100 : 0;
        public decimal TotalValue => SellingPrice * Quantity;
        public bool IsLowStock => Quantity <= MinStock;
        public bool IsOutOfStock => Quantity == 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}