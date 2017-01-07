using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tieba
{
    using System.Drawing;
    using System.IO;
    using System.Xml;
    using tieba;
    public class RuoKuaiCode
    {
        RuoKuaiHttp http = new RuoKuaiHttp();
        //Image image = null;
        static string Id = "73693";
        static string Key = "b16f5d05850840caaae2ddd02403f56f";
        string type = "4040";
        static string timeout = "90";
        string username = string.Empty;
        string password = string.Empty;
        public RuoKuaiCode(string user, string pass)
        {
            username = user;
            password = pass;
        }

        public string UpLoadInputImage(Image m)
        {
             type = "4040";
            var param = new Dictionary<object, object>
            {
                {"username",username },
                {"password",password },
                {"typeid",type },
                {"timeout",timeout },
                {"softid",Id },
                {"softkey",Key }
            };
            var ms = new MemoryStream();
            m.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            var data = ms.ToArray();

            string HttpResult = RuoKuaiHttp.Post("http://api.ruokuai.com/create.xml", param, data);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(HttpResult);
            }
            catch { return "格式错误"; }
            XmlNode idNode = xmlDoc.SelectSingleNode("Root/Id");
            XmlNode resultNode = xmlDoc.SelectSingleNode("Root/Result");
            XmlNode errorNode = xmlDoc.SelectSingleNode("Root/Error");
            string result = string.Empty;
            string topidid = string.Empty;
            if (resultNode != null && idNode != null)
            {
                topidid = idNode.InnerText;
                result = resultNode.InnerText;
            }
            else if (errorNode != null)
            { return "识别错误"; }
            else
            { return "未知错误"; }
            return result;
        }
        public string UpLoadClickImage(Image m)
        {
             type = "6104";
            var param = new Dictionary<object, object>
            {
                {"username",username },
                {"password",password },
                {"typeid",type },
                {"timeout",timeout },
                {"softid",Id },
                {"softkey",Key }
            };
            var ms = new MemoryStream();
            m.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            var data = ms.ToArray();

            string HttpResult = RuoKuaiHttp.Post("http://api.ruokuai.com/create.xml", param, data);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(HttpResult);
            }
            catch { return "格式错误"; }
            XmlNode idNode = xmlDoc.SelectSingleNode("Root/Id");
            XmlNode resultNode = xmlDoc.SelectSingleNode("Root/Result");
            XmlNode errorNode = xmlDoc.SelectSingleNode("Root/Error");

            string result = string.Empty;
            string topidid = string.Empty;
            if (resultNode != null && idNode != null)
            {
                topidid = idNode.InnerText;
                var text = resultNode.InnerText;
                //if (text.Length != 4) return "识别错误";
                for(int i=0;i<text.Length;i++)
                {
                    var number = Convert.ToInt32(text[i]-'0') - 1;
                    const string bs = "000";
                    result+= bs + number%3 + bs + number/3;
                }
            }
            else if (errorNode != null)
            { return "识别错误"; }
            else
            { return "未知错误"; }
            return result;
        }
    }
}
