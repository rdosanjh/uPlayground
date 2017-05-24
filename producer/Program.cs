using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;

namespace producer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var topicName = "test";
                var brokerList = "kafka:9092";
                var config = new Dictionary<string, object> { { "bootstrap.servers", brokerList } };

                using (var producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8)))
                {
                    Console.WriteLine($"{producer.Name} producing on {topicName}. q to exit.");

                    while (true)
                    {
                        Thread.Sleep(1000);
                        var guid = Guid.NewGuid().ToString();
                        Console.WriteLine($"Sending {guid}");
                        var deliveryReport = producer.ProduceAsync(topicName, null, guid);
                        deliveryReport.ContinueWith(task =>
                        {
                            Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                        });
                    }

                    // Tasks are not waited on synchronously (ContinueWith is not synchronous),
                    // so it's possible they may still in progress here.
                    producer.Flush();
                }
            }
            catch (System.Exception e)
            {
                
                Console.WriteLine(e.ToString());
            }
       
        }
    }
}
