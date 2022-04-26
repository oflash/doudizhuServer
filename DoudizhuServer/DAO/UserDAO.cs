using GameServer.Model;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DAO
{
	class UserDAO
	{
		/// <summary>
		/// 玩家连接服务器时，插入新数据，id,ip,time
		/// </summary>
		/// <param name="con"></param>
		/// <param name=""></param>
		/// <returns></returns>
		public bool InsertUser(MySqlConnection con, User user) {
			//Console.WriteLine("插入数据：\nid:" + id + ", username:" + username + ", sex:" + sex + ", room_index:" + room_index);
			try {
				Console.WriteLine("正在插入玩家...");     // 暂时只插入最基本的数据, 房间号暂时默认为0

#if WINDOWS
				Console.WriteLine(user.Ip + "\t\t" + user.Link_time);
#endif
				MySqlCommand cmd = new MySqlCommand(string.Format("replace into user(id,ip,link_time) values('{0}','{1}','{2}');", user.Id, user.Ip, user.Link_time.ToString("yyyy-MM-dd HH-mm-ss")), con);
				cmd.ExecuteNonQuery();
				return true;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "UserDAO.cs/InsertUser", user.Id));
				//Console.WriteLine("连接失败");
			}
			return false;
		}

		/// <summary>
		/// 将玩家信息, 插入数据数据库,id,username,sex,coin
		/// </summary>
		/// <param name="con"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool UpdateUserInfo(MySqlConnection con, User user) {
			//Console.WriteLine("插入数据：\nid:" + id + ", username:" + username + ", sex:" + sex + ", room_index:" + room_index);
			try {
				Console.WriteLine("正在插入玩家信息数据...");     // 暂时只插入最基本的数据, 房间号暂时默认为0
				MySqlCommand cmd = new MySqlCommand(string.Format("update user set username = '{0}', sex = {1}, coin = {2} where id = '{3}';", user.Username, user.Sex, user.Coin, user.Id), con);
				cmd.ExecuteNonQuery();
				return true;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "UserDAO.cs/UpdateUserInfo", user.Id));
				Console.WriteLine("更新玩家数据失败");
			}
			return false;
		}


		/// <summary>
		/// 判断用户是否存在表中
		/// </summary>
		/// <param name="con"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public bool JudgeUserExit(MySqlConnection con, User user) {
			lock (con) {
				MySqlDataReader reader = null;
				try {
					Console.WriteLine("查询用户是否存在...");
					//string sql = "select count(*) from user;";
					string sql = string.Format("select id from user where id = '{0}';", user.Id);

					MySqlCommand cmd = new MySqlCommand(sql, con);
					reader = cmd.ExecuteReader();
					if (reader.Read()) {
						return true;
					} else {
						return false;
					}
				} catch (Exception e) {
					ErrorDAO.InsertErrorMessage(new Error(e.Message, "UserDAO.cs/JudgeUserExit"));
					Console.WriteLine("获取失败!!!");
					return false;
				} finally {
					if (reader != null)
						reader.Close();
				}
			}
		}



		/// <summary>
		/// 获取目前表中有多少个玩家
		/// </summary>
		/// <param name="con"></param>
		/// <returns></returns>
		public int GetUserCount(MySqlConnection con) {
			lock (con) {
				MySqlDataReader reader = null;
				try {
					Console.WriteLine("正在获取历史人数...");
					//string sql = "select count(*) from user;";
					string sql = "explain select * from user;";

					MySqlCommand cmd = new MySqlCommand(sql, con);
					reader = cmd.ExecuteReader();
					if (reader.Read()) {
						//int count = reader.GetInt32("count(*)");
						int count = reader.GetInt32("rows");
						return count;
					}
					return -1;
				} catch (Exception e) {
					ErrorDAO.InsertErrorMessage(new Error(e.Message, "UserDAO.cs/GetUserCount"));
					Console.WriteLine("获取失败!!!");
				} finally {
					if (reader != null)
						reader.Close();
				}
				return -1;
			}
		}


		/// <summary>
		/// 修改玩家豆子变化信息
		/// </summary>
		public int UpdateUserCoin(MySqlConnection con, User user) {
			MySqlDataReader reader = null;
			try {
				Console.WriteLine("尝试修改玩家豆子值...");
				lock (con) {
					MySqlCommand cmd = new MySqlCommand(string.Format("select coin from user where id = '{0}';", user.Id), con);
					reader = cmd.ExecuteReader();
					if (reader.Read()) {
						int oldCoin = reader.GetInt32("coin");                      // 数据库中原始数据
						user.Coin = Math.Max(0, oldCoin + user.Coin);               // 修改新的金币值
						Console.WriteLine("金币修改：" + oldCoin + " -> " + user.Coin);
					}
					if (reader != null)
						reader.Close();
				}

				MySqlCommand cmd1 = new MySqlCommand(string.Format("update user set coin = {0} where id = '{1}';", user.Coin, user.Id), con);
				cmd1.ExecuteNonQuery();

				Console.WriteLine("修改成功...");
				return user.Coin;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "UserDAO.cs/UpdateUserCoin", user.Id));
				Console.WriteLine("修改失败!!!");
				return -1;
			} finally {

			}
		}

	}
}
