using EmlakCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmlakOfisimGUI
{


    #region Json Deseriaze Sınıfları
    
    public class adresDB
    {
        public ilçeler ilçe;
        public mahalleler mh;
        public iller il;
        public Sokaklar sk;
    }
    public class ilçeler
    {
        public string a, b, c;//silinmesi hataya sebebiyet verir gereksiz olduğu için kullanılmamıştır ancak atama uygulanır
        [JsonProperty(PropertyName = "data")]
        public List<ilçe> ilçeList;
        public List<ilçe> ilçeGetir(string plaka)
        {
            var sonuc = ilçeList.Where(a => a.ilce_sehirkey == plaka).Select(a => a).ToList();
            return sonuc;
        }
    }
    public class ilçe
    {
        public string ilce_id;
        public string ilce_title;
        public string ilce_key;
        public string ilce_sehirkey;
    }

    public class mahalleler
    {
        public string a, b, c;
        [JsonProperty(PropertyName = "data")]
        public List<mahalle> mhList;
        public List<mahalle> mhGetir(string ilçeKey)
        {
            var sonuc = mhList.Where(a => a.mahalle_ilcekey == ilçeKey).ToList();
            return sonuc;
        }

    }
    public class mahalle
    {
        public string mahalle_id;
        public string mahalle_title;
        public string mahalle_key;
        public string mahalle_ilcekey;
    }
    public class iller
    {
        public string a, b, c;
        [JsonProperty(PropertyName = "data")]
        public List<il> ilList;
    }
    public class il
    {
        public string sehir_id;
        public string sehir_title;
        public string sehir_key;
    }
    public class Sokaklar
    {

        public string a, b, c;
        [JsonProperty(PropertyName = "data")]
        public List<Sokak> skList;
        public List<Sokak> skGetir(string mhKey)
        {
            var sonuc = skList.Where(a => a.sokak_cadde_mahallekey == mhKey).ToList();
            return sonuc;
        }
    }
    public class Sokak
    {
        public string sokak_cadde_id;
        public string sokak_cadde_title;
        public string sokak_cadde_mahallekey;

    }
    #endregion


    
    public class EmlakIO
    {
        static Logger  log = Logger.Oluştur();
        public static Image resimDeepCopy(string fileName)
        {
            Image theImage = null;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open,FileAccess.Read))
            {
                byte[] img;
                img = new byte[fileStream.Length];
                fileStream.Read(img, 0, img.Length);
                fileStream.Close();
                theImage = Image.FromStream(new MemoryStream(img));
                img = null;
                log.logEkle(fileName + " - resim tekrar silinebilecek şekilde kopyalandı", LOGSEVIYE.bilgi);
            }       
            return theImage;
        }

        private static EmlakIO io;
        private EmlakIO()
        {
            log.logEkle("EmlakIO singleton oluşturuldu", LOGSEVIYE.bilgi);
        }
        public static EmlakIO Oluştur()
        {
            if (io==null)
            {
                io = new EmlakIO();
            }
            return io;
        }
        public void JsonOku()
        {
            if (adresJsonData != null) return;
           
            string yol = Global.AppDir + "db\\adres.json";
            adresDB adresData = null;
            using (StreamReader sr = new StreamReader(yol))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                reader.SupportMultipleContent = true;

                var serializer = new JsonSerializer();
                while (reader.Read())
                {
                    //adresDB cf = serializer.Deserialize<adresDB>(reader);
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        ilçeler c1 = serializer.Deserialize<ilçeler>(reader);
                        reader.Read();
                        mahalleler c2 = serializer.Deserialize<mahalleler>(reader);
                        reader.Read();
                        iller c3 = serializer.Deserialize<iller>(reader);
                        reader.Read();
                        Sokaklar c4 = serializer.Deserialize<Sokaklar>(reader);

                         adresData = new adresDB() { ilçe = c1, mh = c2, il = c3, sk = c4, };
                        break;
                    }
                }
            }
             adresJsonData=adresData;
        }

        Dictionary<int, Dictionary<string, string>> CboxS = new Dictionary<int, Dictionary<string, string>>()
        {
            { 0,new Dictionary<string, string>()},
            { 1, new Dictionary<string, string>() },
            { 2, new Dictionary<string, string>() },
            { 3, new Dictionary<string, string>() }
        };
        adresDB adresJsonData;
        public string[] AdresYükle(int boxSıra,string boxText)
        {
            string[] sonucData=null;
            switch (boxSıra)
            {
                case 0:
                    il[] ils = adresJsonData.il.ilList.OrderBy(a => a.sehir_title).ToArray();
                    CboxS[0] = ils.ToDictionary(aa => aa.sehir_key, aa => aa.sehir_title);
                    sonucData = ils.Select(a => a.sehir_title).ToArray();
                    break;
                case 1:
                    var sonuc = adresJsonData.il.ilList.OrderBy(a => a.sehir_title).First(b => b.sehir_title == boxText).sehir_key;

                    List<ilçe> ilçs = adresJsonData.ilçe.ilçeGetir(sonuc.ToString());
                    CboxS[1] = new Dictionary<string, string>();
                    foreach (ilçe var in ilçs)
                    {
                        if (!CboxS[1].Keys.Contains(var.ilce_key))
                            CboxS[1].Add(var.ilce_key, var.ilce_title);
                    }               
                    sonucData = ilçs.Select(a => a.ilce_title).ToArray();
                    break;
                case 2:
                    sonuc = CboxS[1].First(a => a.Value.ToString() == boxText).Key;

                    List<mahalle> mhs = adresJsonData.mh.mhGetir(sonuc.ToString());
                    CboxS[2] = new Dictionary<string, string>();
                    foreach (mahalle var in mhs)
                    {
                        if (!CboxS[2].Keys.Contains(var.mahalle_key))
                            CboxS[2].Add(var.mahalle_key, var.mahalle_title);
                    }
                    sonucData = mhs.Select(a => a.mahalle_title).ToArray();                  
                    break;
                case 3:
                    sonuc = CboxS[2].First(a => a.Value.ToString() == boxText).Key;

                    List<Sokak> sks = adresJsonData.sk.skGetir(sonuc);
                    CboxS[3] = new Dictionary<string, string>();
                    foreach (Sokak var in sks)
                    {
                        if (!CboxS[3].Keys.Contains(var.sokak_cadde_id))
                            CboxS[3].Add(var.sokak_cadde_id, var.sokak_cadde_title);
                    }
                    sonucData = sks.Select(a => a.sokak_cadde_title).ToArray();
                    break;
            }
            return sonucData;
        }
      
        public void OtomatikScale(ref Label lbl)
        {
            log.logEkle($"'{lbl.Text}' otomatik Hizalandı", LOGSEVIYE.bilgi);
            string txt = lbl.Text;
            float best_size = 0;
            if (txt.Length > 0)
            {
                best_size = 100;
                float wid = lbl.DisplayRectangle.Width - 3;
                float hgt = lbl.DisplayRectangle.Height - 3;
                using (Graphics gr = lbl.CreateGraphics())
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        using (Font test_font =new Font(lbl.Font.FontFamily, i))
                        {
                            SizeF text_size =gr.MeasureString(txt, test_font);
                            if ((text_size.Width > wid) ||(text_size.Height > hgt))
                            {
                                best_size = i - 1;
                                break;
                            }
                        }
                    }
                }              
                if (best_size <= 0)
                {
                    best_size = 1;
                }
                lbl.Font = new Font(lbl.Font.FontFamily, best_size-1, lbl.Font.Style);
            }        
        }
    }
}
