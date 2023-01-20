using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using FWIConnection;
using System.Security.Policy;
using FWIConnection.Message;

namespace FWIClient
{
    internal class ClientManager
    {
        bool hasPrivillegeTrace;
        PrivillegeElevationResult? privillegeTraceResult;
        public Socket? ServerSocket { get; set; }

        public ClientManager()
        {
            privillegeTraceResult = null;
            hasPrivillegeTrace = false;
        }

        public PrivillegeElevationResult RequestPrivillegeTrace()
        {
            if (ServerSocket == null) return new MustDeniedPrivillegeElevationResult("내부 요청 거절: 연결되지 않음");
            if (hasPrivillegeTrace) return new MustDeniedPrivillegeElevationResult("내부 요청 거절: 권한이 존재");

            var nonce = Nonce();
            privillegeTraceResult?.Deny("중복 요청, 오래된 요청을 만료함");
            privillegeTraceResult = new()
            {
                Progress = PrivilegeElevation.Requested,
                Nonce = nonce,
            };

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.RequestPrivillegeTrace);
            bw.Write(nonce);
            ServerSocket.Send(bw.ToBytes());

            return privillegeTraceResult;
        }

        static short Nonce()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            return (short)rand.Next(0, short.MaxValue);
        }

        public void ElevateUpdatePrivillege(short nonce, bool accepted)
        {
            var result = privillegeTraceResult;

            if (result is null) return;
            else if (result.Progress != PrivilegeElevation.Requested) return;
            else if (result.Nonce != nonce) return;
            
            if (accepted)
            {
                hasPrivillegeTrace = true;
                result.Accept();
            }
            else result.Deny("Request Denied");

            privillegeTraceResult = null;
        }

        public void SendAFK()
        {
            if (ServerSocket == null) return;

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.SetAFK);
            bw.WriteDateTime(DateTime.Now);

            ServerSocket.Send(bw.ToBytes());
        }

        public void SendNoAFK()
        {
            if (ServerSocket == null) return;

            var bw = new ByteWriter();
            bw.Write((short)MessageOp.SetNoAFK);
            bw.WriteDateTime(DateTime.Now);

            ServerSocket.Send(bw.ToBytes());
        }
    }
}
