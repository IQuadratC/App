using System;
using Newtonsoft.Json.Linq;
using SharedFiles.Utility;
using UnityEngine;

namespace Server
{
	public static class ClientDatabase
	{
		public static void UpdateServers()
        {
	        Database.instance.AddGetAction((json) =>
            {
	            try
	            {
		            Client.instance.serverDatas.Clear();
		            JObject jo = JObject.Parse(json);
		            JToken jServers = jo;
		            foreach (JToken jServer in jServers)
		            {
			            ServerData serverData = JsonUtility.FromJson<ServerData>(jServer.First.First.First.ToString());
			            Client.instance.serverDatas.Add(serverData);
		            }
	            }
	            catch (Exception e)
	            {
		            Console.WriteLine("No Servers online");
	            }
            });
            
            Database.instance.Request(Database.instance.firebase.Child("Servers"));
        }
	}
}