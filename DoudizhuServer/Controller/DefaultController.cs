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
	class DefaultController : BaseController
	{
		public DefaultController(ManagerController mngController) {
			this.mngController = mngController;
		}


		/// <summary>
		/// 服务器发送提示给客户端
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="client"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public Content Prompt(object obj, Client client, Server server) {
			PromptInfo promptInfo = JsonConvert.DeserializeObject<PromptInfo>(obj.ToString());

			// 发送提示信息
			Content send = new Content(ReturnCode.Success, ActionCode.Prompt, ContentType.Prompt, promptInfo.sendTo, promptInfo.info);
			return send;
		}
	}
}