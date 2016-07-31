using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompairMT4.model {
    public class FxData {

        public FxData() {

        }

        public FxData(string serverName, string symbol, string bid, string ask) {
            this.ServerName = serverName;
            this.Symbol = symbol;
            this.Bid = bid;
            this.Ask = ask;
        }

        public string ServerName { get; set; }
        public string Symbol { get; set; }
        public string Bid { get; set; }
        public string Ask { get; set; }

    }
}
