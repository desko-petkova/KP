using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_exercises
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        // Задача 1
        private async void button1_Click(object sender, EventArgs e)
        {
            label2.Text = "Status: Writing into file....";

            await File.WriteAllTextAsync("numbers.txt", richTextBox1.Text);
            await Task.Delay(2000);


            label2.Text = "Status: All data is writed";
        }

        // Задача 2
        private async void button2_Click(object sender, EventArgs e)
        {
            string[] lines = await File.ReadAllLinesAsync("numbers.txt");

            List<int> numbers = lines
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .ToList();

            int sum = await Task.Run(() =>
            {
                int result = 0;

                foreach (int number in numbers)
                {
                    Thread.Sleep(1000); 
                    result += number;
                }

                return result;
            });

            label1.Text = $"Sum: {sum}";
        }

        // Задача 3
        private async void button3_Click_1(object sender, EventArgs e)
        {
            Task t1 = Task.Delay(2000);
            Task t2 = Task.Delay(4000);
            Task t3 = Task.Delay(1000);

            await Task.WhenAll(t1, t2, t3);

            label3.Text = "WhenAll button: All tasks finished";
        }


        // Задача 4
        private async void button4_Click_1(object sender, EventArgs e)
        {

            Task<string> t1 = Finished("Task 1", 2000);
            Task<string> t2 = Finished("Task 2", 3000);
            Task<string> t3 = Finished("Task 3", 1000);

            Task<string> first = await Task.WhenAny(t1, t2, t3);

            label4.Text = $"WhenAny button: First is {first.Result}";
        }

        private async Task<string> Finished(string name, int delay)
        {
            await Task.Delay(delay);
            return name;
        }

    }
}
