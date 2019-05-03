using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace InterparkChecker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeControls();
        }

        private void InitializeControls()
        {
            // camplist
            var campList = Enum.GetValues(typeof(CampName)).Cast<CampName>();
            foreach (var type in campList)
            {
                clbCamp.Items.Add(type.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckProcessInterpark();
        }

        private void CheckProcessInterpark()
        {
            string flag = "UseCheckIn";
            string goodsCode = "18007398";
            string placeCode = "18000660";
            string playDate = "20190505";
            string callback = "fnPlayDateChangeCallBack";
            
            string dateSelectionUrl = @"http://ticket.interpark.com/Ticket/Goods/GoodsInfoJSON.asp?Flag={0}&GoodsCode={1}&PlaceCode={2}&PlayDate={3}&Callback={4}";
            dateSelectionUrl = string.Format(dateSelectionUrl, flag, goodsCode, placeCode, playDate, callback);

            flag = "RemainSeat";
            callback = "fnPlaySeqChangeCallBack";
            string remainCountUrl = @"http://ticket.interpark.com/Ticket/Goods/GoodsInfoJSON.asp?Flag=RemainSeat&GoodsCode=18007398&PlaceCode=18000660&PlaySeqList=321&CampingYN=Y&Callback=";
            dateSelectionUrl = string.Format(dateSelectionUrl, flag, goodsCode, placeCode, playDate, callback);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string url = "http://forest.maketicket.co.kr/camp/reserve/calendar.jsp";
            List<Querystring> paramList = new List<Querystring>();
            paramList.Add(new Querystring("idkey", "5M4240"));
            paramList.Add(new Querystring("gd_seq", "GD84"));
            paramList.Add(new Querystring("yyyymmdd", "20190524"));
            paramList.Add(new Querystring("sd_date", "20190524"));
            var resultText = Scrapper.Scraping(url, Method.PORT, paramList);

            MessageBox.Show(resultText);
               
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WebBrowser web = new WebBrowser();
            var directory = Application.StartupPath;
            Uri uri = new Uri(string.Format("file:///{0}/html/maketicket.html", directory));
            web.Url = uri;
            var text = web.DocumentText;
            MessageBox.Show(text);

            var doc = web.Document;
            var element = doc.GetElementById("calendar");
            var calendar = element.InnerText;
            MessageBox.Show(calendar);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            CallInterparkCalendar();
            CallInterparkSelectDate();
        }

        private void CallInterparkCalendar()
        {
         //   /Ticket/Goods/ifrCalendar.asp?GoodsCode=18007398&PlaceCode=18000660&OnlyDeliver=68006&DBDay=12&ExpressDelyDay=0&YM=201905
            string url = "http://ticket.interpark.com/Ticket/Goods/ifrCalendar.asp";
            List<Querystring> paramList = new List<Querystring>();
            paramList.Add(new Querystring("GoodsCode", "18007398"));
            paramList.Add(new Querystring("PlaceCode", "18000660"));
            paramList.Add(new Querystring("OnlyDeliver", "68006"));
            paramList.Add(new Querystring("DBDay", "12"));
            paramList.Add(new Querystring("ExpressDelyDay", "0"));
            paramList.Add(new Querystring("YM", "201905"));
            
            var result = System.Threading.Tasks.Task.Run<string>(async () => await Scrapper.RequestHttpClient(url, Method.GET, paramList));
            result.Wait();
            var text = result.Result;
        }

        private void CallInterparkSelectDate()
        {
            string url = "http://ticket.interpark.com/Ticket/Goods/GoodsInfoJSON.asp";
            List<Querystring> paramList = new List<Querystring>();
            paramList.Add(new Querystring("Flag", "UseCheckIn"));
            paramList.Add(new Querystring("GoodsCode", "18007398"));
            paramList.Add(new Querystring("PlaceCode", "18000660"));
            paramList.Add(new Querystring("PlayDate", "20190505"));
            paramList.Add(new Querystring("Callback", "fnPlayDateChangeCallBack"));
            
            var result = System.Threading.Tasks.Task.Run<string>(async () => await Scrapper.RequestHttpClient(url, Method.GET, paramList));
            result.Wait();
            var text = result.Result;

        }


        private void button5_Click(object sender, EventArgs e)
        {
            _keepruning = true;
            backgroundWorker.RunWorkerAsync();
        }

        //
        bool _keepruning = true;

        private void StartSearch()
        {
            try
            {
                while (_keepruning)
                {
                    // datelist
                    List<string> dateList = new List<string>();
                    foreach (var date in lbDates.Items)
                    {
                        dateList.Add(date.ToString());
                    }

                    // checkbox
                    foreach (var check in clbCamp.CheckedItems)
                    {
                        var site = check.ToString();
                        CampName campName;
                        Enum.TryParse(site, out campName);

                        foreach (var date in dateList) // 날짜별 표시를 위해
                        {
                            var resultList = MainProcessor.CheckProcessor(campName, new List<string> { date });

                            DisplayTextBox(string.Format(">> {0} [{1}] ----", check.ToString(), date));
                            if (resultList.Any(x => x.RemainCount == 0))
                            {
                                DisplayTextBox("---- FULL !! " + Environment.NewLine);
                            }
                            else
                            {
                                // 예약가능알림
                                DisplayTextBox(MakeString(resultList));
                                AlertUsableSite(resultList);
                            }
                            
                        }
                    }

                    int interval = Int32.Parse(tbInterval.Text);
                    Thread.Sleep(interval * 1000);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AlertUsableSite(List<ResultSet> resultList)
        {
            if (resultList.Any(x => x.RemainCount > 0))
            {
                var usableList = resultList.Where(x => x.RemainCount > 0).ToList();

                MessageBox.Show(MakeString(usableList));
            }
        }

        private void DisplayTextBox(string displayText)
        {
            if (rtbResult.InvokeRequired)
            {
                rtbResult.BeginInvoke(new Action(() => rtbResult.Text += displayText));
                rtbResult.BeginInvoke(new Action(() => rtbResult.ScrollToCaret()));
            }
            else
            {
                rtbResult.Text += displayText;
                rtbResult.ScrollToCaret();
            }
        }

        private string MakeString(List<ResultSet> resultText)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var data in resultText)
            {
                sb.AppendLine((string.Format("{0} : {1}", data.SiteName, data.RemainCount)));
            }

            return sb.ToString();
        }

        private void btnAddDate_Click(object sender, EventArgs e)
        {
            string selectedDate = dateSelector.Value.Date.ToString("yyyyMMdd");

            bool isExists = false;
            foreach (var date in lbDates.Items)
            {
                if (selectedDate == date.ToString())
                    isExists = true;
            }

            if (! isExists)
                lbDates.Items.Add(selectedDate);
        }

        private void btnDeleteDate_Click(object sender, EventArgs e)
        {
            var deleteIndex = lbDates.SelectedIndex;
            if (deleteIndex > -1)
                lbDates.Items.RemoveAt(deleteIndex);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StartSearch();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            _keepruning = false;
        }

        #region notifyIcon 설정

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();

            notifyIcon.Visible = true;

        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;

            this.Dispose();

            Application.Exit();
            Application.ExitThread();
            Environment.Exit(0);
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.ShowInTaskbar = true;

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal; // 최소화를 멈춘다 

            this.Activate(); // 폼을 활성화 시킨다

            this.notifyIcon.Visible = false;
        }

        #endregion
    }
}
