namespace KmToMetersTread
{
    internal static class Program
    {
        public partial class Form1 : Form
        {

            public Form1()
            {
                InitializeComponent();
            }
            //Задача 1 ===========================================
            private Thread workerThread;
            private void button1_Click(object sender, EventArgs e)
            {
                workerThread = new Thread(ConvertKmToMeters);
                workerThread.Start();
            }
            private void ConvertKmToMeters()
            {
                // 0.25 до 5.00 през 0.25
                for (double km = 0.25; km <= 5.0; km += 0.25)
                {
                    double meters = km * 1000.0;
                    string line = $"{km:0.00} km = {meters:0} m";
                    listBox1.Invoke(new Action(() =>
                    {
                        listBox1.Items.Add(line);
                    }));
                    Thread.Sleep(150);
                }
                listBox1.Items.Add("Готово");
            }
            //Задача 2. ============================================================
            private Thread sumThread;
            private void CalculateSum()
            {
                double total = 0;

                string[] lines = null;

                // Четем текста безопасно от UI нишката

                richTextBox1.Invoke(new Action(() =>
                {
                    lines = richTextBox1.Text.Split("\n").ToArray();
                }));

                foreach (string line in lines)
                {
                    if (double.TryParse(line, out double number))
                    {
                        total += number;
                    }

                    Thread.Sleep(100);
                }

                label1.Invoke(new Action(() =>
                {
                    label1.Text = $"Sum = {total}";
                }));
            }

            private void button2_Click(object sender, EventArgs e)
            {
                sumThread = new Thread(CalculateSum);
                //sumThread.IsBackground = true;
                sumThread.Start();
            }
            //Задача 3. ==============================================================
            private Thread rondomThread;
            private bool isRunning = false;
            private Random rng = new Random();

            private void GenerateNumbers()
            {
                while (isRunning)
                {
                    int number;

                    number = rng.Next(0, 101); // 0..100

                    listBox2.Invoke(new Action(() =>
                    {
                        listBox2.Items.Add($"[{DateTime.Now:HH:mm:ss.fff}] {number}");
                        listBox2.TopIndex = listBox2.Items.Count - 1;

                    }));

                    Thread.Sleep(300);
                }

            }

            private void button3_Click(object sender, EventArgs e)
            {
                isRunning = true;

                rondomThread = new Thread(GenerateNumbers);

                rondomThread.Start();

                label2.Text = "Runing.....";
            }

            private void button4_Click(object sender, EventArgs e)
            {
                isRunning = false;
                label2.Text = "Stopped";
            }
            //Задача 4. =========================================================
            private Thread downloadThread;
            private bool isDownloading = false;

            private void button5_Click(object sender, EventArgs e)
            {
                progressBar1.Value = 0;
                label3.Text = "0%";

                isDownloading = true;

                downloadThread = new Thread(DownloadWork);
                downloadThread.Start();
            }
            private void DownloadWork()
            {
                for (int i = 0; i <= 100 && isDownloading; i++)
                {
                    progressBar1.Invoke(new Action(() =>
                    {
                        progressBar1.Value = i;
                        label3.Text = $"{i}%";
                    }));
                    Thread.Sleep(100);
                }

                label3.Invoke(new Action(() => label3.Text = "✅ Download complete!"));
            }
            //Задача 5. ======================================================================

            private Thread primeThread;

            private void button6_Click(object sender, EventArgs e)
            {
                if (!long.TryParse(textBox1.Text, out long number))
                {
                    label4.Text = "Invalid number!";
                    return;
                }

                label4.Text = "Checking...";

                primeThread = new Thread(() => CheckPrime(number));
                primeThread.IsBackground = true;
                primeThread.Start();

            }
            private void CheckPrime(long number)
            {
                bool isPrime = true;
                if (number < 2)
                    isPrime = false;

                if (number == 2)
                    isPrime = true;

                if (number % 2 == 0)
                    isPrime = false;
                long limit = (long)Math.Sqrt(number);
                for (long i = 3; i <= limit; i += 2)
                {
                    if (number % i == 0)
                    {
                        isPrime = false;
                        Thread.Sleep(100);
                    }
                }

                label4.Invoke(new Action(() =>
                {
                    if (isPrime)
                    {
                        label4.Text = $"{number} is PRIME";
                    }
                    else label4.Text = $"{number} is NOT prime";
                }));


            }

            //Задача 6 =====================================================
            private Thread sortThread;

            private void button7_Click(object sender, EventArgs e)
            {
                string text = richTextBox2.Text;

                int[] lines = text.Split("\n", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

                listBox3.Items.Clear();
                listBox3.Items.Add("Start: ");

                // 3) Стартираме нишка за сортиране
                sortThread = new Thread(() => SortArray(lines));
                sortThread.Start();

            }

            private void SortArray(int[] arr)
            {
                Array.Sort(arr);
                foreach (int i in arr)
                {
                    listBox3.Invoke(new Action(() =>
                    {
                        listBox3.Items.Add(i);
                    }));
                    Thread.Sleep(150);
                }

                listBox3.Invoke(new Action(() =>
                {
                    listBox3.Items.Add("Sorted");
                }));
            }
        }
    }
}