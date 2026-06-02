

namespace LiveLock
{
    internal class Program
    {
        static readonly object stateLock = new object();

        static bool clientsInUse = false;
        static bool logInUse = false;

        private static void ReleaseLog()
        {
            lock (stateLock)
            {
                logInUse = false;
            }
        }

        private static void ReleaseClients()
        {
            lock (stateLock)
            {
                clientsInUse = false;
            }
        }

        private static bool TryTakeClients()
        {
            lock (stateLock)
            {
                if (clientsInUse) return false;
                clientsInUse = true;
                return true;
            }
        }
        private static bool TryTakeLog()
        {
            lock (stateLock)
            {
                if (logInUse) return false;
                logInUse = true;
                return true;
            }
        }

        static void Main(string[] args)
        {        

            Task t1 = Task.Run(() => ProcessTask1());
            Task t2 = Task.Run(() => ProcessTask2());

            Task.WaitAll(t1, t2);
            Console.WriteLine("Finished");
        }

        private static void ProcessTask2()
        {

            while (true)
            {
               
                Console.WriteLine("Task 2: опитва log.txt");

                if (TryTakeLog())
                {
                    Console.WriteLine("Task 2: взе log.txt");
                    Thread.Sleep(2000);

                    Console.WriteLine("Task 2: опитва clients.txt");
                    if (TryTakeClients())
                    {
                        Console.WriteLine("Task 2: взе clients.txt");
                        Console.WriteLine("Task 2: операцията е успешна");

                        ReleaseClients();
                        ReleaseLog();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Task 2: clients.txt е зает -> освобождава log.txt и опитва пак");
                        ReleaseLog();
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    Console.WriteLine("Task 2: log.txt е зает");
                    Thread.Sleep(2000);
                }
            }
        }

    

        private static void ProcessTask1()
        {
            while (true)
            {
                Console.WriteLine("Task 1: опитва clients.txt");

                if (TryTakeClients())
                {
                    Console.WriteLine("Task 1: взе clients.txt");
                    Thread.Sleep(200);

                    Console.WriteLine("Task 1: опитва log.txt");
                    if (TryTakeLog())
                    {
                        Console.WriteLine("Task 1: взе log.txt");
                        Console.WriteLine("Task 1: операцията е успешна");

                        ReleaseLog();
                        ReleaseClients();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Task 1: log.txt е зает -> освобождава clients.txt и опитва пак");
                        ReleaseClients();
                        Thread.Sleep(200);
                    }
                }
                else
                {
                    Console.WriteLine("Task 1: clients.txt е зает");
                    Thread.Sleep(200);
                }
            }
        }

     
    }
}
