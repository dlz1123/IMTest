/*
 * 演示如何使用加速度传感器
 * 
 * Accelerometer - 用于访问设备中的加速度计
 *     IsSupported - 设备是否支持加速度传感器
 *     IsDataValid - 是否可从加速度传感器中获取到有效数据
 *     CurrentValue - 加速度传感器当前的数据，AccelerometerReading 类型
 *     TimeBetweenUpdates - 触发 CurrentValueChanged 事件的时间间隔，如果设置的值小于 Accelerometer 允许的最小值，则此属性的值将被设置为 Accelerometer 允许的最小值
 *     State - 加速度计的当前的状态（Microsoft.Devices.Sensors.SensorState 枚举）
 *         NotSupported - 设备不支持加速度传感器
 *         Ready - 加速度传感器已准备好，并且正在解析数据
 *         Initializing - 加速度传感器正在初始化
 *         NoData - 加速度传感器无法获取数据
 *         NoPermissions - 无权限调用加速度传感器
 *         Disabled - 加速度传感器被禁用
 *     Start() - 打开加速度计
 *     Stop() - 关闭加速度计
 *     CurrentValueChanged - 加速度传感器获取到的数据发生改变时所触发的事件，属性 TimeBetweenUpdates 的值决定触发此事件的时间间隔
 *     
 * AccelerometerReading - 加速度传感器数据
 *     Acceleration - 详细数据，Vector3 类型的值
 *     DateTimeOffset - 从加速度传感器中获取到数据的时间点
 *     
 * 
 * 
 * 关于从加速度传感器中获取到的 Vector3 类型的值中 X Y Z 的解释如下
 * 手机坐标系：以手机位置为参照，假设手机垂直水平面放（竖着放），屏幕对着你，那么
 * 1、左右是 X 轴，右侧为正方向，左侧为负方向
 * 2、上下是 Y 轴，上侧为正方向，下侧为负方向
 * 3、里外是 Z 轴，靠近你为正方向，远离你为负方向
 * 以上可以用相对于手机位置的右手坐标系来理解
 * X Y Z 的值为中心点到地平面方向的线与各个对应轴线正方向的夹角的余弦值
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

namespace Demo.Device
{
    public partial class AccelerometerDemo : PhoneApplicationPage
    {
        private Accelerometer _accelerometer;

        public AccelerometerDemo()
        {
            InitializeComponent();

            // 判断设备是否支持加速度传感器
            if (Accelerometer.IsSupported)
            {
                lblAccelerometerStatus.Text = "此设备支持加速度传感器";
            }
            else
            {
                lblAccelerometerStatus.Text = "此设备不支持加速度传感器";
                
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_accelerometer == null)
            {
                // 实例化 Accelerometer，注册相关事件
                _accelerometer = new Accelerometer();
                _accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);
                _accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(_accelerometer_CurrentValueChanged);

                lblTimeBetweenUpdates.Text = "TimeBetweenUpdates 设置为 1 毫秒，实际为 " + _accelerometer.TimeBetweenUpdates.TotalMilliseconds.ToString() + " 毫秒";
            }

            try
            {
                // 打开加速度传感器
                _accelerometer.Start();
                lblAccelerometerStatus.Text = "加速度传感器已打开";
            }
            catch (Exception ex)
            {
                lblAccelerometerStatus.Text = "加速度传感器已打开失败";
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_accelerometer != null)
            {
                // 关闭加速度传感器
                _accelerometer.Stop();
                lblAccelerometerStatus.Text = "加速度传感器已关闭";
            }
        }

        void _accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            // 注：此方法是在后台线程运行的，所以需要更新 UI 的话注意要调用 UI 线程
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }
        
        // 更新 UI
        private void UpdateUI(AccelerometerReading accelerometerReading)
        {
            Vector3 acceleration = accelerometerReading.Acceleration;

            // 输出 X Y Z 的值
            lblMsg.Text = "acceleration.X: " + acceleration.X.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "acceleration.Y: " + acceleration.Y.ToString("0.0");
            lblMsg.Text += Environment.NewLine;
            lblMsg.Text += "acceleration.Z: " + acceleration.Z.ToString("0.0");
        }
    }
}