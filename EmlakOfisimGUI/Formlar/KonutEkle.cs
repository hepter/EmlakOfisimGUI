using EmlakCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmlakOfisimGUI
{
    public partial class KonutEkle : Form
    {
        private Ev ev;
        private int emlakID=0;
        private TUR seciliTUR = TUR.satılık;
        Logger log = Logger.Oluştur();
        public int EmlakID
        {
            get
            {
                if (ev != null)
                {
                    return ev.EmlakNo;
                }
                else if(emlakID == 0)
                {

                    emlakID = new Random().Next(100000000, 999999999);
                    log.logEkle(emlakID + " Yeni konut eklemek için Random ID üretildi", LOGSEVIYE.bilgi);
                    return emlakID;
                }
                else
                {
                    log.logEkle(emlakID + " KonutID Çağrıldı", LOGSEVIYE.bilgi);             
                   return emlakID;
                }
            }
            set
            {
                emlakID = value;
                label13.Text = EmlakID.ToString();
            }
        }
        public Ev Ev
        {
            get
            {
                return ev;
            } 
            set
            {
                ev = value;
                evNesneBilgiYükle(ev);
                log.logEkle("ev nesnesi Düzenlemek için forma yüklendi", LOGSEVIYE.bilgi);
            }
        }

        private KonutEkle()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.MaxDate = DateTime.Now.AddMinutes(1);
            EmlakIO io = EmlakIO.Oluştur();
            io.JsonOku();
            log.logEkle("KonutEkle için Json bilgileri okundu", LOGSEVIYE.bilgi);
            if (ev == null)
            {
                EnumComboboxYükle();
                label13.Text = EmlakID.ToString();
            }
            illeriComboboxYükle();
        }
        private EKLEMETURU aktifEtür;
        public KonutEkle(EKLEMETURU etür):this()
        {
            aktifEtür = etür;
            switch (etür)
            {
                case EKLEMETURU.yeniekle:
                    materialRaisedButton4.Text = "ekle";
                    label1.Text = "Yeni Konut Ekle";
                    log.logEkle("KonutEkle üzerinde yeni konut ekleme açıldı", LOGSEVIYE.bilgi);
                    break;
                case EKLEMETURU.güncelle:
                    materialRaisedButton4.Text = "güncelle";
                    label1.Text = "Konut Güncelle & Görüntüle";
                    log.logEkle("KonutEkle üzerinde konut düzenleme açıldı", LOGSEVIYE.bilgi);
                    break;              
            }

        }


        public void illeriComboboxYükle(EmlakIO iom = null)
        {
            if (comboBox1.Items.Count > 1) return;
            EmlakIO io = EmlakIO.Oluştur();
            comboBox1.DataSource = io.AdresYükle(0, "");
            comboBox1.SelectedIndex = -1;
        }
        public void EnumComboboxYükle()
        {
            comboBox5.Items.AddRange(Enum.GetNames(typeof(EVTIPI)).Select(a => a.Replace("_", " ")).ToArray());
            comboBox5.SelectedIndex = -1;
        }
     

        private void evNesneBilgiYükle(Ev ev)
        {
            this.ev = ev;
            EnumComboboxYükle();     
            if (ev is SatılıkEv)            
                materialRadioButton1.Checked = true;
            
            else            
                materialRadioButton2.Checked = true;
            
            gerekliAdresComboboxSec(ev.Adres);
            materialCheckBox1.Checked = !ev.Aktifmi;
            numericUpDown1.Value = ev.Alan;
            textBox1.Text = ev.Başlık;
            label13.Text = ev.EmlakNo.ToString();
            numericUpDown6.Value = ev.KatNumarası;
            textBox2.Text = ev.Not;
            numericUpDown3.Value = ev.OdaSayısı;           
            comboBox5.SelectedIndex= (int)ev.Tipi;
            dateTimePicker1.Value = ev.YapımTarihi;
            resimDoldur(ev.Resimler);
            if (ev is SatılıkEv)
            {
                numericUpDown2.Value = ((SatılıkEv)ev).Fiyat;
                groupBox13.Visible = false;
                groupBox14.Visible = false;
            }
            else
            {
                numericUpDown4.Value = ((KiralıkEv)ev).Depozito;
                label6.Text = ((KiralıkEv)ev).Kira.ToString();
                groupBox9.Visible = false;
            }
        }

        public void MiniResim_ClickEvent(object o ,EventArgs args)
        {
            pictureBox5.Image = ((PictureBox)o).Image;

            foreach (MiniResim res in flowLayoutPanel2.Controls)            
                res.TıklandıMı = false;
            
            ((MiniResim)((PictureBox)o).Parent).TıklandıMı = true;
        }
            
            

        private void resimDoldur(FileInfo[] resimler)
        {
            flowLayoutPanel2.Controls.Clear();
            if (resimler==null ||resimler.Count()==0)
            {
                pictureBox5.Image = EmlakIO.resimDeepCopy(Global.NullResimYol);               
                return;
            }
            foreach (FileInfo f in resimler)
            {
                MiniResim res = new MiniResim(f.FullName);
                res.pictureBox1.Click += MiniResim_ClickEvent;
                flowLayoutPanel2.Controls.Add(res);
            }
            pictureBox5.Image = EmlakIO.resimDeepCopy(resimler[0].FullName);        
            ((MiniResim)flowLayoutPanel2.Controls[0]).TıklandıMı = true;
        }


        Point yedekFare;
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            yedekFare = new Point(-e.X, -e.Y);
        }
        private void button1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                Point mouse = Control.MousePosition;
                mouse.Offset(yedekFare.X, yedekFare.Y);
                this.Location = mouse;
            }
        }

      

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TümComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;
            ComboBox sonrakiCbox;
            int boxSıra = int.Parse(cbox.Name.Replace("comboBox", ""));              
            
            EmlakIO io = EmlakIO.Oluştur();
            sonrakiCbox = ((ComboBox)cbox.Parent.Controls["comboBox" + (boxSıra + 1)]);
            sonrakiCbox.DataSource = io.AdresYükle(boxSıra, cbox.SelectedValue.ToString());           
            ComboTemizle(boxSıra);
            if (aktifEtür==EKLEMETURU.yeniekle)
            {
                sonrakiCbox.SelectedIndex = -1;
            }
            
        }
        private void gerekliAdresComboboxSec(Adres adres)
        {
            string[] adr = adres.AdresDataStr.Split(Global.böl2);

            for (int i = 0; i < 3; i++)
            {
                ComboBox cBox = (ComboBox)groupBox3.Controls["comboBox" + (i + 1)];
                for (int j = 0; j < cBox.Items.Count; j++)
                {
                    if (cBox.Items[j].ToString()== adr[i])
                    {
                        cBox.SelectedIndex = j;
                    }
                }                 
                TümComboBox_SelectionChangeCommitted(cBox, null);
                if (adr[i] == null || adr[i] == "") break;
            }
        }

        public void ComboTemizle(int HaricOlanNo)
        {
            for (int i = HaricOlanNo + 2; i < 5; i++)
            {

                ComboBox cbox = (ComboBox)groupBox3.Controls["comboBox" + i];
                cbox.DataSource = null;
            }


            for (int i = HaricOlanNo + 1; i < 5; i++)
            {
                ComboBox cbox = (ComboBox)groupBox3.Controls["comboBox" + i];
                if (cbox.Items.Count == 0)
                {
                    cbox.Enabled = false;
                    cbox.DataSource = null;
                }
                else
                {
                    cbox.Enabled = true;
                    ComboTemizle(ref cbox);
                }

            }
        }
        public void ComboTemizle(ref ComboBox box)
        {
            box.DataSource = box.Items.Cast<string>().Select(a => a.ToString()).Where(aa => aa.Trim() != "").ToArray();
        }

        DosyaIO io = DosyaIO.Oluştur();   
        private void materialRaisedButton1_Click(object sender, EventArgs e)//ResimleriEkle
        {
            if (openFileDialog1.ShowDialog()!=DialogResult.OK) return;
            

            foreach (string yol in openFileDialog1.FileNames)
            {
                cacheResimler.Add(io.resimEkle(yol, EmlakID));                
            }
            resimDoldur(io.resimList(EmlakID));
        }
        private List<string> cacheResimler = new List<string>();
        private List<int> silinecekResimler = new List<int>();
        private void materialRaisedButton2_Click(object sender, EventArgs e)//ResimSil
        {
            foreach (MiniResim res in flowLayoutPanel2.Controls)
            {
                if (res.TıklandıMı)
                {               
                    silinecekResimler.Add(res.resimNo);
                    io.randomResim(EmlakID);                    
                    flowLayoutPanel2.Controls.Remove(res);
                    break;
                }
            }

            if (flowLayoutPanel2.Controls.Count==0)
            {
                pictureBox5.Image = EmlakIO.resimDeepCopy(Global.NullResimYol);
            }
        
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)//KlasörAç
        {
            if (Directory.Exists(io.resimDizinKonum(EmlakID)))
            {
                Process.Start(io.resimDizinKonum(EmlakID));
            }
            else
            {
                MessageBox.Show("Resim Mevcut Değil!","Resim yok",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
           
        }
      

        private void mbox(string msg,string başlık)
        {
            MessageBox.Show(msg,başlık,MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }
        private bool girdiKontrol(TUR tür)
        {           
            if (textBox1.Text.Trim().Length <10)
            {
                mbox("Başlık 10 karakterden kısa olamaz!","Başık Kısa");
                textBox1.SelectAll();
                return false;
            }

            if (numericUpDown1.Value == 0)
            {
                mbox("Metrekare Sıfır olamaz!", "Alan girili değil");
                numericUpDown1.Focus();
                return false;
            }

            if (numericUpDown2.Value == 0 && tür== TUR.satılık)
            {
                mbox("Fiyat Bilgisi Sıfır olamaz!", "Fiyat girili değil");
                numericUpDown2.Focus();
                return false;
            }
            if (numericUpDown3.Value == 0)
            {
                mbox("Oda Sayısı Sıfır olamaz!", "Oda Sayısı girili değil");
                numericUpDown3.Focus();
                return false;
            }
           
            if ((DateTime.Now.Subtract(dateTimePicker1.Value).TotalDays/365)>100)
            {
                mbox("Bina 100 Yıldan Eski olamaz!", "Geçersiz Tarih");
                dateTimePicker1.Focus();
                return false;
            }
            if (comboBox1.SelectedIndex==-1)
            {
                mbox("Lütfen Geçerli Bir il Seçiniz!", "İl seçili değil");
                comboBox1.Focus();
                return false;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                mbox("Lütfen Geçerli Bir ilçe Seçiniz!", "İlöe seçili değil");
                comboBox2.Focus();
                return false;
            }
            if (comboBox5.SelectedIndex == -1)
            {
                mbox("Lütfen Geçerli Bir Konut türünü seçin!", "Konut Türü geçerli değil");
                comboBox5.Focus();
                return false;
            }
            return true;
        }

        public void resimİşlemiTamamla(DialogResult snc)//Cancel yapıldığında eklenen resimlerin geçersiz kılınması
        {
            switch (snc)
            {                
                case DialogResult.OK:
                    foreach (int ss in silinecekResimler)
                    {
                        io.resimSil(EmlakID, ss);                       
                    }
                    break;
                case DialogResult.Cancel:
                    foreach (string ss in cacheResimler)
                    {                       
                        File.SetAttributes(ss, FileAttributes.Normal);
                        File.Delete(ss);
                    }
                    break;
            }
            log.logEkle($"Resim düzenlemeri iptal edildi.Kayıt tuşuna basılmadı!", LOGSEVIYE.uyarı);
        }
        private void materialRaisedButton4_Click(object sender, EventArgs e)//Ekle & Güncelle
        {

            if (!girdiKontrol(seciliTUR))
            {
                this.DialogResult = DialogResult.No;
                log.logEkle("Eksik veriler ile konut ekleme uyarısı!",LOGSEVIYE.uyarı);
                return;
            }
                
           
            string adresStr = "";
            string bol = Global.böl2.ToString();
            adresStr += comboBox1.SelectedValue + bol;
            adresStr += comboBox2.SelectedValue + bol;
            adresStr += comboBox3.SelectedValue + bol;
            adresStr += comboBox4.SelectedValue;
            Adres adres = new Adres(adresStr);

            int yedekEvID= EmlakID;
            if (seciliTUR==TUR.satılık)//kiralık veya satılık
            {
                ev = new SatılıkEv((int)numericUpDown3.Value, adres, (int)numericUpDown1.Value, EVTIPI.Daire, true, textBox1.Text, (int)numericUpDown2.Value, (int)numericUpDown6.Value,dateTimePicker1.Value,textBox2.Text);
            }
            else
            {
                ev = new KiralıkEv((int)numericUpDown3.Value, adres, (int)numericUpDown1.Value, EVTIPI.Daire, true, textBox1.Text, (int)numericUpDown4.Value,0, (int)numericUpDown6.Value, dateTimePicker1.Value, textBox2.Text);
            }
           
            if (aktifEtür==EKLEMETURU.yeniekle)
                MessageBox.Show("Konut Başarıyla Eklendi!", "Eklendi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else            
                MessageBox.Show("Konut Başarıyla Güncellendi!","Güncellendi",MessageBoxButtons.OK,MessageBoxIcon.Information);     

            ev.EmlakNo = yedekEvID;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialRadioButton1.Checked)
            {
                groupBox13.Visible = false;
                groupBox14.Visible = false;
                groupBox9.Visible = true;
                seciliTUR = TUR.satılık;
            }
            else
            {
                groupBox13.Visible = true;
                groupBox14.Visible = true;
                groupBox9.Visible = false;
                seciliTUR = TUR.kiralık;
            }
        }

        private void KonutEkle_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(this.DialogResult==DialogResult.OK )&& !(this.DialogResult == DialogResult.Cancel))
            {
                e.Cancel = true;
            }
            resimİşlemiTamamla(this.DialogResult);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            DosyaIO io = DosyaIO.Oluştur();
            label6.Text = (io.FiyatKatsayı * numericUpDown3.Value).ToString();
        }
    }
    
}
