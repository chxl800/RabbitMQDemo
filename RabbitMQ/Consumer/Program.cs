using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumerTwo();
        }

        private static void ConsumerOne()
        {
            //消费者
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            //默认端口
            factory.Port = 5672;
            using (IConnection conn = factory.CreateConnection())
            {
                using (IModel channel = conn.CreateModel())
                {
                    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                    channel.QueueDeclare("MyRabbitMQ", true, false, false, null);

                    //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                    channel.BasicQos(0, 1, false);

                    Console.WriteLine("Listening...");

                    //在队列上定义一个消费者
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                    //消费队列，并设置应答模式为程序主动应答
                    channel.BasicConsume("MyRabbitMQ", false, consumer);

                    while (true)
                    {
                        //阻塞函数，获取队列中的消息
                        BasicDeliverEventArgs ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        byte[] bytes = ea.Body;
                        string str = Encoding.UTF8.GetString(bytes);

                        Console.WriteLine("队列消息:" + str.ToString());
                        //回复确认
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        private static void ConsumerTwo()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            factory.Port = 5672;

            var conne = factory.CreateConnection();
            var model = conne.CreateModel();

            model.QueueDeclare("RabbitMqTwo", true, false, false, null);
            var consumer = new EventingBasicConsumer(model);
            model.BasicConsume("RabbitMqTwo", false, consumer);

            Console.WriteLine("listen...");

            //接受消息
            consumer.Received += (s, e) =>
            {
                var bitmage = e.Body;
                var magess = Encoding.UTF8.GetString(bitmage);
                Console.WriteLine($"接受消息：{magess}");
                model.BasicAck(e.DeliveryTag, false);
                //model.BasicConsume("RabbitMqTwo", true, consumer);
            };

        }
    }
}
