using System;
using System.Collections.Generic;
using System.IO;
using MSC.Brute;

namespace MSC
{
    public class Data
    {
        private string[] SplitNow(string Item, char Sp)
        {
            string[] All = Item.Split(Sp);
            return All;
        }
        private List<String> Combolist { set; get; }
        private List<String> Proxylist { set; get; }
        public Proxy Proxy { set; get; }
        public int ChangeOfProxy { set; get; }
        public int OfProxy { set; get; }
        public int OfCombo { set; get; }
        public char SplitAcount { set; get; }
        public char SplitProxy { set; get; }
        public int ChangedProxy {
            set; get;
        }
        public void LoadComboList(string Filename)
        {
            OfCombo = 0;
            string[] combo = File.ReadAllLines(Filename);
            foreach (string item in combo)
            {
                string[] check = SplitNow(item, SplitAcount);
                if (check.Length == 2)
                    Combolist.Add(item);
            }
        }
        public void LoadProxyList(string Filename)
        {
            OfProxy = 0;
            string[] proxy = File.ReadAllLines(Filename);
            foreach (string item in proxy)
            {
                string[] check = SplitNow(item, SplitProxy);
                if (check.Length == 2)
                    Proxylist.Add(item);
            }
        }
        public void NextProxy()
        {
            if (Proxylist != null)
            {
                if (ChangedProxy >= ChangeOfProxy || ChangedProxy == 0)
                    if (OfProxy <= Proxylist.Count - 1)
                    {
                        OfProxy++;
                        string[] All = SplitNow(Proxylist[OfProxy], SplitProxy);
                        Proxy.Ip = All[0];
                        Proxy.Port = int.Parse(All[1]);
                    }
                    else if (OfProxy == Proxylist.Count - 1)
                    {
                        OfProxy = 0;
                        string[] All = SplitNow(Proxylist[OfProxy], SplitProxy);
                        Proxy.Ip = All[0];
                        Proxy.Port = int.Parse(All[1]);
                    }
                    else ChangedProxy++;
            }
        }
        public Account GetNextAccout(int index)
        {
            Account account = new Account();
            string[] userpass = Combolist[OfCombo].Split(SplitAcount);
            account.Username = userpass[0];
            account.Password = userpass[1];
            OfCombo++;
            return account; 
        }
    }
}
