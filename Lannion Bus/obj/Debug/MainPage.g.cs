﻿#pragma checksum "C:\Users\baptiste\Documents\Visual Studio 2012\Projects\Lannion Bus\Lannion Bus\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "125BFF47DC4D2BCF3F38852A3D4096DE"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34011
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Lannion_Bus {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid TitlePanel;
        
        internal Microsoft.Phone.Controls.Maps.Map map;
        
        internal System.Windows.Controls.ListBox arrets_proches;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem map_mode;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem aerien_mode;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Lannion%20Bus;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.Grid)(this.FindName("TitlePanel")));
            this.map = ((Microsoft.Phone.Controls.Maps.Map)(this.FindName("map")));
            this.arrets_proches = ((System.Windows.Controls.ListBox)(this.FindName("arrets_proches")));
            this.map_mode = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("map_mode")));
            this.aerien_mode = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("aerien_mode")));
        }
    }
}

