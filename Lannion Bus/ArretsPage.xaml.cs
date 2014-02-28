using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Toolkit.Controls;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;

namespace Lannion_Bus
{
    public partial class ArretPage : PhoneApplicationPage
    {
        CustomMessageBox messageBox;
        bool isEnabled = false;
        public ArretPage()
        {
            InitializeComponent();
           this.datePicker.ValueChanged += new EventHandler<DateTimeValueChangedEventArgs>(picker_ValueChanged);
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            String msg = "";
       
            List<prochains_binding> liste_horaires = new List<prochains_binding>();
            InitializeComponent();
            if (NavigationContext.QueryString.TryGetValue("arret", out msg))
            {
                arret_title.Text = msg;
            }

            liste_horaires = get_arrets.get_bus(arret_title.Text, DateTime.Now);
            liste_horaires = liste_horaires.OrderBy(x => x.time).ToList();
            foreach (prochains_binding item in liste_horaires)
            {
                if(item.time >= int.Parse(DateTime.Now.ToString("HHmm")))
                {
                    prochains_bus.Items.Add(item);
                }
               // aujourdhui.Items.Add(item);
            }
           
        }

        void picker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime date = (DateTime)e.NewDateTime;

            aujourdhui.Items.Clear();

            List<prochains_binding> liste_horaires = new List<prochains_binding>();
            liste_horaires = get_arrets.get_bus(arret_title.Text, date);
            liste_horaires = liste_horaires.OrderBy(x => x.time).ToList();
            foreach (prochains_binding item in liste_horaires)
            {
                if (item.time >= int.Parse(date.ToString("HHmm")))
                {
                    aujourdhui.Items.Add(item);
             
                }
                
            }
        }
        private void pin_Click(object sender, EventArgs e)
        {
            try
            {
              

                StandardTileData NewTileData = new StandardTileData
                {
                    BackgroundImage = new Uri("notification.png", UriKind.Relative),
                    Title = arret_title.Text,
                    Count = 0,
                    BackTitle = arret_title.Text,
                    BackContent = "",
                };

                Uri tileUri = new Uri("/ArretsPage.xaml?arret=" + arret_title.Text, UriKind.Relative);

                ShellTile.Create(tileUri, NewTileData);
            }
            catch
            {
            }
        }
        private void send_sms1_click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxItem contextMenuListItem = (ListBoxItem)(prochains_bus.ItemContainerGenerator.ContainerFromItem(((MenuItem)sender).DataContext));
                prochains_binding item = (prochains_binding)contextMenuListItem.Content;
               
                var input = new InputPrompt();
                input.Completed += input_Completed;
                input.Message = "Votre numéro de téléphone";
                me.current_id = item.id;
                if (IsolatedStorageSettings.ApplicationSettings.Contains("number"))
                {
                    input.Value = (String)IsolatedStorageSettings.ApplicationSettings["number"];
                }

                input.Show();
            }
            catch
            {
            }
        }
        private void send_sms2_click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBoxItem contextMenuListItem = (ListBoxItem)(aujourdhui.ItemContainerGenerator.ContainerFromItem(((MenuItem)sender).DataContext));
                prochains_binding item = (prochains_binding)contextMenuListItem.Content;
                me.current_id = item.id;
                var input = new InputPrompt();
                input.Completed += input_Completed;
                input.Message = "Votre numéro de téléphone";
                
                if (IsolatedStorageSettings.ApplicationSettings.Contains("number"))
                {
                    input.Value = (String)IsolatedStorageSettings.ApplicationSettings["number"];
                }

                input.Show();
            }
            catch
            {
            }
        }
        private void input_Completed(object sender, PopUpEventArgs<string, PopUpResult> e)
        {
            int i = 1;
            isEnabled = false;
           
            List<string> items = new List<string>();
            items.Add(i.ToString() + " minute");
            i++;
            while (i < 60)
            {
                items.Add(i.ToString()+ " minutes");
                i++;
            }
            if ((Regex.IsMatch(e.Result, @"^06[0-9]{8}$|^00[0-9]{11,13}$")) || (Regex.IsMatch(e.Result, @"^07[0-9]{8}$|^00[0-9]{11,13}$")))
            {
                ((InputPrompt)sender).Hide();
                IsolatedStorageSettings.ApplicationSettings["number"] = e.Result;
                IsolatedStorageSettings.ApplicationSettings.Save();
                ListPicker picker = new ListPicker()
                {

                    Header = "Lignes",
                    ItemsSource = items,
                    Margin = new Thickness(12, 42, 24, 18),

                };
                picker.SelectionChanged += input_Completed2;
                picker.Loaded += picker_loaded;
                messageBox = new CustomMessageBox()
                {
                    Caption = "Il faut vous prévenir combien de temps avant?",
                    Content = picker
                };
                messageBox.Show();
            }
            else
            {
                MessageBox.Show("Le numéro de téléphone n'est pas valide");
            }
        }
        private void picker_loaded(object sender, RoutedEventArgs e)
        {
            isEnabled = true;
        }

        private void input_Completed2(object sender, SelectionChangedEventArgs e)
        {
            WebClient webClient = new WebClient();

            if (isEnabled)
            {

                var liste = sender as ListPicker;
                try
                {
                    int minutes = liste.SelectedIndex;
                    webClient.DownloadStringCompleted += (s, b) =>
                    {
                        if (b.Error == null)
                        {
                            if (b.Result.Contains("ok"))
                            {
                                MessageBox.Show("Vous allez recevoir une notification SMS pour cet arrêt " + (minutes+1).ToString()  + " minutes avant son passage");
                            }
                            else
                            {
                                MessageBox.Show("Une erreur a eu lieu");
                            }
                        }
                    };
                    webClient.DownloadStringAsync(new Uri("http://projects.emerginov.org/LannionBus/abonnement.php?numero=" + IsolatedStorageSettings.ApplicationSettings["number"] + "&id="+me.current_id+"&minutes="+minutes.ToString()));
                }
                catch
                {

                }
                messageBox.Dismiss();
            }
            isEnabled = false;
        }
        private void ProchainsBus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }
        private void aujourdhui_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}