using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmlakCore
{
    public static class  Global
    {
        public static string AppDir = Environment.CurrentDirectory+"\\";
        public static string ResimDir = AppDir + "resimler\\";
        public static string RoomCostTxt = AppDir + @"room_cost.txt";
        public static string UserTxt = AppDir + @"users.txt";
        public static string SatilikTxt = AppDir + @"satilik.txt";
        public static string KiralikTxt = AppDir + @"kiralik.txt";
        public static string NullResimYol = AppDir + @"resimler\null.png";
        public static string LoggerTxtYol = AppDir + @"Log.txt";

        public const string RoomCostKatsayı = "200";//varsayılan değer
        public const char böl1 = '|';
        public const char böl2 = '+';
        public static char[] yenisatir = new char[] { '\r','\n' };       
        public const string TarihFormat = "dd.MM.yyyy";

    }
}
