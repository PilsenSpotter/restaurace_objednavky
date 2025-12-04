using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KebabBrnoPOS.App.Models;
using KebabBrnoPOS.App.Services;

namespace KebabBrnoPOS.App;

public partial class KitchenWindow : Window
{
    public KitchenWindow(PosStore store)
    {
        InitializeComponent();
        Store = store;

        PendingOrders = new ListCollectionView(Store.Orders);
        PendingOrders.Filter = o => o is OrderModel order && order.Status != OrderStatus.Hotovo;
        PendingOrders.SortDescriptions.Add(new SortDescription(nameof(OrderModel.CreatedAt), ListSortDirection.Descending));

        DoneOrders = new ListCollectionView(Store.Orders);
        DoneOrders.Filter = o => o is OrderModel order && order.Status == OrderStatus.Hotovo;
        DoneOrders.SortDescriptions.Add(new SortDescription(nameof(OrderModel.CreatedAt), ListSortDirection.Descending));

        DataContext = this;
    }

    public PosStore Store { get; }
    public ICollectionView PendingOrders { get; }
    public ICollectionView DoneOrders { get; }

    private void MarkDone_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderModel order })
        {
            Store.UpdateStatus(order, OrderStatus.Hotovo);
            RefreshViews();
        }
    }

    private void Reopen_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderModel order })
        {
            Store.UpdateStatus(order, OrderStatus.VyrizujeSe);
            RefreshViews();
        }
    }

    private void RefreshViews()
    {
        PendingOrders.Refresh();
        DoneOrders.Refresh();
    }
}
