using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	[Serializable]
	public class PlayerInfo
	{
		public string id;      // 主键, 所有ContentType类都有这一个变量
		public string name;    //
		public int header;      // 头像索引
		public bool sex;
		public int index;       // 玩家进入房间后设置的编号-1, 0, 1, 2		->	-1表示没有加入房间
		public int room_num;    // 房间号

		public object other;       // 其他附属信息

		public PlayerInfo(string id, string name, bool sex) {
			this.id = id;
			this.name = name;
			this.sex = sex;
		}

		public override string ToString() {
			//return base.ToString();
			return "id" + id + "\tname:" + name + "\tsex:" + sex + "\tindex" + index + "\tother:" + other;
		}
	}
}
