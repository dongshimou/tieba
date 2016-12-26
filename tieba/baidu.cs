﻿using System;
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
        private List<string> like = new List<string>();
        public CookieContainer cookie = new CookieContainer();
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
                   ProxyIp = Proxy,
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
        public bool IsgetcodeString(string username)
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
                    ProxyIp = Proxy,
                    ResultCookieType = ResultCookieType.CookieContainer
                });
            if(httpResult.Html== "无法连接到远程服务器")return false;
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
        public bool SetPostCode(string input)
        {
            var url = "http://tieba.baidu.com/f/commit/commonapi/checkVcode";
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
        public bool codereplay(string vcode, string bar, string content)
        {
            var url = "http://tieba.baidu.com/f/commit/post/add";
            var info = new NameValueCollection
            {
                 {"ie", "utf-8"},
                 {"kw", bar},
                 {"fid", fid},
                 {"tid", tid},
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
                 {"__type__","reply" },
            };
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
        public bool codepost(string vcode, string bar, string title, string content)
        {
            var url = "http://tieba.baidu.com/f/commit/thread/add";
            var info = new NameValueCollection
            {
                {"ie", "utf-8"},
                {"kw", bar},
                {"fid", fid},
                {"tid", "0"},
                {"vcode_md5", replaycodestr},
                {"floor_num", "0"},
                {"rich_text", "1"},
                {"tbs", tbs},
                {"content", content},
                {"title", title},
                {"lp_type", "1"},
                {"lp_sub_type", "1"},
                {"repostid", null},
                {"talk_type", null},
                {"new_vcode", "1"},
                {"tag", "11"},
                {"vcode",vcode },
                {"__type__", "thread"},
            };
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
            return (obj["err_code"].ToString() == "0");
        }
        public bool post(string bar, string title, string content)
        {
            replaycodestr = string.Empty;
            var url = "http://tieba.baidu.com/f/commit/thread/add";
            var info = new NameValueCollection
            {
                {"ie", "utf-8"},
                {"kw", bar},
                {"fid", fid},
                {"tid", "0"},
                {"vcode_md5", replaycodestr},
                {"floor_num", "0"},
                {"rich_text", "1"},
                {"tbs", tbs},
                {"content", content},
                {"title", title},
                {"lp_type", "1"},
                {"lp_sub_type", "1"},
                {"repostid", null},
                {"talk_type", null},
                {"new_vcode", "1"},
                {"tag", "11"},
                {"__type__", "thread"},
            };
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
        public bool replay(string bar, string content)
        {
            replaycodestr = string.Empty;
            var url = "http://tieba.baidu.com/f/commit/post/add";
            var info = new NameValueCollection
            {
                 {"ie", "utf-8"},
                 {"kw", bar},
                 {"fid", fid},
                 {"tid", tid},
                 {"vcode_md5", replaycodestr},
                 {"floor_num", "0"},
                 {"rich_text","1"},
                 {"tbs",tbs},
                 {"content",content},
                 {"lp_type", "1"},
                 {"lp_sub_type", "1"},
                 {"repostid", null},
                 {"talk_type", null},
                 {"new_vcode", "1"},
                 {"tag","11" },
                 {"__type__","reply" },
            };
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
        public string Signall()
        {
            string result = string.Empty;
            GetPSTM();
            if (!GetLike())
                return error;
            foreach (var one in like)
            {
                Sign(one);
                Thread.Sleep(3000);
            }
            result = "签到完成";
            return result;
        }
        public bool GetLike()
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
            if (HttpResult.Html.Length < 10001) return false;
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
    }
}
