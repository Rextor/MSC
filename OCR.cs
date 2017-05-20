using DeathByCaptcha;
using MSC.Brute;
using System.Drawing;
using System.IO;
using System.Net;

namespace MSC.OCR
{
    public class OCR
    {
        User user;
        Client client;

        public void SetAccount(Account LoginAccount)
        {
            try
            {
                client = new SocketClient(LoginAccount.Username, LoginAccount.Password);
                user = client.User;
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "Access denied, check your credentials")
                    throw new System.Exception("Invalid Account");
                else throw new System.Exception("Unknown Error: " + ex.Message);
            }
        }
        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }
        public string GetCaptcha(Image image)
        {
            if (user.LoggedIn)
            {
                Captcha cap = client.Decode(imageToByteArray(image));
                return cap.Text;
            }
            else throw new System.Exception("Invalid Account");
        }
    }
}
