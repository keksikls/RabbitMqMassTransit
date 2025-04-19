namespace RabbitMQMassTransit.Models
{
    public class CreateTransaction
    {
        public string CardNumber { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
