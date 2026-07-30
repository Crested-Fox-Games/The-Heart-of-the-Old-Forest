using System.Net;
using System.Net.Sockets;

public static class NetworkUtility
{
    /// <summary>
    /// Gets the local ip address of the client
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIpAddress()
    {
        string localIP = "127.0.0.1";

        //Gets all network addresses assigned to the computer
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach(var ip in host.AddressList)
        {
            //Filters for IPv4 addresses
            if(ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        return localIP;
    }
}
