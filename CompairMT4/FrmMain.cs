using GoldStar.Lib.Helper;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using YuriNET.CoreServer.Http;
using CompairMT4.model;
using System.Collections.Generic;

namespace TextOrder {
    public partial class FrmMain : Form {

        private HttpController controller;
        int serverID = 1;

        public FrmMain() {
            InitializeComponent();

        }

        private void btnStart_Click(object sender, EventArgs e) {
            serverID = 1;
            BtnStop.Enabled = BtnStart.Enabled;
            BtnStart.Enabled = !BtnStop.Enabled;
            new Thread(() => startOperations()).Start();


        }

        private void btnStop_Click(object sender, EventArgs e) {

            BtnStop.Enabled = BtnStart.Enabled;
            BtnStart.Enabled = !BtnStop.Enabled;
            stop();
        }

        private void startOperations() {
            // Initizlial Controller

            int port;
            if (!int.TryParse(TxtPort.Text, out port)) {
                MessageBox.Show("Error port number !", Info.getProductName());
                return;
            }

            controller = new HttpController(port);
            controller.OnUpdateMaster += Controller_OnUpdateMaster;
            controller.StartListen();
        }

        private void Controller_OnUpdateMaster(object sender, UpdateMasterEvenArgs e) {
            // Prevent Cross-thread error
            if (InvokeRequired) {
                Invoke(new Action(() => Controller_OnUpdateMaster(sender, e)));
                return;
            }

            FxData fxData = e.Model;

            // Find exist server name
            List<DataGridViewRow> findRows = DgvData.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.Cells["serverName"].Value.ToString().Equals(fxData.ServerName))
            .ToList();
            
            if (findRows != null && findRows.Count > 0) {
                // Found exist server
                // Console.WriteLine("findRows : {0}", findRows);

                // Find exist symbol
                DataGridViewRow row = findRows
                .Where(r => r.Cells["symbol"].Value.ToString() == fxData.Symbol)
                .FirstOrDefault();
                
                if (row != null) {
                    // Found exist one then update values
                    row.Cells["bid"].Value = fxData.Bid;
                    row.Cells["ask"].Value = fxData.Ask;
                } else {
                    // else add new row
                    DgvData.Rows.Add(serverID,fxData.ServerName, fxData.Symbol, fxData.Bid, fxData.Ask);
                    serverID++;
                }

            } else {
                // Add new row
                DgvData.Rows.Add(serverID,fxData.ServerName, fxData.Symbol, fxData.Bid, fxData.Ask);
                serverID++;
            }
            Compair();

        }

        private void Compair() {
            double ms, cs, total,ask1,ask2,bid1,bid2;
            double tmp = 99999;
            foreach (DataGridViewRow row in DgvData.Rows) {
                foreach (DataGridViewRow row2 in DgvData.Rows) {
                    if (row.Cells["serverName"].Value.ToString() != row2.Cells["serverName"].Value.ToString()) {
                        ask1 = double.Parse(row.Cells["ask"].Value.ToString());
                        ask2 = double.Parse(row2.Cells["ask"].Value.ToString());
                        bid1 = double.Parse(row.Cells["bid"].Value.ToString());
                        bid2 = double.Parse(row2.Cells["bid"].Value.ToString());

                        ms = ask1 - bid2;
                        cs = Math.Abs(bid1 - ask1);
                        total = ms + cs;
                        if (total < tmp) {
                            tmp = total;
                            lb_min.Text = String.Format("{0:F5}", ms) + " [ASK: " + String.Format("{0:F5}", ask1) + " - BID: " +
                                          String.Format("{0:F5}", bid2) + "]";
                            lb_count.Text = String.Format("{0:F5}", cs) + " [BID: " + String.Format("{0:F5}", bid1) + " - ASK: " +
                                          String.Format("{0:F5}", ask2) + "]";
                            lb_Total.Text = String.Format("{0:F5}", total) + " [" + row.Cells["serverName"].Value.ToString() + "/" +
                                          row2.Cells["serverName"].Value.ToString() + "]";
                        }

                        addCompair(row.Cells["serverName"].Value.ToString(), row2.Cells["serverName"].Value.ToString(), ms, cs, total);
                    }
                }
                //More code here
            }


        }
        private void addCompair(string name1,string name2,double ms,double cs,double total) {
            // Find exist server name
            List<DataGridViewRow> findRows = DgvCompair.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.Cells["SV1"].Value.ToString().Equals(name1))
            .ToList();

            if (findRows != null && findRows.Count > 0) {
                // Found exist server
                // Console.WriteLine("findRows : {0}", findRows);

                // Find exist symbol
                DataGridViewRow row = findRows
                .Where(r => r.Cells["SV2"].Value.ToString() == name2)
                .FirstOrDefault();

                if (row != null){ 
                    if (total < double.Parse(row.Cells["TOTAL"].Value.ToString())) {
                        // Found exist one then update values
                        row.Cells["MS"].Value = String.Format("{0:F5}", ms);
                        row.Cells["CS"].Value = String.Format("{0:F5}", cs);
                        row.Cells["TOTAL"].Value = String.Format("{0:F5}", total);
                    }
                } else {
                    // else add new row
                    DgvCompair.Rows.Add(String.Format("{0:F5}", ms), name1, name2, String.Format("{0:F5}", cs), String.Format("{0:F5}", total));
                }

            } else {
                // Add new row
                DgvCompair.Rows.Add(String.Format("{0:F5}", ms), name1, name2, String.Format("{0:F5}", cs), String.Format("{0:F5}", total));
            }
        }

        private void stop() {
            //controller.StopMaster();
            controller.Stop();
        }

        public void setData(int time, int order) {

        }

        public void RefreshSlaves() {
            if (InvokeRequired) {
                Invoke(new Action(() => RefreshSlaves()));
                return;
            }

        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e) {
            BtnStop.PerformClick();
        }

    }
}
