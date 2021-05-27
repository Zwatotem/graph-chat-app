using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class ConcretePanelHandler : IPanelHandler
    {
        private IHandlePanelStrategy handleStrategy;

        public ConcretePanelHandler(IHandlePanelStrategy handleStrategy)
        {
            this.handleStrategy = handleStrategy;
        }

        public int handle(ChatClient client)
        {
            return handleStrategy.handle(client);
        }
    }
}
