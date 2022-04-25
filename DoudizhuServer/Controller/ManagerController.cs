using Common;

using GameServer.Servers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Controller
{
	class ManagerController
	{
		/// <summary>
		/// 用于存储 请求和类
		/// </summary>
		private Dictionary<RequestCode, BaseController> dicController = new Dictionary<RequestCode, BaseController>();
		private Server server;

		public ManagerController(Server server) {
			ControllerInit();
			this.server = server;
		}

		public void ControllerInit() {
			dicController.Add(RequestCode.Default, new DefaultController(this));

			dicController.Add(RequestCode.Room, new RoomController(this));              // 加入, 退出房间
			dicController.Add(RequestCode.Player, new PlayerController(this));          // Chat, Attack, Header
			dicController.Add(RequestCode.PlayGame, new PlayGameController(this));      // ReadyGame
			dicController.Add(RequestCode.Card, new CardController(this));

		}


		/// <summary>
		/// 获取ControllerInit
		/// </summary>
		/// <param name="requestCode"></param>
		public BaseController GetController(RequestCode requestCode) {
			BaseController baseController;
			dicController.TryGetValue(requestCode, out baseController);
			return baseController;
		}


		/// <summary>
		/// 将从clinet接收到的消息解析到具体类的具体的函数
		/// 相当于客户端发送数据来调用服务器端的方法
		/// </summary>
		/// <param name="content"></param>
		/// <param name="client">处理这个客户端发来的请求</param>
		public void HandleRequest(Content content, Client client) {
			BaseController baseController = null;                                   // 1.具体类
			if (!dicController.TryGetValue(content.requestCode, out baseController)) {
				Console.WriteLine("没有该请求");
				return;
			}


			/* 获取方法 */
			string method = Enum.GetName(typeof(ActionCode), content.actionCode);   // 2.具体函数
			MethodInfo mi = baseController.GetType().GetMethod(method);
			if (mi == null) {
				Console.WriteLine("没有该方法");
				return;
			}

			// 调用mi(获取的方法信息)
			object[] objs = new object[] { content.content, client, server };       // 3.参数
			object o = mi.Invoke(baseController, objs);                             // 4.执行方法并返回响应，返回值

			if (o == null || string.IsNullOrEmpty(o.ToString())) return;            // 错误

			Content response = o as Content;

			server.SendResponse(o as Content, client);  // client:如果是Single, 则发送给client
		}

	}
}
