using ProductionService.Services;

class Program
{
    static void Main(string[] args)
    {
        var productionService = new ProductionService.Services.ProductionService();
        productionService.ListenForProductionRequests();

        Console.WriteLine("ProductionService is running. Press enter to exit.");
        Console.ReadLine();
    }
}