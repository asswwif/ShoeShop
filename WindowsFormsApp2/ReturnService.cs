using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class ReturnService
    {
        public DataTable GetAllReturnsData()
        {
            DataTable dt = new DataTable();

            try
            {
                if (!DbConection.ConnectionDB())
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних.", "Помилка БД");
                    return dt;
                }

                string query = @"
                    SELECT
                        R.return_id AS 'ID',
                        R.return_date AS 'Дата повернення',
                        P.product_name AS 'Товар',
                        C.color_name AS 'Колір',
                        S.size_value AS 'Розмір',
                        R.product_quantity AS 'Кількість',
                        R.reason AS 'Причина',
                        CONCAT(E.first_name, ' ', E.last_name) AS 'Працівник'
                    FROM
                        `Return` R
                    JOIN Employee E ON R.employee_id = E.employee_id
                    JOIN Sale_Details SD ON R.sale_detail_id = SD.sale_detail_id
                    JOIN Color_Size CS ON SD.color_size_id = CS.color_size_id
                    JOIN Product P ON CS.product_id = P.product_id
                    JOIN Color C ON CS.color_id = C.color_id
                    JOIN Size S ON CS.size_id = S.size_id
                    ORDER BY
                        R.return_date DESC, R.return_id DESC;";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
                {
                    adapter.Fill(dt);
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження повернень: {ex.Message}", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return dt;
        }

        // Фільтрація повернень за датами
        public DataTable GetReturnsDataByDateRange(DateTime startDate, DateTime endDate)
        {
            DataTable dataTable = new DataTable();

            try
            {
                if (!DbConection.ConnectionDB())
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних.", "Помилка БД");
                    return dataTable;
                }

                string query = @"
            SELECT
                R.return_id AS 'ID',
                R.return_date AS 'Дата повернення',
                P.product_name AS 'Товар',
                C.color_name AS 'Колір',
                S.size_value AS 'Розмір',
                R.product_quantity AS 'Кількість',
                R.reason AS 'Причина',
                CONCAT(E.first_name, ' ', E.last_name) AS 'Працівник'
            FROM
                `Return` R
            JOIN Employee E ON R.employee_id = E.employee_id
            JOIN Sale_Details SD ON R.sale_detail_id = SD.sale_detail_id
            JOIN Color_Size CS ON SD.color_size_id = CS.color_size_id
            JOIN Product P ON CS.product_id = P.product_id
            JOIN Color C ON CS.color_id = C.color_id
            JOIN Size S ON CS.size_id = S.size_id
            WHERE 
                R.return_date >= @startDate AND R.return_date < @endDate
            ORDER BY R.return_date DESC;";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();

                DbConection.msCommand.Parameters.AddWithValue("@startDate", startDate);
                DbConection.msCommand.Parameters.AddWithValue("@endDate", endDate);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
                {
                    adapter.Fill(dataTable);
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при отриманні даних повернень: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return dataTable;
        }

        // Отримання деталей продажу для повернення
        public DataTable GetSaleDetailsForReturn(int saleId)
        {
            DataTable dt = new DataTable();

            try
            {
                if (!DbConection.ConnectionDB()) return dt;

                string query = @"
                    SELECT
                        sd.sale_detail_id, 
                        p.product_name, 
                        c.color_name, 
                        s.size_value, 
                        sd.product_quantity,
                        sd.color_size_id, 
                        sd.unit_price 
                    FROM 
                        Sale_Details sd
                    JOIN Color_Size cs ON sd.color_size_id = cs.color_size_id
                    JOIN Product p ON cs.product_id = p.product_id
                    JOIN Color c ON cs.color_id = c.color_id
                    JOIN Size s ON cs.size_id = s.size_id
                    WHERE sd.sale_id = @sale_id;";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@sale_id", saleId);

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
                {
                    adapter.Fill(dt);
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження деталей продажу: {ex.Message}", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return dt;
        }

        // Додавання повернення
        public bool AddReturn(int saleId, int productId, int quantity, decimal refundAmount, string reason)
        {
            try
            {
                if (!DbConection.ConnectionDB()) return false;

                string query = @"
                    INSERT INTO returns (sale_id, product_id, quantity, refund_amount, reason, return_datetime)
                    VALUES (@saleId, @productId, @quantity, @refundAmount, @reason, @returnDatetime);";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@saleId", saleId);
                DbConection.msCommand.Parameters.AddWithValue("@productId", productId);
                DbConection.msCommand.Parameters.AddWithValue("@quantity", quantity);
                DbConection.msCommand.Parameters.AddWithValue("@refundAmount", refundAmount);
                DbConection.msCommand.Parameters.AddWithValue("@reason", reason);
                DbConection.msCommand.Parameters.AddWithValue("@returnDatetime", DateTime.Now);

                int result = DbConection.msCommand.ExecuteNonQuery();
                DbConection.CloseDB();

                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання повернення: {ex.Message}", "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
                return false;
            }
        }

        public bool ProcessReturn(int saleDetailId, int employeeId, int colorSizeId, int quantity, string reason)
        {
            bool success = false;
            MySqlTransaction transaction = null;

            if (!DbConection.ConnectionDB()) return false;

            try
            {
                transaction = DbConection.msConnection.BeginTransaction();
                DbConection.msCommand.Transaction = transaction;

                // Додаємо запис у таблицю Return
                string insertQuery = @"
                    INSERT INTO `Return` (sale_detail_id, employee_id, return_date, product_quantity, reason)
                    VALUES (@saleDetailId, @employeeId, NOW(), @quantity, @reason);";

                DbConection.msCommand.CommandText = insertQuery;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@saleDetailId", saleDetailId);
                DbConection.msCommand.Parameters.AddWithValue("@employeeId", employeeId);
                DbConection.msCommand.Parameters.AddWithValue("@quantity", quantity);
                DbConection.msCommand.Parameters.AddWithValue("@reason", reason);
                DbConection.msCommand.ExecuteNonQuery();

                // Оновлюємо залишок на складі
                string updateStockQuery = @"
                    UPDATE Color_Size
                    SET stock_quantity = stock_quantity + @quantity
                    WHERE color_size_id = @colorSizeId;";

                DbConection.msCommand.CommandText = updateStockQuery;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@quantity", quantity);
                DbConection.msCommand.Parameters.AddWithValue("@colorSizeId", colorSizeId);
                DbConection.msCommand.ExecuteNonQuery();

                transaction.Commit();
                success = true;
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Помилка при оформленні повернення (Транзакція відкочена): {ex.Message}",
                                "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                DbConection.CloseDB();
            }

            return success;
        }
    }
}
