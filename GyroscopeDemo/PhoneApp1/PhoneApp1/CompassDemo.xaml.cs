/*
 * 演示如何使用数字罗盘传感器
 * 
 * Compass - 用于访问设备中的磁力计
 *     IsSupported - 设备是否支持数字罗盘
 *     IsDataValid - 是否可从数字罗盘中获取到有效数据
 *     CurrentValue - 数字罗盘当前的数据，CompassReading 类型
 *     TimeBetweenUpdates - 触发 CurrentValueChanged 事件的时间间隔，如果设置的值小于 Compass 允许的最小值，则此属性的值将被设置为 Compass 允许的最小值
 *     Start() - 打开磁力计
 *     Stop() - 关闭磁力计
 *     CurrentValueChanged - 数字罗盘传感器获取到的数据发生改变时所触发的事件，属性 TimeBetweenUpdates 的值决定触发此事件的时间间隔
 *     Calibrate - 当系统检测到数字罗盘需要校准时所触发的事件
 *     
 * CompassReading - 数字罗盘传感器数据
 *     HeadingAccuracy - 数字罗盘的精度，其绝对值如果大于 10 则需要校准
 *     TrueHeading - 与地理北极的顺时针方向的偏角（单位：角度），经测试可用
 *     MagneticHeading - 与地磁北极的顺时针方向的偏角（单位：角度）,经测试理解不了
 *     MagnetometerReading - 磁力计的原始数据（单位：微特斯拉），经测试理解不了
 *     DateTimeOffset - 从数字罗盘传感器中获取到数据的时间点
 *     
 * 
 * 
 * 手机坐标系：以手机位置为参照，假设手机垂直水平面放（竖着放），屏幕对着你，那么
 * 1、左右是 X 轴，右侧为正方向，左侧为负方向
 * 2、上下是 Y 轴，上侧为正方向，下侧为负方向
 * 3、里外是 Z 轴，靠近你为正方向，远离你为负方向
 * 以上可以用相对于手机位置的右手坐标系来理解
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

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using System.Windows.Threading;

namespace Demo.Device
{
    public partial class CompassDemo : PhoneApplicationPage
    {
        private Compass _compass;
        private Accelerometer _accelerometer;

        public CompassDemo()
        {
            InitializeComponent();

            // 判断设备是否支持数字罗盘
            if (Compass.IsSupported)
            {
                lblCompassSupported.Text = "此设备支持数字罗盘";
            }
            else
            {
                lblCompassSupported.Text = "此设备不支持数字罗盘";

                btnStart.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_compass == null)
            {
                // 实例化 Compass，注册相关事件
                _compass = new Compass();
                _compass.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);
                _compass.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<CompassReading>>(_compass_CurrentValueChanged);
                _compass.Calibrate += new EventHandler<CalibrationEventArgs>(_compass_Calibrate);

                lblTimeBetweenUpdates.Text = "TimeBetweenUpdates 设置为 1 毫秒，实际为 " + _compass.TimeBetweenUpdates.TotalMilliseconds.ToString() + " 毫秒";

                // 实例化 Accelerometer，注册相关事件，用于判断手机是横放状态还是竖放状态
                _accelerometer = new Accelerometer();
                _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_CurrentValueChanged);
            }

            try
            {
                // 打开数字罗盘
                _compass.Start();
                lblCompassStatus.Text = "数字罗盘已打开";

                _accelerometer.Start();
            }
            catch (Exception ex)
            {
                lblCompassStatus.Text = "数字罗盘打开失败";
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_compass != null && _compass.IsDataValid)
            {
                // 关闭数字罗盘
                _compass.Stop();
                _accelerometer.Stop();

                lblCompassStatus.Text = "数字罗盘已关闭";
            }
        }

        void _compass_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            // 注：此方法是在后台线程运行的，所以需要更新 UI 的话注意要调用 UI 线程
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }

        // 更新 UI
        private void UpdateUI(CompassReading compassReading)
        {
            // 显示从数字罗盘中获取到的各个参数
            lblMsg.Text = "magneticHeading: " + compassReading.MagneticHeading.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "trueHeading: " + compassReading.TrueHeading.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "headingAccuracy: " + Math.Abs(compassReading.HeadingAccuracy).ToString("0.0");
            lblMsg.Text += Environment.NewLine;

            Vector3 magnetometerReading = compassReading.MagnetometerReading;

            lblMsg.Text += "magnetometerReading.X: " + magnetometerReading.X.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "magnetometerReading.Y: " + magnetometerReading.Y.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "magnetometerReading.Z: " + magnetometerReading.Z.ToString("0.0");
        }

        void _compass_Calibrate(object sender, CalibrationEventArgs e)
        {
            // 注：此方法是在后台线程运行的，所以需要更新 UI 的话注意要调用 UI 线程

            // 显示“提示页”，用于提示用户设备需要校准
            Dispatcher.BeginInvoke(() => { gridCalibration.Visibility = Visibility.Visible; });
        }

        private void btnKnown_Click(object sender, RoutedEventArgs e)
        {
            // 隐藏“提示页”
            gridCalibration.Visibility = System.Windows.Visibility.Collapsed;
        }

        // 判断手机是竖放还是横放
        void _accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Vector3 v = e.SensorReading.Acceleration;

            bool isCompassUsingNegativeZAxis = false;

            if (Math.Abs(v.Z) < Math.Cos(Math.PI / 4) && (v.Y < Math.Sin(7 * Math.PI / 4)))
            {
                isCompassUsingNegativeZAxis = true;
            }

            Dispatcher.BeginInvoke(() => { lblOrientation.Text = (isCompassUsingNegativeZAxis) ? "手机竖放" : "手机横放"; });
        }
    }
}