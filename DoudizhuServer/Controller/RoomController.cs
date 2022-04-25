using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;

using Newtonsoft.Json;

namespace GameServer.Controller
{
	class RoomController : BaseController
	{
		public Dictionary<int, RoomInfo> rooms;

		UserDAO userDAO;
		RoomDAO roomDAO;

		public RoomController(ManagerController mngController) {
			this.mngController = mngController;
			rooms = new Dictionary<int, RoomInfo>();
			rooms[0] = new RoomInfo();      // 默认的房间

			userDAO = new UserDAO();
			roomDAO = new RoomDAO();
		}


		/// <summary>
		/// 处理创建房间请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content CreateRoom(object obj, Client client, Server server) {
			Content send = null;
			try {
				int binary = Int32.Parse(obj.ToString());
				int scene_id = binary & 0xff;
				int room_num = binary >> 8;
				Room room = new Room(room_num, scene_id);
				if (roomDAO.CreateRoom(client.con, room)) {     // 创建成功
					send = new Content(ReturnCode.Success, ActionCode.CreateRoom, ContentType.Default, SendTo.Single, "创建成功");
					// 创建该房间所需资源
					Console.WriteLine(room_num);
					rooms[room_num] = new RoomInfo();
				} else {
					send = new Content(ReturnCode.Fail, ActionCode.CreateRoom, ContentType.Default, SendTo.Single, "房间已存在，创建失败");
				}
				return send;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				ErrorDAO.InsertErrorMessage(new Error(e.Message,"RoomController.cs/CreateRoom", client.Id));
				send = new Content(ReturnCode.Fail, ActionCode.CreateRoom, ContentType.Default, SendTo.Single, "房间错误，创建失败");
				return send;
			} finally {

			}
		}


		/// <summary>
		/// 处理加入房间请求
		/// </summary>
		/// <param name="obj">传入过来的玩家信息PlayerInfo</param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content JoinRoom(object obj, Client client, Server server) {
			PlayerInfo player = JsonConvert.DeserializeObject<PlayerInfo>(obj.ToString());

			User user = new User(player.id, player.name, player.sex ? 1 : 0, 6000);
			userDAO.InsertUser(client.con, user);       // 用户信息插入数据库, 不管有没有加入成功房间，都插入数据

			int room_sence = Convert.ToInt32(player.other);
			int room_num = player.room_num;
			Console.WriteLine("房间号：" + room_num);

			RoomInfo ri = null;
			rooms.TryGetValue(room_num, out ri);


			// 尝试修改数据库
			//int res = roomDAO.JoinRoom(client.con, player.id, room_num);
			int res = roomDAO.JoinRoom(client.con, new User(player.id), new Room(room_num));
			string fail = "";
			if (res == -1) {
				fail = "房间号不存在";
			} else if (res == -2) {
				fail = "房间人数已满";
			} else if (res == -3) {
				fail = "系统错误";
			} else {
				client.Room_num = room_num;     // 数据库中有房间
			}

			// 加入成功，且存在key值
			if (ri != null) {
				lock (ri) {
					if (string.IsNullOrEmpty(fail)) {

						if (Convert.ToInt32(player.other) != -1) {  // 创建房间
							ri.roomScene = room_sence;
						} else {                        // 加入房间
							player.other = ri.roomScene as object;
						}
						player.index = ri.players.Count;
						client.playerInfo = player;
						ri.players.Add(player);

						Content send0 = new Content(ReturnCode.Success, ActionCode.JoinRoom,
							ContentType.PlayerInfo, SendTo.InRoom, JsonConvert.SerializeObject(ri.players));       // 注意, 这里传递的是链表
						send0.id = client.Id;

						Console.WriteLine(player.name + " 加入成功");
						return send0;
					} else {
						// 数据库中没有房间，但存在key值
						rooms.Remove(room_num);
					}
				}
			} else if (string.IsNullOrEmpty(fail)) {
				// 数据库中有房间，但不存在key值 -> 删除数据库中的房间
				roomDAO.DeleteRoom(client.con, null, new Room(room_num));
			}
			// 错误 or 加入失败
			Content send = new Content(ReturnCode.Fail, ActionCode.JoinRoom,
						ContentType.Default, SendTo.Single, fail);
			send.id = client.Id;
			Console.WriteLine(player.name + " 加入失败");
			return send;
		}

		/// <summary>
		/// 退出房间
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		public void ExitRoom(object obj, Client client, Server server) {
			RoomInfo ri = null;
			rooms.TryGetValue(client.Room_num, out ri);
			if (ri == null) return;

			lock (ri.players) {
				//players.Remove(player);


			}
		}

		#region 出牌顺序Order
		/// <summary>
		/// 处理申请出牌顺序请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Order(object obj, Client client, Server server) {
			RoomInfo ri = null;
			rooms.TryGetValue(client.Room_num, out ri);

			if (ri == null) {
				Content send0 = new Content(ReturnCode.Fail, ActionCode.Order, ContentType.OrderInfo, SendTo.Single);
				send0.id = client.Id;        // 发给单人, 因为每个客户端都会请求
				return send0;
			}

			lock (ri.nextPlayer) {
				for (int i = 0; i < ri.players.Count; i++) {
					string key = ri.players[i].id;
					string value = ri.players[(i + 1) % ri.players.Count].id;
					AddNextPlayer(key, value, client);
				}
			}

			Content send = new Content(ReturnCode.Success, ActionCode.Order,
				ContentType.OrderInfo, SendTo.Single, JsonConvert.SerializeObject(ri.nextPlayer));
			send.id = client.Id;        // 发给单人, 因为每个客户端都会请求
			Console.WriteLine("出牌顺序：");
			Console.WriteLine(send.content);

			return send;
		}

		public string GetNextPlayer(string id, Client client) {
			string next = "";
			if (!rooms[client.Room_num].nextPlayer.TryGetValue(id, out next)) next = id;
			return next;
		}
		private void AddNextPlayer(string key, string value, Client client) {
			string s;
			if (rooms[client.Room_num].nextPlayer.TryGetValue(key, out s)) {   // 原来有, 使用原来的

				return;
			}
			rooms[client.Room_num].nextPlayer.Add(key, value);
		}
		#endregion


		/// <summary>
		/// 玩家点击了准备开始游戏
		/// </summary>
		/// <param name="obj">None</param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content ReadyGame(object obj, Client client, Server server) {
			RoomInfo ri = null;
			rooms.TryGetValue(client.Room_num, out ri);

			if (ri == null) {
				Content send0 = new Content(ReturnCode.Fail, ActionCode.ReadyGame,
					ContentType.Default, SendTo.InRoom, ri.cnt_player.ToString());
				send0.id = client.Id;
				return send0;
			}

			ri.cnt_player = (ri.cnt_player + 1) % ri.players.Count;          // 1 -> 2 -> 0(0作为满标记)

			Content send = new Content(ReturnCode.Success, ActionCode.ReadyGame,
				ContentType.Default, SendTo.InRoom, ri.cnt_player.ToString());
			send.id = client.Id;
			//Console.WriteLine(cnt_player);
			if (ri.cnt_player == 0) {      // 3人都准备了, 随机确定一个人叫地主
				Random rand = new Random();
				int index = rand.Next(ri.players.Count);
				send.id = ri.players[index].id;        // 这里传送id不表示发送者id, 表示让谁来叫地主
			}

			Console.WriteLine(client.Id + "已准备");

			return send;
		}
	}
}
