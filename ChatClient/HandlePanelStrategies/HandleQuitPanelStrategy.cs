using System;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleQuitPanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if (!Console.IsOutputRedirected) {Console.Clear();}
            Console.WriteLine("Bye bye!");
            client.requestDisconnect();
            return 0;
        }
    }
}
