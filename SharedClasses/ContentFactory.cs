using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    public class ContentFactory
    {
        public static MessageContent getContent(byte[] data, int offset)
        {
            if (data[0 + offset] == 1) //text message
            {
                return new TextContent(Encoding.UTF8.GetString(data, 1 + offset, data.Length - 1 - offset));
            }
            else //unrecognized type of message
            {
                return null;
            }
        }
    }
}
