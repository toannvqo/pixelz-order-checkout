using Microsoft.AspNetCore.Mvc;
using PaymentServiceServices;
using StackExchange.Redis;
using System.Text.Json;
using Order = OrderService.Models.Order;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMessageProducer _messageProducer;
    private readonly IDatabase _cache;
    private readonly PaymentService _paymentClient;
    private static List<Order> Orders = new List<Order>
    {
        new Order { Id = 1, Name = "Order1", TotalAmount = 100, Status = "Pending" },
        new Order { Id = 2, Name = "Order2", TotalAmount = 200, Status = "Pending"  },
        new Order { Id = 3, Name = "Order3", TotalAmount = 300, Status = "Pending" },
        new Order { Id = 4, Name = "Order4", TotalAmount = 400, Status = "Pending"  },
        new Order { Id = 5, Name = "Order5", TotalAmount = 500, Status = "Pending"  },
        new Order { Id = 6, Name = "Order6", TotalAmount = 600, Status = "Pending"  },
        new Order { Id = 7, Name = "Order7", TotalAmount = 700 , Status = "Pending" },
        new Order { Id = 8, Name = "Order8", TotalAmount = 800, Status = "Pending"  },
        new Order { Id = 9, Name = "Order9", TotalAmount = 900, Status = "Pending"  },
        new Order { Id = 10, Name = "Order10", TotalAmount = 1000, Status = "Pending"  },
    };

    public OrderController(IMessageProducer messageProducer, IConnectionMultiplexer redis, PaymentServiceServices.PaymentService paymentClient)
    {
        _messageProducer = messageProducer;
        _cache = redis.GetDatabase();
        _paymentClient = paymentClient;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutOrder([FromBody] int orderId)
    {
        var order = Orders.FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return NotFound();
        }
        // Logic to call PaymentService by gRPC
        var paymentRequest = new PaymentRequest { OrderId = order.Id, Amount = order.TotalAmount };
        var paymentResponse = await _paymentClient.ProcessPayment(paymentRequest, null);

        if (!paymentResponse.Success)
        {
            return BadRequest("Payment failed");
        }

        _messageProducer.PublishToQueue("orderCompletedQueue", order.Id.ToString());
        // ProductionService and EmailService will listen to this queue and process the order

        return Ok(order);
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder(int id)
    {
        var cachedOrder = _cache.StringGet(id.ToString());
        if (!cachedOrder.IsNullOrEmpty)
        {
            return Ok(JsonSerializer.Deserialize<Order>(cachedOrder));
        }

        var order = Orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        _cache.StringSet(order.Id.ToString(), JsonSerializer.Serialize(order));

        return Ok(order);
    }

    [HttpGet]
    public ActionResult<List<Order>> GetOrders(string name = null)
    {
        if (!string.IsNullOrEmpty(name))
        {
            var matchedOrders = Orders
                .Where(o => o.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matchedOrders == null || matchedOrders.Count == 0)
            {
                return NotFound();
            }

            return Ok(matchedOrders);
        }
        return Ok(Orders);
    }
}

