using SharedFiles.Utility;
using UnityEngine;

public static class ServerDatabase
{
	public static void UpdateServer()
	{
		ThreadManager.ExecuteOnMainThread(() =>
		{
			DeleteServer();

			ServerData serverData = new ServerData()
			{
				ip = Server.ip,
				name = GameManager.instance.name,
				maxPlayers = Server.MaxPlayers,
				players = GameManager.instance.players.Count,
				version = Application.version
			};
			
			Database.instance.Push(JsonUtility.ToJson(serverData), Database.instance.firebase.Child("Servers").Child(GameManager.instance.name));
		});
	}
    
	public static  void DeleteServer()
	{
		Database.instance.Delete(Database.instance.firebase.Child("Servers").Child(GameManager.instance.name));
	}
}