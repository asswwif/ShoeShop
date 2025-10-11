using System;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class EmployeeService
    {
        public bool UpdatePersonalInfo(int employeeId, string firstName, string lastName, string phoneNumber)
        {
            try
            {
                if (!DbConection.ConnectionDB())
                {
                    return false;
                }

                string query = @"
                    UPDATE Employee
                    SET first_name = @firstName,
                        last_name = @lastName,
                        phone_number = @phoneNumber
                    WHERE employee_id = @employeeId";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();
                DbConection.msCommand.Parameters.AddWithValue("@firstName", firstName);
                DbConection.msCommand.Parameters.AddWithValue("@lastName", lastName);
                DbConection.msCommand.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                DbConection.msCommand.Parameters.AddWithValue("@employeeId", employeeId);

                int rowsAffected = DbConection.msCommand.ExecuteNonQuery();
                DbConection.CloseDB();

                if (rowsAffected > 0)
                {
                    SessionManager.UpdateCurrentEmployeeData(firstName, lastName, phoneNumber);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
                return false;
            }
        }
    }
}