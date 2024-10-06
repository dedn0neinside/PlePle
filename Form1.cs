using MySqlConnector;

namespace Ple
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            {
                string connectionString = "Server=localhost;Database=ple;User ID=root;Password=D0rlina060720;";
                string password = textBox1.Text;
                string username = GetUsernameByPassword(password, connectionString);

                if (username == null)
                {
                    MessageBox.Show("Неверный пароль.");
                }
                else if (username == "manager")
                {
                    this.Hide();
                    manager f = new manager();
                    f.ShowDialog();
                }
                else if (username == "worker")
                {
                    this.Hide();
                    worker f = new worker();
                    f.ShowDialog();
                }
            }
        }
        private string GetUsernameByPassword(string password, string connectionString)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Username FROM employee WHERE Password = @password";
                    using (MySqlCommand command = new MySqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@password", password);
                        var result = command.ExecuteScalar();
                        return result?.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}");
                    return null;
                }
            }

        }
    }
}
