using System;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    internal class DbConection
    {
        private static string DbConnect = GetConnectionString();

        public static MySqlConnection msConnection;
        public static MySqlCommand msCommand;
        public static MySqlDataAdapter msDataAdapter;

        private static string GetConnectionString()
        {
            try
            {
                // пошук dbconfig.txt в папці програми
                string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dbconfig.txt");

                if (File.Exists(configFile))
                {
                    return File.ReadAllText(configFile).Trim();
                }
                else
                {
                    MessageBox.Show("Файл dbconfig.txt не знайдено! Створіть його з рядком підключення.",
                        "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка читання конфігурації: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
        }

        public static bool ConnectionDB()
        {
            try
            {
                // ініціалізація msConnection 
                msConnection = new MySqlConnection(DbConnect);
                msConnection.Open();
                // ініціалізація msCommand і прив'язка до з'єднання
                msCommand = new MySqlCommand();
                msCommand.Connection = msConnection;
                // ініціалізація адаптера 
                msDataAdapter = new MySqlDataAdapter(msCommand);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка з'єднання з базою даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void CloseDB()
        {
            if (msConnection != null && msConnection.State == System.Data.ConnectionState.Open)
            {
                msConnection.Close();
            }
        }
    }
}