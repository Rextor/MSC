using System;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Security;
using System.Drawing;

namespace MSC.Brute
{
    class Requests
    {
        Logger logger = new Logger();
        public void SetLogger(Logger logg)
        {
            logger = logg;
        }
        public Logger GetLogger()
        {
            return logger;
        }

        private HttpWebRequest SetOtherSetting(Config config, HttpWebRequest httpWebRequest)
        {
            try
            {
                httpWebRequest.AllowAutoRedirect = config.AllowAutoRedirect;
                httpWebRequest.MaximumAutomaticRedirections = config.MaxRedirects;
            }
            catch { }
            if (config.UserAgent != null)
                httpWebRequest.UserAgent = config.UserAgent;

            httpWebRequest.KeepAlive = config.KeepAlive;

            if (config.Referer != null)
                httpWebRequest.Referer = config.Referer;

            if (config.ContectType != null)
                httpWebRequest.ContentType = config.ContectType;
		
            if (config.DecompressionGZip)
                httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;

            httpWebRequest.Method = config.Method.ToString();
		
           if(config.TimeOut != 0)
            httpWebRequest.Timeout = config.TimeOut;

            return httpWebRequest;
        }

        private string SetProxy(Proxy proxy, HttpWebRequest httpWebRequest, out HttpWebRequest res)
        {
            if (ProxyService.UseInAllRequest)
                proxy = ProxyService.proxy;
            if (proxy == null)
                if (ProxyService.proxy.Ip != null)
                    proxy = ProxyService.proxy;
            res = httpWebRequest;
            if (proxy.Ip == null)
                return "Proxy ip is null";
            if (proxy.Port == 0)
                return "Proxy  port is null";
            try
            {
                res.Proxy = new WebProxy(proxy.Ip, proxy.Port);
                if (proxy.Username != null && proxy.Password != null)
                {
                    NetworkCredential net = new NetworkCredential(proxy.Username, proxy.Password);
                    res.Proxy.Credentials = net;
                }
                return "OK";
            }
            catch (Exception ex) { logger.AddMessage("ERROR: can't Set proxy service : " + ex.Message, Log.Type.Error); return ex.Message; }
        }
        private string SetProxy(Proxy proxy, WebClient web, out WebClient res)
        {
            if (ProxyService.UseInAllRequest)
                proxy = ProxyService.proxy;
            if (proxy == null)
                if (ProxyService.proxy.Ip != null)
                proxy = ProxyService.proxy;
            res = web;
            if (proxy.Ip == null)
                return "Proxy ip is null";
            if (proxy.Port == 0)
                return "Proxy  port is null";
            try
            {
                res.Proxy = new WebProxy(proxy.Ip, proxy.Port);
                if (proxy.Username != null && proxy.Password != null)
                {
                    NetworkCredential net = new NetworkCredential(proxy.Username, proxy.Password);
                    res.Proxy.Credentials = net;
                }
                return "OK";
            }
            catch (Exception ex) { logger.AddMessage("ERROR: can't Set proxy service : " + ex.Message, Log.Type.Error); return ex.Message; }
        }


        /// <summary>
		/// GETData base request
		/// </summary>
        /// <param name="config">Config for Request.</param>
        ///  <param name="mange">Keep your request by cookies.</param>
        ///  <param name="proxy">Send request by proxy service.</param>
        internal RequestManage GetPageSource(Config config, RequestManage mange = null, Proxy Proxy = null)
        {
            if (mange != null)
                if (mange.CookiesString != null)
                    config.Cookies += mange.CookiesString;
            RequestManage end = new RequestManage();
            CookieContainer container = new CookieContainer();
            try
            {
                ServicePointManager.ServerCertificateValidationCallback +=
        new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });


                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(config.LoginURL);

                try
                {
                    if (mange.Cookies.Count != 0)
                    {
                        container = mange.Cookies;
                    }
                }
                catch { }

                //Set Proxy
                if (ProxyService.proxy.Ip != null | Proxy != null)
                {
                    string setProxy = SetProxy(Proxy, httpWebRequest, out httpWebRequest);
                    if (setProxy != "OK")
                    {
                        end.Cookies = null;
                        end.Headers = null;
                        end.SourcePage = "ERROR|PROXY|" + setProxy;
                        logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(end.Cookies, config) + "\n\nHeaders:\n" + end.Headers.ToString(), Log.Type.OutPut);

                        return end;
                    }
                }
                else httpWebRequest.Proxy = null;

