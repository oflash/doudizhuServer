using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common
{

	[Serializable]
	public class Content
	{
		#region HeadTable
		//[JsonConverter(typeof(StringEnumConverter))]
		// [NonSerialized]
		public RequestCode requestCode;             // 请求的类

		// public string requestCode;

		//[JsonConverter(typeof(StringEnumConverter))]
		// [NonSerialized]
		public ActionCode actionCode;               // 请求的方法

		// public string actionCode;

		//[JsonConverter(typeof(StringEnumConverter))]
		// [NonSerialized]
		public ContentType contentType;        // 传递的字符串的类型(便于解析数据)

		//[JsonConverter(typeof(StringEnumConverter))]
		// [NonSerialized]
		public ReturnCode returnCode;        // 返回请求结果
		#endregion

		public string id;                   // 如果需要知道是哪个客户端发来的消息

		public string content = "None";     // 一串json格式的字符串

		//[JsonConverter(typeof(StringEnumConverter))]
		public SendTo sendTo;

		// public int id = -1;
		// public bool sex;
		// public int coin;
		// public List<int> list = new List<int>();    // 卡牌id数组

		//[JsonConstructor]
		public Content() { }


		/// <summary>
		/// 客户端发送请求时使用
		/// </summary>
		/// <param name="eContentType"></param>
		/// <param name="eActionCode"></param>
		/// <param name="eRequestCode"></param>
		/// <param name="content"></param>
		public Content(ContentType contentType = ContentType.Default, ActionCode actionCode = ActionCode.None,
			RequestCode requestCode = RequestCode.None, string content = "None") {
			this.contentType = contentType;
			this.actionCode = actionCode;
			this.requestCode = requestCode;
			this.content = content;
		}

		/// <summary>
		/// 用于服务器端发送响应
		/// </summary>
		/// <param name="returnCode"></param>
		/// <param name="actionCode"></param>
		/// <param name="contentType"></param>
		public Content(ReturnCode returnCode, ActionCode actionCode = ActionCode.None,
		  ContentType contentType = ContentType.Default, SendTo sendTo = SendTo.Everything, string content = "None") {
			this.returnCode = returnCode;
			this.actionCode = actionCode;
			this.contentType = contentType;
			this.sendTo = sendTo;
			this.content = content;
		}

		public override string ToString() {
			// return base.ToString();
			return Newtonsoft.Json.JsonConvert.SerializeObject(this);
		}


		/// <summary>
		/// Content对象序列化
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static byte[] GetBytes(Content content) {
			// string json = JsonUtility.ToJson(content);
			string json = JsonConvert.SerializeObject(content);
			byte[] data = Encoding.UTF8.GetBytes(json);
			byte[] len = BitConverter.GetBytes(data.Length);    // 不包括自己的数据长度

			byte[] res = len.Concat(data).ToArray();

			return res;
		}
	}
}
