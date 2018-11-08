using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EmlakCore
{
    public class Logger
    {
        private static Logger log;
        private object kilit=new object();
        private Logger()
        {
            logYaz(String.Format("\r\n\r\n{0,50}{1,-40}", "_Program çalıştırıldı","_"));
            logEkle($"Logger Singleton Oluşturuldu", LOGSEVIYE.bilgi);
        }
        public static Logger Oluştur()
        {
            if (log==null)
            {
                log = new Logger();
              
            }
            return log;
        }

        public void logEkle(string veri,LOGSEVIYE seviye)
        {
            string saat = DateTime.Now.ToString("dd.MM.yyy hh:mm:ss.ffffff");
            string ek = "";     
            switch (seviye)
            {
                case LOGSEVIYE.bilgi:
                    ek = "BİLGİ:";
                    break;
                case LOGSEVIYE.uyarı:
                    ek = "! - UYARI:";
                    break;
                case LOGSEVIYE.hata:
                    ek = "!!! - HATA:";
                    break;                
            }          
            logYaz(String.Format("{0,-30} {1,10} {2}", saat, ek, veri));
        }
        private void logYaz(string veri)
        {
            lock (kilit)
            {
                string str = veri + new string(Global.yenisatir);
                File.AppendAllText(Global.LoggerTxtYol, str);
                Console.WriteLine(str);
            }
        }








    }
}
