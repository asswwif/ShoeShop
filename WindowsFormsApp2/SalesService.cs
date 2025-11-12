using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    public class SalesService
    {
        public DataTable GetSalesHistory(DateTime startDate, DateTime endDate)
        {
            DataTable salesDt = new DataTable();

            try
            {
                if (!DbConection.ConnectionDB()) return salesDt;

                int currentEmployeeId = SessionManager.CurrentEmployeeId;

                string query = @"
                    SELECT
                        S.sale_id,
                        S.sale_datetime,
                        IFNULL(CONCAT(C.first_name, ' ', C.last_name), 'Гість') AS CustomerName,
                        CONCAT(E.first_name, ' ', E.last_name) AS EmployeeName,
                        S.sale_total
                    FROM
                        `Sale` S
                    LEFT JOIN
                        Customer C ON S.customer_id = C.customer_id
                    JOIN
                        Employee E ON S.employee_id = E.employee_id
                    WHERE
                        DATE(S.sale_datetime) BETWEEN @start_date AND @end_date
                        AND S.employee_id = @employee_id_filter -- <--- ДОДАНО ФІЛЬТР
                    ORDER BY
                        S.sale_datetime DESC;";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@start_date", startDate.Date);
                DbConection.msCommand.Parameters.AddWithValue("@end_date", endDate.Date);
                DbConection.msCommand.Parameters.AddWithValue("@employee_id_filter", currentEmployeeId); // <--- ДОДАНО ПАРАМЕТР

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
                {
                    adapter.Fill(salesDt);
                }

                salesDt.Columns.Add("Товари (Деталі)", typeof(string));

                foreach (DataRow row in salesDt.Rows)
                {
                    int saleId = Convert.ToInt32(row["sale_id"]);
                    row["Товари (Деталі)"] = GetSaleItemsString(saleId);
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження історії продажів: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return salesDt;
        }

        // Метод для експорту в Excel
        public DataTable GetSalesHistoryForExport(DateTime startDate, DateTime endDate)
        {
            DataTable salesDt = new DataTable();

            try
            {
                if (!DbConection.ConnectionDB()) return salesDt;

                int currentEmployeeId = SessionManager.CurrentEmployeeId;

                string query = @"
                    SELECT
                        S.sale_id,
                        S.sale_datetime,
                        IFNULL(CONCAT(C.first_name, ' ', C.last_name), 'Гість') AS customer_name,
                        P.product_name,
                        Col.color_name,
                        Sz.size_value,
                        SD.product_quantity AS quantity,
                        SD.unit_price
                    FROM
                        `Sale` S
                    LEFT JOIN Customer C ON S.customer_id = C.customer_id
                    INNER JOIN Sale_Details SD ON S.sale_id = SD.sale_id
                    INNER JOIN Color_Size CS ON SD.color_size_id = CS.color_size_id
                    INNER JOIN Product P ON CS.product_id = P.product_id
                    INNER JOIN Color Col ON CS.color_id = Col.color_id
                    INNER JOIN Size Sz ON CS.size_id = Sz.size_id
                    WHERE
                        DATE(S.sale_datetime) BETWEEN @start_date AND @end_date
                        AND S.employee_id = @employee_id_filter -- <--- ДОДАНО ФІЛЬТР
                    ORDER BY
                        S.sale_datetime DESC, S.sale_id DESC;";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@start_date", startDate.Date);
                DbConection.msCommand.Parameters.AddWithValue("@end_date", endDate.Date);
                DbConection.msCommand.Parameters.AddWithValue("@employee_id_filter", currentEmployeeId); // <--- ДОДАНО ПАРАМЕТР

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
                {
                    adapter.Fill(salesDt);
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних для експорту: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return salesDt;
        }

        private string GetSaleItemsString(int saleId)
        {
            DataTable itemsDt = new DataTable();
            StringBuilder itemsString = new StringBuilder();

            string itemsQuery = @"
                SELECT
                    P.product_name,
                    C.color_name,
                    S.size_value,
                    SD.product_quantity,
                    SD.unit_price
                FROM
                    Sale_Details SD
                JOIN
                    Color_Size CS ON SD.color_size_id = CS.color_size_id
                JOIN
                    Product P ON CS.product_id = P.product_id
                JOIN
                    Color C ON CS.color_id = C.color_id
                JOIN
                    Size S ON CS.size_id = S.size_id
                WHERE
                    SD.sale_id = @sale_id;";

            DbConection.msCommand.CommandText = itemsQuery;
            DbConection.msCommand.Parameters.Clear();
            DbConection.msCommand.Parameters.AddWithValue("@sale_id", saleId);

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(DbConection.msCommand))
            {
                adapter.Fill(itemsDt);
            }

            foreach (DataRow row in itemsDt.Rows)
            {
                string name = row["product_name"].ToString();
                string color = row["color_name"].ToString();
                string size = row["size_value"].ToString();
                int qty = Convert.ToInt32(row["product_quantity"]);
                double price = Convert.ToDouble(row["unit_price"]) * qty;

                itemsString.AppendLine(
                    $"{name} ({color} / {size}) — {qty} од. — {price:F2} грн"
                );
            }

            return itemsString.ToString().TrimEnd('\r', '\n');
        }
    }
}
