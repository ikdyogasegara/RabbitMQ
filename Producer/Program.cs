using System.Text;
using Newtonsoft.Json;
using Producer;
using RabbitMQ.Client;

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

    channel.BasicPublish(exchange: "sipintar.pdam[1]","user.process",null,body);

    channel.BasicPublish(exchange: "sipintar.pdam[1]","order.process",null,body);

    Console.WriteLine($"Send Message : {message}");

    await Task.Delay(3000);
}