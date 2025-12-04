using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using KebabBrnoPOS.App.Models;
using KebabBrnoPOS.App.Services;

namespace KebabBrnoPOS.App;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly CultureInfo _czCulture = new("cs-CZ");
    private string _selectedCategory = "Vše";
    private OrderModel _aktivniObjednavka;
    private OrderModel? _naposledyOdeslana;
    private KitchenWindow? _kitchenWindow;

    public MainWindow()
    {
        InitializeComponent();

        Store = new PosStore();
        Categories = new ObservableCollection<string>(new[] { "Vše" }.Concat(Store.Menu.Select(m => m.Category).Distinct()));
        _aktivniObjednavka = new OrderModel();
        MenuView = CollectionViewSource.GetDefaultView(Store.Menu);
        MenuView.Filter = MenuFilter;

        foreach (var item in Store.Menu)
        {
            item.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(MenuItemModel.Stock))
                {
                    OnPropertyChanged(nameof(LowStockInfo));
                }
            };
        }

        DataContext = this;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PosStore Store { get; }
    public ObservableCollection<string> Categories { get; }
    public ICollectionView MenuView { get; }

    public OrderModel AktivniObjednavka
    {
        get => _aktivniObjednavka;
        private set
        {
            _aktivniObjednavka = value;
            OnPropertyChanged(nameof(AktivniObjednavka));
        }
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (_selectedCategory == value) return;
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));
            MenuView.Refresh();
        }
    }

    public string LowStockInfo
    {
        get
        {
            var low = Store.Menu.Where(m => m.Stock <= 5).ToList();
            return low.Any()
                ? "Nízký sklad: " + string.Join(", ", low.Select(l => $"{l.Name} ({l.Stock} ks)"))
                : "Sklad v pořádku";
        }
    }

    private bool MenuFilter(object obj)
    {
        if (obj is not MenuItemModel item) return false;
        if (SelectedCategory == "Vše") return true;
        return item.Category == SelectedCategory;
    }

    private void AddItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: MenuItemModel item }) return;

        if (!item.IsAvailable)
        {
            MessageBox.Show($"{item.Name} není skladem.", "Sklad", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var defaultOption = item.Options.FirstOrDefault() ?? string.Empty;
        var existing = AktivniObjednavka.Items.FirstOrDefault(i => i.Item.Id == item.Id && i.SelectedOption == defaultOption);
        if (existing is null)
        {
            AktivniObjednavka.Items.Add(new OrderItemModel(item, 1, defaultOption));
        }
        else
        {
            existing.Quantity += 1;
        }
    }

    private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderItemModel item })
        {
            item.Quantity += 1;
        }
    }

    private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: OrderItemModel item }) return;

        if (item.Quantity <= 1)
        {
            AktivniObjednavka.Items.Remove(item);
        }
        else
        {
            item.Quantity -= 1;
        }
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderItemModel item })
        {
            AktivniObjednavka.Items.Remove(item);
        }
    }

    private void SubmitOrder_Click(object sender, RoutedEventArgs e)
    {
        if (Store.TryCreateOrder(AktivniObjednavka, out var error))
        {
            _naposledyOdeslana = AktivniObjednavka;
            MessageBox.Show($"Objednávka #{_naposledyOdeslana.DisplayCode} odeslána do kuchyně.", "Odesláno", MessageBoxButton.OK, MessageBoxImage.Information);
            AktivniObjednavka = new OrderModel();
            OnPropertyChanged(nameof(LowStockInfo));
        }
        else
        {
            MessageBox.Show(error ?? "Objednávku se nepodařilo odeslat.", "Chyba", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ResetOrder_Click(object sender, RoutedEventArgs e)
    {
        AktivniObjednavka = new OrderModel();
    }

    private void PrintReceipt_Click(object sender, RoutedEventArgs e)
    {
        var order = _naposledyOdeslana ?? AktivniObjednavka;
        if (!order.Items.Any())
        {
            MessageBox.Show("Není co tisknout. Přidejte položky nebo odešlete objednávku.", "Tisk", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        PrintReceipt(order);
    }

    private void OpenKitchenDisplay_Click(object sender, RoutedEventArgs e)
    {
        if (_kitchenWindow is { IsVisible: true })
        {
            _kitchenWindow.Activate();
            return;
        }

        _kitchenWindow = new KitchenWindow(Store)
        {
            Owner = this
        };
        _kitchenWindow.Show();
    }

    private void Restock_Click(object sender, RoutedEventArgs e)
    {
        Store.RestockAll();
        OnPropertyChanged(nameof(LowStockInfo));
    }

    private void PrintReceipt(OrderModel order)
    {
        try
        {
            var doc = BuildReceiptDocument(order);
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, $"Objednávka {order.DisplayCode}");
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show($"Tisk selhal: {ex.Message}", "Tisk", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private FlowDocument BuildReceiptDocument(OrderModel order)
    {
        var doc = new FlowDocument
        {
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12,
            PagePadding = new Thickness(20),
            ColumnWidth = 300
        };

        doc.Blocks.Add(new Paragraph(new Bold(new Run("Kebab Brno")))
        {
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 4)
        });

        doc.Blocks.Add(new Paragraph(new Run($"Objednávka #{order.DisplayCode}")) { TextAlignment = TextAlignment.Center });
        doc.Blocks.Add(new Paragraph(new Run($"Čas: {order.CreatedAt:dd.MM.yyyy HH:mm}")) { TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 0, 0, 10) });
        doc.Blocks.Add(new Paragraph(new Run("Položky:")) { Margin = new Thickness(0, 0, 0, 4) });

        foreach (var item in order.Items)
        {
            var line = $"{item.Quantity}x {item.Item.Name} ({item.SelectedOption}) - {item.Total.ToString("C", _czCulture)}";
            doc.Blocks.Add(new Paragraph(new Run(line)) { Margin = new Thickness(0, 0, 0, 2) });

            if (!string.IsNullOrWhiteSpace(item.Note))
            {
                doc.Blocks.Add(new Paragraph(new Run($"  Pozn.: {item.Note}")) { Margin = new Thickness(0, 0, 0, 2) });
            }
        }

        doc.Blocks.Add(new Paragraph(new Bold(new Run($"Celkem: {order.Total.ToString("C", _czCulture)}"))) { Margin = new Thickness(0, 6, 0, 2) });

        if (!string.IsNullOrWhiteSpace(order.Note))
        {
            doc.Blocks.Add(new Paragraph(new Run($"Poznámka: {order.Note}")) { Margin = new Thickness(0, 0, 0, 4) });
        }

        doc.Blocks.Add(new Paragraph(new Run("Děkujeme!")) { TextAlignment = TextAlignment.Center, Margin = new Thickness(0, 6, 0, 0) });

        return doc;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
