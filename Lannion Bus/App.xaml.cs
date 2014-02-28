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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using Newtonsoft.Json;

namespace Lannion_Bus
{
    public partial class App : Application
    {
        /// <summary>
        /// Permet d'accéder facilement au frame racine de l'application téléphonique.
        /// </summary>
        /// <returns>Frame racine de l'application téléphonique.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructeur pour l'objet Application.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Initialisation spécifique au téléphone
            InitializePhoneApplication();
            ThemeManager.ToLightTheme();
            RootFrame = new TransitionFrame();
            // Affichez des informations de profilage graphique lors du débogage.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Affichez les compteurs de fréquence des trames actuels.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Affichez les zones de l'application qui sont redessinées dans chaque frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // qui montre les zones d'une page sur lesquelles une accélération GPU est produite avec une superposition colorée.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Attention :- À utiliser uniquement en mode de débogage. Les applications qui désactivent la détection d'inactivité de l'utilisateur continueront de s'exécuter
                // et seront alimentées par la batterie lorsque l'utilisateur ne se sert pas du téléphone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            }

        }

        // Code à exécuter lorsque l'application démarre (par exemple, à partir de Démarrer)
        // Ce code ne s'exécute pas lorsque l'application est réactivée
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            //On charge le json des arrêts de bus
            Stream json_stream = Application.GetResourceStream(new Uri("Content/arrets.json", UriKind.Relative)).Stream;
            StreamReader reader = new StreamReader(json_stream);
            string json = reader.ReadToEnd();

            var j = JsonConvert.DeserializeObject<Arrets>(json);
            //on stocke ça dans une liste
            bus.arrets = j;
            charger_arrets();

            Stream json_stream2 = Application.GetResourceStream(new Uri("Content/horaires.json", UriKind.Relative)).Stream;
            StreamReader reader2 = new StreamReader(json_stream2);
            json = reader2.ReadToEnd();

            var j2 = JsonConvert.DeserializeObject<RootObject>(json);

            //on stocke ça dans une liste
            bus.horaires = j2;
            

           
            Stream json_stream3= Application.GetResourceStream(new Uri("Content/vacances.json", UriKind.Relative)).Stream;
            StreamReader reader3 = new StreamReader(json_stream3);
            json = reader3.ReadToEnd();

            var j3 = JsonConvert.DeserializeObject<Hollidays>(json);
            //on stocke ça dans une liste
            bus.vacances = new List<Holliday>();
            bus.vacances = j3.hollidays;

            RootFrame.Navigated += DatePicker_Navigated;
            RootFrame.Navigated += TimePicker_Navigated;
        }
         private void charger_arrets()
        {
            int i = 0;
             bus.arrets_bus = new List<Arret>();

             foreach (Ligne ligne_bus in bus.arrets.lignes)
             {
                 
                 foreach (Arret arret_bus in ligne_bus.arrets)
                 {
                     
                     if(!bus.arrets_bus.Any(x=> x.arret==arret_bus.arret))
                     {
                        arret_bus.ligne = ligne_bus.ligne;
                        bus.arrets_bus.Add(arret_bus);
                     }
                 }
             }
          
        }
        // Code à exécuter lorsque l'application est activée (affichée au premier plan)
        // Ce code ne s'exécute pas lorsque l'application est démarrée pour la première fois
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code à exécuter lorsque l'application est désactivée (envoyée à l'arrière-plan)
        // Ce code ne s'exécute pas lors de la fermeture de l'application
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code à exécuter lors de la fermeture de l'application (par exemple, lorsque l'utilisateur clique sur Précédent)
        // Ce code ne s'exécute pas lorsque l'application est désactivée
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }
       

        private void DatePicker_Navigated(object sender, NavigationEventArgs e)
        {
            //  this is a dirty hack to circumvent the hard coded title of the datepicker

            try
            {
                if (e.Uri == null || e.Content == null || !(e.Content is DatePickerPage)  || e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/DatePickerPage.xaml")
                    return;

                DatePickerPage objDatePickerPage = (DatePickerPage)e.Content;
                FrameworkElement objSystemTrayPlaceholder = (FrameworkElement)objDatePickerPage.FindName("SystemTrayPlaceholder");
                Grid objParentGrid = (Grid)objSystemTrayPlaceholder.Parent;
                TextBlock objTitleTextBox = (TextBlock)objParentGrid.Children.First(c => c.GetType() == typeof(TextBlock));
                objTitleTextBox.Text = "Veuillez choisir une date"; // put your resource access here
            
            }
            catch
            {

            }
        }
        private void TimePicker_Navigated(object sender, NavigationEventArgs e)
        {
            //  this is a dirty hack to circumvent the hard coded title of the datepicker

            try
            {
                if (e.Uri == null || e.Content == null || !(e.Content is TimePickerPage)  || e.Uri.OriginalString != "/Microsoft.Phone.Controls.Toolkit;component/DateTimePickers/TimePickerPage.xaml")
                    return;

                TimePickerPage objDatePickerPage = (TimePickerPage)e.Content;
                FrameworkElement objSystemTrayPlaceholder = (FrameworkElement)objDatePickerPage.FindName("SystemTrayPlaceholder");
                Grid objParentGrid = (Grid)objSystemTrayPlaceholder.Parent;
                TextBlock objTitleTextBox = (TextBlock)objParentGrid.Children.First(c => c.GetType() == typeof(TextBlock));
                objTitleTextBox.Text = "Veuillez choisir une heure"; // put your resource access here
            
            }
            catch
            {

            }
        }
        // Code à exécuter en cas d'échec d'une navigation
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Échec d'une navigation ; arrêt dans le débogueur
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code à exécuter sur les exceptions non gérées
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Une exception non gérée s'est produite ; arrêt dans le débogueur
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Initialisation de l'application téléphonique

        // Éviter l'initialisation double
        private bool phoneApplicationInitialized = false;

        // Ne pas ajouter de code supplémentaire à cette méthode
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Créez le frame, mais ne le définissez pas encore comme RootVisual ; cela permet à l'écran de
            // démarrage de rester actif jusqu'à ce que l'application soit prête pour le rendu.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Gérer les erreurs de navigation
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Garantir de ne pas retenter l'initialisation
            phoneApplicationInitialized = true;
        }

        // Ne pas ajouter de code supplémentaire à cette méthode
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Définir le Visual racine pour permettre à l'application d'effectuer le rendu
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Supprimer ce gestionnaire, puisqu'il est devenu inutile
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}