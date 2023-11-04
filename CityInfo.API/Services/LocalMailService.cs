namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        private string _mailTo = "admin@gv.com";
        private string _mailFrom = "noreply@gv.com";

        public void Send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
