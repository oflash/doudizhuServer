using Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Servers
{
	class Message //: IAsyncResult
	{

		/// <summary>
		/// 数组长度
		/// </summary>
		public int Length => data.Length;

		/// <summary>
		/// 字节数组, 包括4个字节的存储数据长度
		/// </summary>
		public byte[] Data => data;
		private byte[] data;


		/* Message对象一经创建, this.Data就一定占有内存 */
		#region 构造函数
		/// <summary>
		/// 实例化一个没有内容的Message容器(用于接收消息, 可能一次接收多个消息)
		/// </summary>
		/// <param name="len"></param>
		public Message(int len) {
			this.data = new byte[len];
		}

		/// <summary>
		/// 实例化一个需要发送的消息(用于发送消息, 一次发送一个消息)
		/// </summary>
		/// <param name="content"></param>
		public Message(string msg) {
			this.data = GetMessageBytes(msg);
		}
		#endregion


		/// <summary>
		/// 获取自身Message.Data(单个消息)的字符串(发送消息前)
		/// </summary>
		/// <returns></returns>
		public string GetMessageString() {
			// if (size <= 4) return null;
			int len = BitConverter.ToInt32(data, 0);        // len不包括自身

			string msg = Encoding.UTF8.GetString(data, 4, len);
			return msg;
		}

		/// <summary>
		/// 解析Message byte[], (接收消息后)
		/// </summary>
		/// <param name="offset">偏移量</param>
		/// <param name="size">有效数据长度</param>
		/// <returns>黏包数据切割结果</returns>
		public string[] GetMessageStrings(int offset, int size) {
			if (size <= 4) return null;

			List<string> msgs = new List<string>();

			int len;
			for (int i = offset; i < size && i < data.Length; i += 4 + len) {
				len = BitConverter.ToInt32(data, i);

				string msg = Encoding.UTF8.GetString(data, i + 4, len);
				msgs.Add(msg);
			}
			return msgs.ToArray();
		}


		/// <summary>
		/// 字符串数据, 转化成byte[], (发送消息前)
		/// </summary>
		/// <param name="actionCode"></param>
		/// <param name="msg"></param>
		/// <returns></returns>
		public byte[] GetMessageBytes(string msg) {
			byte[] data = Encoding.UTF8.GetBytes(msg);
			byte[] len = BitConverter.GetBytes(data.Length);    // 不包括自己的数据长度
																//Console.WriteLine(data.Length);
			byte[] res = len.Concat(data).ToArray();


			return res;
		}
	}
}
