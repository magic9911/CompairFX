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

        public FrmMain() {
            InitializeComponent();

        }

        private void btnStart_Click(object sender, EventArgs e) {

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
                    DgvData.Rows.Add(fxData.ServerName, fxData.Symbol, fxData.Bid, fxData.Ask);
                }

            } else {
                // Add new row
                DgvData.Rows.Add(fxData.ServerName, fxData.Symbol, fxData.Bid, fxData.Ask);
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
