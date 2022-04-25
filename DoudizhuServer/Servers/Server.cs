using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Common;

using GameServer.Controller;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

namespace GameServer.Servers
{
	class Server
	{
		// public static MySqlConnection con;    // 数据库
		private IPEndPoint iPEndPoint;
		private Socket server;
		//private List<Client> clients = new List<Client>();
		private Dictionary<string, Client> clients => Client.allClient;
		public ManagerController managerController;

		private Server() { }
		public static Server Instance => instance;
		static Server instance = new Server();


		public void OnInit(string ipString, int port) {
			iPEndPoint = new IPEndPoint(IPAddress.Parse(ipString), port);
			managerController = new ManagerController(this);
		}

		public void Start() {
			server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			server.Bind(iPEndPoint);
			server.Listen(100);
			server.BeginAccept(AcceptCallBack, null);
		}

		/// <summary>
		/// 接收客户端链接
		/// </summary>
		/// <param name="ar"></param>
		public void AcceptCallBack(IAsyncResult ar) {
			//Console.WriteLine("ar" + ar);
			lock (clients) {

				Socket cliSocket = server.EndAccept(ar);
				Client client = new Client(cliSocket, this);

				//clients.Add(client);
				//Console.WriteLine(client);
			}
			server.BeginAccept(AcceptCallBack, null);
		}


		/// <summary>
		/// 移除没用的客户端
		/// </summary>
		/// <param name="client"></param>
		public void RemoveClient(Client client) {
			lock (clients) {
				//clients.Remove(client);
			}
		}


		/// <summary>
		/// 收到的消息(请求)进行处理(转化为用户响应)
		/// </summary>
		/// <param name="content"></param>
		public void HandleRequest(Content content, Client client) {
			lock (managerController) {          // 一次处理一个请求(不同客户端可能同时发送请求)
				managerController.HandleRequest(content, client);
			}
		}

		/// <summary>
		/// 发送响应, 要将处理完成的请求的结果
		/// 返回给客户端
		/// </summary>
		/// <param name="content">发送内容</param>
		/// <param name="client">发送给哪个客户端</param>
		public void SendResponse(Content content, Client cli) {
			lock (clients) {
				List<Client> invalid = new List<Client>();
				foreach (Client item in clients.Values) {
					if (!item.Connected) invalid.Add(item);
				}
				//Console.WriteLine("无效客户端数：" + invalid.Count);
				foreach (Client item in invalid) {
					item.Close();
				}
				List<Client> sends = new List<Client>();
				try {
					switch (content.sendTo) {
						case SendTo.Nothing:
							break;
						case SendTo.Everything:
							foreach (Client client in clients.Values)
								sends.Add(client);
							break;
						case SendTo.InRoom:
							BaseController controller = managerController.GetController(RequestCode.Room);
							RoomController roomController = controller as RoomController;
							foreach (PlayerInfo player in roomController.rooms[cli.Room_num].players) {
								Client client = null;
								clients.TryGetValue(player.id, out client);
								if (client != null) {
									sends.Add(client);
								}
							}
							break;
						case SendTo.Single:
							sends.Add(cli);
							break;
						case SendTo.Server:
							break;
						case SendTo.Double:         // 发送给两个客户端
							sends.Add(cli);                             // 发起者
							sends.Add(Client.GetClient(content.id));    // 接受者
							break;
						default:
							break;
					}
					foreach (var client in sends) {
						client.SendMessage(content);
					}
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					DAO.ErrorDAO.InsertErrorMessage(new Model.Error(e.Message, "Server.cs/SendResponse", cli.Id));
				}
			}

		}
	}
}
