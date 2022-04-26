//#define WINDOWS

using GameServer.Servers;
using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using GameServer.Tools;
using GameServer.DAO;

namespace DoudizhuServer
{
	class Program
	{
		static void Main(string[] args) {
			//Console.ReadLine();

#if WINDOWS
			if (args.Length < 2) {
				Console.WriteLine("需要两个命令行参数");
				return;
			}
			string ip = args[0];
			int port = int.Parse(args[1]);
#elif LINUX
			string ip = "172.16.38.160";
			int port = 9876;
#endif
			Console.WriteLine("请勿关闭此窗口!!!");
			Console.WriteLine("请让其他玩家输入ip和房间号加入房间：");


			Console.WriteLine(ip);
			Console.WriteLine(port);


			//Console.WriteLine("开启服务器, 请先输入ip和port!");
			//Console.Write("ip:");
			//ip = Console.ReadLine();
			//Console.Write("port");
			//port = int.Parse(Console.ReadLine());


			Server server = Server.Instance;
			server.OnInit(ip, port);
			server.Start();




			//ErrorDAO.InsertErrorMessage(new GameServer.Model.Error("new_test", "Program.cs/Main"));


			//MySqlConnection con = ConnectHelper.Connect();
			//Console.WriteLine(new UserDAO().GetUserCount(con));
			//int a = new RoomDAO().GetRoomID(con, 5484);
			//Console.WriteLine(a);

			//ConnectHelper.CloseSql(con);


			//Console.ReadKey();
			while (true) {
				string s = Console.ReadLine();
				Content content = new Content(ReturnCode.Success, ActionCode.Prompt, ContentType.Prompt, SendTo.Everything);
				content.content = s;
				server.SendResponse(content, null);
				//Content cont =
			}
		}
	}
}
