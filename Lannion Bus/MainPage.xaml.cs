using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

using Microsoft.Phone.Controls.Maps;
using System.Device.Location;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls.Maps.Platform;
using Lannion_Bus.RouteService2;
namespace Lannion_Bus
{
    public partial class MainPage : PhoneApplicationPage
    {
        //Classe pour le routing des routes
        private RoutingHelper routing;
        bool isEnabled = false;
        CustomMessageBox messageBox;
        // Constructeur
        public MainPage()
        {
            InitializeComponent();
           


            //On centre la vue sur lannion
            map.SetView(new GeoCoordinate(48.737173, -3.458), 13);


           
        }


        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    // location is unsupported on this device
                    break;
                case GeoPositionStatus.NoData:
                    // data unavailable
                    break;
            }
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            var epl = e.Position.Location;
            me.lat = epl.Latitude;
            me.lon = epl.Longitude;
            
            e.Position.Timestamp.LocalDateTime.ToString();
            charger_arrets_proches();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            charger_arrets();
            charger_routes();
            charger_arrets_proches();

            //On créé un objet pour controler la géoloc
            GeoCoordinateWatcher watcher;

            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
            {
                MovementThreshold = 20
            };


            watcher.PositionChanged += this.watcher_PositionChanged;
            watcher.StatusChanged += this.watcher_StatusChanged;
            watcher.Start();

       
     
          
        }
        private void InitRouting()
        {
            if (routing == null)
            {
                routing = new RoutingHelper(((ApplicationIdCredentialsProvider)map.CredentialsProvider).ApplicationId);
                routing.RouteCalculated += new RoutingHelper.RouteCalculateEventHandler(routing_RouteCalculated);
            }
        }
        void routing_RouteCalculated(MapLayer layer, LocationRect rect, string directions)
        {
                     
            map.Children.Add(layer);
            map.SetView(rect);
        }
        private void charger_arrets()
        {
             map.Children.Clear();
             List<Pushpin> wayPoints = new List<Pushpin>();
        
             foreach (Arret arret_bus in bus.arrets_bus)
             {
 
                 Pushpin pin = new Pushpin()
                 {
                     Location = new GeoCoordinate(arret_bus.lon, arret_bus.lat),
                 };



                 pin.Opacity = 0.8;
                 pin.Content = arret_bus.arret;
                 pin.MouseLeftButtonUp += BusStop_MouseLeftButtonUp;
                 pin.Background = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
                 if (arret_bus.ligne == me.ligne)
                 {
                    
                     map.Children.Add(pin);
                 }
             }
        }
        private void charger_routes()
        {
            List<Pushpin> wayPoints = new List<Pushpin>();
            List<Ligne> temp = new List<Ligne>();


            foreach (Ligne ligne_bus in bus.arrets.lignes)
            {
                if (me.ligne == ligne_bus.ligne)
                {
                    //On vérifie qu'on a pas déjà tracé une ligne dans le sens inverse
                    if (!temp.Any(x => x.to == ligne_bus.from) && !temp.Any(x => x.from == ligne_bus.to))
                    {
                        InitRouting();
                        wayPoints = new List<Pushpin>();
                        foreach (Arret arret_bus in ligne_bus.arrets)
                        {

                            Pushpin ps = new Pushpin()
                            {
                                Location = new GeoCoordinate(arret_bus.lon, arret_bus.lat),
                            };
                            wayPoints.Add(ps);

                            //On ajoute la ligne à la liste des lignes tracées

                        }
                        temp.Add(ligne_bus);

                        if (wayPoints.Count() > 25)
                        {
                            int i = (wayPoints.Count() / 2) - 6;
                            while (i < ((wayPoints.Count() / 2) + 6))
                            {
                                wayPoints.RemoveAt(i);
                                i++;
                            }

                        }
                        routing.CalculateRoute(wayPoints);


                    }
                }
            }

            

            routing.CalculateRoute(wayPoints);
    
        }
             
        
             

        
        
         private void charger_arrets_proches()
         {
             int i= 0;
             foreach (Arret arret_bus in bus.arrets_bus)
             {
                 bus.arrets_bus[i].distance = calc.getDistanceFromLatLonInKm(arret_bus.lon, arret_bus.lat, me.lat, me.lon);
                 i++;
             }
             bus.arrets_bus = bus.arrets_bus.OrderBy(x => x.distance).ToList();
             arrets_proches.Items.Clear();
             i = 0;
             foreach (Arret arret_bus in bus.arrets_bus)
             {
                 if (i <= 10)
                 {
                     arret_binding arret_item = new arret_binding();
                     arret_item.name = arret_bus.arret;
                     arret_item.distance = arret_bus.distance.ToString().Substring(0, 4) + " km";
                     arrets_proches.Items.Add(arret_item);
                 }
                 i++;
             }
         }
        private void map_mode_click(object sender, EventArgs e)
        {
            map.Mode = new RoadMode();
        }
        private void aerien_mode_click(object sender, EventArgs e)
        {
           map.Mode = new AerialMode();
        }
        
        private void add_screen_click(object sender, EventArgs e)
        {
            try
            {
                ListBoxItem contextMenuListItem = (ListBoxItem)(arrets_proches.ItemContainerGenerator.ContainerFromItem(((MenuItem)sender).DataContext));
                arret_binding item = (arret_binding)contextMenuListItem.Content;

                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("notification.png", UriKind.Relative),
                    Title = item.name,
                    Count = 0,
                    BackTitle = item.name,
                    BackContent = "",
                };

                Uri tileUri = new Uri("/ArretsPage.xaml?arret=" + item.name, UriKind.Relative);

                ShellTile.Create(tileUri, NewTileData);
            }
            catch
            {
            }

        }
       
        void BusStop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Pushpin pin = (Pushpin)sender;

            NavigationService.Navigate(new Uri("/ArretsPage.xaml?arret=" + pin.Content.ToString(), UriKind.Relative));
        }
        private void ListeArrets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ListBoxItem contextMenuListItem = (ListBoxItem)(ListeDroplets.ItemContainerGenerator.ContainerFromItem(((MenuItem)sender).DataContext));
            //Dropletslist item = (Dropletslist)contextMenuListItem.Content;
            //Vars.current_droplet = item.id;
            try
            {
                arret_binding liste = (arret_binding)arrets_proches.SelectedItem;

                NavigationService.Navigate(new Uri("/ArretsPage.xaml?arret=" + liste.name, UriKind.Relative));
            }
            catch
            {

            }
        }

        private void locate_Click(object sender, EventArgs e)
        {
            charger_arrets();
            map.SetView(new GeoCoordinate(me.lat, me.lon), 13);
            Pushpin pin = new Pushpin()
            {
                Location = new GeoCoordinate(me.lat, me.lon),
            };


            map.Children.Add(pin);
            
           // charger_arrets_proches();
        }
        private void line_Click(object sender, EventArgs e)
        {
            isEnabled = false;
            List<string> items = new List<string>();
            items.Add(me.ligne);
            foreach (Ligne ligne_bus in bus.arrets.lignes)
            {
                if (!items.Contains(ligne_bus.ligne))
                {
                items.Add(ligne_bus.ligne);
                }
            }
            ListPicker picker = new ListPicker()
            {
                
                Header = "Lignes",
                ItemsSource = items,
                Margin = new Thickness(12, 42, 24, 18),

            };
            picker.SelectionChanged += input_Completed;
            picker.Loaded += picker_loaded;
            messageBox = new CustomMessageBox()
            {
                Caption = "Afficher une autre ligne",
                Message = "Sélectionnez une autre ligne dans la liste",
                Content = picker
            };
            messageBox.Show();
        }
        private void picker_loaded(object sender, RoutedEventArgs e)
        {
            isEnabled = true;
        }
      
        private void input_Completed(object sender, SelectionChangedEventArgs e)
        {
            if (isEnabled)
            {

                var liste = sender as ListPicker;
                try
                {
                    me.ligne = liste.SelectedItem.ToString();
                    charger_arrets();
                    charger_routes();
                }
                catch
                {

                }
                messageBox.Dismiss();
            }
            isEnabled = false;
        }
    }
}