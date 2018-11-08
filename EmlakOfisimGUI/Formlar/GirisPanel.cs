using EmlakCore;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace EmlakOfisimGUI
{
    public partial class GirisPanel : MaterialForm
    {
        public GirisPanel()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal200, Primary.Teal500, Primary.DeepOrange400, Accent.DeepOrange700, TextShade.WHITE);

        }
        Logger log;
        private void GirisPanel_Load(object sender, EventArgs e)
        {
            log = Logger.Oluştur();
        }


        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {

            kullanıcı kull = new kullanıcı(materialSingleLineTextField1.Text, materialSingleLineTextField2.Text);
            DosyaIO io = DosyaIO.Oluştur();
            if (!io.Kullanıcı.isNotNull)
            {
                MessageBox.Show("Giriş bilgileri dosyası eksik.\nGiriş yapılamaz!");
                log.logEkle($"Giriş Dosyası eksik.{Global.UserTxt} Yolunda dosya olduğundan emin olun", LOGSEVIYE.hata);
            }

            else if (io.Kullanıcı == kull)
            {
                AnaSayfa sayfa = new AnaSayfa();
                materialRaisedButton1.Text = "Giriş Yapılıyor...";
                Application.DoEvents();

                //bunu eklemezsek yeni oluşan form kapanır ancak bu arkaplanca açık kalacaktır
                //yeni formun kapatma eventine bu formunkini verirsek bu kapanınca hiyerarşik olarak her ikisi kapanacaktır
                sayfa.Closed += (a, b) => this.Close();
                sayfa.Show();

                log.logEkle("başarıyla giriş yapıldı", LOGSEVIYE.bilgi);
                this.Hide();

            }
            else
            {
                MessageBox.Show("Hatalı giriş bilgieri");
                log.logEkle("hatalı giriş denemesi", LOGSEVIYE.uyarı);
            }
        }
    }
}
