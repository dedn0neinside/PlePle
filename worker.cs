using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ple
{
    public partial class worker : Form
    {
        string connectionString = "Server=localhost;Database=ple;User ID=root;Password=D0rlina060720;";
        public worker()
        {
            InitializeComponent();
            LoadComboBoxItems();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            login f = new login();
            f.ShowDialog();
        }
 
        private void comboBoxCategories_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBoxCategories.SelectedItem != null)
            {
                string selectedCategory = comboBoxCategories.SelectedItem.ToString();
                LoadData(selectedCategory);
            }
        }
        private void LoadComboBoxItems()
        {
            comboBoxCategories.Items.Clear();
            comboBoxCategories.Items.Add("Напитки");
            comboBoxCategories.Items.Add("Десерты");
            comboBoxCategories.Items.Add("ТТК напитки");
            comboBoxCategories.Items.Add("ТТК десерты");
            comboBoxCategories.Items.Add("Чек-лист");
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
                    databaseW.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }
    }
}
