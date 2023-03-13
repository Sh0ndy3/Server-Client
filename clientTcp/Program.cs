using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace clientTcp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Получение ip-Адреса

            Console.Write("Введите ip-адрес: ");
            string ip = Console.ReadLine();
            Console.Clear();
            int port = 8080;

            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port); // Точка для подключение

            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Параметры настройки сети(сокета)

            tcpSocket.Connect(tcpEndPoint); // Подключение к точке

            while (true)
            {
                //Кодировка и отправка сообщения
                Console.Write("Напишите сообщение: ");
                string message = Console.ReadLine().ToString();

                byte[] data = Encoding.Unicode.GetBytes(message);

                try
                {
                    tcpSocket.Send(data);
                }
                catch
                {
                    Console.WriteLine("Сервер закрыт администротором!");
                    CloseClientConnect(tcpSocket);
                    break;
                }

                // Получение ответа от сервера

                byte[] buffer = new byte[256];
                int size;
                StringBuilder answer = new StringBuilder();


                do
                {
                    size = tcpSocket.Receive(buffer);
                    answer.Append(Encoding.Unicode.GetString(buffer, 0, size));
                }
                while (tcpSocket.Available > 0);



                Console.WriteLine(answer.ToString());

                // Проверка на точку выхода

                if (answer.ToString() == "Досвидания.")
                {
                    CloseClientConnect(tcpSocket);
                    break;
                }
            }
        }

        private static void CloseClientConnect(Socket tcpSocket) // Отключение от сокета
        {
                Console.ReadKey();
                tcpSocket.Shutdown(SocketShutdown.Both);
                tcpSocket.Close();
        }
    }
}