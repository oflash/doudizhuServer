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
	class CardController : BaseController
	{

		RoomController roomController;
		public CardController(ManagerController mngController) {
			this.mngController = mngController;
			roomController = mngController.GetController(RequestCode.Room) as RoomController;
		}


		/// <summary>
		/// 洗牌操作, 服务器洗牌然后发送给客户端
		/// </summary>
		private void Shuffle(List<int> cardIds) {
			for (int i = 0; i < 54; i++) cardIds.Add(i + 1);
			Random rand = new Random();

			int a, t;
			for (int i = cardIds.Count - 1; i >= 0; i--) {
				a = rand.Next(i + 1);
				t = cardIds[i];
				cardIds[i] = cardIds[a];
				cardIds[a] = t;
			}
		}


		/// <summary>
		/// 处理发牌请求
		/// </summary>
		/// <returns></returns>
		public Content Divide(object obj, Client client, Server server) {
			ref int shuffle = ref roomController.rooms[client.Room_num].shuffle;
			ref List<int> cardIds = ref roomController.rooms[client.Room_num].cardIds;

			lock (cardIds) {
				if (shuffle == 0) {
					cardIds.Clear();
					Shuffle(cardIds);
				}

				Content send = new Content(ReturnCode.Success, ActionCode.Divide, ContentType.CardList,
						SendTo.Single);
				send.id = client.Id;

				List<int> list = new List<int>();
				// for (int i = shuffle * 17; i < (shuffle + 1) * 17; i++) {
				// 	list.Add(cardIds[i]);
				// }
				for (int i = shuffle; i < 51; i += 3) {
					list.Add(cardIds[i]);
				}
				for (int i = 51; i < 54; i++) {     // 地主牌
					list.Add(cardIds[i]);
				}
				send.content = JsonConvert.SerializeObject(list);

				Console.WriteLine(client.Id + "收到卡牌");

				shuffle = (shuffle + 1) % roomController.rooms[client.Room_num].players.Count;
				return send;
			}
		}

	}
}