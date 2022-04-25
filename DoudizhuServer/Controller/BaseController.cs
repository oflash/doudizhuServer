using GameServer.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GameServer.Controller
{
	abstract class BaseController
	{
		protected ManagerController mngController;

		public RequestCode RequestCode { get => requestCode; }
		RequestCode requestCode = RequestCode.None;


		public virtual string RequestHandle(string msg, Client client, Server server) {
			return null;
		}

	}
}
