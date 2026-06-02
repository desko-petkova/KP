namespace RaceConditionLockDeadlock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Споделен референтен тип
        class SharedCounter
        {
            public int Value;
        }

        private SharedCounter counter = new SharedCounter();

        // lock за брояча
        private readonly object counterLock = new object();

        

        // Безопасно писане в ListBox от Task
        private void AddMessage(string message)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action(() =>
                {
                    listBox1.Items.Add(message);
                }));
            }
            else
            {
                listBox1.Items.Add(message);
            }
        }

        // Безопасно обновяване на Label
        private void SetCounterLabel(string text)
        {
            if (labelCounter.InvokeRequired)
            {
                labelCounter.Invoke(new Action(() =>
                {
                    labelCounter.Text = text;
                }));
            }
            else
            {
                labelCounter.Text = text;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            counter.Value = 0;
            SetCounterLabel("Работи без lock...");

            Task t1 = Task.Run(() =>
            {
                AddMessage("Task 1 started (no lock)");
                for (int i = 0; i < 10000000; i++)
                {
                    counter.Value++;
                }
                AddMessage("Task 1 finished (no lock)");
            });

            Task t2 = Task.Run(() =>
            {
                AddMessage("Task 2 started (no lock)");
                for (int i = 0; i < 10000000; i++)
                {
                    counter.Value++;
                }
                AddMessage("Task 2 finished (no lock)");
            });

            await Task.WhenAll(t1, t2);

            SetCounterLabel("Counter = " + counter.Value);
            AddMessage("Expected: 20000000, Actual: " + counter.Value);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            counter.Value = 0;
            SetCounterLabel("Работи с lock...");

            Task t1 = Task.Run(() =>
            {
                AddMessage("Task 1 started (with lock)");
                for (int i = 0; i < 10000000; i++)
                {
                    lock (counterLock)
                    {
                        counter.Value++;
                    }
                }
                AddMessage("Task 1 finished (with lock)");
            });

            Task t2 = Task.Run(() =>
            {
                AddMessage("Task 2 started (with lock)");
                for (int i = 0; i < 10000000; i++)
                {
                    lock (counterLock)
                    {
                        counter.Value++;
                    }
                }
                AddMessage("Task 2 finished (with lock)");
            });

            await Task.WhenAll(t1, t2);

            SetCounterLabel("Counter = " + counter.Value);
            AddMessage("Expected: 20000000, Actual: " + counter.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            counter.Value = 0;
            labelCounter.Text = "Counter = 0";
            listBox1.Items.Clear();
        }
        // два lock обекта за deadlock демонстрацията
        private readonly object lockA = new object();
        private readonly object lockB = new object();

        private async void button4_Click(object sender, EventArgs e)
        {
            AddMessage("Starting deadlock demo...");

            Task t1 = Task.Run(() =>
            {
                AddMessage("Task 1 wants A");
                lock (lockA)
                {
                    AddMessage("Task 1 locked A");
                    Thread.Sleep(1000);

                    AddMessage("Task 1 wants B");
                    lock (lockB)
                    {
                        AddMessage("Task 1 locked B");
                    }
                }
            });

            Task t2 = Task.Run(() =>
            {
                AddMessage("Task 2 wants B");
                lock (lockB)
                {
                    AddMessage("Task 2 locked B");
                    Thread.Sleep(1000);

                    AddMessage("Task 2 wants A");
                    lock (lockA)
                    {
                        AddMessage("Task 2 locked A");
                    }
                }
            });

            // Няма да чакаме безкрайно, за да не "увисне" интерфейсът напълно
            Task allTasks = Task.WhenAll(t1, t2);
            Task timeout = Task.Delay(4000);

            Task finished = await Task.WhenAny(allTasks, timeout);

            if (finished == timeout)
            {
                AddMessage("Possible deadlock detected!");
                SetCounterLabel("Deadlock / блокиране");
            }
            else
            {
                AddMessage("No deadlock this time.");
                SetCounterLabel("Завърши");
            }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            AddMessage("Starting safe lock order demo...");

            Task t1 = Task.Run(() =>
            {
                AddMessage("Task 1 wants A");
                lock (lockA)
                {
                    AddMessage("Task 1 locked A");
                    Thread.Sleep(500);

                    AddMessage("Task 1 wants B");
                    lock (lockB)
                    {
                        AddMessage("Task 1 locked B");
                        Thread.Sleep(500);
                    }
                }

                AddMessage("Task 1 finished safely");
            });

            Task t2 = Task.Run(() =>
            {
                AddMessage("Task 2 wants A");
                lock (lockA)
                {
                    AddMessage("Task 2 locked A");
                    Thread.Sleep(500);

                    AddMessage("Task 2 wants B");
                    lock (lockB)
                    {
                        AddMessage("Task 2 locked B");
                        Thread.Sleep(500);
                    }
                }

                AddMessage("Task 2 finished safely");
            });

            await Task.WhenAll(t1, t2);

            SetCounterLabel("Без deadlock");
            AddMessage("Both tasks finished normally.");
        }
    }
}
