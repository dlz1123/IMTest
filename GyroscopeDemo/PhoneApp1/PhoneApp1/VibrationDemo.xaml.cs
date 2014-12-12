/*
 * 演示如何使用震动器
 * 
 * VibrateController - 用于控制震动器
 *     Default - 获取 VibrateController 实例
 *     Start(TimeSpan duration) - 指定震动时长，并使设备震动。有效时长在 0 - 5 秒之间，否则会抛出异常
 *     Stop() - 停止设备的震动
 */

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

using Microsoft.Devices;

namespace Demo.Device
{
    public partial class VibrationDemo : PhoneApplicationPage
    {
        public VibrationDemo()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            // 震动 5 秒
            VibrateController.Default.Start(TimeSpan.FromMilliseconds(5 * 1000));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            // 停止震动
            VibrateController.Default.Stop();
        }
    }
}