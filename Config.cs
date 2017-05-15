
using System.Collections.Generic;

namespace MSC
{
    public class Capture
    {
        public string Redirect { set; get; }
        public string Regex { set; get; }
        public bool RemoveLines { set; get; }
        public bool RemoveSpaseChars { set; get; }
        public bool Remove2SpaseChars { set; get; }
        public bool UseCookies { set; get; }
        public string[] Lable;
        public int MaxMatchs { set; get; }
    }
    public class Config
    {
        public int MaxRedirects { set; get; }
        public bool AllowAutoRedirect { set; get; }
        public List<Hash.HashType> Varible = new List<Hash.HashType>();

        public void AddAuthorization(string input)
        {
            Headers += "Authorization: Basic " + Hash.Hashs.StringToBase64(input) + "|";
        }
        public void AddHeader(string input)
        {
            Headers += input + "|";
        }

        public string SourceSuccess
        {
            set;
            get;
        }
        public string DataSet
        {
            set;
            get;
        }
        public string Cookies
        {
            set;
            get;
        }
        public string Headers
        {
            set;
            get;
        }
        public string PostData
        {
            set;
            get;
        }

        public string Referer
        {
            set;
            get;
        }

        public string LoginURL
        {
            set;
            get;
        }

        public string UserAgent
        {
            set;
            get;
        }

        public string ContectType
        {
            set;
            get;
        }

        public bool KeepAlive
        {
            set;
            get;
        }

        public Method Method
        {
            set;
            get;
        }

        public bool Contains
        {
            set;
            get;
        }

    }
    public enum Method
    {
        POST,
        GET,
        PUT
    }
}
