using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
	class Error
	{

		public Error(string error_message, string error_func, string user_id = "default_id") {
			error_message = error_message.Replace('\'', '_');
			error_message = error_message.Replace('\"', '_');
			this.error_message = error_message;
			//error_time = DateTime.UtcNow;

			error_time = DateTime.Now;
			this.error_func = error_func;
			this.user_id = user_id;
		}

		private int error_id;
		private string error_message;
		private string user_id;
		private DateTime error_time;
		private string error_func;

		public int Error_id { get => error_id; set => error_id = value; }
		public string Error_message { get => error_message; set => error_message = value; }
		public DateTime Error_time { get => error_time; set => error_time = value; }
		public string User_id { get => user_id; set => user_id = value; }
		public string Error_func { get => error_func; set => error_func = value; }
	}
}
