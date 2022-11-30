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
    private static int total = 0;
    private static int clears = 0;
    private static int i = 0;
    private static List<ProductModel> list = new();
    //private readonly static AppDbContext _dbContext = new AppDbContext();
    //private readonly static ProductRepository _productRepository = new ProductRepository(_dbContext);


    private static void TimerCallback(object obj)
    {
        if (i != 0)
        {
            //Console.WriteLine("====================================");
            clears++;
            //Console.Clear();
            Console.WriteLine($"{i}/per second");
            Console.WriteLine($"{total / clears} avg");
            Console.WriteLine("total" + total);
            Console.WriteLine("work time " + clears);

            i = 0;
        }
    }
    //private static List<ProductModel> products = new List<ProductModel>();

    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() {HostName = "localhost", DispatchConsumersAsync = true };
        //await Task.Run(()=>StartTheConsumer(factory));
        //Console.ReadKey();

        var task1 = Task.Run(() => StartTheConsumer(factory));
        //var task2 = Task.Run(() => StartTheConsumer(factory));
        //var task3 = Task.Run(() => StartTheConsumer(factory));
        //var task4 = Task.Run(() => StartTheConsumer(factory));
        //var task5 = Task.Run(() => StartTheConsumer(factory));



        // var task3 = Task.Run(() => { var timer = new Timer(TimerCallback, null, 0, 1000); });

        //await task3;

        //await Task.WhenAll(task1,task2,task3,task4,task5);
        await task1;

        //x5 =56%
        //x1=44%

        Console.ReadKey();
    }

    
    private static async Task StartTheConsumer(ConnectionFactory factory)
    {
        Console.WriteLine(Task.CurrentId+"---");

        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);


                consumer.Received += async (model, ea) =>
                {

                    //channel.BasicAck(deliveryTag:ea.DeliveryTag,multiple:false);
                    total++;
                    i++;
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var responseModel = JsonSerializer.Deserialize<ProductModel>(message);
                    responseModel.Id = Guid.NewGuid().ToString();

                    using (AppDbContext dbContext = new AppDbContext())
                    {

                        //    //Console.WriteLine(list.Count());
                        if (list.Count() >= 1000)
                        {
                            await dbContext.BulkInsertAsync<ProductModel>(list, opt =>
                            {
                                opt.BatchSize = 1000;

                            });
                            list.Clear();
                            //await dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            list.Add(responseModel);
                        }


                        //    //dbContext.BulkInsert(customers, options => {
                        //    //    options.InsertIfNotExists = true;
                        //    //    options.PrimaryKeyExpression = customer => customer.Code;
                        //    //});
                        //    //dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                        //    //ProductRepository productRepository = new ProductRepository(dbContext);
                        //    //await productRepository.Create(ResponseModel);
                    };






                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                var timer = new Timer(TimerCallback, null, 0, 1000);
                channel.BasicConsume(queue: "hello",
                    autoAck: false,
                    consumer: consumer);
                Console.ReadKey();

            }
        }

    }
}

//normal
//300-800
//560+
//60

// dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
//100-900
//483
//60


//only one task run = total -16414