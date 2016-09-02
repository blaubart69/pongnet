/*
 * --------------------------------------------------------------------------------------------------------
 * "THE BEER-WARE LICENSE" (Revision 42):
 * <bernhard.spindler.75@gmail.com> schrieb diese Datei. Solange Sie diesen Vermerk nicht entfernen, können
 * Sie mit dem Material machen, was Sie möchten. Wenn wir uns eines Tages treffen und Sie
 * denken, das Material ist es wert, können Sie mir dafür ein Bier ausgeben. Bernhard Spindler
 * --------------------------------------------------------------------------------------------------------
 */
using System;
using System.Net;
using System.Net.Sockets;

namespace pongnet
{
    class Pong
    {
        static void PrintUsage()
        {
            Console.WriteLine("usage: {0} {{host|ip}} {{port}}",
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
        }
        static int CheckArgs(string[] args, out string HostOrIp, out uint port)
        {
            HostOrIp = args[0];
            port = 0;
            if (args.Length != 2)
            {
                PrintUsage();
                return 4;
            }
            if (!uint.TryParse(args[1], out port))
            {
                Console.Error.WriteLine("Value [{0}] for port is wrong.", args[1]);
                return 6;
            }
            return 0;
        }
        static int Main(string[] args)
        {
            int rc = 99;
            try
            {
                
                string HostOrIp;
                uint Port;

                if ((rc = CheckArgs(args, out HostOrIp, out Port)) != 0)
                {
                    return rc;
                }
                IPAddress[] IPs;
                try
                {
                    IPs = Dns.GetHostAddresses(HostOrIp);
                }
                catch (SocketException sex)
                {
                    Console.Error.WriteLine(sex.Message);
                    return 8;
                }
                if (IPs.Length > 1)
                {
                    Console.WriteLine("found [{1}] DNS records. using first one.");
                }
                
                try
                {
                    using (var sock = new System.Net.Sockets.Socket(IPs[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp))
                    {
                        sock.Connect(IPs[0], (int)Port);
                    }
                    rc = 0;
                    Console.WriteLine("tcp connect successfull to IP [{0}]", IPs[0]);
                }
                catch (SocketException sex)
                {
                    rc = 10;
                    Console.Error.WriteLine("could not connect to host. [{0}]", sex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            return rc;
        }
    }
}
