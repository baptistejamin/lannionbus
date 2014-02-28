using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Lannion_Bus
{
    public class bus
    {
        public static Arrets arrets;
        public static RootObject horaires;
        public static List<Arret> arrets_bus;
        public static List<Holliday> vacances;
    }
    public class arret_binding
    {
        public string name { get; set; }
        public string distance { get; set; }
    }
    public class prochains_binding
    {
        public string hour { get; set; }
        public string description { get; set; }
        public string ligne { get; set; }
        public string id { get; set; }
        public Visibility bike { get; set; }
        public int time { get; set; }
    }
    public class me
    {
        public static double lat;
        public static double lon;
        public static string ligne = "A";
        public static string current_id;

    }
    public class calc
    {
        public static double getDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            int R=6371; // Radius of the earth in km
            double  dLat = calc.deg2rad(lat2-lat1);
            double dLon = calc.deg2rad(lon2 - lon1); 
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Sin(dLon/2) * Math.Sin(dLon/2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)); 
            double d = R * c; // Distance in km
            return d;
        }
        public static double deg2rad(double deg) 
        {
            return deg * (Math.PI/180);
        }
    }
    public static class get_arrets
    {
        public static List<prochains_binding> get_bus(string nom_arret, DateTime date)
        {
            string minutes = "";
            string hour = "";

            bool show_horaire =false;
            List<prochains_binding> liste_horaires = new List<prochains_binding>();
            
           
            foreach (Horaire horaire in bus.horaires.horaires)
            {
                foreach (Tournee tournee in horaire.tournees)
                {
                    foreach (Horaires horaires in tournee.horaires)
                    {
                        if (horaires.arret == nom_arret)
                        {
                         
                            prochains_binding horaire_item = new prochains_binding();
                            if(horaires.horaire.ToString().Count()==3)
                            {
                                minutes = horaires.horaire.ToString().Substring(1, 2);
                                hour =  horaires.horaire.ToString().Substring(0, 1);
                            }
                            else
                            {
                                minutes = horaires.horaire.ToString().Substring(2, 2);
                                hour =  horaires.horaire.ToString().Substring(0, 2);
                            }

                            horaire_item.hour = hour + "h" + minutes;
                            horaire_item.description = "Direction " + horaire.to;
                            horaire_item.ligne = "Ligne " + horaire.ligne;
                            horaire_item.time = horaires.horaire;
                            horaire_item.id = horaires.id;
                            if (tournee.velo)
                            {
                                horaire_item.bike = Visibility.Visible;
                            }
                            else
                            {
                                horaire_item.bike = Visibility.Collapsed;
                            }
                            switch (date.DayOfWeek.ToString().ToLower())
                            {
                                case "monday":
                                    if(horaire.monday==1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "tuesday":
                                    if(horaire.tuesday==1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "wednesday":
                                    if (horaire.wednesday == 1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "thursday":
                                    if (horaire.thursday == 1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "friday":
                                    if (horaire.friday == 1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "saturday":
                                    if (horaire.saturday == 1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                case "sunday":
                                    if (horaire.sunday == 1)
                                    {
                                        show_horaire = true;
                                    }
                                    break;
                                default:
                                    show_horaire = false;
                                    break;
                            }
                            

                            if (show_horaire && !dates.isFerie())
                            {
                                if (horaire.type == "normal")
                                {
                                    liste_horaires.Add(horaire_item);
                                }
                                if (horaire.type == "scolaire" && !dates.isHollidays())
                                {
                                    liste_horaires.Add(horaire_item);
                                }
                                if (horaire.type == "scolaire" && dates.isHollidays())
                                {
                                    liste_horaires.Add(horaire_item);
                                }
                            }
                        }
                    }
                }
            }
            return liste_horaires; 
        }
    }

 
}
