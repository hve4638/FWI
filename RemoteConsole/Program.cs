namespace RemoteConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = args[0];
            var port = int.Parse(args[1]);

            var client = new SimpleConnection.Client(ip, port);
            client.Connect();
            Task.Factory.StartNew(client.Receive);

            string? message;
            while((message = Console.ReadLine()) != null) client.Send(message);
        }
    }
}