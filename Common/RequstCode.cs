using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	/// <summary>
	/// 请求的是哪个类的任务
	/// </summary>

	[Serializable]
	public enum RequestCode
	{
		None,
		User,
		Room,       // CreateRoom, JionRoom, ExitRoom, ReadyGame,
		Player,     // Chat, Attack, Header,

		Card,       //

		PlayGame,   //
		Default = None,     // 默认


	}
}
