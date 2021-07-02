using System;

namespace Server
{
	[Serializable]
	public class ServerData
	{
		public string ip;
		public string name;
		public int maxPlayers;
		public int players;
		public string version;
		public string password;
	}
}