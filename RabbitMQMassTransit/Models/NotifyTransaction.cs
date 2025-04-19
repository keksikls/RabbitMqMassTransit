namespace RabbitMQMassTransit.Models
{
    public class NotifyTransaction
    {
        public string CardNumber { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
