using System;
using System.Collections.Generic;

namespace MSC
{
    public class MessageReceivedArge : EventArgs
    {
        public Log log { set; get; }
    }

    public class Logger
    {
        
        public event EventHandler<MessageReceivedArge> OnMessageReceived;

        List<Log> list = new List<Log>();

        protected virtual void OnMessage(MessageReceivedArge e)
        {
            EventHandler<MessageReceivedArge> handler = OnMessageReceived;
            if (handler != null)
                handler(this, e);
        }

        public void AddMessage(string Message = "Empty Message", Log.Type type = Log.Type.Unkhown)
        {
            Log log = new Log();
            log.SetMessage(Message, type);
            list.Add(log);

            MessageReceivedArge arge = new MessageReceivedArge();
            arge.log = log;
            OnMessage(arge);
        }
        public string GetMessages(bool WhitTime = false)
        {
            string neh = ""; try {
                foreach (Log item in list)
                {
                    string beh = item.GetMessage(WhitTime);
                    if (!string.IsNullOrWhiteSpace(beh) || beh != "\n")
                        neh += item.GetMessage(WhitTime) + "\n";
                }
            }
            catch (Exception ex) { neh = "ERROR to Get Logs! => " + ex.Message; }
            return neh;
        }
        public void Clean()
        {
            list.Clear();
        }
        public string GetMessages(bool WhitTime = false, Log.Type JustType = Log.Type.Unkhown)
        {
            string neh = ""; try {
                foreach (Log item in list)
                {
                    if (item.typeT == JustType)
                    {
                        string beh = item.GetMessage(WhitTime);
                        if (!string.IsNullOrWhiteSpace(beh))
                            neh += item.GetMessage(WhitTime) + "\n";
                    }
                }
            }
            catch(Exception ex) { neh = "ERROR to Get Logs! => " + ex.Message; }
            return neh;
        }
        public string GetMessages(int index, bool WhitTime = false, Log.Type JustType = Log.Type.Unkhown)
        {
            if (list[index].typeT == JustType)
                return list[index].GetMessage(WhitTime);
            else return null;
        }
        public string GetMessages(int index, bool WhitTime = false)
        {
            string neh = list[index].GetMessage(WhitTime);
            list.RemoveAt(index);
            return neh;
        }
    }
    public class Log
    {
        public enum Type
        {
            OutPut,
            Error,
            Infomation,
            Unkhown
        }
        DateTime time { set; get; }
        string MessageT { set; get; }
        public Type typeT
        {
            set; get;
        }

        public void SetMessage(string Message, Type type = Type.Unkhown)
        {
            time = DateTime.Now;
            typeT = type;
            MessageT = Message;
        }
        public string GetMessage(bool WhitTime = false)
        {
            if (WhitTime)
            {
                return time.ToLongTimeString() + " => " + MessageT;
            }
            else return MessageT;
        }
        public override string ToString()
        {
            return time.ToLongTimeString() + " => " + MessageT;
        }
    }
}
