using SmartPOS.Shared.DTOs.Products;

namespace SmartPOS.Web.Services.Shahzain;

/// <summary>
/// Scoped cart state service â€” one instance per Blazor Server circuit (per user session).
/// Because AddScoped on Blazor Server = per SignalR connection lifetime, both Shop.razor
/// and Cart.razor receive the exact same instance, enabling seamless cart persistence
/// across navigation without query-string serialisation or JS sessionStorage.
///
/// Usage:
///   â€¢ In Shop.razor  â†’ AddItem / UpdateQuantity / RemoveItem / Clear
///   â€¢ In Cart.razor  â†’ Read Items, SubTotal, Count; call Clear on order success
///   â€¢ Subscribe to OnCartChanged and call StateHasChanged for reactive UI updates
/// </summary>
public sealed class CartStateService
{
    // â”€â”€ Internal mutable list â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private readonly List<CartItem> _items = new();

    /// <summary>Read-only view of current cart items.</summary>
    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Fires whenever the cart contents change (add / remove / update / clear).
    /// Subscribers should call InvokeAsync(StateHasChanged) to re-render.
    /// </summary>
    public event Action? OnCartChanged;

    // â”€â”€ Computed Properties â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    public int     Count    => _items.Sum(i => i.Quantity);
    public decimal SubTotal => _items.Sum(i => i.Product.Price * i.Quantity);

    // â”€â”€ Mutation Methods â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    /// <summary>Add a product to the cart. If it already exists, increments quantity.</summary>
    public void AddItem(ProductDto product, int qty = 1)
    {
        var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
        if (existing is not null)
            existing.Quantity += qty;
        else
            _items.Add(new CartItem { Product = product, Quantity = qty });

        NotifyChange();
    }

    /// <summary>Remove a product line from the cart entirely.</summary>
    public void RemoveItem(int productId)
    {
        _items.RemoveAll(i => i.Product.Id == productId);
        NotifyChange();
    }

    /// <summary>
    /// Adjust quantity by delta (+1 or -1). Removes the item if quantity drops to zero or below.
    /// </summary>
    public void UpdateQuantity(int productId, int delta)
    {
        var item = _items.FirstOrDefault(i => i.Product.Id == productId);
        if (item is null) return;

        item.Quantity += delta;
        if (item.Quantity <= 0)
            _items.Remove(item);

        NotifyChange();
    }

    /// <summary>Empty the cart (called after successful order placement).</summary>
    public void Clear()
    {
        _items.Clear();
        NotifyChange();
    }

    // â”€â”€ Private Helpers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    private void NotifyChange() => OnCartChanged?.Invoke();

    // â”€â”€ Nested DTO â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
    /// <summary>A single line in the cart: one product + its chosen quantity.</summary>
    public sealed class CartItem
    {
        public ProductDto Product  { get; set; } = new();
        public int        Quantity { get; set; } = 1;
    }
}
