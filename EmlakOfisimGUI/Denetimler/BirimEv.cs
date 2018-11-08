using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EmlakCore;
using System.IO;

namespace EmlakOfisimGUI
{
    public partial class BirimEv : UserControl
    {
        public Size ilkBoyut;
        public float BoyutOran { get => ((float)ilkBoyut.Height / (float)ilkBoyut.Width); }
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   
                return cp;
            }
        }
        public Ev ev;       
        public BirimEv(Ev ev)
        {
            this.ev = ev;
            InitializeComponent();

            label11.Text = ev.Adres.Il +"/"+ ev.Adres.Ilçe;          
            arsivlabel.Visible = !ev.Aktifmi;
            label4.Text = ev.Alan.ToString();
            label9.Text = ev.Başlık;
            label15.Text = ev.EmlakNo.ToString();
            label6.Text = ev.KatNumarası.ToString();           
            label2.Text = ev.OdaSayısı.ToString();         
            label13.Text = ev.YapımTarihi.ToString("dd MMMM yyyy");
            if (ev is SatılıkEv)
            {
                label10.Text = "SATILIK";
                label10.BackColor = Color.Purple;
            }
            else
            {
                label10.Text = "KİRALIK";
                label10.BackColor = Color.Orchid;
            }

        }


        private void BirimEv_Load(object sender, EventArgs e)
        {
            label15.BackColor = label10.BackColor;
            label11.Parent = pictureBox1;
            button1.Parent = pictureBox1;
            panel5.Parent = pictureBox1;
            panel6.Parent = pictureBox1;
            panel1.Parent = pictureBox1;
            panel1.Location = kordinatfark(panel1.PointToClient(Point.Empty), pictureBox1.PointToClient(Point.Empty));
            label11.Location = kordinatfark(label11.PointToClient(Point.Empty), pictureBox1.PointToClient(Point.Empty));
            panel5.Location = kordinatfark(panel5.PointToClient(Point.Empty), pictureBox1.PointToClient(Point.Empty));            
            Random rnd = new Random();
            panel1.BackColor = Color.FromArgb(185, rnd.Next(150,240), rnd.Next(150, 240), rnd.Next(150, 240));           
            ilkBoyut = this.Size;



            button1_Click_1(null, null);
        }

        Point kordinatfark(Point p1,Point  p2)
        {
            Point yeni = Point.Subtract(p2, (Size)p1);
            return yeni;
        }
        public void aktifDurumYenile()
        {
           arsivlabel.Visible = !ev.Aktifmi;          
        }
     
        private void materialCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           panel6.Visible = !panel6.Visible;
        }

        FileInfo[] resimList;
        int resimSıra;

        private void button1_Click_1(object sender, EventArgs e)
        {
            resimÖnizlemeSıraAtla();
        }
        public void resimÖnizlemeSıraAtla(bool baslangıcSıfırla = false)
        {
            if (resimList == null)
            {
                DosyaIO io = DosyaIO.Oluştur();
                resimList = io.resimList(ev.EmlakNo);
                if (resimList == null)
                {
                    resimList = new FileInfo[] { new FileInfo(Global.NullResimYol) };
                }
            }
            if (baslangıcSıfırla)
            {
                resimSıra = 0;
            }
            pictureBox1.Image = EmlakIO.resimDeepCopy(resimList[resimSıra].FullName);            
            label12.Text = (resimSıra + 1).ToString() + "/" + resimList.Count().ToString();
            resimSıra++;
            if (resimSıra == resimList.Count())
            {
                resimSıra = 0;
            }
        }
       
        private void label9_SizeChanged(object sender, EventArgs e)
        {
            EmlakIO eio = EmlakIO.Oluştur();
            eio.OtomatikScale(ref label9);

        }

        private void label9_TextChanged(object sender, EventArgs e)
        {
            label9_SizeChanged(null, null);
        }
    }
}
