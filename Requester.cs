using System;
using System.Collections.Generic;
using System.Net;
using MSC.Hash;
using System.Text.RegularExpressions;
using System.Drawing;

namespace MSC.Brute
{
    public class RequestManage
    {
        public WebHeaderCollection Headers { set; get; }
        public string SourcePage { set; get; }
        public CookieContainer Cookies { set; get; }
        public int StatusCode { set; get; }
        public string CookiesString { set; get; }
        public bool ErrorAst { get; set; }
        public string Location { get; set; }
        public string RedirectedUrl { set; get; }
        public byte[] Bytes { set; get; }
        public Image Image { set; get; }
        public override string ToString()
        {
            return string.Format("Headers: {0}\nStatusCode: {1}\nCookies: {2}\nErrorAst: {3}\nLocation: {4}\nRedirectedUrl: {5}", Headers.ToString(), StatusCode.ToString(), CookiesString, ErrorAst.ToString(), Location, RedirectedUrl);
        }
    }

    public class Token
    {
        public string RegexPattern { set; get; }
        public List<string> GrpValue { set; get; }
        public MatchCollection Matchs { get; set; }
        public RequestManage Manage { set; get; }
    }


    public class Proxy
    {
        public string Ip { set; get; }
        public int Port { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public override string ToString()
        {
            return string.Format("{0}:{1}@{2};{3}", Ip, Port.ToString(), Username, Password);
        }
    }

    public class Account
    {
        public string Username { set; get; }
        public string Password { set; get; }
        public string Capture { set; get; }
        public bool Hit { set; get; }
        public override string ToString()
        {
            return string.Format("{0}:{1}", Username, Password);
        }
    }

    public class Requester
    {
        public Logger logger = new Logger();
        Requests Rs = new Requests();
        public void SetLogger(Logger logg, bool SetOnCore = false)
        {
            logger = logg;
            if (SetOnCore)
                Rs.SetLogger(logger);
        }
        public Logger GetCoreLogger()
        {
            return Rs.GetLogger();
        }
        public Logger GetBaseLogger()
        {
            return logger;
        }

        /// <summary>
		/// GetToken in page for Secound request (List Token)
		/// </summary>
        /// <param name="Tokken">Token List array for get all value grupe in request.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public List<Token> GetToken(List<Token> Tokken, Config config, RequestManage manage = null, Proxy Proxy = null)
        {
            config.Method = Method.GET;
            logger.AddMessage("Method config automatic seted on GET", Log.Type.Infomation);

            RequestManage man = GETData(config, manage, Proxy);

            List<Token> end = new List<Token>();
            foreach (Token tok in Tokken)
            {
                Token tik = new Token();
                tik = tok;


                Match match = Regex.Match(man.SourcePage, tik.RegexPattern);
                tik.Matchs = Regex.Matches(man.SourcePage, tik.RegexPattern);
                tik.GrpValue = new List<string>();
                for (int i = 1; i <= match.Groups.Count; i++)
                {
                    if (match.Groups[i].Success)
                        tik.GrpValue.Add(match.Groups[i].Value.ToString());
                }
                logger.AddMessage("Found " + match.Groups.Count.ToString() + " Groups in Regexing.", Log.Type.Infomation);

                tik.Manage = man;

                logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + man.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(man.Cookies, config) + "\n\nHeaders:\n" + man.Headers.ToString(), Log.Type.OutPut);

                end.Add(tik);
            }
            return end;
        }

        /// <summary>
		/// GetToken in page for Secound request (One Token)
		/// </summary>
        /// /// <param name="Tokken">Token for get all value grupe in request.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public Token GetToken(Token Tokken, Config config, RequestManage manage = null, Proxy Proxy = null)
        {
            Token end = new Token();
            end = Tokken;
            config.Method = Method.GET;
            logger.AddMessage("Method config automatic seted on GET", Log.Type.Infomation);

            RequestManage man = GETData(config, manage, Proxy);

            Match match = Regex.Match(man.SourcePage, Tokken.RegexPattern);
            end.Matchs = Regex.Matches(man.SourcePage, Tokken.RegexPattern);
            end.GrpValue = new List<string>();
            for (int i = 1; i <= match.Groups.Count; i++)
            {
                if (match.Groups[i].Success)
                    end.GrpValue.Add(match.Groups[i].Value.ToString());
            }
            logger.AddMessage("Found " + match.Groups.Count.ToString() + " Groups in Regexing.", Log.Type.Infomation);

            end.Manage = man;

            logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + man.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(man.Cookies, config) + "\n\nHeaders:\n" + man.Headers.ToString(), Log.Type.OutPut);

