using Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Controller
{
	class RoomInfo
	{

		#region 原RoomController

		public List<PlayerInfo> players;                // 成功加入到房间的人
		public Dictionary<string, string> nextPlayer;   // 出牌顺序
		public int roomScene;                           // 房间场景
		public int cnt_player;                          // 点击开始游戏的人数

		#endregion


		#region 原PlayGame

		public Queue<string> queueRDZ;          // 判断地主所用队列
		public int cnt_NoRob;                   // 选择不抢的次数
		public string one_id = "one_id";        // 第一人id
		public string one_call = "one_call";    // 第个call人id
		public int cnt_mul;                     // 加倍次数

		#endregion

		#region 原CardController

		public int shuffle;
		public List<int> cardIds;     // 存卡牌id


		#endregion


		public RoomInfo()
		{
			players = new List<PlayerInfo>();
			nextPlayer = new Dictionary<string, string>();
			queueRDZ = new Queue<string>();
			cardIds = new List<int>();
		}

	}
}
