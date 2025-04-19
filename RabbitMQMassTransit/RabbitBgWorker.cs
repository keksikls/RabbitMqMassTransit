using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMQMassTransit
{
    //фоновый сервис - подключается к кролику,берет очеред кьюху и обрабатывает сообщения приходящие 
    public class RabbitBgWorker : BackgroundService
    {
        //IConnection — соединение с брокером RabbitMQ.
        //IModel — канал(виртуальное соединение внутри основного), через который происходят операции.
        private IConnection _connection;
        private IModel _channel;

        public RabbitBgWorker()
        {
            //Создает фабрику подключений к RabbitMQ(localhost: 5672).

            //Устанавливает соединение(CreateConnection).

            //Открывает канал(CreateModel).

            //Объявляет очередь TestQueue(если её нет).

            //Параметры QueueDeclare:

            //durable: false — очередь не сохранится после перезапуска RabbitMQ.

            //exclusive: false — очередь не приватная(другие потребители могут подключаться).

            //autoDelete: false — очередь не удалится, когда отключатся все потребители.

            var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "TestQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
//            Проверяет, не запрошена ли отмена(stoppingToken).

//Создает потребителя(EventingBasicConsumer).

//Подписывает метод Consume на событие Received(вызывается при получении сообщения).

//Запускает прослушивание очереди TestQueue с autoAck: false(ручное подтверждение обработки).

//Важно:

//Возвращает Task.CompletedTask, потому что BasicConsume работает асинхронно в фоне.

//Если нужно, чтобы сервис работал долго, можно использовать while (!stoppingToken.IsCancellationRequested).

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consume;

            _channel.BasicConsume("TestQueue", false, consumer);

            return Task.CompletedTask;
        }

        private void Consume(object? ch, BasicDeliverEventArgs eventArgs)
        {
//            Декодирует тело сообщения из byte[] в строку.

//Выводит сообщение в консоль(здесь может быть любая логика: запись в БД, вызов API и т.д.).

//Подтверждает обработку(BasicAck), чтобы RabbitMQ удалил сообщение из очереди.

//Почему autoAck: false ?

//Если autoAck: true, сообщение удаляется из очереди сразу после доставки(даже если обработка упала с ошибкой).

//С autoAck: false мы сами контролируем удаление через BasicAck/ BasicNack.

            var content = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        }

        public override void Dispose()
        {
            //явно освобождаем ресурсы канала и конекшена
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
