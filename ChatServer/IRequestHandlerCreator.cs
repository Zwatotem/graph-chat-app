using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public interface IRequestHandlerCreator
    {
        IRequestHandler createRequestHandler(byte typeByte);
    }
}
