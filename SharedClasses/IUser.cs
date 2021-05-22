using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatModel
{
    public interface IUser
    {
		string Name { get; }

		List<Conversation> Conversations { get; }

		bool matchWithConversation(Conversation conversation1);

		bool unmatchWithConversation(Conversation conversation1);
	}
}
