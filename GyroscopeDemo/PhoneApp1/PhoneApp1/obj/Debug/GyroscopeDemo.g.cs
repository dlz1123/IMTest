﻿#pragma checksum "D:\RongCloud\MyIM\IMTest\GyroscopeDemo\PhoneApp1\PhoneApp1\GyroscopeDemo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "90830336C3AF18D19AC5C2065AFEA8BD"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18449
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
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


namespace Demo.Device {
    
    
    public partial class GyroscopeDemo : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock lblGyroscopeSupported;
        
        internal System.Windows.Controls.Button btnStart;
        
        internal System.Windows.Controls.Button btnStop;
        
        internal System.Windows.Controls.TextBlock lblGyroscopeStatus;
        
        internal System.Windows.Controls.TextBlock lblTimeBetweenUpdates;
        
        internal System.Windows.Controls.TextBlock lblMsg;
        
        internal System.Windows.Controls.Image img;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/PhoneApp1;component/GyroscopeDemo.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.lblGyroscopeSupported = ((System.Windows.Controls.TextBlock)(this.FindName("lblGyroscopeSupported")));
            this.btnStart = ((System.Windows.Controls.Button)(this.FindName("btnStart")));
            this.btnStop = ((System.Windows.Controls.Button)(this.FindName("btnStop")));
            this.lblGyroscopeStatus = ((System.Windows.Controls.TextBlock)(this.FindName("lblGyroscopeStatus")));
            this.lblTimeBetweenUpdates = ((System.Windows.Controls.TextBlock)(this.FindName("lblTimeBetweenUpdates")));
            this.lblMsg = ((System.Windows.Controls.TextBlock)(this.FindName("lblMsg")));
            this.img = ((System.Windows.Controls.Image)(this.FindName("img")));
        }
    }
}

