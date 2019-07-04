using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ClientWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void Printer(string data);
        private delegate void Cleaner();
        Printer printer;
        Cleaner cleaner;
        private Socket serverSocket;
        private Thread clientThread;
        private string serverHost;
        private int serverPort;
        public MainWindow()
        {
            InitializeComponent();

            printer = new Printer(Print);
            cleaner = new Cleaner(ClearChat);
        }
        
        private void Listener()
        {
            while (serverSocket.Connected)
            {
                byte[] buffer = new byte[8196];
                int bytesRec = serverSocket.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                if (data.Contains("#updatechat"))
                {
                    UpdateChat(data);
                    continue;
                }
            }
        }
        private void Connect()
        {
            serverHost = "localhost";
            serverPort = 12345;
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(serverHost);
                IPAddress ipAddress = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, serverPort);
                serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Connect(ipEndPoint);
            }
            catch { Print("Сервер недоступен!"); }

            clientThread = new Thread(Listener);
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        private void ClearChat()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(cleaner);
                return;
            }
            chatTextBox.Clear();
        }
        private void UpdateChat(string data)
        {
            ClearChat();
            string[] messages = data.Split('&')[1].Split('|');
            int countMessages = messages.Length;
            if (countMessages <= 0) return;
            for (int i = 0; i < countMessages; i++)
            {
                try
                {
                    if (string.IsNullOrEmpty(messages[i])) continue;
                    Print(String.Format("[{0}]: {1}.", messages[i].Split('~')[0], messages[i].Split('~')[1]));
                }
                catch { continue; }
            }
        }
        private void Send(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int bytesSent = serverSocket.Send(buffer);
            }
            catch { Print("Связь с сервером прервалась..."); }
        }
        private void Print(string msg)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(printer, msg);
                return;
            }
            if (chatTextBox.Text.Length == 0)
                chatTextBox.AppendText(msg);
            else
                chatTextBox.AppendText(Environment.NewLine + msg);
        }

        private void SendMessage()
        {
            try
            {
                string data = messageTextBox.Text;
                if (string.IsNullOrEmpty(data)) return;
                Send("#newmsg&" + data);
                messageTextBox.Text = string.Empty;
            }
            catch { MessageBox.Show("Ошибка при отправке сообщения!"); }
        }
        
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            Connect();
            string Name = nickTextBox.Text;
            if (string.IsNullOrEmpty(Name)) return;
            Send("#setname&" + Name);
            chatTextBox.IsEnabled = true;
            messageTextBox.IsEnabled = true;
            sendButton.IsEnabled = true;
            nickTextBox.IsEnabled = false;
            signInButton.IsEnabled = false;
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(messageTextBox.Text))
                SendMessage();
        }

        private void ClearChatButton_Click(object sender, RoutedEventArgs e)
        {
            ClearChat();
        }
    }
}
