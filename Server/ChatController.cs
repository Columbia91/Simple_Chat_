using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class ChatController
    {
        private const int maxMessage = 100;
        public static List<Message> сhat = new List<Message>();
        public struct Message
        {
            public string userName;
            public string data;
            public Message(string name, string msg)
            {
                userName = name;
                data = msg;
            }
        }
        public static void AddMessage(string userName, string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(msg)) return;
                int countMessages = сhat.Count;
                if (countMessages > maxMessage) ClearChat();
                Message newMessage = new Message(userName, msg);
                сhat.Add(newMessage);
                Console.WriteLine("Новое сообщение от {0}.", userName);
                Server.UpdateAllChats();
            }
            catch (Exception exp) { Console.WriteLine("Произошла ошибка при добавлении сообщения: {0}.", exp.Message); }
        }
        public static void ClearChat()
        {
            сhat.Clear();
        }
        public static string GetChat()
        {
            try
            {
                string data = "#updatechat&";
                int countMessages = сhat.Count;
                if (countMessages <= 0) return string.Empty;
                for (int i = 0; i < countMessages; i++)
                {
                    data += String.Format("{0}~{1}|", сhat[i].userName, сhat[i].data);
                }
                return data;
            }
            catch (Exception exp) { Console.WriteLine("Произошла ошибка при получении данных чата: {0}", exp.Message); return string.Empty; }
        }
    }
}
