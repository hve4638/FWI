using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FWIClient
{
    public class CancellationTokenManager
    {
        readonly Dictionary<string, CancellationTokenSource> tokens;

        public CancellationTokenManager()
        {
            tokens = new Dictionary<string, CancellationTokenSource>();
        }

        public CancellationTokenSource MakeNewToken(string key)
        {
            var cancelToken = new CancellationTokenSource();
            Update(key, cancelToken);

            return cancelToken;
        }

        void Update(string key, CancellationTokenSource source)
        {
            if (tokens.TryGetValue(key, out CancellationTokenSource? old))
            {
                tokens.Remove(key);
                old.Cancel();
            }

            tokens.Add(key, source);
        }

        public void Clear()
        {
            foreach (var token in tokens.Values)
            {
                token.Cancel();
            }
            tokens.Clear();
        }

        public CancellationTokenSource this[string key]
        {
            get
            {
                if (tokens.TryGetValue(key, out CancellationTokenSource? token)) return token;
                else return MakeNewToken(key);
            }
        }
    }
}
