using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace neverland.aliyun.ddns
{
    public sealed class IPHelper
    {
        public static async Task<string> GetNetworkIPv4(CancellationToken cancelllationToken = new CancellationToken())
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Contracts.QUERY_IPADDRESS_URL);

                //不使用这个是因为有使用限制,https://ip-api.com/docs/api:json
                //var query = await client.GetFromJsonAsync<IPModelResult>(Contracts.QUERY_IPADDRESS_RESOURCE, cancelllationToken)
                //     .ConfigureAwait(false);

                using var response = await client.GetAsync(Contracts.QUERY_IPADDRESS_RESOURCE, cancelllationToken)
                     .ConfigureAwait(false);
                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                //检查受限情况
                var ri = response.Headers.FirstOrDefault(it => it.Key == Contracts.QUERY_IPADDRESS_HEADER_RI).Value;
                if (ri != null)
                {
                    if (ri.ElementAt(0) == "0")
                    {
                        var ttl = response.Headers.FirstOrDefault(it => it.Key == Contracts.QUERY_IPADDRESS_HEADER_TTL).Value;
                        Console.WriteLine($"ip地址查询受限,等待{ttl.ElementAt(0)}秒后重试");
                        return string.Empty;
                    }
                }
                var jsonResponse = await response.Content.ReadFromJsonAsync<IPModelResult>();
                if (jsonResponse != null)
                {
                    if (jsonResponse.Status != null && jsonResponse.Status == "success") return jsonResponse.Query!;
                }
            }
            return string.Empty;
        }

        public static void Search(string server="")
        {
            // Define a regular expression to parse user's input.
            // This is a security check. It allows only
            // alphanumeric input string between 2 to 40 character long.
            Regex rex = new Regex(@"^[a-zA-Z]\w{1,39}$");

            if (string.IsNullOrEmpty(server))
            {
                // If no server name is passed as an argument to this program, use the current
                // server name as default.
                server = Dns.GetHostName();
                Console.WriteLine("Using current host: " + server);
            }
            else
            {
                if (!(rex.Match(server)).Success)
                {
                    Console.WriteLine("Input string format not allowed.");
                    return;
                }
            }

            // Get the list of the addresses associated with the requested server.
            IPAddresses(server);

            // Get additional address information.
            IPAddressAdditionalInfo();
        }

        /**
       * The IPAddresses method obtains the selected server IP address information.
       * It then displays the type of address family supported by the server and its
       * IP address in standard and byte format.
       **/
        private static void IPAddresses(string server)
        {
            try
            {
                ASCIIEncoding ASCII = new ASCIIEncoding();

                // Get server related information.
                IPHostEntry heserver = Dns.GetHostEntry(server);

                // Loop on the AddressList
                foreach (IPAddress curAdd in heserver.AddressList)
                {
                    // Display the type of address family supported by the server. If the
                    // server is IPv6-enabled this value is: InterNetworkV6. If the server
                    // is also IPv4-enabled there will be an additional value of InterNetwork.
                    Console.WriteLine("AddressFamily: " + curAdd.AddressFamily.ToString());

                    // Display the ScopeId property in case of IPV6 addresses.
                    if (curAdd.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                        Console.WriteLine("Scope Id: " + curAdd.ScopeId.ToString());


                    // Display the server IP address in the standard format. In
                    // IPv4 the format will be dotted-quad notation, in IPv6 it will be
                    // in in colon-hexadecimal notation.
                    Console.WriteLine("Address: " + curAdd.ToString());

                    // Display the server IP address in byte format.
                    Console.Write("AddressBytes: ");

                    Byte[] bytes = curAdd.GetAddressBytes();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        Console.Write(bytes[i]);
                    }

                    Console.WriteLine("\r\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[DoResolve] Exception: " + e.ToString());
            }
        }

        // This IPAddressAdditionalInfo displays additional server address information.
        private static void IPAddressAdditionalInfo()
        {
            try
            {
                // Display the flags that show if the server supports IPv4 or IPv6
                // address schemas.
                Console.WriteLine("\r\nSupportsIPv4: " + Socket.OSSupportsIPv4);
                Console.WriteLine("SupportsIPv6: " + Socket.OSSupportsIPv6);

                if (Socket.OSSupportsIPv6)
                {
                    // Display the server Any address. This IP address indicates that the server
                    // should listen for client activity on all network interfaces.
                    Console.WriteLine("\r\nIPv6Any: " + IPAddress.IPv6Any.ToString());

                    // Display the server loopback address.
                    Console.WriteLine("IPv6Loopback: " + IPAddress.IPv6Loopback.ToString());

                    // Used during autoconfiguration first phase.
                    Console.WriteLine("IPv6None: " + IPAddress.IPv6None.ToString());

                    Console.WriteLine("IsLoopback(IPv6Loopback): " + IPAddress.IsLoopback(IPAddress.IPv6Loopback));
                }
                Console.WriteLine("IsLoopback(Loopback): " + IPAddress.IsLoopback(IPAddress.Loopback));
            }
            catch (Exception e)
            {
                Console.WriteLine("[IPAddresses] Exception: " + e.ToString());
            }
        }
    }
}
