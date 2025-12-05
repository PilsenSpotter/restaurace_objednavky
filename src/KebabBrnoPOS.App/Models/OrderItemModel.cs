using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        PortionStatesKitchen = new ObservableCollection<PortionState>();
        PortionStatesPickup = new ObservableCollection<PortionState>();

        PortionStatesKitchen.CollectionChanged += PortionStatesKitchenOnCollectionChanged;
        PortionStatesPickup.CollectionChanged += PortionStatesPickupOnCollectionChanged;

        SyncPortionStates();
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
            SyncPortionStates();
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

    public ObservableCollection<PortionState> PortionStatesKitchen { get; }
    public ObservableCollection<PortionState> PortionStatesPickup { get; }
    public bool IsFullyDoneKitchen => PortionStatesKitchen.Count == Quantity && PortionStatesKitchen.All(p => p.IsDone);
    public bool IsFullyDonePickup => PortionStatesPickup.Count == Quantity && PortionStatesPickup.All(p => p.IsDone);
    public decimal Total => Item.Price * Quantity;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SyncPortionStates()
    {
        SyncSinglePortionList(PortionStatesKitchen);
        SyncSinglePortionList(PortionStatesPickup);
        OnPropertyChanged(nameof(IsFullyDoneKitchen));
        OnPropertyChanged(nameof(IsFullyDonePickup));
    }

    private void SyncSinglePortionList(ObservableCollection<PortionState> list)
    {
        while (list.Count < Quantity)
        {
            list.Add(new PortionState());
        }

        while (list.Count > Quantity)
        {
            list.RemoveAt(list.Count - 1);
        }
    }

    private void PortionStatesKitchenOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        HandlePortionStatesCollectionChange(e, PortionStateOnKitchenChanged);
        OnPropertyChanged(nameof(IsFullyDoneKitchen));
    }

    private void PortionStatesPickupOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        HandlePortionStatesCollectionChange(e, PortionStateOnPickupChanged);
        OnPropertyChanged(nameof(IsFullyDonePickup));
    }

    private static void HandlePortionStatesCollectionChange(NotifyCollectionChangedEventArgs e, PropertyChangedEventHandler handler)
    {
        if (e.OldItems != null)
        {
            foreach (var oldState in e.OldItems.OfType<PortionState>())
            {
                oldState.PropertyChanged -= handler;
            }
        }

        if (e.NewItems != null)
        {
            foreach (var newState in e.NewItems.OfType<PortionState>())
            {
                newState.PropertyChanged += handler;
            }
        }
    }

    private void PortionStateOnKitchenChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PortionState.IsDone))
        {
            OnPropertyChanged(nameof(IsFullyDoneKitchen));
        }
    }

    private void PortionStateOnPickupChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PortionState.IsDone))
        {
            OnPropertyChanged(nameof(IsFullyDonePickup));
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class PortionState : INotifyPropertyChanged
{
    private bool _isDone;

    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (_isDone == value) return;
            _isDone = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDone)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
