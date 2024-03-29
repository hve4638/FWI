﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using FWIConnection;
using FWI.Results;
using System.Security.Policy;
using FWI;
using FWI.Message;


namespace FWIClient
{
    public class ClientSender
    {
        static readonly Random nonceGenerater = new(DateTime.Now.Millisecond);
        readonly Client client;
        public bool Connected { get; set; }
        public bool IsTarget { get; set; }
        public bool IsAFK { get; set; }
        public bool DebugMode { get; set; }

        public ClientSender(Client client)
        {
            this.client = client;
        }
        
        public static int GetNonce()
        {
            return nonceGenerater.Next(1, int.MaxValue);
        }

        public void Send(ISerializableMessage serializableMessage)
        {
            var bytes = serializableMessage.Serialize(debug: DebugMode);

            client.Send(bytes);
        }

        public void SendMessage(string text)
        {
            var message = new TextMessage()
            {
                Text = text,
            };

            Send(message);
        }

        public void SendEcho(string text)
        {
            var message = new EchoMessage()
            {
                Text = text,
            };

            Send(message);
        }

        public void SendWI(WindowInfoLegacy wi)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else
            {
                IsAFK = false;

                var message = new UpdateWIMessage()
                {
                    Date = wi.Date,
                    Name = wi.Name,
                    Title = wi.Title,
                };

                Send(message);
            }
        }

        public void SendAFK(DateTime from, DateTime to)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else if (IsAFK) return;
            else
            {
                var message = new AFKMessage()
                {
                    FromDate = from,
                    ToDate = to,
                };

                IsAFK = true;
                Send(message);
            }
        }

        public void SendNoAFK(DateTime date)
        {
            if (!Connected) return;
            else if (!IsTarget) return;
            else if (!IsAFK) return;
            else
            {
                var message = new NoAFKMessage()
                {
                    FromDate = date,
                    ToDate = date,
                };

                IsAFK = false;
                Send(message);
            }
        }

        public void SendTimelineRequest()
        {
            if (!Connected) return;
            else
            {
                var message = new TimelineRequest();

                Send(message);
            }
        }

        public async Task<TimelineResponse> SendRequestTimeline(DateTime? begin = null, DateTime? end = null)
        {
            if (!Connected) return TimelineResponse.Fail();
            else
            {
                var requestMessage = new TimelineRequest()
                {
                    Id = GetNonce(),
                    BeginDate = begin,
                    EndDate = end,
                };

                Send(requestMessage);

                return TimelineResponse.Fail();
            }
        }

        public void SendRequestRank()
        {
            if (!Connected) return;
            else
            {
                var message = new RequestRankMessage();

                Send(message);
            }
        }

        public void SendServerCall(string command)
        {
            if (!Connected) return;
            else
            {
                var message = new ServerCallMessage()
                {
                    Command = command,
                };

                Send(message);
            }
        }
    }
}
