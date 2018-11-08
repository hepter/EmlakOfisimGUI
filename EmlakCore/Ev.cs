using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EmlakCore
{
    public class Adres
    {
        Logger log = Logger.Oluştur();
        private string adresDataStr;
        private string[] adresler;
        public string Il { get => adresler[0]; set => adresler[0] = value; }
        public string Ilçe { get => adresler[1]; set => adresler[1] = value; }
        public string Mahalle { get => adresler[2]; set => adresler[2] = value; }
        public string Sokak { get => adresler[3]; set => adresler[3] = value; }
        public string AdresDataStr
        {
            get
            {
                return String.Join(Global.böl2.ToString(), adresler);
            }
            set
            {
                adresDataStr = value;
                adresÇözümle();
            }
        }


        public Adres()
        {
            log.logEkle($"Boş adres sınıfı oluşturldu", LOGSEVIYE.uyarı);
            adresler = new string[4] {"","","",""};
            adresDataStr = "";
        }

        public Adres(string AdresDataStr)
        {
            adresTxtYükle(AdresDataStr);
            log.logEkle($"Adres sınıfı oluşturuldu ve Çözümlendi", LOGSEVIYE.bilgi);
        }


        public void adresTxtYükle(string adresTxt)
        {
            adresler  = adresTxt.Split(Global.böl2);
            AdresDataStr = adresTxt;
            adresÇözümle();
        }

        public string adresBilgileri(ADRES adres)
        {            
            return adresler[(int)adres];
        }

        public void adresÇözümle()
        {
            string[] adresdata =adresDataStr.Split(Global.böl2);
            if (adresdata.Length < 1) return;   
            Il = adresdata[0].Trim();
            if (adresdata.Length < 2) return;
            Ilçe = adresdata[1].Trim();
            if (adresdata.Length < 3) return; 
            Mahalle = adresdata[2].Trim();
            if (adresdata.Length < 4) return;
            Sokak = adresdata[3].Trim();
        }
    }


    public abstract class Ev
    {
        private int odaSayısı;
        private int katNumarası;
        private Adres adres;
        private int alan;
        private DateTime yapımTarihi;
        private EVTIPI tipi;
        private bool aktifmi;
        private int emlakNo;
        private FileInfo[] resimler;
        private string not;
        private string başlık;       
        public int OdaSayısı
        {
            get
            {
                return odaSayısı;
            }
            set
            {
                odaSayısı = (value < 0) ? 0 : value;
            }
        }
        public int KatNumarası { get => katNumarası; set => katNumarası = value; }
        public Adres Adres {get => adres;set => adres = value;}
        public int Alan { get => alan; set => alan = value; }
        public DateTime YapımTarihi { get => yapımTarihi; set => yapımTarihi = value; }
        public EVTIPI Tipi { get => tipi; set => tipi = value; }
        public bool Aktifmi { get => aktifmi; set => aktifmi = value; }
        public  Logger log = Logger.Oluştur();
        public int EmlakNo
        {
            get
            {
                if (emlakNo == 0)
                {                    
                    emlakNo = new Random().Next(100000000, 999999999);
                    log.logEkle(emlakNo+" Ev sınıfı EmlakID boştu.Yeniden oluşturuldu!", LOGSEVIYE.uyarı);
                }
                return emlakNo;
            }
            set => emlakNo = value;
        }
       

        public int EvYas
        {
            get
            {
                    TimeSpan yas = DateTime.Now.Subtract(yapımTarihi);
                    return (int)(yas.Days / 365);              
            }
        }

        public FileInfo[] Resimler {
            get
            {
                log.logEkle($"Resim listesi Çağırıldı", LOGSEVIYE.bilgi);
                DosyaIO io = DosyaIO.Oluştur();
                return io.resimList(EmlakNo);             
            } private set => resimler = value; }
        public string Not { get => (not!=null)?not:""; set => not = value; }
        public string Başlık { get => (başlık != null) ? başlık : ""; set => başlık = value; }

        protected Ev(){}

        public Ev(string TxtSatır)
        {
            string[] veri = TxtSatır.Split(Global.böl1);

           
            this.OdaSayısı = int.Parse(veri[0]);//*
            this.KatNumarası = int.Parse(veri[1]);
            this.Adres = new Adres(veri[2]); //*
            this.Alan = int.Parse(veri[3]);//*
            this.YapımTarihi = DateTime.ParseExact(veri[4],Global.TarihFormat, CultureInfo.InvariantCulture);
            this.Tipi = (EVTIPI)Enum.Parse(typeof(EVTIPI), veri[5]);//*
            this.Aktifmi = (int.Parse(veri[6])==1)?true:false;//*
            this.emlakNo = int.Parse(veri[7]);//*
            this.Not = veri[8];
            this.Başlık = veri[9];//*

        }

        public Ev(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi,string Başlık) : this()
        {
            this.OdaSayısı = OdaSayısı;
            this.Adres = adres;
            this.Alan = Alan;
            this.Tipi = EvTipi;
            this.Aktifmi = AktifMi;
            this.Başlık = Başlık;
        }
        public Ev(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık,int KatNumarası) : this(OdaSayısı, adres, Alan,  EvTipi,  AktifMi,  Başlık)
        {            
            this.KatNumarası = KatNumarası;
        }
        public Ev(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık, int KatNumarası, DateTime YapımTarihi) : this(OdaSayısı, adres, Alan, EvTipi, AktifMi, Başlık, KatNumarası)
        {
            this.YapımTarihi = YapımTarihi;
        }
        public Ev(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık, int KatNumarası, DateTime YapımTarihi,string Not) : this(OdaSayısı, adres, Alan, EvTipi, AktifMi, Başlık, KatNumarası, YapımTarihi)
        {
            this.Not = Not;
        }       

        public virtual string EvBilgileri()
        {
            log.logEkle($"Ev bilgileri Oluşturuldu!", LOGSEVIYE.bilgi);
            StringBuilder str = new StringBuilder();
            string bol = Global.böl1.ToString();
            str.Append(OdaSayısı.ToString() + bol);
            str.Append(KatNumarası.ToString() + bol);
            str.Append(adres.AdresDataStr + bol);//*
            str.Append(Alan.ToString() + bol);//*
            str.Append(YapımTarihi.ToString(Global.TarihFormat) + bol);
            str.Append(Tipi.ToString() + bol);//*
            str.Append(((Aktifmi) ? 1 : 0).ToString() + bol);//*
            str.Append(EmlakNo.ToString() + bol);//*
            str.Append(Not + bol);
            str.Append(Başlık + bol);
            return str.ToString();
        }
        abstract public int FiyatHesapla();
   
    }


    public class KiralıkEv : Ev
    {
        private int depozito;
        private int kira;
        public int Depozito { get => depozito; set => depozito = value; }
        public int Kira { get => FiyatHesapla(); set => kira = value; }

     
        private void KiralıkEvConstructor(int Depozito, int Kira)
        {
            this.Depozito = Depozito;
            this.Kira = Kira;
        }

        private KiralıkEv(){}

        public KiralıkEv(string TxtSatır) : base(TxtSatır)
        {
            
            string[] veri = TxtSatır.Split(Global.böl1);         
            this.Depozito = int.Parse(veri[10]);
            this.Kira = int.Parse(veri[11]);          
        }
        public KiralıkEv(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık,int Kira) : base( OdaSayısı,  adres,  Alan,  EvTipi,  AktifMi,  Başlık)
        {
            this.Kira = Kira;
        }
        public KiralıkEv(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık,int Depozito ,int Kira, int KatNumarası, DateTime YapımTarihi, string Not) : base(OdaSayısı,  adres,  Alan,  EvTipi,  AktifMi,  Başlık,  KatNumarası,  YapımTarihi,  Not)
        {
            this.Depozito = Depozito;
            this.Kira = Kira;           
        }
        public override string EvBilgileri()
        {
            StringBuilder str = new StringBuilder(base.EvBilgileri());
            str.Append(Depozito.ToString() + Global.böl1.ToString());
            str.Append(Kira.ToString() + Global.böl1.ToString());//*
            return str.ToString();
        }
        public override int FiyatHesapla()
        {            
            DosyaIO oku = DosyaIO.Oluştur();          
            return oku.FiyatKatsayı * base.OdaSayısı;
        }       
    }
    public class SatılıkEv : Ev
    {
        private int fiyat;
        public int Fiyat { get => FiyatHesapla(); set => fiyat = value; }

        public SatılıkEv(string TxtSatır) : base(TxtSatır)
        {
            string[] veri = TxtSatır.Split(Global.böl1);
            this.Fiyat =int.Parse(veri[10]);
        }
        public SatılıkEv(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık, int Fiyat) : base(OdaSayısı, adres, Alan, EvTipi, AktifMi, Başlık)
        {
            this.Fiyat = Fiyat;
        }
        public SatılıkEv(int OdaSayısı, Adres adres, int Alan, EVTIPI EvTipi, bool AktifMi, string Başlık, int Fiyat, int KatNumarası, DateTime YapımTarihi, string Not) : base(OdaSayısı, adres, Alan, EvTipi, AktifMi, Başlık, KatNumarası, YapımTarihi, Not)
        {
            this.Fiyat = Fiyat;
        }
        public override string EvBilgileri()
        {
            StringBuilder str = new StringBuilder(base.EvBilgileri());
            str.Append(Fiyat.ToString() +Global.böl1.ToString());
            return str.ToString();
        }
        public override int FiyatHesapla()
        {
            log.logEkle($"Evin Kira fiyatı hespalandı", LOGSEVIYE.bilgi);
            DosyaIO oku = DosyaIO.Oluştur();
            return oku.FiyatKatsayı * base.OdaSayısı;
        }
    }
}
