using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	[Serializable]
	public enum SendTo
	{
		Nothing,
		Everything,     // 所有Client
		InRoom,         // 在房间中的人
		Single,         // 发送给单个人
		Double,         // 发送给两个人(私聊):发起者, 目标

		Server,         // 客户端发送给服务器的
	}
}
