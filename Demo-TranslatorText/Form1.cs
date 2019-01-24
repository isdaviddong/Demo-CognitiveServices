using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo_TranslatorText
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = MSTranslatorUtility.Detect(this.textBox1.Text);
            this.textBox2.Text = $"最有可能的語系是 : { result.FirstOrDefault().language}";
            this.textBox2.Text += $"\r\n分數 : { result.FirstOrDefault().score}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = MSTranslatorUtility.Translate(this.textBox1.Text, "en"); //zh-Hant 為翻譯成中文
            this.textBox2.Text = $"目標語系 : { result.FirstOrDefault().translations.FirstOrDefault().to}";
            this.textBox2.Text += $"\r\n翻譯結果 : { result.FirstOrDefault().translations.FirstOrDefault().text}";
        }
    }

    public class MSTranslatorUtility
    {
        const string MSTranslatorTextKey = ""; //填入你自己的key
        const string MSTranslatorTextHost = "https://api.cognitive.microsofttranslator.com";

        public static List<MSTranslatorTextDetectResult> Detect(string text)
        {
            string path = "/detect?api-version=3.0"; //偵測語系
            string uri = MSTranslatorTextHost + path;
            System.Object[] body = new System.Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", MSTranslatorTextKey);
                //取得http get結果
                var response = client.SendAsync(request).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                //將取得的結果反序列化為物件
                var result = JsonConvert.DeserializeObject<List<MSTranslatorTextDetectResult>>(responseBody);
                return result;
            }
        }

        public static List<MSTranslatorTextTranslateResult> Translate(string text, string LanguageCode)
        {
            // Translate to German and Italian.
            string path = "/translate?api-version=3.0"; //翻譯
            string params_ = "&to=" + LanguageCode; //目標語系

            string uri = MSTranslatorTextHost + path + params_;

            System.Object[] body = new System.Object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", MSTranslatorTextKey);

                var response = client.SendAsync(request).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MSTranslatorTextTranslateResult>>(responseBody);
                return result;
            }
        }
    }

    public class MSTranslatorTextDetectResult
    {
        public string language { get; set; }
        public double score { get; set; }
        public bool isTranslationSupported { get; set; }
        public bool isTransliterationSupported { get; set; }
        public List<Alternative> alternatives { get; set; }
    }

    public class Alternative
    {
        public string language { get; set; }
        public double score { get; set; }
        public bool isTranslationSupported { get; set; }
        public bool isTransliterationSupported { get; set; }
    }

    public class DetectedLanguage
    {
        public string language { get; set; }
        public double score { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }

    public class MSTranslatorTextTranslateResult
    {
        public DetectedLanguage detectedLanguage { get; set; }
        public List<Translation> translations { get; set; }
    }
}
