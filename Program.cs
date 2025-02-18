using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine($"Invalid number arguments.");
                return;
            }

            if (int.TryParse(args[0], out int port))
            {
                if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    Console.Error.WriteLine($"Invalid port number: {port}");
                    return;
                }

                await ListenAsync(port);
            }
            else
            {
                Console.Error.WriteLine($"Could not parse argument to integer value.");
            }
        }

        private static async Task ListenAsync(int port)
        {
            TcpListener listener = new(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"Listening on all interfaces on port: {port}.");

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");

                    using TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Connected to: {client.Client.RemoteEndPoint}");

                    using NetworkStream stream = client.GetStream();

                    // send welcome message
                    byte[] welcome = Encoding.Default.GetBytes("Welcome to EchoServer.\r\n");
                    await stream.WriteAsync(welcome.AsMemory(0, welcome.Length));

                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    StringBuilder messageBuilder = new StringBuilder();

                    // loop to receive all data from client
                    while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length))) != 0)
                    {
                        string data = Encoding.Default.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received {bytesRead} bytes: \"{data.ToControlCodeString()}\".");

                        // Handle Telnet commands (IAC sequences)
                        if (buffer[0] == 255) // IAC (Interpret As Command)
                        {
                            HandleTelnetCommand(buffer, bytesRead);
                            continue;
                        }

                        // Check for Ctrl+x (ASCII 24)
                        if (data.Contains('\u0018'))
                        {
                            Console.WriteLine("Received Ctrl+x. Disconnecting client.");
                            break;
                        }

                        messageBuilder.Append(data);

                        // Check if the message contains the newline character
                        if (data.Contains("\n"))
                        {
                            string completeMessage = messageBuilder.ToString();

                            // Check for the 'quit' keyword
                            if (completeMessage.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Received 'quit' command. Disconnecting client.");
                                break;
                            }

                            byte[] response = Encoding.Default.GetBytes(completeMessage);
                            await stream.WriteAsync(response.AsMemory(0, response.Length));
                            Console.WriteLine("Sent response.");

                            // Clear the message builder for the next message
                            messageBuilder.Clear();
                        }
                    }

                    // end connection
                    Console.WriteLine("Closing connection.");
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private static void HandleTelnetCommand(byte[] buffer, int bytesRead)
        {
            // Basic handling of Telnet commands (IAC sequences)
            // This is a simplified example and may need to be expanded for full Telnet support
            for (int i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 255) // IAC
                {
                    if (i + 2 < bytesRead)
                    {
                        byte command = buffer[i + 1];
                        byte option = buffer[i + 2];
                        Console.WriteLine($"Received Telnet command: IAC {command} {option}");
                        i += 2; // Skip the command and option bytes
                    }
                }
            }
        }
    }

}
