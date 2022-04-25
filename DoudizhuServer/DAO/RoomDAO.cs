using GameServer.Controller;
using GameServer.Model;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DAO
{
	class RoomDAO
	{

		/// <summary>
		/// 查询是否已经存在房间号
		/// </summary>
		/// <param name="con"></param>
		/// <param name="room_num"></param>
		/// <returns></returns>
		private bool JudgeRoomExits(MySqlConnection con, int room_num) {
			lock (con) {
				MySqlDataReader reader = null;
				try {
					MySqlCommand cmd = new MySqlCommand(string.Format("select room_num from room where room_num = {0};", room_num), con);
					reader = cmd.ExecuteReader();
					// 存在该房间号
					if (reader.Read()) {
						return true;
					} else {
						return false;
					}
				} catch (Exception e) {
					ErrorDAO.InsertErrorMessage(new Error(e.Message, "RoomDAO.cs/JugdeRoomExits"));
					return false;
				} finally {
					if (reader != null)
						reader.Close();
				}
			}
		}

		/// <summary>
		/// 获取房间人数
		/// </summary>
		/// <param name="con"></param>
		/// <param name="room_num"></param>
		private int GetRoomCount(MySqlConnection con, int room_num) {
			lock (con) {
				MySqlDataReader reader = null;
				try {
					MySqlCommand cmd = new MySqlCommand(string.Format("select count(*) from user where room_num = {0};", room_num), con);
					reader = cmd.ExecuteReader();
					if (reader.Read()) {
						int cnt = reader.GetInt32("count(*)");
						Console.WriteLine("获取房间人数成功");
						return cnt;
					} else {
						return int.MaxValue;        // 表示房间不存在
					}
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					ErrorDAO.InsertErrorMessage(new Error(e.Message, "RoomDAO.cs/GetRoomCount"));
					return int.MaxValue;
				} finally {
					if (reader != null)
						reader.Close();
				}
			}
		}


		/// <summary>
		/// 创建房间
		/// </summary>
		/// <param name="con"></param>
		/// <param name="room_num">房间号</param>
		public bool CreateRoom(MySqlConnection con, Room room) {
			try {

				Console.WriteLine("正在创建房间");

				if (JudgeRoomExits(con, room.Room_num)) {
					Console.WriteLine("该房间号已存在!!");
					return false;
				}

				MySqlCommand cmd = new MySqlCommand(string.Format("insert into room(room_num, status, scene_id) values({0},{1},{2});", room.Room_num, room.Status, room.Scene_id), con);
				cmd.ExecuteNonQuery();

				Console.WriteLine("创建成功");
				return true;
			} catch (Exception e) {
				Console.WriteLine("创建失败");
				ErrorDAO.InsertErrorMessage(new Error(e.Message,"RoomDAO.cs/CreateRoom"));
				Console.WriteLine(e.Message);
				return false;
			}
		}

		/// <summary>
		/// 玩家加入房间
		/// </summary>
		/// <param name="con"></param>
		/// <param name="user">需要Id</param>
		/// <param name="room_num">需要房间号</param>
		public int JoinRoom(MySqlConnection con, User user, Room room) {

			try {
				if (!JudgeRoomExits(con, room.Room_num)) {
					Console.WriteLine("该房间号不存在!!!");
					return -1;
				} else if (GetRoomCount(con, room.Room_num) >= 3) {
					Console.WriteLine("房间已满!!");
					return -2;
				}

				string mysql = string.Format("update user set room_num = {0} where id = '{1}';", room.Room_num, user.Id);
				Console.WriteLine(mysql);
				// 修改外键房间号
				MySqlCommand cmd = new MySqlCommand(mysql, con);

				cmd.ExecuteNonQuery();

				Console.WriteLine("加入成功");
				return 0;
			} catch (Exception e) {
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "RoomDAO.cs/JoinRoom", user.Id));
				Console.WriteLine("加入失败");
				return -3;
			}
		}

		/// <summary>
		/// 玩家退出房间，最后一个玩家退出则删除房间
		/// </summary>
		/// <param name="con"></param>
		/// <param name="user">需要id</param>
		/// <param name="room">需要room_num</param>
		public void DeleteRoom(MySqlConnection con, User user, Room room) {
			try {
				//int room_count = 0;
				if (user != null) {

					MySqlCommand cmd = new MySqlCommand(string.Format("update user set room_num = 0 where id = '{0}';", user.Id), con);
					cmd.ExecuteNonQuery();      // 修改用户表房间号为0
					Console.WriteLine(user.Username + "退出了房间");
				} else {
					// 表示删除无用的房间号
					MySqlCommand cmd = new MySqlCommand(string.Format("update user set room_num = 0 where room_num = {0};", room.Room_num), con);
					cmd.ExecuteNonQuery();      // 修改用户表房间号为0
				}

				int room_count = GetRoomCount(con, room.Room_num);       // 获取房间剩余人数

				if (room_count == 0) {
					MySqlCommand cmd1 = new MySqlCommand(string.Format("delete from room where room_num = {0};", room.Room_num), con);
					cmd1.ExecuteNonQuery();     // 删除该房间号
					Console.WriteLine("删除成功");
				}
			} catch (Exception e) {
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "RoomDAO.cs/DeleteRoom", user.Id));
				Console.WriteLine(e.Message);
				Console.WriteLine("删除失败");
			}
		}

	}
}
