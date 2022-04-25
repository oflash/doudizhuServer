using System;


namespace Common
{

	[Serializable]
	public class AttackInfo
	{
		public int attackType;  // 攻击类型
		public string target;   // 攻击目标

		public AttackInfo(int attackType, string target) {
			this.attackType = attackType;
			this.target = target;
		}
	}
}

