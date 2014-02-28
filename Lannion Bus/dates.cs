using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lannion_Bus
{
    class dates
    {
        public static DateTime EasterSunday(int YearToCheck)
        {
            int Y = YearToCheck;
            int a = Y % 19;
            int b = Y / 100;
            int c = Y % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int L = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * L) / 451;
            int Month = (h + L - 7 * m + 114) / 31;
            int Day = ((h + L - 7 * m + 114) % 31) + 1;
            DateTime dtEasterSunday = new DateTime(YearToCheck, Month, Day);
            return dtEasterSunday;
        }
        public static bool isFerie()
        {
            bool ferie=false;
            int jour = DateTime.Now.Day;
            int mois = DateTime.Now.Month;
            int annee = DateTime.Now.Year;
            //on vérifie pour paques
            if (DateTime.Now.Date.ToString() == dates.EasterSunday(DateTime.Now.Year).Date.ToString())
            {
                ferie=true;
            }

            // dates fériées fixes
            if(jour == 1 && mois == 1) { ferie = true;} // 1er janvier
            if(jour == 1 && mois == 5) { ferie = true;} // 1er mai
            if(jour == 8 && mois == 5) { ferie = true;} //8 mai
            if(jour == 14 && mois == 7) { ferie = true;} //14 juillet
            if(jour == 15 && mois == 8) { ferie = true;} //15 aout
            if(jour == 1 && mois == 11) { ferie = true;} // 1 novembre
            if(jour == 11 && mois == 11) { ferie = true;} // 11 novembre
            if(jour == 25 && mois == 12) { ferie = true;} // 25 décembre

            //Lundi de paques
            if (DateTime.Now.Date.ToString() == dates.EasterSunday(DateTime.Now.Year).AddDays(1).Date.ToString())
            {
                ferie=true;
            }
            //Ascenssion
            if (DateTime.Now.Date.ToString() == dates.EasterSunday(DateTime.Now.Year).AddDays(39).Date.ToString())
            {
                ferie=true;
            }
            //Pentecote
            if (DateTime.Now.Date.ToString() == dates.EasterSunday(DateTime.Now.Year).AddDays(49).Date.ToString())
            {
                ferie=true;
            }
             //Lundi de Pentecote
            if (DateTime.Now.Date.ToString() == dates.EasterSunday(DateTime.Now.Year).AddDays(50).Date.ToString())
            {
                ferie=true;
            }

            return ferie;
        }
        public static bool isHollidays()
        {
            bool hollidays = false;
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            foreach (Holliday holliday in bus.vacances)
            {
                if ((unixTimestamp >= holliday.start) && (unixTimestamp <= holliday.start))
                {
                    hollidays = true;
                }
            }
            return hollidays;
        }
    }
}
