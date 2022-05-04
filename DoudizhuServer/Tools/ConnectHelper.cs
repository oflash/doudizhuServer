//#define WINDOWS

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tools
{
	class ConnectHelper
	{
#if WINDOWS
		static readonly string sql = "database = my_doudizhu; port = 3306; data source = liuzhiyi.me; user id = liuzhiyi; password = 622411";
#elif LINUX
		static readonly string sql = "database = my_doudizhu; port = 3306; data source = localhost; user id = liuzhiyi; password = 622411";
#endif
		public ConnectHelper() { }


		/// <summary>
		/// 连接数据库
		/// </summary>
		/// <returns></returns>
		public static MySqlConnection Connect() {
			try {

				MySqlConnection con = new MySqlConnection(sql);
				if (con.State == ConnectionState.Closed) {
					Console.WriteLine("尝试连接数据库...");
					con.Open();
					Console.WriteLine("已连接到{0}数据库...", con.Database);
					Console.WriteLine();
					return con;
				}
				return null;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				Console.WriteLine("数据库打开失败!!!");
				return null;
			}
		}

		/// <summary>
		/// 关闭数据库
		/// </summary>
		/// <param name="con"></param>
		public static void CloseSql(MySqlConnection con) {
			try {
				con.Close();
			} catch (Exception) {
				Console.WriteLine("数据库关闭失败");
			}
		}

	}
}
