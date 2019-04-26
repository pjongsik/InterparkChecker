using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InterparkChecker
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
            
            string url = "http://forest.maketicket.co.kr/camp/reserve/calendar.jsp";
            List<Querystring> paramList = new List<Querystring>();
            paramList.Add(new Querystring("idkey", "5M4240"));
            paramList.Add(new Querystring("gd_seq", "GD84"));
            paramList.Add(new Querystring("yyyymmdd", "20190524"));
            paramList.Add(new Querystring("sd_date", "20190524"));
            var resultText = Scrapper.RequestUrlMsdn(url, Method.PORT, paramList);

            MessageBox.Show(resultText);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var resultText = MainProcessor.CheckProcessor(CampName.용자휴, new List<string>() { "20190505" });
                MessageBox.Show(resultText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
