using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TelnetServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 23;
            var server = new TcpListener(IPAddress.Any, port);

            server.Start();

            Console.WriteLine("Servidor BBS iniciado na porta {0}", port);

            while (true)
            {
                var client = server.AcceptTcpClient();
                var clientThread = new Thread(() => HandleClient(client));

                clientThread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            var bytesRead = 0;

            var stream = client.GetStream();

            var buffer = new byte[1024];
            
            buffer.Initialize();

            var sendBytes = Encoding.ASCII.GetBytes("\n\r\n\r----------------------------\n\rBBS do Flavio H M Bergamini! \n\rComo posso ajudar? ");
            
            stream.Write(sendBytes, 0, sendBytes.Length);

            while (true)
            {
                var command = "";

                while (!command.Contains("\r"))
                {
                    bytesRead += stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    command += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                }
                command = command.Replace("m\0", "");
                command = command.Replace("\0", "");
                Console.WriteLine("\n\rComando enviado pelo Cliente: {0}", command);

                Console.Write("\n\rResponda: ");
                var send = Console.ReadLine();

                var response = $"\n\rComando enviado pelo Servidor: {send}\n\r Responda: ";

                var responseBytes = Encoding.ASCII.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
                //Console.Write(send);
            }

            stream.Close();
            client.Close();
        }
    }
}