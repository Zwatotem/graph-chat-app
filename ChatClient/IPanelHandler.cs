﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public interface IPanelHandler
    {
        int handle(ChatClient client);
    }
}
