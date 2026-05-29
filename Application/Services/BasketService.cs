using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BasketService
    {
        private List<BasketBag> _items = new();

        public IReadOnlyList<BasketBag> Items => _items;

        public event Action? OnChange;

        public void AddToBasket(BasketBag item)
        {
            var existing = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                _items.Add(item);
            }
            NotifyStateChanged();
        }

        public void RemoveFromBasket(int productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                _items.Remove(item);
                NotifyStateChanged();
            }
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    RemoveFromBasket(productId);
                }
                else
                {
                    item.Quantity = quantity;
                    NotifyStateChanged();
                }
            }
        }

        public void ClearBasket()
        {
            _items.Clear();
            NotifyStateChanged();
        }

        public int TotalItemsCount => _items.Sum(i => i.Quantity);

        public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