            return end;
        }
        /// <summary>
		/// GetToken in page for Secound request (One Value)
		/// </summary>
        /// <param name="RegexP">Regex Pattenrn for get request value.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public RequestManage GetToken(string RegexP, Config config, RequestManage manage = null, Proxy Proxt = null)
        {
            config.Method = Method.GET;
            logger.AddMessage("Method config automatic seted on GET", Log.Type.Infomation);

            RequestManage get = GETData(config, manage, Proxt);

            Match match = Regex.Match(get.SourcePage, RegexP);
            logger.AddMessage("Found " + match.Groups.Count.ToString() + " Groups in Regexing.", Log.Type.Infomation);

            RequestManage end = new RequestManage();
            end.SourcePage = match.Groups[1].ToString();
            end.Cookies = get.Cookies;
            end.Headers = get.Headers;
            logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(end.Cookies, config) + "\n\nHeaders:\n" + end.Headers.ToString(), Log.Type.OutPut);

            return end;
        }
        /// <summary>
		/// Check account and set capture by your setting in params
		/// </summary>
        /// <param name="ac">Account for check.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        /// <param name="Caper">Capture array list for get capture account.</param>
        public Account CheckAccount(Config config, Account ac, Capture[] Caper, RequestManage manage = null, Proxy proxy = null)
        {
            RequestManage Get = new RequestManage();
            if (config.Method == Method.POST)
                Get = POSTData(config, manage, proxy);
            else if (config.Method == Method.GET)
                Get = GETData(config, manage, proxy);

            ac.Hit = isHit(config, Get.SourcePage);

            if (Caper != null)
                ac = CaptureAccount(Get.SourcePage, Caper, ac, config, manage, proxy);
            logger.AddMessage("Account:\nUsername=" + ac.Username + "\nPassword=" + ac.Password + "\nHit=" + ac.Hit + "\nCapture=" + ac.Capture, Log.Type.OutPut);
            return ac;
        }

        /// <summary>
		/// Check account for Value
		/// </summary>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public bool CheckAccount(Config config, RequestManage Manage = null, Proxy proxy = null)
        {
            RequestManage Get = new RequestManage();
            if (config.Method == Method.POST)
                Get = POSTData(config, Manage, proxy);
            else if (config.Method == Method.GET)
                Get = GETData(config, Manage, proxy);

            bool End = isHit(config, Get.SourcePage);
            logger.AddMessage("Hit=" + End.ToString(), Log.Type.OutPut);

            return End;
            
        }

        /// <summary>
		/// Normal PostData
		/// </summary>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public RequestManage POSTData(Config config, RequestManage Manage = null, Proxy proxy = null)
        {
            config.Method = Method.POST;
            logger.AddMessage("Method config automatic seted on POST", Log.Type.Infomation);

            RequestManage Get = Rs.GetRequestByData(config, Manage, proxy);
            return Get;
        }

        public RequestManage GETBytes(Config config, Proxy proxy = null, bool GetImage = false)
        {
            return Rs.GetBytesRequest(config, proxy, GetImage);
        }

        /// <summary>
		/// Normal GETData
		/// </summary>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        public RequestManage GETData(Config config, RequestManage Manage = null, Proxy proxy = null)
        {
            config.Method = Method.GET;
            logger.AddMessage("Method config automatic seted on GET", Log.Type.Infomation);

            RequestManage Get = Rs.GetPageSource(config, Manage, proxy);
            return Get;
        }

        /// <summary>
		/// Repleace username and password account in source whit Varible seeting in config
		/// </summary>
        /// <param name="ac">Account for replace in source.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="source">The base text for replace account into it.</param>
        public string ReplaceAccount(Account ac, string source, Config config)
        {
            logger.AddMessage("Account:\nUsername=" + ac.Username + "\nPassword=" + ac.Password + "\nHit=" + ac.Hit + "\nCapture=" + ac.Capture, Log.Type.OutPut);

            if (config.Varible.Count != 0)
            {
                foreach (HashType Item in config.Varible)
                {
                    switch (Item)
                    {
                        case HashType.MD5:
                            ac.Password = Hashs.MD5(ac.Password);
                            logger.AddMessage("Password changed to MD5 : " + ac.Password, Log.Type.Infomation);
                            break;
                        case HashType.SHA1:
                            ac.Password = Hashs.SHA1(ac.Password);
                            logger.AddMessage("Password changed to SHA1 : " + ac.Password, Log.Type.Infomation);
                            break;
                        case HashType.SHA256:
                            ac.Password = Hashs.SHA256(ac.Password);
                            logger.AddMessage("Password changed to SHA256 : " + ac.Password, Log.Type.Infomation);
                            break;
                        case HashType.SHA512:
                            ac.Password = Hashs.SHA512(ac.Password);
                            logger.AddMessage("Password changed to SHA512 : " + ac.Password, Log.Type.Infomation);
                            break;
                        case HashType.Base64:
                            ac.Password = Hashs.StringToBase64(ac.Password);
                            logger.AddMessage("Password changed to Base64 : " + ac.Password, Log.Type.Infomation);
                            break;
                    }
                }
            }
            try {
                string[] userpass = config.DataSet.Split('*');
                source = source.Replace(userpass[0], ac.Username).Replace(userpass[1], ac.Password);
            }
            catch(Exception ex) { logger.AddMessage("Error: " + ex.Message, Log.Type.Error); }
            logger.AddMessage("OutPut Source: " + source, Log.Type.OutPut);
            return source;
        }

        /// <summary>
		/// Capture account whit setting
		/// </summary>
        /// <param name="Account">Account for capture.</param>
        /// <param name="config">Config for Request.</param>
        /// <param name="manage">Keep your request by cookies.</param>
        /// <param name="Proxy">Send request by proxy service.</param>
        /// <param name="Capture">Capture array list for get capture account.</param>
        /// <param name="source">The source page for search values in Regex Capture.</param>
        public Account CaptureAccount(string source, Capture[] Capture, Account Account, Config config, RequestManage manage = null, Proxy Proxy = null)
        {
            RequestManage manage2 = new RequestManage();
            string end = "";
            manage2.SourcePage = source;

            foreach (Capture cap in Capture)
            {
                manage2.SourcePage = source;
                config.LoginURL = cap.Redirect;
                if (cap.Redirect != null)
                {
                    if (cap.UseCookies)
                    {
                        if (config.Method == Method.GET)
                            manage2 = GETData(config, manage, Proxy);
                        else if (config.Method == Method.POST)
                        {
                            manage2 = POSTData(config, manage, Proxy);
                        }
                    }
                    else
                    {
                        if (config.Method == Method.GET)
                            manage2 = GETData(config, null, Proxy);
                        else if (config.Method == Method.POST)
                        {
                            manage2 = POSTData(config, null, Proxy);
                        }
                    }
                }
                if (cap.RemoveLines)
                    manage2.SourcePage = source.Replace("\n", "");
                if (cap.RemoveSpaseChars)
                    manage2.SourcePage = source.Replace(" ", "");
                if (cap.Remove2SpaseChars)
                    manage2.SourcePage = source.Replace("  ", "");

                MatchCollection match = Regex.Matches(manage2.SourcePage, cap.Regex);
                for (int i = 0; i < match.Count; i++)
                    if (i < cap.MaxMatchs)
                        for (int j = 1; j <= match[i].Groups.Count; j++)
                            if (match[i].Groups[j].Success)
                                end += cap.Lable[i] + match[i].Groups[j].ToString() + " # ";
                end += " | ";
            }
            Account.Capture = end;
            logger.AddMessage("Account:\nUsername=" + Account.Username + "\nPassword=" + Account.Password + "\nHit=" + Account.Hit + "\nCapture=" + Account.Capture, Log.Type.OutPut);

            return Account;
        }

        public bool isHit(Config config, string SourcePage)
        {
            bool flag = false;
            bool End;
            if (!flag)
            {
                End = SourcePage.Contains(config.SourceSuccess);
                if (config.Contains)
                {
                    if (End)
                        flag = true;
                    else flag = false;
                }
                else
                {
                    if (!End)
                        flag = true;
                    else flag = false;
                }
            }
            return flag;
        }

    }
}
