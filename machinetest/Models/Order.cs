namespace machinetest.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryExpected { get; set; }
        public bool ContainsGift { get; set; }
        public string DeliveryAddress { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
