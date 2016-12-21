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
using System.Net.Http;
using System.Threading;
using tieba;
namespace tieba
{
    public class baidu
    {
        private string codeString = string.Empty;
        private string verifycode = string.Empty;
        private string vcodetype = string.Empty;
        //private HttpWebResponse response;
        private string token { get; set; }
        private string rsakey { get; set; }
        private string publickey { get; set; }
        private string cookies { get; set; }
        private string verifyStr { get; set; }
        private string pstm { get; set; }
        public string error { get; set; }
        private List<string> like = new List<string>();
        public CookieContainer cookie = new CookieContainer();
        public event Form1.MessageHandler SignMessage;
        public baidu()
        {
        }
        public string init()
        {
            string url = "https://passport.baidu.com/v2/api/?getapi&";
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
            var json0 = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html)
                as Dictionary<string, object>;
            var j1 = json0["data"] as Dictionary<string, object>;
            token = j1["token"] as string;
            url = "https://passport.baidu.com/v2/getpublickey?";
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
            var json1 = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html)
                as Dictionary<string, object>;
            var errno = json1["errno"] as string;
            if (errno != "0") return "获取key失败";
            publickey = json1["pubkey"] as string;
            publickey = publickey.Replace("\n", "");
            publickey = publickey.Replace("-----BEGIN PUBLIC KEY-----", "");
            publickey = publickey.Replace("-----END PUBLIC KEY-----", "");
            rsakey = json1["key"] as string;
            return "初始化成功";
        }
        public bool isgetcodeString(string username)
        {
            var url = "https://passport.baidu.com/v2/api/?logincheck&";
            var nvc = new NameValueCollection
            {
                //{"logincheck","" },
                {"token",token},
                {"tpl","pp" },
                {"apiver","v3" },
                {"tt",DateTime.Now.Ticks.ToString () },
                {"username", username},
                {"isphone","false" },
                { "sub_source","leadsetpwd"},
                {"callback","bd__cbs__29t9z0" },
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var str = httpResult.Html.Replace("bd__cbs__29t9z0(", "");
            str = str.Remove(str.Length - 1, 1);
            var json1 = new JavaScriptSerializer().
                DeserializeObject(str)
                as Dictionary<string, object>;
            var j1 = json1["data"] as Dictionary<string, object>;
            if (!j1.ContainsKey("codeString") || j1["codeString"] == null)
                return false;
            codeString = j1["codeString"].ToString();
            if (j1.ContainsKey("vcodetype"))
                vcodetype = j1["vcodetype"].ToString();
            else
                vcodetype = string.Empty;
            return !string.IsNullOrEmpty(codeString);
        }
        public Image getCNCode()
        {
            var url = "https://passport.baidu.com/cgi-bin/genimage?" + codeString;
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer,
                    ResultType = ResultType.Byte
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return Image.FromStream(new MemoryStream(httpResult.ResultByte));
            }
            else
                return null;
        }
        public bool setCNCode(string input)
        {
            var url = "https://passport.baidu.com/v2/?checkvcode&";
            verifycode = HttpUtility.HtmlEncode(input);
            var nvc = new NameValueCollection
            {
                {"apiver", "v3"},
                {"token", token},
                {"tpl", "pp"},
                {"tt", DateTime.Now.Ticks.ToString()},
                {"codestring",codeString },
                {"verifycode",verifycode },
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
                error = "验证成功";
                return true;
            }
            else
            {
                if (httpResult.RedirectUrl.Contains("error"))
                    error = "验证码出错";
                return false;
            }
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
                 //{"subpro","" },
                 {"tt", DateTime.Now.Ticks.ToString()},
                 {"u", "https://passport.baidu.com/"},
                 {"isPhone", "false"},
                 {"detect", "1"},
                 {"quick_user", "0"},
                 {"rsakey", rsakey},
                 {"crypttype", "12"},
                 {"ppui_logintime", "111111"},
                 {"verifycode",verifycode },
                 {"codestring",codeString },
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
            cookies = httpResult.Cookie;
            if (string.IsNullOrEmpty(cookies)) return false;
            return httpResult.StatusCode.Equals(HttpStatusCode.OK);
        }
        public void getPSTM()
        {
            string url = "https://www.baidu.com";
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
                    pstm = str.Remove(0, str.IndexOf("PSTM", StringComparison.Ordinal) + 5);
                    return;
                }
            }
        }
        public string signall()
        {
            string result = string.Empty;
            getPSTM();
            if (!getLike())
                return error;
            foreach (var one in like)
            {
                sign(one);
                Thread.Sleep(500);
            }
            result = "签到完成";
            return result;
        }
        public bool getLike()
        {
            string url = "http://tieba.baidu.com/p/getLikeForum?";
            //uid = DateTime.Now.Ticks.ToString();
            var nvc = new NameValueCollection
            {
                {"t",pstm },
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
            if ((json["errno"] as object).ToString() != "0")
            {
                error = json["errmsg"].ToString();
                return false;
            }
            var j1 = json["data"] as Dictionary<string, object>;
            var j2 = j1["info"] as object[];
            foreach (var temp in j2)
            {
                var one = temp as Dictionary<string, object>;
                like.Add(one["forum_name"] as string);
            }
            return true;
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
            SignMessage?.Invoke(ba);
        }

        public void getBar(string bar)
        {
            var url = "http://tieba.baidu.com/f?";
            var info = new NameValueCollection
            {
                {"kw", HttpUtility.HtmlEncode(bar)},
                {"ie", "utf-8"},
            };
            var HttpResult=new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url+HttpHelper.DataToString(info),
                    Method = "GET",
                    CookieContainer = cookie,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            /*var pageData;
            var user;
            var froum;*/
        }
    }
}
