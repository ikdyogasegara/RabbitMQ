﻿using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory()
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    Port = 5672,
    AutomaticRecoveryEnabled = true,
    VirtualHost = "SiPintarv5"
};

using var connection = factory.CreateConnection();

using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "sipintar.pdam[1]", type: ExchangeType.Topic);

var queueName = channel.QueueDeclare(queue:"Com1", durable:true, autoDelete:false, exclusive:false).QueueName;

channel.QueueBind(queue: queueName, exchange: "sipintar.pdam[1]", routingKey: "user.#");

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Consumer1 - Received new message : {message}");  

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        Console.WriteLine($"Done");
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