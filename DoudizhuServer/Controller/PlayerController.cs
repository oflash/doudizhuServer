using Common;

using GameServer.Servers;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Controller
{
	class PlayerController : BaseController
	{

		public PlayerController(ManagerController mngController) {
			this.mngController = mngController;
		}

		/// <summary>
		/// 处理发送聊天请求
		/// </summary>
		/// <param name="obj">ChatInfo类</param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Chat(object obj, Client client, Server server) {
			ChatInfo chatInfo = JsonConvert.DeserializeObject<ChatInfo>(obj.ToString());

			Content send = new Content(ReturnCode.Success, ActionCode.Chat,
				ContentType.ChatInfo, SendTo.InRoom, JsonConvert.SerializeObject(chatInfo));
			send.id = client.Id;

			Console.WriteLine(client.Id + "发送了聊天信息");
			return send;
		}

		/// <summary>
		/// 处理攻击请求
		/// </summary>
		/// <param name="obj">AttackInfo类</param>
		/// <param name="client">攻击者</param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Attack(object obj, Client client, Server server) {
			AttackInfo attackInfo = JsonConvert.DeserializeObject<AttackInfo>(obj.ToString());

			// 回复给被攻击者
			Content send = new Content(ReturnCode.Success, ActionCode.Attack,
				ContentType.AttackInfo, SendTo.InRoom, JsonConvert.SerializeObject(attackInfo));
			send.id = client.Id;    // 攻击者id

			// attackInfo.target	:被攻击中id
			System.Console.WriteLine(client.Id + " --攻击了--> " + attackInfo.target);
			return send;
		}

		public Content Header(object obj, Client client, Server server) {
			// obj为头像对应的索引

			RoomController roomController = mngController.GetController(RequestCode.Room) as RoomController;

			foreach (PlayerInfo player in roomController.rooms[client.Room_num].players) {
				if (player.id == client.Id) {
					player.header = System.Convert.ToInt32(obj);
					break;
				}
			}

			Content send = new Content(ReturnCode.Success, ActionCode.Header, ContentType.Default, SendTo.InRoom, obj.ToString());
			send.id = client.Id;        // 修改头像者id

			Console.WriteLine(client.Id + " 修改了头像");
			return send;
		}
	}
}
