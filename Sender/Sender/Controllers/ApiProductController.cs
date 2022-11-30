using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Sender.Models;

namespace Sender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiProductController : ControllerBase
    {

        [HttpPost]
        [Route("100000")]

        public IActionResult Send100000( ProductModel model)
        {
            
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello"
                        , durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    string buf = model.Title;

                    for (int i = 0; i < 100000; i++)
                    {
                        model.Title = buf + i;
                        string message = JsonSerializer.Serialize(model);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "hello",
                            basicProperties: null,
                            body: body);
                    }

                    //string message = JsonSerializer.Serialize(model);

                    //var body = Encoding.UTF8.GetBytes(message);

                    //channel.BasicPublish(exchange: "",
                    //    routingKey: "hello",
                    //    basicProperties: null,
                    //    body: body);

                    return Ok("sent ");
                }
            }

            return Ok("error");
        }



        [HttpPost]
        [Route("1000")]

        public IActionResult Send1000(ProductModel model)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello"
                        , durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    string buf = model.Title;

                    for (int i = 0; i < 1000; i++)
                    {
                        model.Title = buf + i;
                        string message = JsonSerializer.Serialize(model);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "hello",
                            basicProperties: null,
                            body: body);
                    }

                    //string message = JsonSerializer.Serialize(model);

                    //var body = Encoding.UTF8.GetBytes(message);

                    //channel.BasicPublish(exchange: "",
                    //    routingKey: "hello",
                    //    basicProperties: null,
                    //    body: body);

                    return Ok("sent ");
                }
            }

            return Ok("error");
        }

        [HttpPost]
        [Route("10")]
        public IActionResult Send10(ProductModel model)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello"
                        , durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    string buf = model.Title;

                    for (int i = 0; i < 10; i++)
                    {
                        model.Title = buf + i;
                        string message = JsonSerializer.Serialize(model);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: "hello",
                            basicProperties: null,
                            body: body);
                    }

                    //string message = JsonSerializer.Serialize(model);

                    //var body = Encoding.UTF8.GetBytes(message);

                    //channel.BasicPublish(exchange: "",
                    //    routingKey: "hello",
                    //    basicProperties: null,
                    //    body: body);

                    return Ok("sent ");
                }
            }

            return Ok("error");
        }
    }
}
