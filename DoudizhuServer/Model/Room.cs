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
			this.create_time = DateTime.Now;
		}


		private int room_id;			// 房间id, 主键
		private int room_num;			// 房间号

		private int status;				// 房间状态, 等待加入0, 游戏中1
		private int scene_id;			// 房间背景场景编号
		private DateTime create_time;	// 房间的创建时间


		public int Room_num { get => room_num; set => room_num = value; }
		public int Status { get => status; set => status = value; }
		public int Scene_id { get => scene_id; set => scene_id = value; }
		public int Room_id { get => room_id; set => room_id = value; }
		public DateTime Create_time { get => create_time; set => create_time = value; }
		//internal List<User> Users { get => users; set => users = value; }
	}
}
