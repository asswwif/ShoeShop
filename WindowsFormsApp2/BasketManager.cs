using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApp2
{
    public class BasketItem
    {
        public int ColorSizeId { get; set; }
        public string ArticleNumber { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    // Статичний клас для управління глобальним станом кошика.
    public static class BasketManager
    {
        private static List<BasketItem> items = new List<BasketItem>();

        public static List<BasketItem> GetItems()
        {
            return items;
        }

        public static void AddItem(BasketItem newItem)
        {
            var existingItem = items.FirstOrDefault(i => i.ColorSizeId == newItem.ColorSizeId);

            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                items.Add(newItem);
            }
        }

        public static bool RemoveItem(int colorSizeId)
        {
            var itemToRemove = items.FirstOrDefault(i => i.ColorSizeId == colorSizeId);
            if (itemToRemove != null)
            {
                return items.Remove(itemToRemove);
            }
            return false;
        }

        public static bool UpdateQuantity(int colorSizeId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                return RemoveItem(colorSizeId);
            }

            var existingItem = items.FirstOrDefault(i => i.ColorSizeId == colorSizeId);
            if (existingItem != null)
            {
                existingItem.Quantity = newQuantity;
                return true;
            }
            return false;
        }

        public static void ClearBasket()
        {
            items.Clear();
        }

        public static decimal GetTotalAmount()
        {
            return items.Sum(i => i.TotalPrice);
        }
    }
}