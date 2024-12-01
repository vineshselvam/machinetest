using Dapper;
using machinetest.Models;
using System.Data.SqlClient;

namespace machinetest.Repositories
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CustomerOrderResponse> GetMostRecentOrderAsync(string email, int customerId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                
                var customerQuery = @"
                SELECT CUSTOMERID, FIRSTNAME, LASTNAME, EMAIL, HOUSENO, STREET, TOWN, POSTCODE
                FROM CUSTOMERS
                WHERE EMAIL = @Email AND CUSTOMERID = @CustomerId";

                
                var orderQuery = @"
                SELECT TOP 1 
                    o.ORDERID, o.ORDERDATE, o.DELIVERYEXPECTED, o.CONTAINSGIFT,
                    CONCAT(c.HOUSENO, ' ', c.STREET, ', ', c.TOWN, ', ', c.POSTCODE) AS DeliveryAddress
                FROM ORDERS o
                INNER JOIN CUSTOMERS c ON o.CUSTOMERID = c.CUSTOMERID
                WHERE c.EMAIL = @Email AND c.CUSTOMERID = @CustomerId
                ORDER BY o.ORDERDATE DESC";

               
                var orderItemsQuery = @"
                SELECT 
                    oi.ORDERITEMID, oi.ORDERID, oi.PRODUCTID, oi.QUANTITY, oi.PRICE,
                    CASE WHEN o.CONTAINSGIFT = 1 THEN 'Gift' ELSE p.PRODUCTNAME END AS PRODUCTNAME
                FROM ORDERITEMS oi
                INNER JOIN PRODUCTS p ON oi.PRODUCTID = p.PRODUCTID
                INNER JOIN ORDERS o ON oi.ORDERID = o.ORDERID
                WHERE oi.ORDERID = @OrderId";

               
                var customer = await connection.QueryFirstOrDefaultAsync<Customer>(customerQuery, new { Email = email, CustomerId = customerId });

                
                if (customer == null)
                {
                    throw new InvalidOperationException("Invalid email and customer ID combination.");
                }

               
                var order = await connection.QueryFirstOrDefaultAsync<Order>(orderQuery, new { Email = email, CustomerId = customerId });

                if (order != null)
                {
                    
                    var orderItems = await connection.QueryAsync<OrderItem>(orderItemsQuery, new { OrderId = order.OrderId });
                    order.OrderItems = orderItems.AsList();
                }

                
                return new CustomerOrderResponse
                {
                    Customer = customer,
                    Order = order
                };
            }
            catch (InvalidOperationException ex)
            {
               
                return new CustomerOrderResponse
                {
                    ErrorMessage = ex.Message
                };
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
