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
    using HtmlAgilityPack;
    using System.Runtime.Serialization.Formatters.Binary;

    public class baidu
    {
        public delegate void SignDelegate(string s);

        public event SignDelegate SignEvent;
        public string username { get; set; }
        public string password { get; set; }
        private string replaycodestr = string.Empty;
        private string replaycodetype = string.Empty;
        public string Proxy { get; set; }
        public string BarName { get; set; }
        private string vcodemd5 = string.Empty;
        private string tbs = string.Empty;
        private string fid = string.Empty;
        private string uid = string.Empty;
        private string tid = string.Empty;
        private string codeString = string.Empty;
        private string verifycode = string.Empty;
        private string vcodetype = string.Empty;
        private string token { get; set; }
        private string rsakey { get; set; }
        private string publickey { get; set; }
        private string cookies { get; set; }
        private string verifyStr { get; set; }
        private string pstm { get; set; }
        public string error { get; set; }
        public List<string> like = new List<string>();
        public CookieContainer cookie = new CookieContainer();
        public Dictionary<string, string> barReplay = new Dictionary<string, string>();
        public Dictionary<string, string> barTitle = new Dictionary<string, string>();
        public baidu()
        {
            Proxy = "ieproxy";
        }
        public string Init()
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
                    ProxyIp = Proxy,
                    Timeout = 2000,
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
                    ProxyIp = Proxy,
                    Timeout = 2000,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            try
            {
                var json = new JavaScriptSerializer().
                    DeserializeObject(httpResult.Html);
            }
            catch
            {
                return "初始化失败";
            }
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
                    ProxyIp = Proxy,
                    Timeout = 2000,
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

        public int getCodeType()
        {
            return Convert.ToInt32(replaycodetype);
        }
        public Image getEmoji(int i)
        {
            string index = i.ToString();
            if (i < 10)
                index = "0" + index;
            var url = "http://tb2.bdstatic.com/tb/editor/images/face/i_f" + index + ".png";
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return Image.FromStream(new MemoryStream(httpResult.ResultByte));
            }
            else
                return null;
        }
        public string IsgetcodeString(string username)
        {
            var url = "https://passport.baidu.com/v2/api/?logincheck&";
            var nvc = new NameValueCollection
            {
                //{"logincheck","" },
                {"token", token},
                {"tpl", "pp"},
                {"apiver", "v3"},
                {"tt", DateTime.Now.Ticks.ToString()},
                {"username", username},
                {"isphone", "false"},
                {"sub_source", "leadsetpwd"},
                //{"callback","cb" },
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url + HttpHelper.DataToString(nvc),
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    Timeout = 2000,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html);
            }
            catch
            {
                return "操作超时";
            }
            var j1 = (obj as Dictionary<string, object>)["data"]
                as Dictionary<string, object>;
            if (!j1.ContainsKey("codeString") || string.IsNullOrEmpty(j1["codeString"].ToString()))
                return "不需要验证码";
            codeString = j1["codeString"].ToString();
            if (j1.ContainsKey("vcodetype"))
                vcodetype = j1["vcodetype"].ToString();
            else
                vcodetype = string.Empty;
            return string.Empty;
        }
        public Image GetLoginCode()
        {
            var url = "https://passport.baidu.com/cgi-bin/genimage?" + codeString;
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem()
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
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
        public string SetLoginCode(string input)
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
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html);
            }
            catch { return "网络错误"; }
            var err = (obj as Dictionary<string, object>)["errInfo"]
                as Dictionary<string, object>;
            if (err["no"].ToString() == "0")
                return "验证成功";
            return (err["msg"]).ToString();
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
                    ProxyIp = Proxy,
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
                        ProxyIp = Proxy,
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
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return true;
            }
            return false;
        }
        public bool Login(string username, string password)
        {
            this.username = username;
            this.password = password;
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
                ProxyIp = Proxy,
                ResultCookieType = ResultCookieType.CookieContainer
            });
            cookies = httpResult.Cookie;
            if (string.IsNullOrEmpty(cookies)) return false;
            return httpResult.StatusCode.Equals(HttpStatusCode.OK);
        }
        public bool GetPSTM()
        {
            string url = "https://www.baidu.com";
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            cookies = result.Cookie;
            if (cookies == null) return false;
            var split = cookies.Split(';');
            foreach (var str in split)
            {
                if (str.Contains("PSTM"))
                {
                    pstm = str.Remove(0, str.IndexOf("PSTM", StringComparison.Ordinal) + 5);
                    return true;
                }
            }
            return false;
        }
        public Image GetPostCode()
        {
            var url = "http://tieba.baidu.com/cgi-bin/genimg?" + replaycodestr + "&t=0.6";
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if (httpResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                return Image.FromStream(new MemoryStream(httpResult.ResultByte));
            }
            else
                return null;
        }
        public bool SetPostCode(string input, int type)
        {
            var url = string.Empty;
            if (type == 1)
                url = "http://tieba.baidu.com/f/commit/commonapi/checkVcode";
            else if (type == 4)
                url = "http://tieba.baidu.com/sign/checkVcode";
            var info = new NameValueCollection
            {
                { "captcha_vcode_str",replaycodestr },
                { "captcha_code_type",replaycodetype},
                {"captcha_input_str",input },
                {"fid",fid },
            };
            var httpResult = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "POST",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    Postdata = HttpHelper.DataToString(info),
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var obj = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html)
                as Dictionary<string, object>;
            return obj["anti_valve_err_no"].ToString() == "0";
        }
        public bool codereplay(string vcode, string bar, string content, string title = "")
        {
            var url = string.Empty;
            if (title == "")
                url = "http://tieba.baidu.com/f/commit/post/add";
            else
                url = "http://tieba.baidu.com/f/commit/thread/add";
            var info = new NameValueCollection
            {
                 {"ie", "utf-8"},
                 {"kw", bar},
                 {"fid", fid},
                 {"vcode_md5", replaycodestr},
                 {"floor_num", "0"},
                 {"rich_text","1"},
                 {"tbs",tbs},
                 {"content",content},
                 {"lp_type", "1"},
                 {"lp_sub_type", "1"},
                 {"repostid", null},
                 {"talk_type", null},
                 {"vcode", vcode},
                 {"tag","11" },
            };
            if (title != "")
            {
                info.Add("title", title);
                info.Add("__type__", "thread");
                info.Add("tid", "0");
            }
            else
            {
                info.Add("__type__", "reply");
                info.Add("tid", tid);
            }
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "POST",
                    Postdata = HttpHelper.DataToString(info),
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var obj = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            return obj["err_code"].ToString() == "0";
        }
        public string replay(string bar, string content, string title = "")
        {
            replaycodestr = string.Empty;
            var url = string.Empty;
            if (title == "")
                url = "http://tieba.baidu.com/f/commit/post/add";
            else
                url = "http://tieba.baidu.com/f/commit/thread/add";
            var info = new NameValueCollection
            {
                 {"ie", "utf-8"},
                 {"kw", bar},
                 {"fid", fid},
                 {"vcode_md5", replaycodestr},
                 {"floor_num", "0"},
                 {"rich_text","1"},
                 {"tbs",tbs},
                 {"content",content},
                 {"files","[]"},
                 {"lp_type", "1"},
                 {"lp_sub_type", "1"},
                 {"repostid", null},
                 {"talk_type", null},
                 {"new_vcode", "1"},
                 {"tag","11" },
            };
            if (title != "")
            {
                info.Add("title", title);
                info.Add("__type__", "thread");
                info.Add("tid", "0");
            }
            else
            {
                info.Add("__type__", "reply");
                info.Add("tid", tid);
            }
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "POST",
                    Postdata = HttpHelper.DataToString(info),
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            var obj = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            if (obj["err_code"].ToString() == "40")
            {
                obj = obj["data"] as Dictionary<string, object>;
                obj = obj["vcode"] as Dictionary<string, object>;
                replaycodestr = obj["captcha_vcode_str"].ToString();
                replaycodetype = obj["captcha_code_type"].ToString();
                return "需要验证码";
            }
            else if (obj["err_code"].ToString() == "0")
            {
                return "回复成功";
            }
            return "回复失败";
        }
        public bool AddLike(string bar)
        {
            var url = "http://tieba.baidu.com/f/like/commit/add";
            var info = new NameValueCollection
            {
                {"fid",fid },
                {"fname",bar },
                {"uid", username},
                {"ie","utf-8" },
                {"tbs",tbs },
            };
            var HttpResult = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "POST",
                    Postdata = HttpHelper.DataToString(info),
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
                DeserializeObject(HttpResult.Html);
            }
            catch
            {
                return false;
            }
            var json = obj as Dictionary<string, object>;
            if (json["no"].ToString() == "0" || json["no"].ToString() == "221")
                return true;
            return false;
        }
        public bool ClientSign(string bar)
        {
            var url = "http://c.tieba.baidu.com/c/c/forum/sign";
            string bduss = string.Empty;
            CookieCollection c = null;
            try
            {
                c = cookie.GetCookies(new Uri("http://www.baidu.com"));
            }
            catch
            {
                return false;
            }
            foreach (var one in c)
            {
                var d = (Cookie)one;
                if (d.Name == "BDUSS")
                {
                    bduss = d.Value;
                    break;
                }
            }
            var signtbs = getTBS();
            if (string.IsNullOrEmpty(signtbs)) return false;
            GetBarInfo(bar);
            var info = new NameValueCollection
            {
                {"BDUSS",bduss },
                {"fid",fid },
                {"kw", bar},
                {"tbs",signtbs },
            };
            string sign = "BDUSS=" + bduss + "fid=" + fid + "kw=" + bar + "tbs=" + signtbs;
            sign = MyMD5(sign + "tiebaclient!!!").ToUpper();
            info.Add("sign", sign);
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "POST",
                    Postdata = HttpHelper.DataToString(info),
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
                DeserializeObject(result.Html);
            }
            catch { return false; }
            var json = obj as Dictionary<string, object>;
            if (json["error_code"].ToString() == "0" || json["error_code"].ToString() == "160002")
                return true;
            return false;
        }
        private string MyMD5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] md5data = md5.ComputeHash(data);
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
                str += md5data[i].ToString("x2").PadLeft(2, '0');
            return str;
        }
        private string getTBS()
        {
            var url = "http://tieba.baidu.com/dc/common/tbs";//&
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url,
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
                DeserializeObject(result.Html);
            }
            catch
            {
                return string.Empty;
            }
            var json = obj as Dictionary<string, object>;
            return json["tbs"] as string;
        }
        public bool Sign(string bar)
        {
            var signtbs = getTBS();
            if (string.IsNullOrEmpty(signtbs)) return false;
            var info = new NameValueCollection
            {
                {"ie","utf-8" },
                {"kw", bar},
                {"tbs",signtbs },
            };
            var result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = "http://tieba.baidu.com/sign/add",
                    Method = "POST",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer,
                    Postdata = HttpHelper.DataToString(info)
                });
            object obj = null;
            try
            {
                obj = new JavaScriptSerializer().
             DeserializeObject(result.Html);
            }
            catch { return false; }
            var json = obj as Dictionary<string, object>;
            if (json["no"].ToString() == "0" || json["no"].ToString() == "1101")
                return true;
            else if (json["error"].ToString() == "need vcode")
            {
                var data = json["data"] as Dictionary<string, object>;
                replaycodestr = data["captcha_vcode_str"].ToString();
                replaycodetype = data["captcha_code_type"].ToString();
            }
            return false;
        }
        public string SignReady()
        {
            if (!GetPSTM()) return "未知错误";
            if (!GetLike())
                return "获取关注错误";
            else
                return "获取关注成功";
        }
        public string UpLoadImage(string path, string refre)
        {
            var imageTbs = getImageTbs();
            //getImageTbs();
            var url = "http://upload.tieba.baidu.com/upload/pic?";
            var nvc = new NameValueCollection
            {
                {"tbs",imageTbs },
                {"fid",fid },
                {"save_yun_album","1" },
            };
            url += HttpHelper.DataToString(nvc);
            /*StringBuilder opsb = new StringBuilder();
            HttpWebRequest oprequest = (HttpWebRequest)WebRequest.Create(url);
            oprequest.Method = "OPTIONS";
            oprequest.Host = "upload.tieba.baidu.com";
            oprequest.KeepAlive = true;
            oprequest.CookieContainer = cookie;
            oprequest.Headers.Add("Access-Control-Request-Method: " + "POST");
            oprequest.Headers.Add("Origin: " + "http://tieba.baidu.com");
            oprequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0";
            oprequest.Headers.Add("Access-Control-Request-Headers: ");
            oprequest.Accept = "* / *";
            oprequest.Referer = "http://tieba.baidu.com/p/" + refre;
            var opresponce = oprequest.GetResponse();*/
            //var h = opresponce.Headers;
            //StreamReader opsr = new StreamReader(opresponce.GetResponseStream());
            //var re = opsr.ReadToEnd();


            string boundary = "----WebKitFormBoundaryfFFfDOyOEV0oVn3w";
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            if (Proxy != "ieproxy")
            {
                string[] plist = Proxy.Split(':');
                webrequest.Proxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
            }

            webrequest.Method = "POST";
            webrequest.Host = "upload.tieba.baidu.com";
            webrequest.KeepAlive = true;
            webrequest.Headers.Add("Origin: " + "http://tieba.baidu.com");
            webrequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Accept = "*/*";
            webrequest.Referer = "http://tieba.baidu.com/p/" + refre;
            webrequest.Headers.Add("Accept-Encoding: gzip, deflate");
            webrequest.Headers.Add("Accept-Language: en-US,en;q=0.8");
            webrequest.CookieContainer = cookie;
            //百度服务器不支持 Expect: 100-continue,必须设置false.
            webrequest.ServicePoint.Expect100Continue = false;

            StringBuilder sb = new StringBuilder();
            var list = path.Split('\\');
            var filename = list[list.Length - 1];
            list = filename.Split('.');
            var filetype = list[list.Length - 1];
            sb.Append("--" + boundary + "\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + filename + "\"\r\n");
            sb.Append("Content-Type: image/" + filetype + "\r\n\r\n");
            var test = sb.ToString();
            byte[] postHeaderBytes = Encoding.Default.GetBytes(test);
            byte[] boundaryBytes = Encoding.Default.GetBytes("\r\n--" + boundary + "--\r\n\r\n");

            var fs = File.OpenRead(path);
            var length = postHeaderBytes.Length + fs.Length + boundaryBytes.Length;
            webrequest.ContentLength = length;
            webrequest.ReadWriteTimeout = 10000;

            byte[] buffer = new byte[length];
            Array.Copy(postHeaderBytes, buffer, postHeaderBytes.Length);
            fs.Read(buffer, postHeaderBytes.Length, Convert.ToInt32(fs.Length));
            Array.Copy(boundaryBytes, 0, buffer, postHeaderBytes.Length + fs.Length, boundaryBytes.Length);

            Stream requestStream = webrequest.GetRequestStream();

            //requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            requestStream.Write(buffer, 0, buffer.Length);
            //requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            webrequest.Timeout = 10000;

            WebResponse responce = null;
            try
            {
                responce = webrequest.GetResponse();
                var rrrr = responce.ToString();
            }
            catch
            {
                return "网络错误";
            }
            Stream s = responce.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            var HttpResult = sr.ReadToEnd();
            var obj = new JavaScriptSerializer().
                DeserializeObject(HttpResult)
                as Dictionary<string, object>;
            if (obj["err_no"].ToString() != "0")
                return "上传错误";
            var info = obj["info"] as Dictionary<string, object>;
            var pic_id_encode = info["pic_id_encode"].ToString();
            var fullpic_width = info["fullpic_width"].ToString();
            var fullpic_height = info["fullpic_height"].ToString();
            var pic_type = info["pic_type"].ToString();
            var width = Convert.ToInt32(fullpic_width);
            var height = Convert.ToInt32(fullpic_height);
            //图片宽最大560
            const int WMAX = 128;
            //表情包设置为128
            if (width > WMAX)
            {
                var k = WMAX*1.0 / width;
                width = Convert.ToInt32(width * k);
                height = Convert.ToInt32(height * k);
            }
            string result = "[img pic_type=" + 0;
            result += " width=" + width + " height=" + height + "]";
            result += "http://imgsrc.baidu.com/forum/pic/item/" + pic_id_encode + ".jpg";
            result += "[/img][br]";
            return result;
        }
        private string getImageTbs()
        {
            var url = "http://tieba.baidu.com/dc/common/imgtbs";
            var result = new HttpHelper().GetHtml(
                    new HttpItem
                    {
                        URL = url,
                        Method = "GET",
                        CookieContainer = cookie,
                        ProxyIp = Proxy,
                        ResultCookieType = ResultCookieType.CookieContainer
                    });
            var obj = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            if (obj["no"].ToString() != "0")
                return "error";
            var data = obj["data"] as Dictionary<string, object>;
            return data["tbs"].ToString();
        }
        public string Signall()
        {
            foreach (var one in like)
            {
                //if (Sign(one))//电脑签到
                if (ClientSign(one))//手机客户端签到
                    OnSignEvent(one + "," + "success");
                else
                    OnSignEvent(one + "," + "fail");
                Thread.Sleep(5000);
            }
            return "over";
        }
        private bool GetAllLike()
        {
            var request = WebRequest.CreateHttp("http://tieba.baidu.com/f/like/mylike?&pn=1");
            request.CookieContainer = cookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            /*StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();*/
            for (int i = 1; i < 30; i++)
            {
                string url = "http://tieba.baidu.com/f/like/mylike?&pn=" + i;
                var result = new HttpHelper().GetHtml(
                    new HttpItem
                    {
                        URL = url,
                        Method = "GET",
                        CookieContainer = cookie,
                        ProxyIp = Proxy,
                        ResultCookieType = ResultCookieType.CookieContainer
                    });
                HtmlDocument html = new HtmlDocument();
                html.LoadHtml(result.Html);
                var root = html.DocumentNode;
                var total = root.SelectNodes("//tr");
                if (total == null || total.Count == 1) break;
                foreach (var one in total)
                {
                    var two = one.SelectNodes("td");
                    if (two == null) continue;
                    like.Add(two[0].InnerText);
                }
            }
            return true;
        }
        private bool GetTopLike()
        {
            string url = "http://tieba.baidu.com/p/getLikeForum?";
            //uid = getTime_t().ToString ();
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
                    ProxyIp = Proxy,
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
        public bool GetLike()
        {
            like = new List<string>();
            return GetAllLike();
            //return GetTopLike();
        }
        private ulong getTime_t()
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var currTime = DateTime.Now - startTime;
            var time_t = Convert.ToUInt64(Math.Abs(currTime.TotalMilliseconds));
            return time_t;
        }
        public void Gettid(string address)
        {
            var total = address.Split('/');
            tid = total[total.Length - 1];
        }
        public bool ReadCookies(string name)
        {
            try
            {
                var fs = File.Open(name + ".cookie", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                cookie = (CookieContainer)formatter.Deserialize(fs);
                fs.Close();
                username = name;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool SaveCookies(string name)
        {
            try
            {
                var stream = File.Create(name + ".cookie");
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookie);
                stream.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool GetBarInfo(string bar)
        {
            var url = "http://tieba.baidu.com/f?";
            var info = new NameValueCollection
            {
                {"kw", HttpUtility.HtmlEncode(bar)},
                {"ie", "utf-8"},
            };
            var HttpResult = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = url + HttpHelper.DataToString(info),
                    Method = "GET",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(HttpResult.Html);
            barReplay = new Dictionary<string, string>();
            barTitle = new Dictionary<string, string>();
            var postList = html.GetElementbyId("thread_list");
            if (postList == null) return false;
            foreach (var li in postList.SelectNodes("//li"))
            {
                var one = li.ChildAttributes("data-field");
                if (one == null || one.ToList().Count == 0) continue;
                foreach (var two in one)
                {
                    var c = two.Value;
                    c = c.Replace("&quot;", "\"");
                    var obj = new JavaScriptSerializer()
                        .DeserializeObject(c) as
                        Dictionary<string, object>;
                    barReplay.Add(obj["id"].ToString(), obj["reply_num"].ToString());
                    var a = li.SelectSingleNode(li.XPath + "/div[1]/div[2]/div[1]/div[1]/a[1]");
<<<<<<< HEAD
                    if(a!=null)
=======
                    if(a==null)
                        a = li.SelectSingleNode(li.XPath + "/div[2]/div[2]/div[1]/div[1]/a[1]");
>>>>>>> 3c0ba29a802c128e45d831527f7469f76223048a
                    barTitle.Add(obj["id"].ToString(), a.InnerText);
                }
            }
            string text = string.Empty;
            try
            {
                text = HttpResult.Html.Remove(10000);
            }
            catch
            {
                return false;
            }
            //(?<=script).*(?=script)
            Regex rx = new Regex(@"(?<=tbs).*(\})",
          RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Match tbsmatch = rx.Match(text);
            string value = tbsmatch.Value;
            value = value.Replace('"', ' ');
            value = value.Replace('}', ' ');
            value = value.Replace('\'', ' ');
            value = value.Replace(':', ' ');
            tbs = value.Trim();

            rx = new Regex(@"(?<='id':).*(,)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(text);
            bool first = true;
            foreach (Match match in matches)
            {
                string id = match.Value;
                id = id.Replace(',', ' ');
                if (first)
                {
                    first = false;
                    uid = id.Trim();
                }
                else
                {
                    fid = id.Trim();
                    break;
                }
            }
            if (string.IsNullOrEmpty(tbs) ||
                string.IsNullOrEmpty(uid) ||
                string.IsNullOrEmpty(fid))
                return false;
            else return true;
        }
        protected virtual void OnSignEvent(string s)
        {
            SignEvent?.Invoke(s);
        }
    }
}
