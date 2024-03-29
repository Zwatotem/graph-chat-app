﻿using System;

namespace ChatClient.HandlePanelStrategies
{
    public class HandleWelcomePanelStrategy : IHandlePanelStrategy
    {
        public int handle(ChatClient client)
        {
            if(!Console.IsOutputRedirected) {Console.Clear(); }
            Console.WriteLine("Welcome to GUIChatClient!");
            Console.WriteLine("Type in a number to proceed:");
            Console.WriteLine("1 - register new user\t2 - log in\t0 - quit");
            int decision;
            bool isNum = int.TryParse(Console.ReadLine(), out decision);
            if (!isNum || decision < 0 || decision > 2)
            {
                return 10;
            }
            return (decision == 0) ? decision : 1000 + decision;
        }
    }
}
