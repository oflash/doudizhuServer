using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

	[Serializable]
	public enum ActionCode
	{
		None,
		Initialization, // 客户端第一次连接服务器时, 服务器发送一些初始化信息给客户端

		CreateRoom,     // 创建房间
		JoinRoom,       // 加入房间
		ExitRoom,       // 退出房间

		Chat,           // 文字聊天
		Attack,         // 攻击
		Header,         // 修改头像

		Order,          // 出牌顺序

		ReadyGame,      // 准备游戏
		Divide,         // 发牌


		RobDiZhu,       // 抢地主相关(抢或不抢)
		OutCard,        // 出牌
		Pass,           // 不要
		Double,         // 加倍
		MingCard,       // 明牌

		Win,            // 胜利者发来胜利消息
		Result,         // 游戏结果, 客户端发送自己
		Remaining,      // 败家剩余卡牌

		Prompt,			// 服务器单方面发送给客户端的提示信息

	}
}
