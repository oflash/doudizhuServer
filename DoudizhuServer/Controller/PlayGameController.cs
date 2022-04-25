using Common;

using GameServer.DAO;
using GameServer.Model;
using GameServer.Servers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Controller
{
	class PlayGameController : BaseController
	{
		RoomController roomController;
		//int multiple = 5;               // 本局游戏倍数

		public PlayGameController(ManagerController mngController) {
			this.mngController = mngController;
			roomController = mngController.GetController(RequestCode.Room) as RoomController;
		}

		/// <summary>
		/// 处理抢地主请求
		/// </summary>
		/// <param name="obj">bool值, 表示是否抢地主</param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content RobDiZhu(object obj, Client client, Server server) {
			Queue<string> queueRDZ = roomController.rooms[client.Room_num].queueRDZ;
			//int cnt_NoRob = ref roomController.rooms[client.Room_num].cnt_NoRob;
			ref int cnt_NoRob = ref roomController.rooms[client.Room_num].cnt_NoRob;
			ref string one_id = ref roomController.rooms[client.Room_num].one_id;
			ref string one_call = ref roomController.rooms[client.Room_num].one_call;

			bool call = queueRDZ.Count == 0;        // 如果没有人叫过
			if (Convert.ToBoolean(obj)) {
				if (queueRDZ.Count == 0) one_call = client.Id;
				queueRDZ.Enqueue(client.Id);        // 直接入队
			} else {
				cnt_NoRob++;                        // 不抢
			}
			if (one_id == "one_id") one_id = client.Id;
			ReturnCode returnCode = ReturnCode.Fail;

			if (cnt_NoRob == 3 ||                               // 3人都不抢地主, 随机选择一个地主
				cnt_NoRob == 2 && queueRDZ.Count != 0 ||        // 第4轮不抢, 且队列不空, 包含3次决胜负情况
				cnt_NoRob == 1 && queueRDZ.Count == 3 ||        // 第4轮抢
				cnt_NoRob == 0 && queueRDZ.Count == 4) {        // 全部抢
				returnCode = ReturnCode.Success;
			}

			Console.WriteLine("0 " + client.Id);
			string next = roomController.GetNextPlayer(client.Id, client);  // 下一人, 默认Id
			Console.WriteLine("1 " + next);


			if (returnCode == ReturnCode.Fail) {            // 没有确定出地主, 找出下一个发言人
				int total = cnt_NoRob + queueRDZ.Count;                 // 进行到第几轮了
				Console.WriteLine("2 " + total);
				if (total == 3 && one_id != one_call) {
					next = roomController.GetNextPlayer(next, client);          // 跳过第一人
				}
			} else if (returnCode == ReturnCode.Success) {  // 确定出地主
				if (queueRDZ.Count != 0) {
					next = queueRDZ.Last();
				} else {        // 3人都不叫地主
					Random rand = new Random();
					next = roomController.rooms[client.Room_num].players[rand.Next(3)].id;     // 随机一人为地主
				}
				cnt_NoRob = 0;
				queueRDZ.Clear();
				one_id = "one_id";
				one_call = "one_call";
			}

			Console.WriteLine("3 " + next);

			// queueRDZ.Count > 1 表示除了我之外有人叫过地主
			RobInfo info = new RobInfo(call, next, Convert.ToBoolean(obj));

			Content send = new Content(returnCode, ActionCode.RobDiZhu,
				ContentType.RobInfo, SendTo.InRoom, JsonConvert.SerializeObject(info));
			send.id = client.Id;

			Console.WriteLine(client.Id + "地主了");
			return send;
		}


		/// <summary>
		/// 处理地主玩家明牌请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content MingCard(object obj, Client client, Server server) {

			Content send = new Content(ReturnCode.Success, ActionCode.MingCard, ContentType.CardList, SendTo.InRoom, obj.ToString());
			send.id = client.Id;
			return send;
		}


		/// <summary>
		/// 处理加倍请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Double(object obj, Client client, Server server) {
			ref int cnt_mul = ref roomController.rooms[client.Room_num].cnt_mul;

			cnt_mul = (cnt_mul + 1) % roomController.rooms[client.Room_num].players.Count;
			int num = int.Parse(obj.ToString());
			// multiple *= num;
			if (cnt_mul == 0) {
				//multiple = 10;
			}

			Content send = new Content(ReturnCode.Success, ActionCode.Double, ContentType.Default,
				SendTo.InRoom, num.ToString());
			send.id = client.Id;

			return send;
		}


		/// <summary>
		/// 处理出牌请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content OutCard(object obj, Client client, Server server) {

			Content send = new Content(ReturnCode.Success, ActionCode.OutCard, ContentType.CardList, SendTo.InRoom, obj.ToString());
			send.id = client.Id;
			return send;
		}


		/// <summary>
		/// 处理不出请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Pass(object obj, Client client, Server server) {

			Content send = new Content(ReturnCode.Success, ActionCode.Pass, ContentType.Default, SendTo.InRoom);
			send.id = client.Id;
			return send;
		}


		/// <summary>
		/// 处理玩家胜利请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Win(object obj, Client client, Server server) {

			Content send = new Content(ReturnCode.Success, ActionCode.Win, ContentType.Default, SendTo.InRoom);
			send.id = client.Id;
			return send;
		}

		/// <summary>
		/// 处理结果信息请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Result(object obj, Client client, Server server) {
			ResultInfo info = JsonConvert.DeserializeObject<ResultInfo>(obj.ToString());
			// 如果需要处理info
			UserDAO userDAO = new UserDAO();
			//userDAO.UpdateUserCoin(client.con, client.Id, info.Douzi);
			info.Douzi = userDAO.UpdateUserCoin(client.con, new User(client.Id, "", 0, info.Douzi));

			Content send = new Content(ReturnCode.Success, ActionCode.Result,
				ContentType.ResultInfo, SendTo.InRoom, JsonConvert.SerializeObject(info));
			send.id = client.Id;

			return send;
		}

		/// <summary>
		/// 游戏结束后，处理 其他玩家发送剩余牌 请求
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Remaining(object obj, Client client, Server server) {
			int[] ids = JsonConvert.DeserializeObject<int[]>(obj.ToString());

			Content send = new Content(ReturnCode.Success, ActionCode.Remaining,
				ContentType.CardList, SendTo.InRoom, obj.ToString());
			send.id = client.Id;

			return send;
		}
	}
}