                httpWebRequest = SetOtherSetting(config, httpWebRequest);

                //Add Headres
                if (config.Headers != null)
                {
                    httpWebRequest.Headers = Utils.SetHeaders(config);
                }

                //Add Cookies
                if (config.Cookies != null)
                {
                    container = Utils.SetCookies(config, container, httpWebRequest.Host);
                }

                httpWebRequest.CookieContainer = container;


                logger.AddMessage("Getting Response", Log.Type.Infomation);

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();


                end = Utils.GetManage(response, container);
                end.CookiesString = Utils.GetCookiesString(container, config);

                logger.AddMessage("RequestManage OutPut\nCookies:\n" + Utils.GetCookiesString(end.Cookies, config) + "\n\nHeaders:\n" + end.Headers.ToString() + "\nCode: " + end.StatusCode.ToString(), Log.Type.OutPut);

                return end;

            }
            catch (WebException ex)
            {
                string error = ex.Message;

                if (ex.Response != null)
                {
                    end = Utils.GetManage(ex.Response, container);
                    end.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                }
                end.ErrorAst = true;
                logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCode: " + end.StatusCode.ToString(), Log.Type.OutPut);

                return end;
            }
        }

        /// <summary>
		/// POSTData base request
		/// </summary>
        /// <param name="config">Config for Request.</param>
        ///  <param name="mange">Keep your request by cookies.</param>
        ///  <param name="proxy">Send request by proxy service.</param>
        internal RequestManage GetRequestByData(Config config, RequestManage mange = null, Proxy Proxy = null)
        {
            if (mange != null)
                if (mange.CookiesString != null)
                    config.Cookies += mange.CookiesString;
            RequestManage end = new RequestManage();
            CookieContainer container = new CookieContainer();
            try
            {
                //SetConfig
                ServicePointManager.ServerCertificateValidationCallback +=
new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(config.LoginURL);

                if (ProxyService.proxy.Ip != null | Proxy != null)
                {
                    string setProxy = SetProxy(Proxy, httpWebRequest, out httpWebRequest);
                    if (setProxy != "OK")
                    {
                        end.Cookies = null;
                        end.ErrorAst = true;
                        end.Headers = null;
                        end.SourcePage = "ERROR|PROXY|" + setProxy;
                        logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(end.Cookies, config) + "\n\nHeaders:\n" + end.Headers.ToString(), Log.Type.OutPut);

                        return end;
                    }
                }
                else httpWebRequest.Proxy = null;


                byte[] bytes = new ASCIIEncoding().GetBytes(config.PostData);
                httpWebRequest.ContentLength = bytes.Length;


                httpWebRequest = SetOtherSetting(config, httpWebRequest);

                httpWebRequest.CookieContainer = container;

                //Add Headers
                if (config.Headers != null)
                {
                    httpWebRequest.Headers = Utils.SetHeaders(config);
                }

                //Add Cookies
                if (config.Cookies != null)
                {
                    container = Utils.SetCookies(config, container, httpWebRequest.Host);
                }

                //GetRequest
                try
                {
                    if (mange.Cookies.Count != 0)
                    {
                        container = mange.Cookies;
                        container.ToString();
                    }
                }
                catch { }
                httpWebRequest.CookieContainer = container;
                
                logger.AddMessage("Getting Response", Log.Type.Infomation);

                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                requestStream.Close();
                container.Add(response.Cookies);

                end = Utils.GetManage(response, container);
                end.CookiesString = Utils.GetCookiesString(container, config);

                end.StatusCode = (int)response.StatusCode;

                logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCookies:\n" + Utils.GetCookiesString(end.Cookies, config) + "\n\nHeaders:\n" + end.Headers.ToString() + "\nCode: " + end.StatusCode.ToString(), Log.Type.OutPut);


                return end;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    end = Utils.GetManage(ex.Response, container);
                    end.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                }
                end.ErrorAst = true;
                logger.AddMessage("RequestManage OutPut\nSoucePage:\n" + end.SourcePage + "\n\nCode: " + end.StatusCode.ToString(), Log.Type.OutPut);

                return end;
            }

        }
    }
}
