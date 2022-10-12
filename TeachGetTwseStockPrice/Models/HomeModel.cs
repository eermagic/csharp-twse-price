using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TeachGetTwseStockPrice.Models
{
    public class HomeModel
    {
        /// <summary>
        /// [即時股價]參數
        /// </summary>
        public class GetRealtimePriceIn
        {
            public string Sample1_Symbol { get; set; }
        }

        /// <summary>
        /// [即時股價]回傳
        /// </summary>
        public class GetRealtimePriceOut
        {
            public string ErrMsg { get; set; }
            public string realPrice { get; set; }
        }

        /// <summary>
        /// [每日收盤行情]參數
        /// </summary>
        public class GetDayPriceIn
        {
            public string Sample2_Date { get; set; }
        }

        /// <summary>
        /// [每日收盤行情]回傳
        /// </summary>
        public class GetDayPriceOut
        {
            public string ErrMsg { get; set; }
            public List<StockPriceRow> gridList { get; set; }
        }

        /// <summary>
        /// [當月各日成交資訊]參數
        /// </summary>
        public class GetMonthPriceIn
        {
            public string Sample3_Symbol { get; set; }
            public string Sample3_Date { get; set; }
        }

        /// <summary>
        /// [當月各日成交資訊]回傳
        /// </summary>
        public class GetMonthPriceOut
        {
            public string ErrMsg { get; set; }
            public List<StockPriceRow> gridList { get; set; }
        }

        public class StockPriceRow
        {
            public string symbolCode { get; set; }
            public string symbolName { get; set; }
            public string date { get; set; }
            public string open { get; set; }
            public string high { get; set; }
            public string low { get; set; }
            public string close { get; set; }
            public string volume { get; set; }
        }

        public class TwsePriceSchema
        {
            public QueryTime queryTime { get; set; }
            public string referer { get; set; }
            public string rtmessage { get; set; }
            public string exKey { get; set; }
            public IList<MsgArray> msgArray { get; set; }
            public int userDelay { get; set; }
            public string rtcode { get; set; }
            public int cachedAlive { get; set; }
        }

        public class QueryTime
        {
            public int stockInfoItem { get; set; }
            public string sessionKey { get; set; }
            public string sessionStr { get; set; }
            public string sysDate { get; set; }
            public int sessionFromTime { get; set; }
            public int stockInfo { get; set; }
            public bool showChart { get; set; }
            public int sessionLatestTime { get; set; }
            public string sysTime { get; set; }
        }

        public class MsgArray
        {
            public string n { get; set; }
            public string g { get; set; }
            public string u { get; set; }
            public string mt { get; set; }
            public string o { get; set; }
            public string ps { get; set; }
            public string tk0 { get; set; }
            public string a { get; set; }
            public string tlong { get; set; }
            public string t { get; set; }
            public string it { get; set; }
            public string ch { get; set; }
            public string b { get; set; }
            public string f { get; set; }
            public string w { get; set; }
            public string pz { get; set; }
            public string l { get; set; }
            public string c { get; set; }
            public string v { get; set; }
            public string d { get; set; }
            public string tv { get; set; }
            public string tk1 { get; set; }
            public string ts { get; set; }
            public string nf { get; set; }
            public string y { get; set; }
            public string p { get; set; }
            public string i { get; set; }
            public string ip { get; set; }
            public string z { get; set; }
            public string s { get; set; }
            public string h { get; set; }
            public string ex { get; set; }
        }
    }
}