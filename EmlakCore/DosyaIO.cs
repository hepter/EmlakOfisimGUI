using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace EmlakCore
{


   public class DosyaIO
    {

        private Logger log = Logger.Oluştur();
        private static DosyaIO io;
        private int fiyatKatsayı = 0;
        private kullanıcı kullanıcı;

        public kullanıcı Kullanıcı
        {
            get
            {
                kullanıcı = (kullanıcı.isNotNull) ? kullanıcı : kullanıcıBilgiOku();
                return kullanıcı;
            }
            set { kullanıcı = value; }
        }

        public int FiyatKatsayı
        {
            get
            {

                if (fiyatKatsayı == 0) fiyatKatsayı = KatsayıOku();
                return fiyatKatsayı;
            }
        }
        private DosyaIO()
        {
            log.logEkle("DosyaIO singleton oluşturuldu!", LOGSEVIYE.bilgi);
        }
        public static DosyaIO Oluştur()
        {
            if (io == null)
            {
                io = new DosyaIO();
            }
            return io;
        }

        private int KatsayıOku()
        {

            if (File.Exists(Global.RoomCostTxt))
            {
                log.logEkle("Katsayı Okundu.", LOGSEVIYE.bilgi);
                return int.Parse(File.ReadAllText(Global.RoomCostTxt, Encoding.Default));
            }
            else
            {
                log.logEkle("Katsayı bulunamadı 200 varsayılan olarak oluşturuldu!", LOGSEVIYE.uyarı);
                File.WriteAllText(Global.RoomCostTxt, Global.RoomCostKatsayı);
            }
            return 200;
        }
        private kullanıcı kullanıcıBilgiOku()
        {
            kullanıcı kull = new kullanıcı();
            if (File.Exists(Global.UserTxt))
            {
                string bilgi = File.ReadAllText(Global.UserTxt, Encoding.Default);
                kull.KAdi = bilgi.Split(Global.böl1)[0].ToString();
                kull.Sifre = bilgi.Split(Global.böl1)[1].ToString();
                return kull;
            }
            return kull;
        }


        public void emlakVeriEkle(Dictionary<TUR, ArrayList> sözlük)
        {
            foreach (TUR tür in sözlük.Keys)
            {
                ArrayList list;
                if (!sözlük.TryGetValue(tür, out list)) continue;

                for (int i = 0; i < list.Count - 1; i++)
                {                  
                    if (tür == TUR.kiralık)
                        emlakTxtEkle(tür, ((SatılıkEv)list[i]).EvBilgileri());                   
                    else
                        emlakTxtEkle(tür, ((KiralıkEv)list[i]).EvBilgileri());                
                }
            }
        }
        public void emlakVeriEkle(TUR tür, ArrayList sözlük)
        {
            if (sözlük.Count == 0) return;
            for (int i = 0; i < sözlük.Count - 1; i++)
            {             
                if (tür == TUR.kiralık)                                  
                    emlakTxtEkle(tür, ((SatılıkEv)sözlük[i]).EvBilgileri());                
                else                                 
                    emlakTxtEkle(tür,((KiralıkEv)sözlük[i]).EvBilgileri());    

            }
        }
        public void emlakVeriEkle(Ev ev)
        {
            TUR tür;
            if (ev is KiralıkEv)
            {
                tür = TUR.kiralık;
                emlakTxtEkle(tür, ((KiralıkEv)ev).EvBilgileri());
            }
            else
            {
                tür = TUR.satılık;
                emlakTxtEkle(tür, ((SatılıkEv)ev).EvBilgileri());
            }
                  
        }

        public void emlakVeriGüncelle(Ev ev)
        {             
            List<string> yeniVeriSatılık = new List<string>();
            List<string> yeniVeriKiralık = new List<string>();
            string güncellenenData="";
            if (ev is SatılıkEv)
            {
                güncellenenData = ((SatılıkEv)ev).EvBilgileri();
                log.logEkle("Satılık Ev başarıyla güncellendi", LOGSEVIYE.bilgi);
            }
            else
            {
                güncellenenData = ((KiralıkEv)ev).EvBilgileri();
                log.logEkle("Kiralık Ev başarıyla güncellendi", LOGSEVIYE.bilgi);
            }

            foreach (string ln in emlakTxtOku(TUR.satılık))
            {
                string bol = Global.böl1.ToString();
                if (ln.Contains(bol + ev.EmlakNo + bol))
                {
                    if (ev is SatılıkEv)
                    {
                        yeniVeriSatılık.Add(güncellenenData);
                    }
                    else
                    {
                        yeniVeriKiralık.Add(güncellenenData);
                    }
                }
                else
                {
                        yeniVeriSatılık.Add(ln);                   
                }
            }

            foreach (string ln in emlakTxtOku(TUR.kiralık))
            {
                string bol = Global.böl1.ToString();
                if (ln.Contains(bol + ev.EmlakNo + bol))
                {
                    if (ev is KiralıkEv)
                    {
                        yeniVeriKiralık.Add(güncellenenData);
                    }
                    else
                    {
                        yeniVeriSatılık.Add(güncellenenData);
                    }
                }
                else
                {
                    yeniVeriKiralık.Add(ln);
                }

            }

            string satılıkVeri = String.Join(new string(Global.yenisatir), yeniVeriSatılık);
            string kiralıkVeri = String.Join(new string(Global.yenisatir), yeniVeriKiralık);
            File.WriteAllText(Global.SatilikTxt, satılıkVeri, Encoding.UTF8);
            File.WriteAllText(Global.KiralikTxt, kiralıkVeri, Encoding.UTF8);

        }
        public Dictionary<TUR, ArrayList> emlakVeriOku()
        {
            ArrayList konutList = new ArrayList();
            Dictionary<TUR, ArrayList> sözlük = new Dictionary<TUR, ArrayList>(2);
            sözlük.Add(TUR.kiralık, emlakVeriOku(TUR.kiralık));
            sözlük.Add(TUR.satılık, emlakVeriOku(TUR.satılık));
            return sözlük;
        }
        public ArrayList emlakVeriOku(TUR tür)
        {
            ArrayList konutList = new ArrayList();
            string[] txtLines = emlakTxtOku(tür);
            if (txtLines == null)
                return new ArrayList();
            foreach (string satır in txtLines)
            {
                if (satır == "") continue;
                
                if (TUR.kiralık == tür)
                    konutList.Add(new KiralıkEv(satır));
                else
                    konutList.Add(new SatılıkEv(satır));
            }
            log.logEkle("Kayıtlı Emlak okundu!", LOGSEVIYE.bilgi);
            return konutList;
        }


        private void emlakTxtEkle(TUR tür, string veri)
        {
            string TxtYol = (tür == TUR.satılık) ? Global.SatilikTxt : Global.KiralikTxt;
            string TxtVeri = veri + new string(Global.yenisatir);
            File.AppendAllText(TxtYol, TxtVeri, Encoding.UTF8);
        }
        private string[] emlakTxtOku(TUR tür)
        {
            string TxtYol = (tür == TUR.satılık) ? Global.SatilikTxt : Global.KiralikTxt;
            string[] snc;
            if (!File.Exists(TxtYol))
                snc= new string[] { ""};
            else
                snc = File.ReadAllLines(TxtYol, Encoding.UTF8);
            if (snc.Count()==0)
            {
                snc = new string[] { "" };
            }
            return snc;
        }

       



        public string resimEkle(string resimYol, int emlakId)
        {
            int resimId=1;
            if (!resimCore(emlakId))            
                resimId += resimSay(emlakId);

            log.logEkle($"{emlakId} Idli Konuğa resim eklendi", LOGSEVIYE.bilgi);
            string yeniYol = Global.ResimDir + emlakId+"\\"+resimId+".jpg";

            while (File.Exists(yeniYol))
            {
                log.logEkle($"{yeniYol} aynı isimli resim Tesbit Edildi! Resim sırasi bir arttırıldı", LOGSEVIYE.uyarı);
                resimId += 1;
                yeniYol = Global.ResimDir + emlakId + "\\" + resimId + ".jpg";
            }

            File.Copy(resimYol,yeniYol);
            return yeniYol;
        }
        public string resimEkle(Uri url, int emlakId)
        {
            int resimId = 1;
            if (!resimCore(emlakId))
                resimId += resimSay(emlakId);
            string yeniYol = Global.ResimDir + emlakId + "\\" + resimId + ".jpg";

            WebClient indir = new WebClient();
            indir.DownloadFile(url, yeniYol);
            return yeniYol;
        }
        public bool resimSil(int emlakId, int resimNo)
        {
            string dizin = Global.ResimDir + emlakId;
            string yol = dizin + "\\" + resimNo + ".jpg";

            if (resimCore(emlakId) || !File.Exists(yol))
            {
                return false;
            }
            else
            {

                File.SetAttributes(yol, FileAttributes.Normal);               
                File.Delete(yol);
                log.logEkle($"{emlakId} Idli Konuğun resmi silindi!", LOGSEVIYE.uyarı);
                if (resimSay(emlakId) == 0)
                {
                    Directory.Delete(dizin, true);
                    log.logEkle($"{emlakId} Idye ait resim kalmadı ve resim Dizini silindi!", LOGSEVIYE.uyarı);
                }
                return true;
            }
        }
       
        public FileInfo[] resimList(int emlakId)
        {
            string yol = Global.ResimDir + emlakId;
            if (!Directory.Exists(yol))
            {
                log.logEkle($"{emlakId} Idye ait resimList boş döndü", LOGSEVIYE.uyarı);
                return null;
            }
                                   
            FileInfo[] sonuc = Directory.GetFiles(yol).Select(a => new FileInfo(a)).OrderBy(f => int.Parse(f.Name.Replace(f.Extension,""))).ToArray();
            return sonuc;
        }

        public string randomResim(int emlakId)
        {
            FileInfo[] sonuc = resimList(emlakId);
            if (sonuc!=null &&sonuc.Count()>0)
            {
                Random rnd = new Random();
                return sonuc[rnd.Next(0,sonuc.Count())].FullName;
            }
            return Global.NullResimYol;
        }
        public string resimDizinKonum(int emlakId)
        {           
            return Global.ResimDir + emlakId;
        }

        private bool resimCore(int emlakId, bool silinsinmi = false)
        {
            string resimKlasör = Global.ResimDir + emlakId;
            if (!Directory.Exists(resimKlasör) && !silinsinmi)
            {
                Directory.CreateDirectory(resimKlasör);
                return true;
            }
            else if (Directory.Exists(resimKlasör) && silinsinmi)
            {
                Directory.Delete(resimKlasör, true);
                return true;
            }
            return false;
        }
        private int resimSay(int emlakId)
        {            
            string dizin = Global.ResimDir + emlakId;
            int say= Directory.GetFiles(dizin).Count();
            log.logEkle($"{emlakId} Idye ait resim sayıldı.Sayı:{say}", LOGSEVIYE.uyarı);
            return say;
        }
    }
}
