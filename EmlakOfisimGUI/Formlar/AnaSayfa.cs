using EmlakCore;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace EmlakOfisimGUI
{
    public partial class AnaSayfa : MaterialForm
    {
        Logger log = Logger.Oluştur();
        //DoubleBuffered etiketini tüm elemanlar için açar
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private int seçiliEvSayısı
        {
            get
            {
                int seçiliSayı = 0;
                foreach (BirimEv ev in flowLayoutPanel1.Controls)
                {
                    if (ev.materialCheckBox1.Checked) seçiliSayı++;
                }
                return seçiliSayı;
            }
        }
        private int evSayısı
        {
            get
            {
                return flowLayoutPanel1.Controls.Count;
            }
        }
        public AnaSayfa()
        {

            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal400, Primary.Teal600, Primary.DeepOrange400, Accent.DeepOrange200, TextShade.WHITE);

        }

        public void EnumComboboxYükle()
        {
            comboBox5.Items.AddRange(Enum.GetNames(typeof(EVTIPI)).Select(a => a.Replace("_", " ")).ToArray());
            comboBox5.SelectedIndex = -1;
        }

        private void AnaSayfa_Load(object sender, EventArgs e)
        {

            DosyaIO dio = DosyaIO.Oluştur();
            dateTimePicker1.MaxDate = DateTime.Now;
            foreach (SatılıkEv item in dio.emlakVeriOku(TUR.satılık))
                yeniBirimEvEkle(item);
            foreach (KiralıkEv item in dio.emlakVeriOku(TUR.kiralık))
                yeniBirimEvEkle(item);

            EnumComboboxYükle();
            panelOtoSündür();
            EmlakIO io = EmlakIO.Oluştur();
            io.JsonOku();
            illeriComboboxYükle(io);
            FiltreUygula();
        }



        private void TümComboBoxAnasayfa_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox cbox = (ComboBox)sender;
            ComboBox sonrakiCbox;
            int boxSıra = int.Parse(cbox.Name.Replace("comboBox", ""));
            EmlakIO io = EmlakIO.Oluştur();
            sonrakiCbox = ((ComboBox)cbox.Parent.Controls["comboBox" + (boxSıra + 1)]);
            sonrakiCbox.DataSource = io.AdresYükle(boxSıra, cbox.SelectedValue.ToString());
            ComboTemizle(boxSıra);
            sonrakiCbox.SelectedIndex = -1;
            label16.ForeColor = Color.Red;

        }
        private bool comboBoxFiltreTest(BirimEv ev)
        {

            if (!(comboBox1.SelectedIndex == -1 || ev.ev.Adres.Il == comboBox1.SelectedValue.ToString()))
                return false;
            if (!(comboBox2.SelectedIndex == -1 || ev.ev.Adres.Ilçe == comboBox2.SelectedValue.ToString()))
                return false;
            if (!(comboBox3.SelectedIndex == -1 || ev.ev.Adres.Mahalle == comboBox3.SelectedValue.ToString()))
                return false;
            if (!(comboBox4.SelectedIndex == -1 || ev.ev.Adres.Sokak == comboBox4.SelectedValue.ToString()))
                return false;
            return true;
        }

        private bool aramaFiltreTest(BirimEv ev)
        {

            if (materialSingleLineTextField1.Text.Trim() == "") return true;
            string metin = ev.ev.Başlık;
            if (materialCheckBox1.Checked) metin += ev.ev.Not;
            metin = metin.ToLower();
            if (metin.Contains(materialSingleLineTextField1.Text.Trim().ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private bool fiyatFiltreTest(BirimEv ev)
        {
            if (label2.ForeColor == Color.Black) return true;
            int fiyat = ev.ev.FiyatHesapla();
            if (fiyat > (int)numericUpDown1.Value && fiyat < (int)numericUpDown2.Value)
            {
                return true;
            }
            return false;
        }
        private bool alanFiltreTest(BirimEv ev)
        {
            if (label9.ForeColor == Color.Black) return true;
            int alan = ev.ev.Alan;
            if (alan > (int)numericUpDown6.Value && alan < (int)numericUpDown3.Value)
            {
                return true;
            }
            return false;
        }
        private bool durumFiltreTest(BirimEv ev)
        {
            TUR tip = (ev.ev is SatılıkEv) ? TUR.satılık : TUR.kiralık;
            switch (tip)
            {
                case TUR.satılık:
                    if (materialCheckBox3.Checked) return true;
                    break;
                case TUR.kiralık:
                    if (materialCheckBox4.Checked) return true;
                    break;
            }
            return false;
        }
        private bool odaFiltreTest(BirimEv ev)
        {
            if (label6.ForeColor == Color.Black) return true;
            int oda = ev.ev.OdaSayısı;
            if (oda > (int)numericUpDown5.Value && oda < (int)numericUpDown4.Value)
            {
                return true;
            }
            return false;
        }
        private bool konutFiltreTest(BirimEv ev)
        {
            if (label13.ForeColor == Color.Black) return true;
            if (ev.ev.Tipi.ToString() == comboBox5.SelectedValue.ToString())
            {
                return true;
            }
            return false;
        }
        private bool tarihFiltreTest(BirimEv ev)
        {
            if (label14.ForeColor == Color.Black) return true;
            int seçiliRadioNo = int.Parse(panel19.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Name.ToString().Replace("materialRadioButton", ""));
            DateTime tarih = DateTime.Now;
            switch (seçiliRadioNo)
            {
                case 1:
                    tarih = DateTime.Now.AddHours(-24);
                    break;
                case 2:
                    tarih = DateTime.Now.AddDays(-3);
                    break;
                case 3:
                    tarih = DateTime.Now.AddDays(-7);
                    break;
                case 4:
                    tarih = DateTime.Now.AddDays(-30);
                    break;
                case 5:
                    tarih = new DateTime(DateTime.Now.Subtract(dateTimePicker1.Value).Ticks);
                    break;
            }

            if (ev.ev.YapımTarihi > tarih)
            {
                return true;
            }
            return false;
        }
        private bool sadeceResimFiltreTest(BirimEv ev)
        {
            DosyaIO io = DosyaIO.Oluştur();
            FileInfo[] resim = io.resimList(ev.ev.EmlakNo);


            if ((resim != null && resim.Count() > 0) || (!materialCheckBox5.Checked))
            {
                return true;
            }
            return false;
        }
        private bool gizliFiltreTest(BirimEv ev)
        {
            if (materialCheckBox6.Checked)
            {
                if (!ev.ev.Aktifmi)
                {
                    ev.Visible = true;
                }
                return true;
            }
            else
            {
                if (ev.ev.Aktifmi)
                {
                    return true;
                }
            }
            return false;
        }


        public void ComboTemizle(int HaricOlanNo)
        {
            for (int i = HaricOlanNo + 2; i < 5; i++)
            {
                ComboBox cbox = (ComboBox)tableLayoutPanel6.Controls["comboBox" + i];
                cbox.DataSource = null;
            }


            for (int i = HaricOlanNo + 1; i < 5; i++)
            {
                ComboBox cbox = (ComboBox)tableLayoutPanel6.Controls["comboBox" + i];
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


        private void tümEvleriGöster()
        {
            foreach (BirimEv ev in tableLayoutPanel1.Controls)
            {
                ev.Visible = true;
            }
        }
        private void materialCheckBox2_CheckedChanged(object sender, EventArgs e)
        {

            Size fark = (Size)kordinatfark(button6.PointToClient(Point.Empty), panel1.PointToClient(Point.Empty));
            Size fark2 = (Size)kordinatfark(button4.PointToClient(Point.Empty), panel1.PointToClient(Point.Empty));
            int en = button4.Size.Width;
            if (materialCheckBox2.Checked)
            {
                panel1.Size = new Size(en, fark.Height + button6.Size.Height);
                materialCheckBox2.Text = "Filtrele ▼";
                materialFlatButton1.Enabled = true;
            }
            else
            {
                panel1.Size = new Size(en, fark2.Height + button4.Size.Height);
                materialCheckBox2.Text = "Filtrele ▲";
                materialFlatButton1.Enabled = false;
            }

        }


        Point kordinatfark(Point p1, Point p2)
        {
            Point yeni = Point.Subtract(p2, (Size)p1);
            return yeni;
        }
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            panelOtoSündür();
        }
        public void panelOtoSündür()
        {
            foreach (Control item in tableLayoutPanel1.Controls)
            {
                item.Size = new Size(splitContainer1.SplitterDistance - 23, item.Size.Height);
            }

            panel1.Size = new Size(splitContainer1.SplitterDistance - 23, panel1.Size.Height);
            panel3.Size = new Size(splitContainer1.SplitterDistance - 23, panel3.Size.Height);
            tableLayoutPanel2.Size = new Size(splitContainer1.SplitterDistance - 23, tableLayoutPanel2.Size.Height);

            int en = (flowLayoutPanel1.Width - 20) / 2;
            foreach (BirimEv con in flowLayoutPanel1.Controls)
            {
                con.Size = new Size(en, (int)(con.BoyutOran * en));
            }
        }

        public void illeriComboboxYükle(EmlakIO iom = null)
        {
            EmlakIO io = iom;
            if (iom == null)
            {
                io = EmlakIO.Oluştur();
            }
            comboBox1.DataSource = io.AdresYükle(0, "");
            comboBox1.SelectedIndex = -1;
        }


        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            ComboTemizle(0);
            adresFiltreSıfırla();

        }



        private void AnaSayfa_MaximizedBoundsChanged(object sender, EventArgs e)
        {
            panelOtoSündür();
        }


        private void AnaSayfa_ResizeEnd(object sender, EventArgs e)
        {
            panelOtoSündür();
        }


        private void button8_Click(object sender, EventArgs e)
        {
            KonutEkle konut = new KonutEkle(EKLEMETURU.yeniekle);

            if (konut.ShowDialog() == DialogResult.OK)
            {
                DosyaIO io = DosyaIO.Oluştur();
                io.emlakVeriEkle(konut.Ev);
                yeniBirimEvEkle(konut.Ev);
                log.logEkle($"Yeni konut eklendi!", LOGSEVIYE.bilgi);
            }
            panelOtoSündür();
        }
        private void BirimEv_ClickEvent(object sender, EventArgs e)
        {

            KonutEkle konut = new KonutEkle(EKLEMETURU.güncelle);
            BirimEv tıklanmışBirimEv = ((BirimEv)(((Control)sender).Parent.Parent.Parent));
            konut.Ev = tıklanmışBirimEv.ev;
            konut.Ev.EmlakNo = tıklanmışBirimEv.ev.EmlakNo;
            if (DialogResult.OK == konut.ShowDialog())
            {
                int konum = tıklanmışBirimEv.Parent.Controls.GetChildIndex(tıklanmışBirimEv);

                BirimEv modifiyeTıklanmışBirimEv = yeniBirimEvEkle(konut.Ev);
                tıklanmışBirimEv.Parent.Controls.Remove(tıklanmışBirimEv);
                modifiyeTıklanmışBirimEv.Parent.Controls.SetChildIndex(modifiyeTıklanmışBirimEv, konum);
                DosyaIO io = DosyaIO.Oluştur();
                io.emlakVeriGüncelle(konut.Ev);
                panelOtoSündür();
                seçimStringGüncelle();
            }

        }
        private void panel6blurEkranı_VisibleChanged(object sender, EventArgs e)
        {
            seçimStringGüncelle();
        }
        private BirimEv yeniBirimEvEkle(Ev ev)
        {
            BirimEv yenibirim = new BirimEv(ev);
            flowLayoutPanel1.Controls.Add(yenibirim);
            yenibirim.materialFlatButton1.Click += BirimEv_ClickEvent;
            yenibirim.panel6.VisibleChanged += panel6blurEkranı_VisibleChanged;
            seçimStringGüncelle();
            return yenibirim;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            if (seçiliEvSayısı == 0) return;
            string msj = String.Format("Seçilen {0} adet Konutu kaldırmak istediğinize emin misiniz?\n(Konutlar arşivlenecektir)", seçiliEvSayısı);
            DialogResult sonuc = MessageBox.Show(msj, "Kaldırma Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (sonuc == DialogResult.Yes)
            {
                foreach (BirimEv ev in flowLayoutPanel1.Controls)
                {
                    if (ev.materialCheckBox1.Checked)
                    {
                        ev.materialCheckBox1.Checked = false;
                        ev.ev.Aktifmi = false;
                        ev.Visible = false;
                        ev.aktifDurumYenile();
                        DosyaIO io = DosyaIO.Oluştur();
                        io.emlakVeriGüncelle(ev.ev);
                        seçimStringGüncelle();

                    }
                }
                log.logEkle($"{seçiliEvSayısı} adet ev silindi!", LOGSEVIYE.bilgi);
            }
        }

        private void seçimStringGüncelle()
        {
            label12.Text = seçiliEvSayısı + "/" + evSayısı;
        }

        public void FiltreUygula()
        {
            foreach (BirimEv ev in flowLayoutPanel1.Controls)
            {
                if (alanFiltreTest(ev) &&
                    aramaFiltreTest(ev) &&
                    comboBoxFiltreTest(ev) &&
                    durumFiltreTest(ev) &&
                    fiyatFiltreTest(ev) &&
                    gizliFiltreTest(ev) &&
                    konutFiltreTest(ev) &&
                    odaFiltreTest(ev) &&
                    sadeceResimFiltreTest(ev) &&
                    tarihFiltreTest(ev))
                {
                    ev.Visible = true;
                }
                else
                {
                    ev.Visible = false;
                }
            }
            log.logEkle($"Filtreler Uygulandı!", LOGSEVIYE.bilgi);
        }
        public void FiltreSıfırla()
        {
            materialSingleLineTextField1.Text = "";
            materialCheckBox1.Checked = false;
            adresFiltreSıfırla();
            fiyatFiltreSıfırla();
            durumFiltreSıfırla();
            odaFiltreSıfırla();
            konutFiltreSıfırla();
            alanFiltreSıfırla();
            tarihFiltreSıfırla();
            digerFiltreSıfırla();
            log.logEkle($"Filtreler sıfırlandı!", LOGSEVIYE.bilgi);
        }

        private void adresFiltreSıfırla()
        {
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
            label16.ForeColor = Color.Black;
        }
        private void fiyatFiltreSıfırla()
        {

            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            button1.Visible = false;
            label2.ForeColor = Color.Black;
        }
        private void durumFiltreSıfırla()
        {
            materialCheckBox3.Checked = true;
            materialCheckBox4.Checked = true;
        }
        private void odaFiltreSıfırla()
        {
            numericUpDown4.Value = 0;
            numericUpDown5.Value = 0;
            button12.Visible = false;
            label6.ForeColor = Color.Black;
        }
        private void konutFiltreSıfırla()
        {
            comboBox5.SelectedIndex = -1;
            button7.Visible = false;
            label13.ForeColor = Color.Black;
        }
        private void alanFiltreSıfırla()
        {
            numericUpDown3.Value = 0;
            numericUpDown6.Value = 0;
            button9.Visible = false;
            label9.ForeColor = Color.Black;
        }
        private void tarihFiltreSıfırla()
        {
            materialRadioButton5.Checked = true;
            dateTimePicker1.MaxDate = DateTime.Now.AddMinutes(1);
            dateTimePicker1.Value = DateTime.Now;
            button13.Visible = false;
            label14.ForeColor = Color.Black;
            materialRadioButton5.Focus();
        }
        private void digerFiltreSıfırla()
        {
            materialCheckBox5.Checked = false;
            materialCheckBox6.Checked = false;
        }
        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            FiltreUygula();
        }


        private void fiyatValueChanged(object sender, EventArgs e)
        {
            label2.ForeColor = Color.Red;
            button1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fiyatFiltreSıfırla();
        }

        private void odaValueChanged(object sender, EventArgs e)
        {
            label6.ForeColor = Color.Red;
            button12.Visible = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            odaFiltreSıfırla();
        }

        private void konutSelectionCommitted(object sender, EventArgs e)
        {
            button7.Visible = true;
            label13.ForeColor = Color.Red;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            konutFiltreSıfırla();
        }

        private void alanValueChanged(object sender, EventArgs e)
        {
            button9.Visible = true;
            label9.ForeColor = Color.Red;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            alanFiltreSıfırla();
        }

        private void tarihCheckedChanged(object sender, EventArgs e)
        {
            button13.Visible = true;
            label14.ForeColor = Color.Red;
        }
        private void button13_Click(object sender, EventArgs e)
        {
            tarihFiltreSıfırla();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FiltreUygula();
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            FiltreSıfırla();
            FiltreUygula();
        }
    }
}
