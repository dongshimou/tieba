using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Web;
using System.Drawing;
using System.Threading;
using tieba;
namespace tieba
{
    public class baidu
    {
        //private HttpWebResponse response;
        private string token;
        private string rsakey;
        private string publickey;
        private string cookies;
        private string verifyStr;
        private string valcode;
        private string uid;
        private List<string> like=new List<string>();
        public CookieContainer cookie = new CookieContainer();
        public baidu()
        {
            string url = "https://passport.baidu.com/v2/api/?getapi&";
            /*url += "tpl=pp&tt=";
            url += DateTime.Now.Ticks.ToString();
            url += "&class=&apiver=v3&logintype=basicLogin";//&callback=customName
            var request =(HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.CookieContainer = cookie;
            response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine(response.StatusDescription);
            var headers = response.Headers["Set-Cookie"];
            var objs = headers.Split(';');
            var cook = objs[0] + ';' + objs[4].Remove(0, 10);

            request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers["Cookie"] = cook;
            response = (HttpWebResponse)request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();*/
            var nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"class", ""},
                {"logintype", "basicLogin"},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"class", ""},
                {"logintype", "basicLogin"},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
            };
            httpResult = new HttpHelper().GetHtml(
               new HttpItem()
               {
                   URL = url + HttpHelper.DataToString(nvc),
                   Method = "GET",
                   CookieContainer = cookie,
                   ResultCookieType = ResultCookieType.CookieContainer
               });

            string responseFromServer = httpResult.Html;
            var json0 = new JavaScriptSerializer().
                DeserializeObject(responseFromServer)
                as Dictionary<string, object>;
            var j1 = json0["data"] as Dictionary<string, object>;
            token = j1["token"] as string;


            url = "https://passport.baidu.com/v2/getpublickey?apiver=v3";
            /*request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.CookieContainer= cookie;
            response = (HttpWebResponse)request.GetResponse();
            //var head = response.Headers;
            dataStream = response.GetResponseStream();
            reader = new StreamReader(dataStream);
            responseFromServer = reader.ReadToEnd();
*/
            nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"token", token},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
            };
            httpResult = new HttpHelper().GetHtml(
               new HttpItem()
               {
                   URL = url + HttpHelper.DataToString(nvc),
                   Method = "GET",
                   CookieContainer = cookie,
                   ResultCookieType = ResultCookieType.CookieContainer
               });
            responseFromServer = httpResult.Html;
            var json1 = new JavaScriptSerializer().
                DeserializeObject(responseFromServer)
                as Dictionary<string, object>;

            publickey = json1["pubkey"] as string;
            publickey = publickey.Replace("\n", "");
            publickey = publickey.Replace("-----BEGIN PUBLIC KEY-----", "");
            publickey = publickey.Replace("-----END PUBLIC KEY-----", "");
            rsakey = json1["key"] as string;
            //Console.WriteLine(responseFromServer);
        }
        public Image GetValCode()
        {
            var url = "https://passport.baidu.com/v2/?reggetcodestr&";
            var nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"fr", "login"},
                {"token", token},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                var json = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html)
                as Dictionary<string, object>;
                var j1 = json["data"] as Dictionary<string, object>;
                verifyStr = j1["verifyStr"] as string;
                var verifySign = j1["verifySign"];
                url = "https://passport.baidu.com/cgi-bin/genimage?";
                httpResult = new HttpHelper().GetHtml(
                    new HttpItem()
                    {
                        URL = url + verifyStr,
                        Method = "GET",
                        CookieContainer = cookie,
                        ResultCookieType = ResultCookieType.CookieContainer,
                        ResultType = ResultType.Byte
                    });
                if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
                {
                    return Image.FromStream(new MemoryStream(httpResult.ResultByte));
                }
            }
            return null;
        }

        public bool CheckValCode(string code)
        {
            var url = "https://passport.baidu.com/v2/?checkvcode&";
            var nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"token", token},
                {"tpl", "pp"},
                {"fr", "login"},
                {"token", token},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
                {"verifycode", code},
                {"codestring", verifyStr},
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return true;
            }
            return false;
        }
        public bool login(string username, string password)
        {
            var url = "https://passport.baidu.com/v2/api/?login";
            var info = new NameValueCollection
            {
                 {"apiver", "v3"},
                 {"staticpage", "https://passport.baidu.com/static/passpc-account/html/v3Jump.html"},
                 {"charset", "UTF-8"},
                 {"token", token},
                 {"isPhone","false" },
                 {"tpl", "pp"},
                 {"tt", DateTime.Now.Ticks.ToString()},
                 {"u", "https://passport.baidu.com/"},
                 {"isPhone", "false"},
                 {"detect", "1"},
                 {"quick_user", "0"},
                 {"rsakey", rsakey},
                 {"crypttype", "12"},
                 {"ppui_logintime", "111111"},
                 {"verifycode","" },
                 {"username", username},
                 {"password", RSAHelper.RSAEncrypt(publickey,password)},
            };
            var httpResult = new HttpHelper().GetHtml(
            new HttpItem
            {
                URL = url,
                Method = "POST",
                ContentType = "application/x-www-form-urlencoded",
                Postdata = HttpHelper.DataToString(info),
                PostDataType = PostDataType.String,
                CookieContainer = cookie,
                ResultCookieType = ResultCookieType.CookieContainer
            });
            //cookies = httpResult.Cookie;
            return httpResult.StatusCode.Equals(HttpStatusCode.OK);
        }
        public void getPSTM()
        {
            string url = "http://www.baidu.com";
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            cookies = result.Cookie;
            var split = cookies.Split(';');
            foreach (var str in split)
            {
                if (str.Contains("PSTM"))
                {
                    uid = str.Remove(0, str.IndexOf("PSTM", StringComparison.Ordinal) + 5);
                    return;
                }
            }
        }

        public string signall()
        {
            string result = string.Empty;
            getPSTM();
            getLike();
            foreach (var one in like)
            {
                sign(one);
                Thread.Sleep(1000);
            }
            result = "签到完成";
            return result;
        }
        public void getLike()
        {
            string url = "http://tieba.baidu.com/p/getLikeForum?";
            var nvc = new NameValueCollection
            {
                {"t",uid },
            };
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var json = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            var j1 = json["data"] as Dictionary<string, object>;
            var j2 = j1["info"] as object[];
            foreach (var temp in j2)
            {
                var one = temp as Dictionary<string, object>;
                like.Add(one["forum_name"] as string);
            }
        }
        public void sign(string ba)
        {
            var url = "http://tieba.baidu.com/dc/common/tbs";//&
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var json1 = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            var tbs = json1["tbs"] as string;
            var info = new NameValueCollection
            {
                {"ie","utf-8" },
                {"kw", ba},
                {"tbs",tbs },
            };
            result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = "http://tieba.baidu.com/sign/add",
                    Method = "POST",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer,
                    Postdata = HttpHelper.DataToString(info)
                });
        }
    }
}
