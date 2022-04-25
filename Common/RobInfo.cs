

using System;

namespace Common
{

	[Serializable]
	public class RobInfo
	{
		public bool call;       //有人叫了出地主?
		public string next;     // 下一个出牌人
		public bool rob;        // 上一个人是否抢地主

		public RobInfo(bool call, string next, bool rob) {
			this.call = call;
			this.next = next;
			this.rob = rob;
		}
	}
}