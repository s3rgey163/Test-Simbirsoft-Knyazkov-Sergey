using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace test
{
   
    class HtmlProc
    {
        public bool GetHtmlPage(string url, out string page)
        {          
            bool download = false;
            page = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UseDefaultCredentials = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        page = reader.ReadToEnd();
                    }
                }
                response.Close();

                download = true;
            }
            catch (ArgumentException e)
            {
                Logs.Error(e);
            }
            catch (UriFormatException e)
            {
                Logs.Error(e);
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                {
                    Logs.Error(e.InnerException);
                }
                else
                {
                    Logs.Error(e);
                }
            }
            catch (NotSupportedException e)
            {
                Logs.Error(e);
            }
            return download;
        }

        public static string GetName(string url)
        {
            byte i = 0;
            string temp;
            if (url.Contains("www"))
            {
                temp = url.Split('.')[1];
            }
            else
            {
                temp = url.Split('.')[0];
                while (!temp[i].Equals(':'))
                {
                    i++;
                }
                i = (byte)(i + 3);
                temp = temp.Substring(i);
            }
           
            return temp;
        }

        public Dictionary<string,int> GetPairs(string[] words)
        {
            Dictionary<string, int> wordsPairs = new Dictionary<string, int>();
            for(int i = 0; i < words.Length;i++)
            {
                if(!wordsPairs.ContainsKey(words[i].ToLower()))
                {
                    wordsPairs.Add(words[i].ToLower(), 1);
                }
                else
                {
                    int value;
                    wordsPairs.TryGetValue(words[i].ToLower(), out value);
                    wordsPairs.Remove(words[i].ToLower());
                    value++;
                    wordsPairs.Add(words[i].ToLower(), value);
                }
            }
            return wordsPairs;
        }


        public bool GetAllWords(string url, out string[] words)
        {
            string temp ="" ;
            words = null;
            bool isDone = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UseDefaultCredentials = true;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    temp = reader.ReadToEnd();
                }
                response.Close();

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(temp);
                temp = "";

                Regex regex = new Regex(@"[a-z]\w*|[а-я]\w*", RegexOptions.IgnoreCase);

                var nodes = htmlDoc.DocumentNode.Descendants().Where(n =>
                    n.NodeType == HtmlNodeType.Text &&
                    n.ParentNode.Name != "script" &&
                    n.ParentNode.Name != "style");
                foreach (HtmlNode node in nodes)
                {
                    if (regex.IsMatch(node.InnerText))
                    {
                        temp += node.InnerText +"\n";
                    }

                }

                string pattern = "\",/|!?;.':»«&*@#$[](){}-–—~`+=^%0123456789" + "\n" + "\r" + "\t" + " " + " ";//список разделителей
                words = temp.Split(pattern.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                isDone = true;
                }
            catch (ArgumentException e)
            {
                Logs.Error(e);
            }
            catch (UriFormatException e)
            {
                Logs.Error(e);
            }
            catch (WebException e)
            {
                if (e.InnerException != null)
                {
                    Logs.Error(e.InnerException);
                }
                else
                {
                    Logs.Error(e);  
                }
            }
            catch (NotSupportedException e)
            {
                Logs.Error(e);
            }
            return isDone;
        }    
    }
}
