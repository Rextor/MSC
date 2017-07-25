using MSC;
using MSC.Brute;

namespace MSC
{
    class TelegramService
    {
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
        
        public void SendMessage(string text)
        {
            Config config = new Config();
            Account ac = new Account();
            ac.Username = ID;
            ac.Password = Token;

            config.Method = Method.POST;
            string pst = "https://api.telegram.org/bot<TOKEN>/sendMessage?chat_id=<ID>&text=<TEXT>";
            config.DataSet = "<ID>*<TOKEN>";
            pst = Rer.ReplaceAccount(ac, pst, config);
            config.LoginURL = pst.Replace("<TEXT>", text);
            config.AllowAutoRedirect = true;
            RequestManage send = Rer.GETData(config);
        }
    }
}
