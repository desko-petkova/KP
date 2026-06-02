namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            label1.Text = "Зареждане...";

            listBox1.Items.Clear();

            List<string> users = await LoadUsersAsync();

            foreach (string user in users)
            {
                listBox1.Items.Add(user);
            }

            label1.Text = "Готово";

            button1.Enabled = true;
        }
        private async Task<List<string>> LoadUsersAsync()
        {
          
            await Task.Delay(3000);

            return new List<string>
            {
                "Иван",
                "Мария",
                "Георги",
                "Петя",
                "Николай"
            };
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            int result = await CalAsync();

            listBox1.Items.Add(result);
        }

        private async Task<int> CalAsync()
        {
            return await Task.Run(() =>
            {
                int sum = 0;

                for (int i = 1; i <= 100; i++)
                {
                    sum += i;

                    Thread.Sleep(20);
                }

                return sum;
            });
        }
    }
}
