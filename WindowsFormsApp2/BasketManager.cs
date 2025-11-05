using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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

    public static class BasketManager
    {
        private static List<BasketItem> items = new List<BasketItem>();

        public static event EventHandler BasketChanged;

        public static List<BasketItem> GetItems()
        {
            return new List<BasketItem>(items);
        }

        public static bool AddItem(BasketItem newItem)
        {
            try
            {
                // Валідація вхідних даних
                if (newItem == null)
                {
                    MessageBox.Show("Помилка: товар не може бути null", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (newItem.ColorSizeId <= 0)
                {
                    MessageBox.Show("Помилка: невірний ідентифікатор товару", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(newItem.Size))
                {
                    MessageBox.Show("Помилка: розмір не може бути порожнім", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(newItem.Name))
                {
                    MessageBox.Show("Помилка: назва товару не може бути порожньою", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (newItem.Quantity <= 0)
                {
                    MessageBox.Show("Помилка: кількість повинна бути більше 0", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (newItem.Price < 0)
                {
                    MessageBox.Show("Помилка: ціна не може бути від'ємною", "Помилка валідації",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Перевірка чи товар вже є в кошику
                var existingItem = items.FirstOrDefault(i => i.ColorSizeId == newItem.ColorSizeId);

                if (existingItem != null)
                {
                    // Оновлюємо кількість існуючого товару
                    existingItem.Quantity += newItem.Quantity;
                }
                else
                {
                    // Додаємо новий товар
                    items.Add(newItem);
                }

                // Викликаємо подію про зміну кошика
                OnBasketChanged();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні товару до кошика: {ex.Message}",
                    "Критична помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool RemoveItem(int colorSizeId)
        {
            try
            {
                // Шукаємо індекс товару за colorSizeId
                int indexToRemove = -1;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].ColorSizeId == colorSizeId)
                    {
                        indexToRemove = i;
                        break;
                    }
                }

                // Якщо знайшли - видаляємо
                if (indexToRemove >= 0 && indexToRemove < items.Count)
                {
                    items.RemoveAt(indexToRemove);
                    // Викликаємо подію про зміну кошика
                    OnBasketChanged();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при видаленні товару: {ex.Message}\n\nStack: {ex.StackTrace}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool UpdateQuantity(int colorSizeId, int newQuantity)
        {
            try
            {
                if (newQuantity <= 0)
                {
                    return RemoveItem(colorSizeId);
                }

                var existingItem = items.FirstOrDefault(i => i.ColorSizeId == colorSizeId);
                if (existingItem != null)
                {
                    existingItem.Quantity = newQuantity;
                    // Викликаємо подію про зміну кошика
                    OnBasketChanged();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при оновленні кількості: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void ClearBasket()
        {
            try
            {
                items.Clear();
                // Викликаємо подію про зміну кошика
                OnBasketChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при очищенні кошика: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static decimal GetTotalAmount()
        {
            try
            {
                return items.Sum(i => i.TotalPrice);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при розрахунку суми: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public static int GetItemCount()
        {
            return items.Count;
        }

        public static bool HasItem(int colorSizeId)
        {
            return items.Any(i => i.ColorSizeId == colorSizeId);
        }

        // Метод для виклику події
        private static void OnBasketChanged()
        {
            BasketChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}