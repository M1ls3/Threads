using System;
using System.Collections.Generic;
using System.Threading;

namespace MessageQueueImplementation
{
    public class Message
    {
        private readonly string _value;

        public Message(string value)
        {
            _value = value;
        }

        public string GetValue() => _value;
    }

    public interface IMessageQueue
    {
        void Add(Message message);
        Message Poll();
    }

    public class MessageQueue : IMessageQueue
    {
        private readonly Queue<Message> _messages = new Queue<Message>();
        private readonly int _maxSize;

        public MessageQueue(int maxSize)
        {
            if (maxSize <= 0)
            {
                throw new ArgumentException("Size must be greater than zero");
            }
            _maxSize = maxSize;
        }

        public void Add(Message message)
        {
            lock (_messages)
            {
                while (_messages.Count == _maxSize)
                {
                    Monitor.Wait(_messages);
                }

                if (_messages.Count == 0)
                {
                    Monitor.PulseAll(_messages);
                }

                _messages.Enqueue(message);
            }
        }

        public Message Poll()
        {
            lock (_messages)
            {
                while (_messages.Count == 0)
                {
                    Monitor.Wait(_messages);
                }

                if (_messages.Count == _maxSize)
                {
                    Monitor.PulseAll(_messages);
                }

                return _messages.Dequeue();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            IMessageQueue messageQueue = new MessageQueue(5); // Максимальна кількість елементів у черзі

            // Приклад використання
            ThreadPool.QueueUserWorkItem(state =>
            {
                for (int i = 0; i < 10; i++)
                {
                    var message = new Message($"Message {i}");
                    messageQueue.Add(message);
                    Console.WriteLine($"Added: {message.GetValue()}");
                    Thread.Sleep(100); 
                }
            });

            ThreadPool.QueueUserWorkItem(state =>
            {
                for (int i = 0; i < 10; i++)
                {
                    var message = messageQueue.Poll();
                    Console.WriteLine($"Polled: {message.GetValue()}");
                    Thread.Sleep(200); 
                }
            });

            Console.ReadLine();
        }
    }
}
