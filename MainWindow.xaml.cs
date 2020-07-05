using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Net;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
using System.Collections.ObjectModel;
using System.IO;

namespace LocalNetworkChat
{

    public partial class MainWindow : Window
    {
        static ObservableCollection<string> a = new ObservableCollection<string>();
        static string remoteAddress; 
        static int remotePort;
        static int localPort; 
        /*public string cmd(string command)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "C:\\C.bat";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            return output;
        }*/
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenChat(object sender, RoutedEventArgs e)
        {
            a.Clear();
            localPort = Int32.Parse(Vport.Text);
            remotePort =Int32.Parse(Iport.Text);
            remoteAddress = IP.Text;
           
            //здесь потом надо загрузить старый чат
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
        }
        private void SendMessage(object sender, RoutedEventArgs e)
        {
            UdpClient send = new UdpClient(); // создаем UdpClient для отправки сообщений
            try
            {
                    string message = msg.Text; // сообщение для отправки
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    send.Send(data, data.Length, remoteAddress, remotePort); // отправка
                    string writePath = @"\" + remoteAddress + @".txt";
                    using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                        sw.WriteLine(message);
                    a.Add(message);
                    ListofMessages.ItemsSource = a;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                send.Close();
            }
        }

        private  void ReceiveMessage()
        {
            UdpClient receiver = new UdpClient(localPort); // UdpClient для получения данных
            IPEndPoint remoteIp = null; // адрес входящего подключения
            try
            {
                while (true)
                {
                    byte[] data = receiver.Receive(ref remoteIp); // получаем данные
                    string message = Encoding.Unicode.GetString(data);
                    string writePath = @"\" + remoteAddress + @".txt";
                    using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                        sw.WriteLine(message);
                    a.Add(message);                
                    ListofMessages.ItemsSource = a;
                    //Console.WriteLine("Собеседник: {0}", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                receiver.Close();
            }
        }
    }
}