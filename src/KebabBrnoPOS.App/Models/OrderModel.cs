using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace KebabBrnoPOS.App.Models;

public class OrderModel : INotifyPropertyChanged
{
    public OrderModel(string? note = null)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.Now;
        Note = note;
        Items = new ObservableCollection<OrderItemModel>();
        Items.CollectionChanged += ItemsOnCollectionChanged;
        Status = OrderStatus.Nova;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    public string? Note { get; set; }

    private OrderStatus _status;
    public OrderStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    public ObservableCollection<OrderItemModel> Items { get; }

    public string DisplayCode => Id.ToString("N")[..6].ToUpperInvariant();
    public decimal Total => Items.Sum(i => i.Total);

    public event PropertyChangedEventHandler? PropertyChanged;

    private void ItemsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems.OfType<OrderItemModel>())
            {
                item.PropertyChanged -= ItemOnPropertyChanged;
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems.OfType<OrderItemModel>())
            {
                item.PropertyChanged += ItemOnPropertyChanged;
            }
        }

        OnPropertyChanged(nameof(Total));
    }

    private void ItemOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrderItemModel.Quantity) || e.PropertyName == nameof(OrderItemModel.Total))
        {
            OnPropertyChanged(nameof(Total));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
