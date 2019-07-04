using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Server
    {
        public static List<Client> clients = new List<Client>();
        public static void NewClient(Socket handle)
        {
            try
            {
                Client newClient = new Client(handle);
                clients.Add(newClient);
                Console.WriteLine("Новый участник подключился: {0}", handle.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Произошла ошибка при добавлении нового участника: {0}.", exp.Message); }
        }
        public static void EndClient(Client client)
        {
            try
            {
                client.End();
                clients.Remove(client);
                Console.WriteLine("Участник {0} вышел из сети.", client.UserName);
            }
            catch (Exception exp) { Console.WriteLine("Произошла ошибка при выходе участника: {0}.", exp.Message); }
        }
        public static void UpdateAllChats()
        {
            try
            {
                int countUsers = clients.Count;
                for (int i = 0; i < countUsers; i++)
                {
                    clients[i].UpdateChat();
                }
            }
            catch (Exception exp) { Console.WriteLine("Произошла ошибка при обновлении чата: {0}.", exp.Message); }
        }
    }
}
