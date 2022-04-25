
using System;

namespace Common
{

	[Serializable]
	public class ResultInfo
	{
		public int Double;  // 倍数
		public int Douzi;   // 豆增减情况
		public int Bottom;  // 底分


		public ResultInfo(int Double, int Douzi, int Bottom) {
			this.Double = Double;
			this.Douzi = Douzi;
			this.Bottom = Bottom;
		}

	}
}