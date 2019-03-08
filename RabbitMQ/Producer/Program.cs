using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            ProducerTwo();
        }

        private static void ProducerOne()
        {
            Console.WriteLine("Writing...");
            //生产者
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            //默认端口
            factory.Port = 5672;
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //channel.ExchangeDeclare("",ExchangeType.Direct);
                    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                    channel.QueueDeclare("MyRabbitMQ", true, false, false, null);
                    while (true)
                    {
                        string message = string.Format("Message_{0}", Console.ReadLine());
                        byte[] buffer = Encoding.UTF8.GetBytes(message);
                        IBasicProperties properties = channel.CreateBasicProperties();
                        properties.DeliveryMode = 2;
                        channel.BasicPublish("", "MyRabbitMQ", properties, buffer);
                        Console.WriteLine("消息发送成功：" + message);
                    }
                }
            }
        }

        private static void ProducerTwo()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.Port = 5672;

            var conne = factory.CreateConnection();
            var model = conne.CreateModel();
            model.QueueDeclare("RabbitMqTwo", true, false, false, null);

            Console.WriteLine("write....");
            //发消息
            while (true)
            {
                string magess = Console.ReadLine();
                var bitmage = Encoding.UTF8.GetBytes(magess);
                var proper = model.CreateBasicProperties();
                proper.DeliveryMode = 2;

                model.BasicPublish("", "RabbitMqTwo", proper, bitmage);
                Console.WriteLine($"发送消息：{magess}");
            }

        }
    }
}
