using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class sockets : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if ((theString.Contains("WSASocket")) ||
                (theString.Contains("WSASend")) ||
                (theString.Contains("WSARecv")) ||
                (theString.Contains("WSAConnect")) ||
                (theString.Contains("WSAIoctl")) ||
                (theString.Contains("WSAConnect")))
            {
                results.AppendResult("Winsock", "WSA", 1, 0);
            }
            else if ((theString.Contains("socket")) ||
                (theString.Contains("send")) ||
                (theString.Contains("recv")) ||
                (theString.Contains("connect")) ||
                (theString.Contains("ioctlsocket")) ||
                (theString.Contains("closesocket")))
            {
                results.AppendResult("Winsock", "Generic", 1, 0);
            }
            else if (theString.Contains("getpeername"))
            {
                results.AppendResult("Host Query", "Peer", 1, 0);
            }
            else if (theString.Contains("gethostbyname"))
            {
                results.AppendResult("Host Query", "ByName", 1, 0);
            }
            else if (theString.Contains("gethostbyaddr"))
            {
                results.AppendResult("Host Query", "ByAddr", 1, 0);
            }
            else if ((theString.Contains("inet_addr")) ||
                     (theString.Contains("inet_ntoa")) ||
                     (theString.Contains("htons")) ||
                     (theString.Contains("htonl")))
            {
                results.AppendResult("Winsock address conversion", "yes", 1, 0);
            }
            else if ((theString.Contains("WSAEnumNetworkEvents")) ||
                     (theString.Contains("WSAAsync")) ||
                     (theString.Contains("WSAEnumNameSpaceProviders")))
            {
                results.AppendResult("Advanced WSA Winsock", "yes", 1, 0);
            }

            return true;
        }
    }
}
