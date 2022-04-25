using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

	[Serializable]
	public class PromptInfo
	{
		public string info;     // 发送的提示信息
		public SendTo sendTo;   // 接收的对象

		public PromptInfo(string info, SendTo sendTo) {
			this.info = info;
			this.sendTo = sendTo;
		}
	}
}
