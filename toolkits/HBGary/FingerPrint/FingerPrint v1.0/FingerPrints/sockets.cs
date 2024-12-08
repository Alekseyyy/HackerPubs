//------------------------------------------------------------------------
//
// Copyright © 2010 HBGary, Inc.  All Rights Reserved. 
//
//------------------------------------------------------------------------

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
        override public bool OnEvaluateStringSet(Dictionary<string, bool> stringSet, ScanResultCollection results)
        {
            if ((stringSet.ContainsKey("WSASocket".ToLower())) ||
                (stringSet.ContainsKey("WSASend".ToLower())) ||
                (stringSet.ContainsKey("WSARecv".ToLower())) ||
                (stringSet.ContainsKey("WSAConnect".ToLower())) ||
                (stringSet.ContainsKey("WSAIoctl".ToLower())) ||
                (stringSet.ContainsKey("WSAConnect".ToLower())))
            {
                results.AppendResult("Winsock", "WSA", 1, 0);
            }
            else if ((stringSet.ContainsKey("socket".ToLower())) ||
                (stringSet.ContainsKey("send".ToLower())) ||
                (stringSet.ContainsKey("recv".ToLower())) ||
                (stringSet.ContainsKey("connect".ToLower())) ||
                (stringSet.ContainsKey("ioctlsocket".ToLower())) ||
                (stringSet.ContainsKey("closesocket".ToLower())))
            {
                results.AppendResult("Winsock", "Generic", 1, 0);
            }
            else if (stringSet.ContainsKey("getpeername".ToLower()))
            {
                results.AppendResult("Host Query", "Peer", 1, 0);
            }
            else if (stringSet.ContainsKey("gethostbyname".ToLower()))
            {
                results.AppendResult("Host Query", "ByName", 1, 0);
            }
            else if (stringSet.ContainsKey("gethostbyaddr".ToLower()))
            {
                results.AppendResult("Host Query", "ByAddr", 1, 0);
            }
            else if ((stringSet.ContainsKey("inet_addr".ToLower())) ||
                     (stringSet.ContainsKey("inet_ntoa".ToLower())) ||
                     (stringSet.ContainsKey("htons".ToLower())) ||
                     (stringSet.ContainsKey("htonl".ToLower())))
            {
                results.AppendResult("Winsock address conversion", "yes", 1, 0);
            }
            else if ((stringSet.ContainsKey("WSAEnumNetworkEvents".ToLower())) ||
                     (stringSet.ContainsKey("WSAAsync".ToLower())) ||
                     (stringSet.ContainsKey("WSAEnumNameSpaceProviders".ToLower())))
            {
                results.AppendResult("Advanced WSA Winsock", "yes", 1, 0);
            }

            return true;
        }
    }
}
