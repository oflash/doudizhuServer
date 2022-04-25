using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	[Serializable]
	public class ChatInfo
	{
		public int chatType;      // 0, 1, 2 : Text, Audio, Emotcion
		public string chat;

		/* other:
		 * 如果是音频信息:int, 都是int型
		 * 如果是表情信息:int
		 */
		public string other;

		public ChatInfo(string chat, int chatType = 0, string other = "None") {
			this.chat = chat;
			this.chatType = chatType;
			this.other = other;
		}
	}
}
