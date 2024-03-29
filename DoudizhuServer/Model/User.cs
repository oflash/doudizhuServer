﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
	class User
	{
		public User(string id, string ip = "no data", string username = "name", int sex = 0, int coin = 0) {
			this.Id = id;
			this.Username = username;
			this.Sex = sex;
			this.Coin = coin;
			this.ip = ip;
			link_time = DateTime.Now;
		}

		private string id;           // 用户ID
		private string username;     // 用户名字
		private int room_index;           // 房间内id号
		private int coin;            // 金币
		private int room_num;        // 所属房间号
		private int sex;            // 性别
		private string ip;          // 连接的ip信息
		private DateTime link_time; // 连接服务器的时间


		public string Id { get => id; set => id = value; }
		public string Username { get => username; set => username = value; }
		public int Coin { get => coin; set => coin = value; }
		public int Room_num { get => room_num; set => room_num = value; }
		public int Room_index { get => room_index; set => room_index = value; }
		public int Sex { get => sex; set => sex = value; }
		public string Ip { get => ip; set => ip = value; }
		public DateTime Link_time { get => link_time; set => link_time = value; }
	}
}
