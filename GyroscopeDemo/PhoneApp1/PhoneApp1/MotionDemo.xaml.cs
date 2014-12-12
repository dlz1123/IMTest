/*
 * 演示如何使用 Motion API
 * 
 * Motion - Motion API，其作用为：整合各个传感器 Accelerometer, Gyroscope, Compass 的数据，通过复杂的运算计算出易用的数据
 *     IsSupported - 设备是否支持 Motion API
 *     IsDataValid - 是否可从 Motion API 中获取到有效数据
 *     CurrentValue - Motion API 当前的数据，MotionReading 类型
 *     TimeBetweenUpdates - 触发 CurrentValueChanged 事件的时间间隔，如果设置的值小于 Motion API 允许的最小值，则此属性的值将被设置为 Motion API 允许的最小值
 *     Start() - 打开 Motion 监测
 *     Stop() - 关闭 Motion 监测
 *     CurrentValueChanged - 当 Motion API 获取到的数据发生改变时所触发的事件，属性 TimeBetweenUpdates 的值决定触发此事件的时间间隔
 *     Calibrate - 当系统检测到 Motion API 用到的数字罗盘传感器需要校准时所触发的事件
 *     
 * MotionReading - Motion API 数据
 *     AttitudeReading - 设备的姿态（AttitudeReading 类型，可以获得 Yaw Pitch Roll 数据）
 *     DeviceAcceleration - 设备的加速度（Vector3 类型）
 *     DeviceRotationRate - 设备的旋转速率（Vector3 类型）
 *     Gravity - 重力（Vector3 类型）
 *     DateTimeOffset - 从 Motion API 中获取到数据的时间点
 *     
 * 注：
 * 如果设备缺少必要的传感器，那么 Motion API 将无法正常工作
 * 例如，如果只有 Accelerometer, Compass 而没有 Gyroscope，那么虽然 Motion API 是被支持的，但是部分数据是不精准的
 * 例如，如果只有 Accelerometer 而没有 Compass, Gyroscope，那么 Motion API 是不被支持的
 * 
 * 
 * 
 * 手机坐标系：以手机位置为参照，假设手机垂直水平面放（竖着放），屏幕对着你，那么
 * 1、左右是 X 轴，右侧为正方向，左侧为负方向
 * 2、上下是 Y 轴，上侧为正方向，下侧为负方向
 * 3、里外是 Z 轴，靠近你为正方向，远离你为负方向
 * 以上可以用相对于手机位置的右手坐标系来理解
 * 
 * Yaw - 围绕 Y 轴旋转
 * Pitch - 围绕 X 轴旋转
 * Roll - 围绕 Z 轴旋转
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

using System.Windows.Navigation;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;

namespace Demo.Device
{
    public partial class MotionDemo : PhoneApplicationPage
    {
        Motion motion;
        public MotionDemo()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Check to see whether the Motion API is supported on the device.
            if (!Motion.IsSupported)
            {
                MessageBox.Show("the Motion API is not supported on this device.");
                return;
            }

            // If the Motion object is null, initialize it and add a CurrentValueChanged
            // event handler.
            if (motion == null)
            {
                motion = new Motion();
                motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
                motion.CurrentValueChanged +=
                    new EventHandler<SensorReadingEventArgs<MotionReading>>(motion_CurrentValueChanged);
            }

            // Try to start the Motion API.
            try
            {
                motion.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("unable to start the Motion API.");
            }
        }
        void motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            // This event arrives on a background thread. Use BeginInvoke to call
            // CurrentValueChanged on the UI thread.
            Dispatcher.BeginInvoke(() => CurrentValueChanged(e.SensorReading));
        }
        private void CurrentValueChanged(MotionReading e)
        {
            // Check to see if the Motion data is valid.
            if (motion.IsDataValid)
            {
                // Show the numeric values for attitude.
                yawTextBlock.Text =
                    "YAW: " + MathHelper.ToDegrees(e.Attitude.Yaw).ToString("0") + "°";
                pitchTextBlock.Text =
                    "PITCH: " + MathHelper.ToDegrees(e.Attitude.Pitch).ToString("0") + "°";
                rollTextBlock.Text =
                    "ROLL: " + MathHelper.ToDegrees(e.Attitude.Roll).ToString("0") + "°";

                // Set the Angle of the triangle RenderTransforms to the attitude of the device.
                ((RotateTransform)yawtriangle.RenderTransform).Angle =
                    MathHelper.ToDegrees(e.Attitude.Yaw);
                ((RotateTransform)pitchtriangle.RenderTransform).Angle =
                    MathHelper.ToDegrees(e.Attitude.Pitch);
                ((RotateTransform)rolltriangle.RenderTransform).Angle =
                    MathHelper.ToDegrees(e.Attitude.Roll);

                // Show the numeric values for acceleration.
                xTextBlock.Text = "X: " + e.DeviceAcceleration.X.ToString("0.00");
                yTextBlock.Text = "Y: " + e.DeviceAcceleration.Y.ToString("0.00");
                zTextBlock.Text = "Z: " + e.DeviceAcceleration.Z.ToString("0.00");

                // Show the acceleration values graphically.
                xLine.X2 = xLine.X1 + e.DeviceAcceleration.X * 100;
                yLine.Y2 = yLine.Y1 - e.DeviceAcceleration.Y * 100;
                zLine.X2 = zLine.X1 - e.DeviceAcceleration.Z * 50;
                zLine.Y2 = zLine.Y1 + e.DeviceAcceleration.Z * 50;
            }
        }

        
    }
}