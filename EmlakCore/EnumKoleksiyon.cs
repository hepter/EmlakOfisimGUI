using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmlakCore
{
    public enum EVTIPI
    {
        Daire,
        Residence,
        Müstakil_Ev,
        Villa,
        Çiftlik_Evi,
        Köşk,
        Yalı,
        Yalı_Dairesi,
        Yazlık
    }
    public enum TUR
    {
        satılık,
        kiralık
    }
    public enum ADRES
    {
        il,
        ilçe,
        mahalle,
        sokak
    }
    public enum EKLEMETURU
    {
      yeniekle,
      güncelle
    }

    public enum LOGSEVIYE
    {
        bilgi,
        uyarı,
        hata
    }


    public struct kullanıcı
    {
        private bool notNull;
        private string kAdi;
        private string sifre;
        public bool isNotNull { get => notNull; private set => notNull = value; }
        public string KAdi
        {
            get =>(kAdi == "") ? "" : kAdi;            
            set
            {
                notNull = true;
                kAdi = value;
            }
        }
        public string Sifre
        {
            get=> (sifre == "") ? "" : sifre;
            set
            {
                notNull = true;
                sifre = value;
            }
        }

        public kullanıcı(string kAdi, string sifre)
        {           
            this.kAdi = kAdi;
            this.sifre = sifre;
            notNull = true;
        }

        public static bool operator ==(kullanıcı c1, kullanıcı c2)
        {
            return c1.Equals(c2);
        }
        public static bool operator !=(kullanıcı c1, kullanıcı c2)
        {
            return !c1.Equals(c2);
        }
    }

}
