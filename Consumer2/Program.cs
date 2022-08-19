using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    Port = 5672,
    AutomaticRecoveryEnabled = true,
    VirtualHost = "DemoApp"
};

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "sipintar.pdam[1]", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare(queue: "Com2", durable:true, autoDelete:false, exclusive:false).QueueName;

var consumer = new EventingBasicConsumer(channel);

channel.QueueBind(queue: queueName, exchange: "sipintar.pdam[1]", routingKey: "order.#");

consumer.Received += (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Consumer2 - Received new message : {message}");  

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    }
    catch (Exception e)
    {
        channel.BasicReject(deliveryTag: ea.DeliveryTag,true);
        Console.WriteLine($"Trying Again..");
    }
};

channel.BasicConsume(queue: queueName, autoAck:false, consumer: consumer);

Console.WriteLine("Consuming");

Console.ReadKey();