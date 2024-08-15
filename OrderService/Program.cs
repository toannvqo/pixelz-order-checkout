var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGrpcClient<PaymentServiceServices.PaymentService>(o =>
{
    o.Address = new Uri("http://localhost:5001");
});

builder.Services.AddSingleton<IConnectionProvider, RabbitMQConnectionProvider>();
builder.Services.AddSingleton<IMessageProducer, RabbitMQProducer>();


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "OrderService_";
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
