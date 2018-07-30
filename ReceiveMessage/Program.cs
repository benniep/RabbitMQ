using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveMessage
{
    class Program
    {
        private const string QueueName = "StandardQueue_ProgramTest";
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        static void Main(string[] args)
        {
            CreateQueue();
            var consumer = new EventingBasicConsumer(_model);
            // we first want to check if the message that we take from the queue is our message we want
            const bool autoAck = false;
            _model.BasicConsume(QueueName, autoAck, consumer);
            // This event will receive the message from the queue.
            consumer.Received += (model, ea) =>
            {
                var msgBody = ea.Body;
                string message = System.Text.Encoding.Unicode.GetString(msgBody);
                if(message.ToLower().Contains("my name is,"))
                { // Test is this is our message with the name to display it
                    if(ValidateMessage(message))
                    {
                        var split = message.Split(',');
                        System.Console.WriteLine("Hello {0}, I am your father!", split[1]);
                        _model.BasicAck(ea.DeliveryTag, false);
                    }
                }
            };
            // Stay in listen mode until the user press enter.
            Console.WriteLine("[*] Waiting for messages." +
                                "To exit press ENTER");
            Console.ReadLine();
            //Close connection to RabbitMQ and exit
            _model.Close();
            _connection.Close();
            
        }
        /// <summary>Validate if this is  valid message</summary>
        /// <param name="msg">Message Text</param>
        private static bool ValidateMessage(string msg)
        {
            var result = false;
            var split = msg.Split(',');
            if(!split[1].Trim().Equals(string.Empty))
            {
                return true;
            }
            return result;
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
            // Open connection to RabbitMQ
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();
            _model.QueueDeclare(QueueName, true, false, false, null);
        }
    }
}
