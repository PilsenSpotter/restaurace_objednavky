using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KebabBrnoPOS.App.Models;
using KebabBrnoPOS.App.Services;

namespace KebabBrnoPOS.App;

public partial class PickupWindow : Window
{
    public PickupWindow(PosStore store)
    {
        InitializeComponent();
        Store = store;

        ReadyOrders = new ListCollectionView(Store.Orders);
        ReadyOrders.Filter = o => o is OrderModel order && order.Status == OrderStatus.NaVydaj;
        ReadyOrders.SortDescriptions.Add(new SortDescription(nameof(OrderModel.CreatedAt), ListSortDirection.Descending));
        ReadyOrders.LiveFilteringProperties.Add(nameof(OrderModel.Status));
        ReadyOrders.IsLiveFiltering = true;

        DataContext = this;
    }

    public PosStore Store { get; }
    public ListCollectionView ReadyOrders { get; }

    private void MarkServed_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderModel order })
        {
            Store.UpdateStatus(order, OrderStatus.Vydano);
            ReadyOrders.Refresh();
        }
    }

    private void Reopen_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderModel order })
        {
            Store.UpdateStatus(order, OrderStatus.VyrizujeSe);
            ReadyOrders.Refresh();
        }
    }
}
