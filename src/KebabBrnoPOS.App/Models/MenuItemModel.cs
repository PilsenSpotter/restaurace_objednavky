using System.Collections.Generic;
using System.ComponentModel;

namespace KebabBrnoPOS.App.Models;

public class MenuItemModel : INotifyPropertyChanged
{
    private int _stock;

    public MenuItemModel(string id, string name, string category, decimal price, int stock, string description, IEnumerable<string> options)
    {
        Id = id;
        Name = name;
        Category = category;
        Price = price;
        _stock = stock;
        Description = description;
        Options = new List<string>(options);
    }

    public string Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal Price { get; }
    public string Description { get; }
    public List<string> Options { get; }

    public int Stock
    {
        get => _stock;
        set
        {
            if (_stock == value) return;
            _stock = value;
            OnPropertyChanged(nameof(Stock));
            OnPropertyChanged(nameof(IsAvailable));
        }
    }

    public bool IsAvailable => Stock > 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
