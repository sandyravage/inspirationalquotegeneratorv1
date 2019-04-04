using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuoteGetter
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<string> quotes = new List<string>();
            //quotes.Add("Test");
            //StringBuilder export = new StringBuilder();
            //int stop = 0;
            //while (stop < 100 && quotes.Count < 100000)
            //{
            //    try {
            //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://api.forismatic.com/api/1.0/?method=getQuote&format=json&lang=en");
            //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            //        using (Stream stream = response.GetResponseStream())
            //        using (StreamReader reader = new StreamReader(stream))
            //        {
            //            var result = reader.ReadToEnd();
            //            NewObject quote = JsonConvert.DeserializeObject<NewObject>(result);
            //            var exists = quotes.Any(x => x == quote.quoteText);
            //            if (exists)
            //            {
            //                stop++;
            //                continue;
            //            }
            //            else
            //            {
            //                stop = 0;
            //                quotes.Add(quote.quoteText);
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        continue;
            //    }
            //}
            //quotes.RemoveAt(0);
            //foreach (string q in quotes)
            //{
            //    export.Append(q + "@");
            //}
            //File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt"), export.ToString());
            string all = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.txt"));
            string[] separatingChars = { "~Delimiter~" };
            string[] quotes = all.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            var newsen = new List<string>();
            List<Word> words = new List<Word>();
            foreach (var q in quotes)
            {
                var sen = q.Split(' ');
                var sb1 = new StringBuilder();
                for (int j = 0; j < sen.Length; j++)
                {
                    var sb2 = new StringBuilder();
                    foreach (char c in sen[j])
                    {
                        if (!char.IsPunctuation(c) || c == '\'')
                        {
                            sb2.Append(c);
                        }
                    }
                    sb1.Append(sb2.ToString().ToLower() + ' ');
                }
                newsen.Add(sb1.ToString().Trim());
            }
            
            foreach (var s in newsen)
            {
                var sen = s.Split(' ');
                for (int i = 0; i < sen.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = true, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                    }
                    else if (i == sen.Length - 2)
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = false, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                            words.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                            words.Add(new Word() { text = sen[i + 1], isEnd = true, isFirst = false });
                        }
                    }
                    else
                    {
                        if (!words.Exists(x => x.text == sen[i]))
                        {
                            words.Add(new Word() { text = sen[i], isFirst = false, isEnd = false });
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                        else
                        {
                            var found = words.FindIndex(x => x.text == sen[i]);
                            words[found].followedBy.Add(new Word() { text = sen[i + 1], isEnd = false, isFirst = false });
                        }
                    }
                 }
            }
            var r = new Random();
            var sb3 = new StringBuilder();
            var sb4 = new StringBuilder();
            var currentWord = new Word();
            var wordList = words.Where(x => x.isFirst == true).ToList();
            var randomStart = wordList[r.Next(0, wordList.Count)];
            for(int z = 0; z < randomStart.text.Length; z++)
            {
                if (z == 0)
                {
                    sb3.Append(randomStart.text[z].ToString().ToUpper());
                }
                else
                {
                    sb3.Append(randomStart.text[z]);
                }
            }
            sb4.Append(sb3.ToString() + ' ');
            sb3.Clear();
            currentWord = randomStart;
            while (!currentWord.isEnd)
            {
                var next = currentWord.followedBy[r.Next(0, currentWord.followedBy.Count)].text;
                var nextWord = words.First(x => x.text == next);
                sb4.Append(nextWord.text + ' ');
                currentWord = nextWord;
            }
            Console.WriteLine(sb4.ToString().Trim() + '.');
            Console.ReadKey();
        }
    }

    public class Word
    {
        public string text;
        public List<Word> followedBy = new List<Word>();
        public bool isFirst;
        public bool isEnd;
    }
}
