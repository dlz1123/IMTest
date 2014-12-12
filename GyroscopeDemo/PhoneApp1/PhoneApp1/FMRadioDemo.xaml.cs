/*
 * 演示如何使用 FM 收音机
 * 
 * FMRadio - 用于操作 FM 收音机的类
 *     Instance - 返回 FMRadio 实例
 *     CurrentRegion - 收音机的区域信息（Microsoft.Devices.Radio.RadioRegion 枚举）
 *         UnitedStates - 美国
 *         Japan - 日本
 *         Europe - 其他地区
 *     Frequency - 指定 FM 调频的频率
 *     PowerMode - 打开或关闭收音机（Microsoft.Devices.Radio.RadioPowerMode 枚举）
 *         On - 打开收音机
 *         Off - 关闭收音机
 *     SignalStrength - 当前频率的信号强度（RSSI - Received Signal Strength Indication）
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

using Microsoft.Devices.Radio;
using System.Windows.Threading;
using System.Threading;

namespace Demo.Device
{
    public partial class FMRadioDemo : PhoneApplicationPage
    {
        private FMRadio _radio;
        private DispatcherTimer _timer;

        public FMRadioDemo()
        {
            InitializeComponent();

            // 实例化 FMRadio，收听 90.5 频率
            _radio = FMRadio.Instance;
            _radio.CurrentRegion = RadioRegion.Europe;
            _radio.Frequency = 90.5;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();

            if (_radio.PowerMode == RadioPowerMode.On)
                lblStatus.Text = "收音机已打开";
            else
                lblStatus.Text = "收音机已关闭";
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            // 实时显示当前频率及信号强度
            lblMsg.Text = "调频：" + _radio.Frequency;
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "RSSI：" + _radio.SignalStrength.ToString("0.00");
        }

        // 打开收音机
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            lblStatus.Text = "收音机打开中。。。";

            // 首次启动收音机可能需要多达 3 秒的时间，以后再启动收音机则会在 100 毫秒以内，所以建议在后台线程打开收音机
            new Thread((x) =>
            {
                _radio.PowerMode = RadioPowerMode.On;
                Dispatcher.BeginInvoke(delegate
                {
                    lblStatus.Text = "收音机已打开";
                });
            }).Start();
        }

        // 关闭收音机
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _radio.PowerMode = RadioPowerMode.Off;
            lblStatus.Text = "收音机已关闭";
        }
    }
}