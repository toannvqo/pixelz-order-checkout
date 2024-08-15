using EmailService.Services;

class Program
{
    static void Main(string[] args)
    {
        var emailService = new EmailService.Services.EmailService();
        emailService.ListenForEmailRequests();

        Console.WriteLine("EmailService is running. Press [enter] to exit.");
        Console.ReadLine();
    }
}