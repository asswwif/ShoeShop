using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp2
{
    internal class DbConection
    {
        // Приватне поле для рядка підключення
        private static string DbConnect = "server = localhost; user = root; password = Admin24&; database = shoestore";

        public static MySqlConnection msConnection;

        public static MySqlCommand msCommand;

        public static MySqlDataAdapter msDataAdapter;


        public static bool ConnectionDB()
        {
            try
            {
                // Ініціалізуємо msConnection 
                msConnection = new MySqlConnection(DbConnect);
                msConnection.Open();

                // Ініціалізуємо msCommand і прив'язуємо до з'єднання
                msCommand = new MySqlCommand();
                msCommand.Connection = msConnection;

                // Ініціалізуємо адаптер 
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
            // Перевіряємо на null, щоб уникнути помилок, якщо з'єднання не відкрилося
            if (msConnection != null && msConnection.State == System.Data.ConnectionState.Open)
            {
                msConnection.Close();
            }
        }
        //public MySqlConnection getConnection()
        //{
        //    return msConnection;
        //}
    }
}