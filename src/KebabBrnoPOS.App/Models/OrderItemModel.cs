using System.ComponentModel;
using System.Linq;

namespace KebabBrnoPOS.App.Models;

public class OrderItemModel : INotifyPropertyChanged
{
    private int _quantity;
    private string _selectedOption;
    private string? _note;

    public OrderItemModel(MenuItemModel item, int quantity, string? option = null, string? note = null)
    {
        Item = item;
        _quantity = quantity;
        _selectedOption = option ?? item.Options.FirstOrDefault() ?? string.Empty;
        _note = note;
    }

    public MenuItemModel Item { get; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            OnPropertyChanged(nameof(Quantity));
            OnPropertyChanged(nameof(Total));
        }
    }

    public string SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (_selectedOption == value) return;
            _selectedOption = value;
            OnPropertyChanged(nameof(SelectedOption));
        }
    }

    public string? Note
    {
        get => _note;
        set
        {
            if (_note == value) return;
            _note = value;
            OnPropertyChanged(nameof(Note));
        }
    }

    public decimal Total => Item.Price * Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
