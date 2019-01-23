using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo_CognitiveServices
{
    public partial class Form1 : Form
    {
        string ComputerVisionServiceKey = "f3wfqx2fdad124129sas0af96feac7a"; //更換成你自己的 Key
        string ComputerVisionServiceEndpoint = "https://southeastasia.api.cognitive.microsoft.com/vision/v1.0";  //更換成你的endpoint

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "*.png|*.png|*.jpg|*.jpg";
            if (d.ShowDialog() == DialogResult.OK)
                this.textBox1.Text = d.FileName;

            this.pictureBox1.Image = Image.FromFile(this.textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var msg = "";

            //取得圖片檔案FileStream
            byte[] file = System.IO.File.ReadAllBytes(this.textBox1.Text);
            Stream MemStream1 = new MemoryStream(file);
            Stream MemStream2 = new MemoryStream(file);
            //繪圖用
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(MemStream2);
            Graphics g = Graphics.FromImage(bmp);
            //ComputerVision instance
            var visionClient = new Microsoft.ProjectOxford.Vision.VisionServiceClient(
               ComputerVisionServiceKey, ComputerVisionServiceEndpoint);
            //分析用
            using (MemStream1)
            {
                //分析圖片
                var Results = visionClient.AnalyzeImageAsync(
                    MemStream1, new VisualFeature[] { VisualFeature.Faces, VisualFeature.Description }).Result;
                //分別保存性別數量
                int isM = 0, isF = 0;
                //如果找到臉，就畫方框標示出來
                foreach (var item in Results.Faces)
                {
                    var faceRect = item.FaceRectangle;
                    //畫框
                    g.DrawRectangle(
                                new Pen(Brushes.Red, 3),
                                new Rectangle(faceRect.Left, faceRect.Top,
                                    faceRect.Width, faceRect.Height));
                    //在方框旁邊顯示年紀
                    var age = 0;
                    if (item.Gender.StartsWith("F")) age = item.Age - 2; else age = item.Age;
                    //劃出數字
                    g.DrawString(age.ToString(), new Font(SystemFonts.DefaultFont.FontFamily, 30, FontStyle.Bold),
                        new SolidBrush(Color.Yellow),
                        faceRect.Left + 3, faceRect.Top + 3);
                    //紀錄性別數量
                    if (item.Gender.StartsWith("M"))
                        isM += 1;
                    else
                        isF += 1;
                }
                //圖片分析結果
                msg += $"\n\r圖片說明：\n\r{Results.Description.Captions[0].Text}";

                //如果update了照片，則顯示新圖
                if (Results.Faces.Count() > 0)
                {
                    msg += String.Format("\n\r找到{0}張臉, \n\r{1}男 {2}女", Results.Faces.Count(), isM, isF);
                }
            }
            this.textBox2.Text = msg;
            this.pictureBox2.Image = bmp;
        }


    }
}
