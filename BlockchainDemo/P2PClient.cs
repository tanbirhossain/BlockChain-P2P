using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;

namespace BlockchainDemo
{
    public class P2PClient
    {
        IDictionary<string, WebSocket> wsDict = new Dictionary<string, WebSocket>();

        /// <summary>
        /// 1. client trying to connect to server . 
        /// 2. ws.Connect(); -- call and connect with server
        /// 3. ws.Send("Hi Server");　 -- send message to server
        /// 4. P2PServer.Send("Hi Client"); -- after getting  message from client -> server send message to client Send("Hi Client")
        /// 
        /// 5.client again send message to -> server with  all transaction and  server also send client to all transaction >>>
        /// *** so basically when two client connected each other firstly they share there all transactions.
        ///     when they recive others transactions they at first they validate the whole transactions
        ///     after that they synched the this thransactrio
        ///     
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void Connect(string url)
        {
            if (!wsDict.ContainsKey(url))
            {
                WebSocket ws = new WebSocket(url);
                ws.OnMessage += (sender, e) =>
                {
                    if (e.Data == "Hi Client")
                    {
                        Console.WriteLine(e.Data);
                    }
                    else
                    {
                        Blockchain newChain = JsonConvert.DeserializeObject<Blockchain>(e.Data);
                        if (newChain.IsValid() && newChain.Chain.Count > Program.PhillyCoin.Chain.Count)
                        {
                            List<Transaction> newTransactions = new List<Transaction>();
                            newTransactions.AddRange(newChain.PendingTransactions);
                            newTransactions.AddRange(Program.PhillyCoin.PendingTransactions);

                            newChain.PendingTransactions = newTransactions;
                            Program.PhillyCoin = newChain;
                        }
                    }
                };
                
                ws.Connect();
                ws.Send("Hi Server");　
                ws.Send(JsonConvert.SerializeObject(Program.PhillyCoin));
                wsDict.Add(url, ws);
            }
        }

        public void Send(string url, string data)
        {
            foreach (var item in wsDict)
            {
                if (item.Key == url)
                {
                    item.Value.Send(data);
                }
            }
        }
        
        public void Broadcast(string data)
        {
            foreach (var item in wsDict)
            {
                item.Value.Send(data);
            }
        }

        public IList<string> GetServers()
        {
            IList<string> servers = new List<string>();
            foreach (var item in wsDict)
            {
                servers.Add(item.Key);
            }
            return servers;
        }

        public void Close()
        {
            foreach (var item in wsDict)
            {
                item.Value.Close();
            }
        }
    }
}
