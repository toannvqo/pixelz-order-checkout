namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
    }
}