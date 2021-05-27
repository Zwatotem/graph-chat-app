using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleWelcomePanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            Console.Clear();
            Console.WriteLine("Welcome to GraphChatApp!");
            Console.WriteLine("Type in a number to proceed:");
            Console.WriteLine("1 - register new user\t2 - log in\t0 - quit");
            int decision;// = Convert.ToInt32(Console.ReadLine());
            bool isNum = int.TryParse(Console.ReadLine(), out decision);
            if (!isNum || decision < 0 || decision > 2)
            {
                return 10;
            }
            return (decision == 0) ? decision : 1000 + decision;
        }
    }
}
