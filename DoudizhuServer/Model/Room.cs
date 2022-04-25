using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
	class Room
	{
		public Room(int room_num, int scene_id = 0, int status = 0) {
			this.room_num = room_num;
			this.scene_id = scene_id;
			this.status = status;
		}



		private int room_num;       // 房间号, 主键

		private int status;         // 房间状态, 等待加入0, 游戏中1
		private int scene_id;       // 房间背景场景编号





		public int Room_num { get => room_num; set => room_num = value; }
		public int Status { get => status; set => status = value; }
		public int Scene_id { get => scene_id; set => scene_id = value; }
		//internal List<User> Users { get => users; set => users = value; }
	}
}
