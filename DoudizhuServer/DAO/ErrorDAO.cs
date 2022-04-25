using GameServer.Model;
using GameServer.Tools;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.DAO
{
	class ErrorDAO
	{



		/// <summary>
		/// 插入报错信息
		/// </summary>
		/// <param name="con"></param>
		/// <param name="error"></param>
		public static void InsertErrorMessage(Error error) {
			try {
				Console.WriteLine("捕捉到异常");
				MySqlConnection con = ConnectHelper.Connect();              // 开启新的数据库连接

				Console.WriteLine(error.Error_time + "  ->  " + error.Error_func);
				string sql = string.Format("insert into error(error_message, error_time, user_id, error_func) " +
					"values('{0}', '{1}', '{2}', '{3}');",
					error.Error_message, error.Error_time.ToString("yyyy-MM-dd HH-mm-ss"), error.User_id, error.Error_func);
				Console.WriteLine(sql);
				MySqlCommand cmd = new MySqlCommand(sql, con);
				cmd.ExecuteNonQuery();

				ConnectHelper.CloseSql(con);                                // 关闭数据库连接
			} catch (Exception e) {
				Console.WriteLine("插入error表时发生异常");
				Console.WriteLine(e.Message);
			}
		}
	}
}
