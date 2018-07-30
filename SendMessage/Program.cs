using System;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace SendMessage
{
    class Program
    {
        private const string QueueName = "StandardQueue_ProgramTest";
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            DisplayInputMessage();
            var name = string.Empty;
            // Do loop until the user wants to end the send message application.
            do  
            {
                name = Console.ReadLine();
                if (!name.Trim().Equals(string.Empty) && !name.ToUpper().Equals("EXIT"))
                {
                    CreateQueue();
                    SendMessageToMQ(name);
                    name = string.Empty;
                    DisplayInputMessage();
                }
                else if(name.ToUpper().Trim().Equals("EXIT"))
                {
                    // Close all connections and exit the program
                    if(_model != null)
                    {
                        _model.Close();
                        _connection.Close();
                    }
                    return;
                }
                else
                {
                    DisplayInputMessage();
                }

            } while (true);
        }
        /// <summer>Connect and create a queue to RabbitMQ</summer>
        private static void CreateQueue()
        {
            _factory = new ConnectionFactory {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                RequestedHeartbeat = 30 
            };
            // Create connection to RabbitMQ
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);
        }

        /// <summary>Send a message to RabbitMQ</summary>
        /// <param name="name">Name entered by the user</param>
        private static void SendMessageToMQ(string name)
        {
            var message = "Hello my name is, " + name;
            var messagebody =  System.Text.Encoding.Unicode.GetBytes(message);
            _model.BasicPublish("", QueueName, null, messagebody);
            Console.WriteLine("[x] Message Sent: {0}", message);
        }
        /// <summary>Display message to the user</sumary>
        private static void DisplayInputMessage()
        {
            Console.WriteLine("Enter your name: (To exit the program, type 'exit'.)");
        }
    }
}
