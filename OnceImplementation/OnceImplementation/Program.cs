using System;
using System.Threading;

namespace OnceImplementation
{
    public interface IOnce
    {
        void Exec(Action action);
    }

    public class Once : IOnce
    {
        private int _executed;

        public void Exec(Action action)
        {
            if (Interlocked.Exchange(ref _executed, 1) == 0)
            {
                action?.Invoke();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IOnce once = new Once();

            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    once.Exec(() => Console.WriteLine("hello world"));
                });
            }

            Console.ReadLine(); 
        }
    }
}
