using MSC.Brute;
using System.Drawing;
using System.IO;
using System.Net;

namespace MSC.OCR
{
    public class OCR
    {
        //User user;
        //Client client;

        //public void SetAccount(Account LoginAccount)
        //{
        //    try {
        //        client = new SocketClient(LoginAccount.Username, LoginAccount.Password);
        //        user = client.User;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        if (ex.Message == "Access denied, check your credentials")
        //            throw new System.Exception("Invalid Account");
        //        else throw new System.Exception("Unknown Error: " + ex.Message);
        //    }
        //}
        //private static byte[] imageToByteArray(Image image)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        image.Save(ms, image.RawFormat);
        //        return ms.ToArray();
        //    }
        //}
        //public string GetCaptcha(Config config, RequestManage manageFull, Proxy proxy = null)
        //{
        //    if (user.LoggedIn)
        //    {
        //        //RequestManage manage = new RequestManage();

        //        //Requester Rer = new Requester();

        //        //config.ContectType = "image/png; encoding='utf-8'";

        //        //manage = Rer.GETData(config, manageFull, proxy);

        //        WebClient wc = new WebClient();
        //        byte[] image = wc.DownloadData(config.LoginURL);

        //        //byte[] image = imageToByteArray(manage.Image);

        //        Captcha cap = client.Decode(image);
        //        return cap.Text;
        //    }
        //    else throw new System.Exception("Invalid Account");
        //}
    }
}
