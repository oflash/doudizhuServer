using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GameServer.Tools;
using GameServer.DAO;
using GameServer.Model;
using Newtonsoft.Json;
using System.Net;

namespace GameServer.Servers
{
	class Client
	{
		#region 静态变量、方法
		public static Dictionary<string, Client> allClient;
		//private static List<Client> allClient;
		static Client() {
			allClient = new Dictionary<string, Client>();
			//allClient = new List<Client>();
		}
		public static Client GetClient(string id) {
			Client res = null;
			allClient.TryGetValue(id, out res);
			return res;
		}
		#endregion

		// public static uint cnt = 0;     // 统计共有几个客户端连接过
		private string id;              // 所有客户端连接后分配一个唯一id账号(这里没有数据库就在Client增加一个)
		public string Id => id;

		public bool Connected => cliSocket.Connected;
		public EndPoint RemoteEndPoint => cliSocket.RemoteEndPoint;

		public int Room_num { get => room_num; set => room_num = value; }
		private int room_num = 0;       // 房间号

		private Socket cliSocket;       // 接收的客户端
		private Server server;          // 服务器
		public MySqlConnection con;    // 数据库
		private Message recv;
		public PlayerInfo playerInfo;   // 玩家信息


		public Client(Socket cliSocket, Server server) {
			this.cliSocket = cliSocket;
			this.server = server;
			// id = "client" + (++cnt);
			con = ConnectHelper.Connect();      // 开启数据库
			Start();
		}

		~Client() {
			allClient.Remove(this.id);
			//allClient.Remove(this);
		}

		private void Start() {
			// SendMessage(init_attr);         // 发送客户端初始化信息
			UserDAO userDAO = new UserDAO();
			int cnt = userDAO.GetUserCount(con);
			Console.WriteLine("历史人数：" + cnt);
			id = "client" + (cnt + 1);
			userDAO.InsertUser(con, new User(id, cliSocket.RemoteEndPoint.ToString()));

			allClient[id] = this;
			Console.WriteLine(allClient.Count);

			Content init_attr = new Content(ReturnCode.Success, ActionCode.Initialization, ContentType.Default, SendTo.Single, Id);
			server.SendResponse(init_attr, this);
			Console.WriteLine(init_attr.actionCode);
			recv = new Message(1024);
			cliSocket.BeginReceive(recv.Data, 0, recv.Length, SocketFlags.None, ReceiveCallBack, null);
		}


		/// <summary>
		/// 接收消息
		/// </summary>
		/// <param name="ar"></param>
		public void ReceiveCallBack(IAsyncResult ar) {

			try {
				if (cliSocket.Poll(10, SelectMode.SelectRead)) {
					// 如果Read 10微秒后没有读到
					Close();
					return;
				}


				//Console.WriteLine(ar);
				int count = cliSocket.EndReceive(ar);
				if (count == 0) {
					Close();
					return;
				}
				string[] msgs = recv.GetMessageStrings(0, count);
				foreach (string msg in msgs) {
					//Console.WriteLine();
					//Console.WriteLine(msg);

					Content content = Newtonsoft.Json.JsonConvert.DeserializeObject<Content>(msg);

					server.HandleRequest(content, this);
				}
				recv = new Message(1024);
				cliSocket.BeginReceive(recv.Data, 0, recv.Length, SocketFlags.None, ReceiveCallBack, null);
			} catch (Exception e) {
				Console.WriteLine("e:" + e);
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "Client.cs/ReceiveCallBack", Id));
				Close();
			}
		}


		/// <summary>
		/// 关闭链接, 并清理资源
		/// </summary>
		public void Close() {
			if (cliSocket == null) return;
			try {
				string s = this.Id + ":退出了连接";
				PromptInfo promptInfo = new PromptInfo(s, SendTo.InRoom);
				Content content = new Content(ContentType.Prompt, ActionCode.Prompt, RequestCode.Default, JsonConvert.SerializeObject(promptInfo));
				server.HandleRequest(content, this);

				Console.WriteLine("断开");
				RoomDAO roomDAO = new RoomDAO();
				if (room_num != 0) roomDAO.DeleteRoom(con, new User(Id), new Room(Room_num));

				cliSocket.Close();
				allClient.Remove(Id);
				ConnectHelper.CloseSql(con);    // 关闭数据库
			} catch (Exception e) {
				ErrorDAO.InsertErrorMessage(new Error(e.Message, "Client.cs/Close", Id));
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// 发送消息给具体的客户端
		/// </summary>
		/// <param name="content"></param>
		public void SendMessage(Content content) {
			string msg = Newtonsoft.Json.JsonConvert.SerializeObject(content);
			//Console.WriteLine(msg);
			Message send = new Message(msg);    // 需要发送的数据

			cliSocket.Send(send.Data);
		}
	}
}
