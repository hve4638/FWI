
namespace FWIClient
{
    public class RemoteConsoleControl
    {
        public int RemoteId => connectId;
        private int lastId = 1;
        private int connectId = 0;

        public int Open(Action<SimpleConnection.Server>? onConnect, Action? onDisconnect)
        {
            if (Connected()) return -1;
            var currentId = lastId++;
            RemoteConsole.Open(7010);

            var task = new Task(() => {
                var server = new SimpleConnection.Server(7010);
                server.Accept();

                connectId = currentId;
                onConnect?.Invoke(server);

                while (!server.Connected)
                {
                    Thread.Sleep(100);
                }
                connectId = 0;
                onDisconnect?.Invoke();
            });
            task.Start();
            return currentId;
        }

        public bool Connected()
        {
            return connectId > 0;
        }
        public bool Connected(int id)
        {
            return Connected() && id == connectId;
        }
    }
}
