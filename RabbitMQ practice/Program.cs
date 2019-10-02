using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_practice
{
    class Program
    {
        /// <summary>
        /// get connection 
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static IConnection GetConnection(string hostName= "localhost", string userName= "guest", string password= "guest")
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = hostName;
            connectionFactory.UserName = userName;
            connectionFactory.Password = password;
            return connectionFactory.CreateConnection();
        }
        /// <summary>
        /// send message tp RMQ server 
        /// </summary>
        /// <param name="queue"> queue name </param>
        /// <param name="data">data want to send </param>
        public static void Send(string queue, string data)
        {
            //get connection 
            using (IConnection connection = GetConnection())
            {
                //create channel 
                using (IModel channel = connection.CreateModel())
                {
                    //create queue 
                    channel.QueueDeclare(queue, true, false, false, null);
                    //send message 
                    channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(data));
                }
            }
        }
        /// <summary>
        /// Receive message  "consumer mostly be in another machine "
        /// </summary>
        /// <param name="queue">queue name </param>
        public static void Receive(string queue)
        {
            //get connection 
            using (IConnection connection = GetConnection())
            {
                //create channel 
                using (IModel channel = connection.CreateModel())
                {
                    //declare queue 
                    channel.QueueDeclare(queue, true, false, false, null);

                    //binding event 
                    var consumer = new EventingBasicConsumer(channel);

                    //get result 
                    BasicGetResult result = channel.BasicGet(queue, true);
                    if (result != null)
                    {
                        string data = Encoding.UTF8.GetString(result.Body);
                        //print result 
                        Console.WriteLine("Message = "+ data);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            //main to test 
            for (int i = 0; i < 100; i++)
            {
                Send("IDG1", "Hello "+i);
             
                Send("IDG", "Hello World11!"+i);
                Receive("IDG");
                Receive("IDG1");
            }
                Console.ReadLine();
        }
    }
}
