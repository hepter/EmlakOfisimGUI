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
    public partial class MiniResim : UserControl
    {

        private static Color varsayılanRenk=Color.DarkOliveGreen;
        private static Color tıklanmışRenk=Color.Red;
        private bool tıklandıMı;
        private string resimKonum;
        public string ResimKonum
        {
            get
            {
                if (resimKonum.Trim()=="")
                {
                    return Global.NullResimYol;
                }
                return resimKonum;

            }
            set => resimKonum = value;
        }
        public int resimNo
        {
            get
            {
                FileInfo fe = new FileInfo(ResimKonum);
                return  int.Parse(fe.Name.Replace(fe.Extension, ""));
            }             
        }

        public MiniResim(string resimKonum)
        {
            InitializeComponent();
            this.resimKonum = resimKonum;
            DosyaIO io = DosyaIO.Oluştur();

            pictureBox1.Image = EmlakIO.resimDeepCopy(ResimKonum);           
            TıklandıMı = false;
        }

        public bool TıklandıMı {
            get => tıklandıMı;
            set
            {
                tıklandıMı = value;
                button1.FlatAppearance.BorderColor = (tıklandıMı) ? tıklanmışRenk : varsayılanRenk; 
            }
        }
    }
}
