using MSC;
using MSC.Brute;

namespace MSC
{
    public class TelegramService
    {
        public enum ParseMode
        {
            Markdown,
            HTML,
            None
        }
        private string Token;
        private string ID;
        Requester Rer;
        public Logger CoreLogger()
        {
            return Rer.GetCoreLogger();
        }
        public Logger BaseLogger()
        {
            return Rer.GetBaseLogger();
        }
        public TelegramService(string BotToken, string UserID)
        {
            Token = BotToken;
            ID = UserID;
            Rer = new Requester();
        }
        
        public void SendMessage(string text, ParseMode mode = ParseMode.None)
        {
            Config config = new Config();
            Account ac = new Account();
            ac.Username = ID;
            ac.Password = Token;

            text = EncodeText(text);
            config.Method = Method.POST;
            string pst = "https://api.telegram.org/bot<TOKEN>/sendMessage?chat_id=<ID>&text=<TEXT>";
            config.DataSet = "<ID>*<TOKEN>";
            pst = Rer.ReplaceAccount(ac, pst, config);
            pst = pst.Replace("<TEXT>", text);

            string modetext = "";
            switch (mode)
            {
                case ParseMode.HTML:
                    modetext = ParseMode.HTML.ToString();
                    break;
                case ParseMode.Markdown:
                    modetext = ParseMode.Markdown.ToString();
                    break;
                case ParseMode.None:
                    modetext = "";
                    break;
            }
            pst += "&parse_mode=" + modetext;

            config.LoginURL = pst;
            config.AllowAutoRedirect = true;
            RequestManage send = Rer.GETData(config);
        }

        private static string EncodeText(string text)
        {
            text = text.Replace("&", "%26");
            return text;
        }
    }
}
