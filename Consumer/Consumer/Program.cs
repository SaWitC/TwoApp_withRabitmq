using System.Drawing.Text;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Consumer.Data;
using Consumer.Data.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sender.Models;



public class Program
{
    private static int clears = 0;
    private static void TimerCallback(object obj)
    {
        Console.WriteLine("====================================");
        clears++;
        Console.Clear();
        Console.WriteLine($"{Handler.Processed / clears} avg per second");
        Console.WriteLine("total " + Handler.Processed);
        Console.WriteLine("work time " + clears);
        Console.WriteLine("====================================");
    }

    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() {HostName = "localhost", DispatchConsumersAsync = true };
        var timer = new Timer(TimerCallback, null, 0, 1000);

        var task1 = Task.Run(() => Handler.StartTheConsumer(factory,"hello"));
        //var task2 = Task.Run(() => Handler.StartTheConsumer(factory, "hello"));
        //var task3 = Task.Run(() => Handler.StartTheConsumer(factory, "hello"));
        //var task4 = Task.Run(() => StartTheConsumer(factory));
        //var task5 = Task.Run(() => StartTheConsumer(factory));

        //await task3;

        //await Task.WhenAll(task1, task2, task3, task4, task5);
        //await Task.WhenAll(task1, task2, task3);

        await task1;
        Console.ReadKey();
    }
}

public static class Handler
{
    private static readonly List<ProductModel> list = new();
    public static long Processed { private set; get; }

    public static async Task StartTheConsumer(ConnectionFactory factory, string queue)
    {
        Console.WriteLine(Task.CurrentId + "---");

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var responseModel = JsonSerializer.Deserialize<ProductModel>(message);
                    responseModel.Id = Guid.NewGuid().ToString();

                    using (AppDbContext dbContext = new AppDbContext())
                    {
                        if (list.Count() >= 10000)
                        {
                            await dbContext.BulkInsertAsync<ProductModel>(list, opt =>
                            {

                                opt.BatchSize = 10000;
                            });
                            list.Clear();
                        }
                        else
                        {
                            list.Add(responseModel);
                        }
                    };
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    Processed++;
                };

                channel.BasicConsume(queue: queue,
                    autoAck: false,
                    consumer: consumer);
                Console.ReadKey();
            }
        }
    }
}