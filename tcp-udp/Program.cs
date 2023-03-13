using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace serverTcp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[1].ToString(); // Получение IP-Aдреса
            Console.WriteLine("Адрес подключения - " + ip);
            Console.WriteLine("Нажмите Escape, чтобы закрыть сервер." + "\n");
            int port = 8080;
            ConsoleKeyInfo pressedKey;

            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); // Точка для подключение

            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Параметры настройки сети

            tcpSocket.Bind(tcpEndPoint); // Связывание сокета с точкой
            tcpSocket.Listen(10); // Прослушивание сокета


            Task.Run(() => // Поток для проверки отключения сервера
            {
                while (true)
                {
                    pressedKey = Console.ReadKey();
                    if (pressedKey.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("\nСервер закрыт");
                        Process.GetCurrentProcess().Kill();
                    }
                }
            });

            Socket listener = tcpSocket.Accept();
            int exist = 1;

            while (true)
            {
                if (exist == 0)
                { 
                    Console.Clear();
                    Console.WriteLine("Адрес подключения - " + ip);
                    Console.WriteLine("Нажмите Escape, чтобы закрыть сервер." + "\n");
                    listener = tcpSocket.Accept();
                    exist = 1;
                }
                byte[] buffer = new byte[256];
                int size;
                StringBuilder data = new StringBuilder();

                do                 // Получение сообщения от клиента
                {
                    size = listener.Receive(buffer);
                    data.Append(Encoding.Unicode.GetString(buffer, 0, size));
                }
                while (listener.Available > 0);

                Console.WriteLine("Введено: " + data);

                if (data.ToString().ToLower() == "пока") // Проверка на точку выхода
                {
                    exist--;
                    listener.Send(Encoding.Unicode.GetBytes("Досвидания."));
                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
                else
                    listener.Send(Encoding.Unicode.GetBytes("Успешно получено сообщение: " + data)); // Отправка ответа клиенту                   
            }
        }
    }   
}