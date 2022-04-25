using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

	/// <summary>
	/// 也是一个类
	/// </summary>
	///
	[Serializable]
	public enum ContentType
	{
		Default,            // 默认不解析, 直接使用字符串
		PlayerInfo,         // 玩家信息(类)
		Prompt,             // 提示信息(字符串)

		ChatInfo,           // 聊天信息(类)
		AttackInfo,         // 攻击信息(类)

		OrderInfo,          // 出牌顺序字典

		RobInfo,            // 抢地主相关信息
		CardList,           // 卡牌组

		ResultInfo,         // 结果信息, 玩家的豆增减情况

	}
}
