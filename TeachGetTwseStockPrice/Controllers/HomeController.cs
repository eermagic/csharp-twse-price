using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using static TeachGetTwseStockPrice.Models.HomeModel;

namespace TeachGetTwseStockPrice.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        /// <summary>
        /// 即時股價
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        public ActionResult GetRealtimePrice(GetRealtimePriceIn inModel)
        {
            GetRealtimePriceOut outModel = new GetRealtimePriceOut();

            // 檢查輸入參數
            if (string.IsNullOrEmpty(inModel.Sample1_Symbol))
            {
                outModel.ErrMsg = "請輸入股票代碼";
                return Json(outModel);
            }

            StringBuilder ExCode = new StringBuilder();
            string[] symbols = inModel.Sample1_Symbol.Split(',');
            foreach (string symbol in symbols)
            {
                ExCode.Append("tse_" + symbol + ".tw|");
            }

            // 呼叫網址
            string url = "https://mis.twse.com.tw/stock/api/getStockInfo.jsp";
            url += "?json=1&delay=0&ex_ch=" + ExCode;

            string downloadedData = "";
            using (WebClient wClient = new WebClient())
            {
                // 取得網頁資料
                wClient.Encoding = Encoding.UTF8;
                downloadedData = wClient.DownloadString(url);
            }
            TwsePriceSchema jsonPrice = null;
            if (downloadedData.Trim().Length > 0)
            {
                jsonPrice = JsonConvert.DeserializeObject<TwsePriceSchema>(downloadedData);
                if (jsonPrice.rtcode != "0000")
                {
                    throw new Exception("取商品價格失敗: " + jsonPrice.rtmessage);
                }
            }

            StringBuilder sbRealPrice = new StringBuilder();
            for (int i = 0; i < jsonPrice.msgArray.Count; i++)
            {
                // 代碼
                string code = jsonPrice.msgArray[i].c;

                // z = 收盤價
                string close = jsonPrice.msgArray[i].z;
                // a = 最低委賣價
                string ask = "";
                if (jsonPrice.msgArray[i].a.IndexOf("_") > -1)
                {
                    ask = jsonPrice.msgArray[i].a.Split('_')[0];
                }
                // b = 最高委買價
                string bid = "";
                if (jsonPrice.msgArray[i].b.IndexOf("_") > -1)
                {
                    bid = jsonPrice.msgArray[i].b.Split('_')[0];
                }
                sbRealPrice.Append("代碼: " + code + " 收盤價: " + close + " 最低委賣價: " + ask + " 最高委買價: " + bid + "<br>");
            }
            outModel.realPrice = sbRealPrice.ToString();

            // 回傳 Json 給前端
            return Json(outModel);
        }

        /// <summary>
        /// 每日收盤行情
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        public ActionResult GetDayPrice(GetDayPriceIn inModel)
        {
            GetDayPriceOut outModel = new GetDayPriceOut();

            // 檢查輸入參數
            if (string.IsNullOrEmpty(inModel.Sample2_Date))
            {
                outModel.ErrMsg = "請輸入日期";
                return Json(outModel);
            }

            // 呼叫網址
            string twseUrl = "https://www.twse.com.tw/exchangeReport/MI_INDEX";
            string download_url = twseUrl + "?response=csv&date=" + inModel.Sample2_Date + "&type=ALL";
            string downloadedData = "";
            using (WebClient wClient = new WebClient())
            {
                // 網頁回傳
                downloadedData = wClient.DownloadString(download_url);
            }
            if (downloadedData.Trim().Length > 0)
            {
                // 回傳前端的資料集
                outModel.gridList = new List<StockPriceRow>();
                string[] lineStrs = downloadedData.Split('\n');
                for (int i = 0; i < lineStrs.Length; i++)
                {
                    string strline = lineStrs[i];
                    if (strline.Trim().Length == 0)
                    {
                        continue;
                    }

                    // 排除非價格部份
                    if (strline.IndexOf("證券代號") > -1 || strline.IndexOf("(元,股)") > -1)
                    {
                        continue;
                    }
                    if (strline.Substring(0, 1) == "=")
                    {
                        strline = strline.TrimStart('=');
                    }

                    ArrayList resultLine = new ArrayList();
                    // 解析資料
                    this.ParseCSVData(resultLine, strline);
                    string[] datas = (string[])resultLine.ToArray(typeof(string));

                    //檢查資料內容
                    if (datas.Length != 17)
                    {
                        continue;
                    }

                    // 股票代碼
                    string symbolCode = datas[0];

                    if (symbolCode.Length == 4)
                    {
                        // 輸出資料
                        StockPriceRow row = new StockPriceRow();
                        row.symbolCode = symbolCode; //股票代碼 
                        row.symbolName = datas[1]; //股票名稱
                        row.open = datas[5]; //開盤價
                        row.high = datas[6]; //最高價
                        row.low = datas[7]; //最低價
                        row.close = datas[8]; //收盤價
                        row.volume = datas[2]; //成交量
                        outModel.gridList.Add(row);
                    }

                }
            }

            // 回傳 Json 給前端
            return Json(outModel);
        }

        /// <summary>
        /// 當月各日成交資訊
        /// </summary>
        /// <param name="inModel"></param>
        /// <returns></returns>
        public ActionResult GetMonthPrice(GetMonthPriceIn inModel)
        {
            GetMonthPriceOut outModel = new GetMonthPriceOut();

            // 檢查輸入參數
            if (string.IsNullOrEmpty(inModel.Sample3_Symbol))
            {
                outModel.ErrMsg = "請輸入股票代碼";
                return Json(outModel);
            }

            if (string.IsNullOrEmpty(inModel.Sample3_Date))
            {
                outModel.ErrMsg = "請輸入日期";
                return Json(outModel);
            }

            // 呼叫網址
            string download_url = "http://www.twse.com.tw/exchangeReport/STOCK_DAY?response=csv&date=" + inModel.Sample3_Date + "&stockNo=" + inModel.Sample3_Symbol;
            string downloadedData = "";
            using (WebClient wClient = new WebClient())
            {
                // 網頁回傳
                downloadedData = wClient.DownloadString(download_url);
            }
            if (downloadedData.Trim().Length > 0)
            {
                outModel.gridList = new List<StockPriceRow>();
                string[] lineStrs = downloadedData.Split('\n');
                for (int i = 0; i < lineStrs.Length; i++)
                {
                    string strline = lineStrs[i];
                    if (i == 0 || i == 1 || strline.Trim().Length == 0)
                    {
                        continue;
                    }
                    // 排除非價格部份
                    if (strline.IndexOf("說明") > -1 || strline.IndexOf("符號") > -1 || strline.IndexOf("統計") > -1 || strline.IndexOf("ETF") > -1)
                    {
                        continue;
                    }

                    ArrayList resultLine = new ArrayList();
                    // 解析資料
                    this.ParseCSVData(resultLine, strline);
                    string[] datas = (string[])resultLine.ToArray(typeof(string));

                    //檢查資料內容
                    if (Convert.ToInt32(datas[1].Replace(",", "")) == 0 || datas[3] == "--" || datas[4] == "--" || datas[5] == "--" || datas[6] == "--")
                    {
                        continue;
                    }

                    // 輸出資料
                    StockPriceRow row = new StockPriceRow();
                    row.date = datas[0]; //日期
                    row.open = datas[3]; //開盤價
                    row.high = datas[4]; //最高價
                    row.low = datas[5]; //最低價
                    row.close = datas[6]; //收盤價
                    row.volume = datas[1]; //成交量
                    outModel.gridList.Add(row);

                }
            }

            // 回傳 Json 給前端
            return Json(outModel);
        }

        private void ParseCSVData(ArrayList result, string data)
        {
            int position = -1;
            while (position < data.Length)
                result.Add(ParseCSVField(ref data, ref position));
        }

        private string ParseCSVField(ref string data, ref int StartSeperatorPos)
        {
            if (StartSeperatorPos == data.Length - 1)
            {
                StartSeperatorPos++;
                return "";
            }

            int fromPos = StartSeperatorPos + 1;
            if (data[fromPos] == '"')
            {
                int nextSingleQuote = GetSingleQuote(data, fromPos + 1);
                StartSeperatorPos = nextSingleQuote + 1;
                string tempString = data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1);
                tempString = tempString.Replace("'", "''");
                return tempString.Replace("\"\"", "\"");
            }

            int nextComma = data.IndexOf(',', fromPos);
            if (nextComma == -1)
            {
                StartSeperatorPos = data.Length;
                return data.Substring(fromPos);
            }
            else
            {
                StartSeperatorPos = nextComma;
                return data.Substring(fromPos, nextComma - fromPos);
            }
        }

        private int GetSingleQuote(string data, int SFrom)
        {
            int i = SFrom - 1;
            while (++i < data.Length)
                if (data[i] == '"')
                {
                    if (i < data.Length - 1 && data[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }
                    else
                        return i;
                }
            return -1;
        }
    }
}