using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using KebabBrnoPOS.App.Data;
using KebabBrnoPOS.App.Models;

namespace KebabBrnoPOS.App.Services;

public class PosStore : INotifyPropertyChanged
{
    private decimal _denniTrzba;
    private int _orderCounter;

    public PosStore()
    {
        Menu = new ObservableCollection<MenuItemModel>(SampleData.Menu);
        Orders = new ObservableCollection<OrderModel>();
    }

    public ObservableCollection<MenuItemModel> Menu { get; }
    public ObservableCollection<OrderModel> Orders { get; }

    public decimal DenniTrzba
    {
        get => _denniTrzba;
        private set
        {
            if (_denniTrzba == value) return;
            _denniTrzba = value;
            OnPropertyChanged(nameof(DenniTrzba));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<OrderModel>? OrderCreated;

    public bool TryCreateOrder(OrderModel order, out string? error)
    {
        if (!order.Items.Any())
        {
            error = "Objednávka je prázdná.";
            return false;
        }

        if (!ValidateStock(order, out error))
        {
            return false;
        }

        order.Number = GetNextOrderNumber();
        DeductStock(order);
        order.Status = OrderStatus.VyrizujeSe;
        Orders.Insert(0, order);
        DenniTrzba += order.Total;
        OrderCreated?.Invoke(order);
        return true;
    }

    public void UpdateStatus(OrderModel order, OrderStatus status)
    {
        order.Status = status;
    }

    public void CancelOrder(OrderModel order)
    {
        if (Orders.Contains(order))
        {
            Orders.Remove(order);
            Restock(order);
        }
    }

    public void RestockAll()
    {
        foreach (var item in Menu)
        {
            item.Stock += 10;
        }
    }

    private bool ValidateStock(OrderModel order, out string? error)
    {
        foreach (var group in order.Items.GroupBy(i => i.Item.Id))
        {
            var requested = group.Sum(g => g.Quantity);
            var menuItem = Menu.FirstOrDefault(m => m.Id == group.Key);
            if (menuItem is null)
            {
                error = $"Položka {group.Key} není v menu.";
                return false;
            }

            if (menuItem.Stock < requested)
            {
                error = $"Nedostatek skladu: {menuItem.Name} (skladem {menuItem.Stock}, požadováno {requested}).";
                return false;
            }
        }

        error = null;
        return true;
    }

    private void DeductStock(OrderModel order)
    {
        foreach (var group in order.Items.GroupBy(i => i.Item.Id))
        {
            var requested = group.Sum(g => g.Quantity);
            var menuItem = Menu.First(m => m.Id == group.Key);
            menuItem.Stock -= requested;
        }
    }

    private void Restock(OrderModel order)
    {
        foreach (var group in order.Items.GroupBy(i => i.Item.Id))
        {
            var amount = group.Sum(g => g.Quantity);
            var menuItem = Menu.First(m => m.Id == group.Key);
            menuItem.Stock += amount;
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int GetNextOrderNumber()
    {
        if (_orderCounter >= 999)
        {
            _orderCounter = 0;
        }

        _orderCounter += 1;
        return _orderCounter;
    }
}
