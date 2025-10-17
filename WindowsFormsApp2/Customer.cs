using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; } = DateTime.MinValue;
        public int DiscountPercent { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public bool HasValidBirthDate => BirthDate != DateTime.MinValue && BirthDate.Year > 1900;

        public string DisplayText
        {
            get
            {
                if (CustomerId == 0)
                {
                    return "Гість";
                }

                string fullName = $"{FirstName} {LastName}".Trim();

                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    string formattedPhone = InfoValidator.FormatPhoneForDisplay(PhoneNumber);
                    return $"{fullName} - {formattedPhone}";
                }

                return fullName;
            }
        }

        public override string ToString()
        {
            return DisplayText;
        }

        public string GetSearchableText()
        {
            return $"{FirstName} {LastName} {PhoneNumber}".ToLower();
        }
    }

    public class DiscountCard
    {
        public int DiscountCardId { get; set; }
        public int CustomerId { get; set; }
        public int DiscountPercent { get; set; }
    }

    public class CustomerService
    {
        public List<Customer> GetAllCustomersWithDiscounts()
        {
            List<Customer> customers = new List<Customer>();

            try
            {
                if (!DbConection.ConnectionDB())
                {
                    MessageBox.Show("Не вдалося підключитися до бази даних для завантаження клієнтів.", "Помилка БД");
                    return customers;
                }

                string query = @"SELECT 
                                            c.customer_id,
                                            c.first_name,
                                            c.last_name,
                                            c.phone_number,
                                            c.birth_date,
                                            COALESCE(dc.discount_percent, 0) as discount_percent
                                           FROM customer c
                                           LEFT JOIN discount_card dc ON c.customer_id = dc.customer_id
                                           ORDER BY c.first_name, c.last_name";

                DbConection.msCommand.CommandText = query;
                DbConection.msCommand.Parameters.Clear();

                using (MySqlDataReader reader = DbConection.msCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime birthDate = DateTime.MinValue;

                        object birthDateValue = reader["birth_date"];
                        if (birthDateValue != null && birthDateValue != DBNull.Value)
                        {
                            birthDate = Convert.ToDateTime(birthDateValue);
                        }

                        Customer customer = new Customer
                        {
                            CustomerId = reader.GetInt32("customer_id"),
                            FirstName = reader["first_name"]?.ToString() ?? "",
                            LastName = reader["last_name"]?.ToString() ?? "",
                            PhoneNumber = reader["phone_number"]?.ToString() ?? "",
                            BirthDate = birthDate,
                            DiscountPercent = reader.GetInt32("discount_percent")
                        };

                        customers.Add(customer);
                    }
                }

                DbConection.CloseDB();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження клієнтів: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return customers;
        }

        public Customer GetCustomerById(int customerId)
        {
            Customer customer = null;

            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = @"SELECT 
                                            c.customer_id,
                                            c.first_name,
                                            c.last_name,
                                            c.phone_number,
                                            c.birth_date,
                                            COALESCE(bc.discount_percent, 0) as discount_percent
                                             FROM customer c
                                             LEFT JOIN discount_card bc ON c.customer_id = bc.customer_id
                                             WHERE c.customer_id = @customerId";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@customerId", customerId);

                    using (MySqlDataReader reader = DbConection.msCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime birthDate = DateTime.MinValue;

                            object birthDateValue = reader["birth_date"];
                            if (birthDateValue != null && birthDateValue != DBNull.Value)
                            {
                                birthDate = Convert.ToDateTime(birthDateValue);
                            }

                            customer = new Customer
                            {
                                CustomerId = reader.GetInt32("customer_id"),
                                FirstName = reader["first_name"]?.ToString() ?? "",
                                LastName = reader["last_name"]?.ToString() ?? "",
                                PhoneNumber = reader["phone_number"]?.ToString() ?? "",
                                BirthDate = birthDate,
                                DiscountPercent = reader.GetInt32("discount_percent")
                            };
                        }
                    }

                    DbConection.CloseDB();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження клієнта: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return customer;
        }

        public bool AddCustomer(Customer customer)
        {
            try
            {
                if (!InfoValidator.ValidateName(customer.FirstName, "Ім'я", out string firstNameError))
                {
                    MessageBox.Show($"Помилка в імені: {firstNameError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!InfoValidator.ValidateName(customer.LastName, "Прізвище", out string lastNameError))
                {
                    MessageBox.Show($"Помилка в прізвищі: {lastNameError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!InfoValidator.ValidatePhone(customer.PhoneNumber, out string normalizedPhone, out string phoneError))
                {
                    MessageBox.Show($"Помилка в номері телефону: {phoneError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                customer.PhoneNumber = normalizedPhone;

                if (DbConection.ConnectionDB())
                {
                    string query;

                    if (customer.BirthDate == DateTime.MinValue || customer.BirthDate.Year <= 1900)
                    {
                        query = @"INSERT INTO customer (first_name, last_name, phone_number, birth_date) 
                                     VALUES (@firstName, @lastName, @phoneNumber, NULL)";
                    }
                    else
                    {
                        query = @"INSERT INTO customer (first_name, last_name, phone_number, birth_date) 
                                     VALUES (@firstName, @lastName, @phoneNumber, @birthDate)";
                    }

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@firstName", customer.FirstName);
                    DbConection.msCommand.Parameters.AddWithValue("@lastName", customer.LastName);
                    DbConection.msCommand.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber);

                    if (customer.BirthDate != DateTime.MinValue && customer.BirthDate.Year > 1900)
                    {
                        DbConection.msCommand.Parameters.AddWithValue("@birthDate", customer.BirthDate);
                    }

                    int result = DbConection.msCommand.ExecuteNonQuery();
                    DbConection.CloseDB();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання клієнта: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return false;
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                if (!InfoValidator.ValidateName(customer.FirstName, "Ім'я", out string firstNameError))
                {
                    MessageBox.Show($"Помилка в імені: {firstNameError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!InfoValidator.ValidateName(customer.LastName, "Прізвище", out string lastNameError))
                {
                    MessageBox.Show($"Помилка в прізвищі: {lastNameError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (!InfoValidator.ValidatePhone(customer.PhoneNumber, out string normalizedPhone, out string phoneError))
                {
                    MessageBox.Show($"Помилка в номері телефону: {phoneError}", "Помилка валідації",
                                     MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                customer.PhoneNumber = normalizedPhone;


                if (DbConection.ConnectionDB())
                {
                    string query;

                    if (customer.BirthDate == DateTime.MinValue || customer.BirthDate.Year <= 1900)
                    {
                        query = @"UPDATE customer 
                                     SET first_name = @firstName, 
                                         last_name = @lastName, 
                                         phone_number = @phoneNumber, 
                                         birth_date = NULL
                                     WHERE customer_id = @customerId";
                    }
                    else
                    {
                        query = @"UPDATE customer 
                                     SET first_name = @firstName, 
                                         last_name = @lastName, 
                                         phone_number = @phoneNumber, 
                                         birth_date = @birthDate
                                     WHERE customer_id = @customerId";
                    }

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@customerId", customer.CustomerId);
                    DbConection.msCommand.Parameters.AddWithValue("@firstName", customer.FirstName);
                    DbConection.msCommand.Parameters.AddWithValue("@lastName", customer.LastName);
                    DbConection.msCommand.Parameters.AddWithValue("@phoneNumber", customer.PhoneNumber);

                    if (customer.BirthDate != DateTime.MinValue && customer.BirthDate.Year > 1900)
                    {
                        DbConection.msCommand.Parameters.AddWithValue("@birthDate", customer.BirthDate);
                    }

                    int result = DbConection.msCommand.ExecuteNonQuery();
                    DbConection.CloseDB();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення клієнта: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return false;
        }

        public bool DeleteCustomer(int customerId)
        {
            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = "DELETE FROM customer WHERE customer_id = @customerId";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@customerId", customerId);

                    int result = DbConection.msCommand.ExecuteNonQuery();
                    DbConection.CloseDB();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка видалення клієнта: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return false;
        }
    }

    public class DiscountCardService
    {
        public bool AddDiscountCard(int customerId, int discountPercent)
        {
            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = @"INSERT INTO discount_card (customer_id, discount_percent) 
                                         VALUES (@customerId, @discountPercent)";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@customerId", customerId);
                    DbConection.msCommand.Parameters.AddWithValue("@discountPercent", discountPercent);

                    int result = DbConection.msCommand.ExecuteNonQuery();
                    DbConection.CloseDB();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка додавання дисконтної картки: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return false;
        }

        public bool UpdateDiscountCard(int customerId, int discountPercent)
        {
            try
            {
                if (DbConection.ConnectionDB())
                {
                    string query = @"UPDATE discount_card 
                                         SET discount_percent = @discountPercent
                                         WHERE customer_id = @customerId";

                    DbConection.msCommand.CommandText = query;
                    DbConection.msCommand.Parameters.Clear();
                    DbConection.msCommand.Parameters.AddWithValue("@customerId", customerId);
                    DbConection.msCommand.Parameters.AddWithValue("@discountPercent", discountPercent);

                    int result = DbConection.msCommand.ExecuteNonQuery();
                    DbConection.CloseDB();

                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оновлення дисконтної картки: {ex.Message}", "Помилка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                DbConection.CloseDB();
            }

            return false;
        }
    }
}
