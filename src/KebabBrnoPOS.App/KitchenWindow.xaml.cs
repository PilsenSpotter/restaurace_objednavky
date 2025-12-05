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

        PendingOrders = CreateView(o => o is OrderModel order && (order.Status == OrderStatus.Nova || order.Status == OrderStatus.VyrizujeSe));

        DataContext = this;
    }

    public PosStore Store { get; }
    public ListCollectionView PendingOrders { get; }

    private void MarkReady_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OrderModel order })
        {
            Store.UpdateStatus(order, OrderStatus.NaVydaj);
            PendingOrders.Refresh();
        }
    }

    private ListCollectionView CreateView(Predicate<object> filter)
    {
        var view = new ListCollectionView(Store.Orders);
        view.Filter = filter;
        view.SortDescriptions.Add(new SortDescription(nameof(OrderModel.CreatedAt), ListSortDirection.Descending));
        view.LiveFilteringProperties.Add(nameof(OrderModel.Status));
        view.IsLiveFiltering = true;
        return view;
    }
}
