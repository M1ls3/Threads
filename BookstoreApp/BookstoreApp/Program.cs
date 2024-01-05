using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BookstoreApp
{
    public class Warehouse
    {
        public enum ItemStatus { AVAILABLE, PRE_ORDER, UNAVAILABLE }

        public ItemStatus FetchBookStatus(string bookTitle)
        {
            Thread.Sleep(2000); // Імітація затримки 2 секунди
            Array values = Enum.GetValues(typeof(ItemStatus));
            Random random = new Random();
            return (ItemStatus)values.GetValue(random.Next(values.Length));
        }
    }

    class Program
    {
        static List<string> titles = new List<string>
        {
            "Harry Potter and the Philosopher's Stone",
            "Harry Potter and the Chamber of Secrets",
            "Harry Potter and the Prisoner of Azkaban",
            "Harry Potter and the Goblet of Fire",
            "Harry Potter and the Half-Blood Prince",
            "Harry Potter and the Deathly Hallows"
        };

        static void Main(string[] args)
        {
            Warehouse warehouse = new Warehouse();
            DateTime timeBeforeFirstFetch = DateTime.Now;
            List<Warehouse.ItemStatus> statuses = GetStatuses(warehouse, titles);
            foreach (var status in statuses)
            {
                Console.WriteLine(status);
            }

            TimeSpan elapsedTime = DateTime.Now - timeBeforeFirstFetch;
            Console.WriteLine($"Time elapsed: {elapsedTime.TotalMilliseconds}ms");
        }

        static List<Warehouse.ItemStatus> GetStatuses(Warehouse warehouse, List<string> titles)
        {
            List<Warehouse.ItemStatus> itemStatuses = new List<Warehouse.ItemStatus>();
            for (int i = 0; i < titles.Count; i++)
            {
                itemStatuses.Add(Warehouse.ItemStatus.UNAVAILABLE); // Початковий статус
            }

            CountdownEvent countdownEvent = new CountdownEvent(titles.Count);

            for (int i = 0; i < titles.Count; i++)
            {
                int index = i; // Захоплення локальної змінної для передачі в лямбда-вираз
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    Warehouse.ItemStatus status = warehouse.FetchBookStatus(titles[index]);
                    itemStatuses[index] = status;
                    countdownEvent.Signal();
                });
            }

            countdownEvent.Wait(); // Очікування завершення всіх завдань

            return itemStatuses;
        }
    }
}
