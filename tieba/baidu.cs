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
    public class baidu
    {
        public delegate void SignDelegate(string s);

        public event SignDelegate SignEvent;
        private string replaycodestr = string.Empty;
        private string replaycodetype = string.Empty;
        public string Proxy { get; set; }
        public string barname { get; set; }
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
        public Dictionary<string, string> barinfo = new Dictionary<string, string>();
        public baidu()
        {
            Proxy = "ieproxy";
        }

        public void setProxy(string ip)
        {
            Proxy = ip;
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
        public string IsgetcodeString(string username)
        {
            //https
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
            try
            {
                var obj = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html);
            }
            catch
            {
                return "操作超时";
            }

            var json1 = new JavaScriptSerializer().
                DeserializeObject(httpResult.Html)
                as Dictionary<string, object>;
            var j1 = json1["data"] as Dictionary<string, object>;
            if (!j1.ContainsKey("codeString") || j1["codeString"] == null)
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
        public bool SetLoginCode(string input)
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
        public void GetPSTM()
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
            if (cookies == null) return;
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
        public bool replay(string bar, string content, string title = "")
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
                return true;
            }
            return false;
        }
        public string Sign(string bar)
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
            var json1 = new JavaScriptSerializer().
                DeserializeObject(result.Html)
                as Dictionary<string, object>;
            var tbs = json1["tbs"] as string;
            var info = new NameValueCollection
            {
                {"ie","utf-8" },
                {"kw", bar},
                {"tbs",tbs },
            };
            result = new HttpHelper().GetHtml(
                new HttpItem
                {
                    URL = "http://tieba.baidu.com/sign/add",
                    Method = "POST",
                    CookieContainer = cookie,
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer,
                    Postdata = HttpHelper.DataToString(info)
                });
            return bar;
        }
        public string SignReady()
        {
            var result = string.Empty;
            GetPSTM();
            if (!GetLike())
                return error;
            else
                return result;
        }
        public string UpLoadImage(string path,string refre)
        {
            var imageTbs = getImageTbs();
            var url = "http://upload.tieba.baidu.com/upload/pic?";
            var nvc = new NameValueCollection
            {
                {"tbs",imageTbs },
                {"fid",fid },
                {"save_yun_album","1" },
            };
            url += HttpHelper.DataToString(nvc);
            string boundary = "----WebKitFormBoundarykz17xp0q7FAd2CVd";
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            webrequest.ContentLength = 0;
            webrequest.KeepAlive = true;
            webrequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            webrequest.Accept = "*/*";
            webrequest.Referer = "http://tieba.baidu.com/p/"+refre;
            webrequest.Host = "upload.tieba.baidu.com";
            webrequest.CookieContainer = cookie;
            //webrequest.Headers.Add("Cookie: " + cookie);
            webrequest.Headers.Add("Origin: " + "http://tieba.baidu.com");


            StringBuilder sb = new StringBuilder();
            var list = path.Split('\\');
            var filename = list[list.Length - 1];
            list = filename.Split('.');
            var filetype = list[list.Length - 1];
            sb.Append("\r\n--"+boundary+"\r\n");
            sb.Append("Content-Disposition: form-data; name=\"file\"; filename=\"" + filename+ "\"\r\n");
            sb.Append("Content-Type: image/"+filetype+"\r\n\r\n");
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());

            byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "-–\r\n");

            var fs = File.OpenRead(path);
            byte[] buffer = new byte[fs.Length];
            var length = postHeaderBytes.Length + fs.Length + boundaryBytes.Length;

            webrequest.ContentLength = length;
            fs.Read(buffer, 0, buffer.Length);
            Stream requestStream = webrequest.GetRequestStream();
            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            StreamReader sr = new StreamReader(s);

            var HttpResult = sr.ReadToEnd();
            var obj = new JavaScriptSerializer().
                DeserializeObject(HttpResult)
                as Dictionary<string, object>;
            if (obj["err_no"].ToString() != "0")
                return "error";
            var info = obj["info"] as Dictionary<string, object>;
            var pic_id_encode = info["pic_id_encode"].ToString();
            var fullpic_width = info["fullpic_width"].ToString();
            var fullpic_height = info["fullpic_height"].ToString();
            var pic_type = info["pic_type"].ToString();
            var width = Convert.ToInt32(fullpic_width);
            var height = Convert.ToInt32(fullpic_height);

            if(width>560)
            {
                var k = 560.0 / width;
                width = Convert.ToInt32(width * k);
                height = Convert.ToInt32(height * k);
            }
            string result = "[img+pic_type=" + pic_type;
            result += "+width=" + width + "+height=" + height + "]";
            result += "http://imgsrc.baidu.com/forum/pic/item/" + pic_id_encode + ".jpg";
            result += "[/img]";
            return result;
            /*
             * pic_id_encode:"8d82c1cb39dbb6fd40cf75420024ab18962b3792"
             * fullpic_width:700
             * fullpic_height:490
             * pic_type:1
             * "[img+pic_type=0+width=32+height=32]
             * http://imgsrc.baidu.com/forum/pic/item/596da5dde71190efd4b163bbc71b9d16fcfa60e1.jpg
             * [/img]"
             max=560;
             <img class="BDE_Image" pic_type="0" width="560" height="392" 
             src="http://imgsrc.baidu.com/forum/pic/item/8d82c1cb39dbb6fd40cf75420024ab18962b3792.jpg" pic_ext="jpeg"  > 
             */
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
                OnSignEvent(Sign(one));
                Thread.Sleep(3000);
            }
            return "签到完成";
        }
        private bool GetAllLike()
        {
            var request = WebRequest.CreateHttp("http://tieba.baidu.com/f/like/mylike?&pn=1");
            request.CookieContainer = cookie;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();
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
                    like.Add(two[0].FirstChild.InnerText);
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
        private uint getTime_t()
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var currTime = DateTime.Now - startTime;
            uint time_t = Convert.ToUInt32(Math.Abs(currTime.TotalMilliseconds));
            return time_t;
        }
        public void Gettid(string address)
        {
            var total = address.Split('/');
            tid = total[total.Length - 1];
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
            barinfo = new Dictionary<string, string>();
            var PostList = html.GetElementbyId("thread_list");
            if (PostList == null) return false;
            foreach (var li in PostList.SelectNodes("//li"))
            {
                var one = li.ChildAttributes("data-field");
                if (one == null) continue;
                foreach (var two in one)
                {
                    var c = two.Value;
                    c = c.Replace("&quot;", "\"");
                    var obj = new JavaScriptSerializer()
                        .DeserializeObject(c) as
                        Dictionary<string, object>;
                    barinfo.Add(obj["id"].ToString(), obj["reply_num"].ToString());
                }
            }
            string text = HttpResult.Html.Remove(10000);
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
