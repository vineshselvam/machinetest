namespace machinetest.Models
{
    public class CustomerOrderResponse
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}
