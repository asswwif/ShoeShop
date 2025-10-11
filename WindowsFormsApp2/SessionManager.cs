using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public static class SessionManager
    {
        public static int CurrentEmployeeId { get; private set; } = 0;
        public static string CurrentEmployeeFirstName { get; private set; } = "";
        public static string CurrentEmployeeLastName { get; private set; } = "";
        public static string CurrentEmployeeUsername { get; private set; } = "";
        public static string CurrentEmployeePosition { get; private set; } = "";
        public static string CurrentEmployeePhone { get; private set; } = "";

        public static bool InitializeSession(string username)
        {
            try
            {
                if (!DbConection.ConnectionDB())
                {
                    return false;
                }

                string query = @"SELECT employee_id, first_name, last_name, username, position, phone_number 
                                FROM Employee 
                                WHERE username = @username";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = DbConection.msCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentEmployeeId = Convert.ToInt32(reader["employee_id"]);
                        CurrentEmployeeFirstName = reader["first_name"]?.ToString() ?? "";
                        CurrentEmployeeLastName = reader["last_name"]?.ToString() ?? "";
                        CurrentEmployeeUsername = reader["username"]?.ToString() ?? "";
                        CurrentEmployeePosition = reader["position"]?.ToString() ?? "";
                        CurrentEmployeePhone = reader["phone_number"]?.ToString() ?? "";


                        DbConection.CloseDB();
                        return true;
                    }
                }

                DbConection.CloseDB();
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка ініціалізації сесії: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
                return false;
            }
        }

        public static void ClearSession()
        {
            CurrentEmployeeId = 0;
            CurrentEmployeeFirstName = "";
            CurrentEmployeeLastName = "";
            CurrentEmployeeUsername = "";
            CurrentEmployeePosition = "";
            CurrentEmployeePhone = "";
        }

        public static bool IsLoggedIn()
        {
            return CurrentEmployeeId > 0;
        }

        public static string GetFullName()
        {
            // Якщо є ім'я або прізвище, формуємо повне ім'я
            if (!string.IsNullOrEmpty(CurrentEmployeeFirstName) || !string.IsNullOrEmpty(CurrentEmployeeLastName))
            {
                return $"{CurrentEmployeeFirstName} {CurrentEmployeeLastName}".Trim();
            }

            // Якщо немає ні імені, ні прізвища, повертаємо username
            return CurrentEmployeeUsername;
        }

        public static void UpdateCurrentEmployeeData(string firstName, string lastName, string phone)
        {
            CurrentEmployeeFirstName = firstName;
            CurrentEmployeeLastName = lastName;
            CurrentEmployeePhone = phone;
        }
    }
}