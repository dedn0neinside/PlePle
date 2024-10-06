using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ple
{
    public partial class manager : Form
    {
        private MySqlConnection connection;
        string connectionString = "Server=localhost;Database=ple;User ID=root;Password=D0rlina060720;";
        public manager()
        {
            InitializeComponent();
            LoadCategories();
            LoadE();
            InitializeConnection();
        }
        private void LoadCategories()
        {
        //можно одной сторокой измения внести с помощью команды comboBox1.Items.AddRange(new string[] { "...", "...", "..." });
            comboBoxCategories.Items.Add("Напитки");
            comboBoxCategories.Items.Add("Десерты");
            comboBoxCategories.Items.Add("ТТК напитки");
            comboBoxCategories.Items.Add("ТТК десерты");
            comboBoxCategories.Items.Add("Чек-лист");
        }
        private void exit_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            login f = new login();
            f.ShowDialog();
        }

        private void LoadData(string category)
        {
            string query = string.Empty;
            switch (category)
            {
                case "Напитки":
                    query = "SELECT * FROM drinks";
                    break;
                case "Десерты":
                    query = "SELECT * FROM desserts";
                    break;
                case "ТТК напитки":
                    query = "SELECT * FROM drink_ingredients";
                    break;
                case "ТТК десерты":
                    query = "SELECT * FROM dessert_ingredients";
                    break;
                case "Чек-лист":
                    query = "SELECT * FROM checklist";
                    break;

            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    databaseM.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
        private void comboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategories.SelectedItem != null)
            {
                string selectedCategory = comboBoxCategories.SelectedItem.ToString();
                LoadData(selectedCategory);
            }
        }

        private void comboBoxE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxE.SelectedItem != null)
            {
                string selectedCategory = comboBoxE.SelectedItem.ToString();
                LoadBase(selectedCategory);
            }
        }
        private void LoadE()
        {
            comboBoxE.Items.Add("Сотрудники");

        }
        private void LoadBase(string category)
        {
            string query = string.Empty;
            switch (category)
            {
                case "Сотрудники":
                    query = "SELECT * FROM employee";
                    break;
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    databaseE.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            login f = new login();
            f.ShowDialog();
        }
        private void InitializeConnection()
        {
            string connectionString = "Server=localhost;Database=ple;User ID=root;Password=D0rlina060720;";
            connection = new MySqlConnection(connectionString);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string password = textBoxCode.Text;
            string surname = textSurname.Text;
            string username = textUsername.Text;

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            string query = $"INSERT INTO employee (Password, username, surname) VALUES ('{password}', '{username}', '{surname}')";

            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Сотрудник успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void deleteE_Click(object sender, EventArgs e)
        {
            if (databaseE.SelectedRows.Count > 0)
            {
                int Id = Convert.ToInt32(databaseE.SelectedRows[0].Cells["id"].Value);

                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand("DELETE FROM employee WHERE id = @id", connection);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                    LoadE();
                    MessageBox.Show("Информация успешно удалена!");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.");
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string name = textNameD.Text;
            string productsDrink = textProductsDrink.Text;
            string name_dessert = textNameDess.Text;
            string name_des = textProductsDessert.Text;
            string description = textdescriptionD.Text;
            string quantity = textquantityD.Text;
            string description_dessert = textdescriptionDess.Text;
            string quantityDessert = textquantityDess.Text;

            try
            {
                connection.Open();

                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = connection;
                        cmd.Transaction = transaction;
                        //мб через вложенный if лучше?
                        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(description))
                        {
                            cmd.CommandText = "INSERT INTO drinks (name_drink, description) VALUES (@nameD, @descriptionD)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@nameD", name);
                            cmd.Parameters.AddWithValue("@descriptionD", description);
                            cmd.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrWhiteSpace(name_dessert) && !string.IsNullOrWhiteSpace(description_dessert))
                        {
                            cmd.CommandText = "INSERT INTO desserts (name_dessert, description_dessert) VALUES (@nameDess, @descriptionDess)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@nameDess", name_dessert);
                            cmd.Parameters.AddWithValue("@descriptionDess", description_dessert);
                            cmd.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrWhiteSpace(productsDrink) && !string.IsNullOrWhiteSpace(quantity))
                        {
                            cmd.CommandText = "INSERT INTO drink_ingredients (name_d, Quantity) VALUES (@productsDrink, @quantityD)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@productsDrink", productsDrink);
                            cmd.Parameters.AddWithValue("@quantityD", quantity);
                            cmd.ExecuteNonQuery();
                        }

                        if (!string.IsNullOrWhiteSpace(name_des) && !string.IsNullOrWhiteSpace(quantityDessert))
                        {
                            cmd.CommandText = "INSERT INTO dessert_ingredients (name_des, Quantity) VALUES (@ProductsDessert, @quantityDess)";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@ProductsDessert", name_des);
                            cmd.Parameters.AddWithValue("@quantityDess", quantity);
                            cmd.ExecuteNonQuery();
                        }

                        // Подтверждаем транзакцию
                        transaction.Commit();
                    }
                }

                MessageBox.Show("Информация успешно добавлена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void deleteSmth_Click(object sender, EventArgs e)
        {
            if (databaseM.SelectedRows.Count > 0)
            {
                int Id = Convert.ToInt32(databaseM.SelectedRows[0].Cells["id"].Value);

                try
                {
                    connection.Open();

                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM drink_ingredients WHERE id = @id", connection))
                        {
                            cmd.Parameters.AddWithValue("@id", Id);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }

                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM dessert_ingredients WHERE id = @id", connection))
                        {
                            cmd.Parameters.AddWithValue("@id", Id);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }

                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM drinks WHERE id = @id", connection))
                        {
                            cmd.Parameters.AddWithValue("@id", Id);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }

                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM desserts WHERE id = @id", connection))
                        {
                            cmd.Parameters.AddWithValue("@id", Id);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Запись успешно удалена.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
