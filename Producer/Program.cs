using System.Text;
using Newtonsoft.Json;
using Producer;
using RabbitMQ.Client;

try
{
    var factory = new ConnectionFactory()
    {
        // Uri = new Uri("amqp://sipintarv5:sipintarv5@202.150.91.227:5672/%2f"),
        HostName = "localhost",
        UserName = "sipintarv5",
        Password = "sipintarv5",
        Port = 5672,
        AutomaticRecoveryEnabled = true,
        VirtualHost = "pegasusv2",
        DispatchConsumersAsync = true,
    };

    Console.WriteLine($"Create factory connection");

    using var connection = factory.CreateConnection();

    Console.WriteLine($"Factory connection Success");

    using var channel = connection.CreateModel();

    channel.ExchangeDeclare(exchange: "sipintar.pdam[1]", type: ExchangeType.Topic);

    while (true)
    {
        #region message

        var data = new List<User>();
        data.Add(new User()
        {
            ID = Guid.NewGuid(),
            Nama = "Yoga"
        });

        var message = JsonConvert.SerializeObject(new ParamMessage()
        {
            Process = "Order",
            Method = "Insert",
            Data = data
        });

        #endregion

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "sipintar.pdam[1]", "user.process", null, body);

        channel.BasicPublish(exchange: "sipintar.pdam[1]", "order.process", null, body);

        Console.WriteLine($"Send Message : {message}");

        await Task.Delay(3000);
    }
}
catch (Exception e)
{
    Console.WriteLine($"error : {e}");
}

