﻿/*
 * 演示如何使用陀螺仪传感器
 * 
 * Gyroscope - 用于访问设备中的陀螺仪
 *     IsSupported - 设备是否支持陀螺仪
 *     IsDataValid - 是否可从陀螺仪中获取到有效数据
 *     CurrentValue - 陀螺仪当前的数据，GyroscopeReading 类型
 *     TimeBetweenUpdates - 触发 CurrentValueChanged 事件的时间间隔，如果设置的值小于 Gyroscope 允许的最小值，则此属性的值将被设置为 Gyroscope 允许的最小值
 *     Start() - 打开陀螺仪
 *     Stop() - 关闭陀螺仪
 *     CurrentValueChanged - 陀螺仪传感器获取到的数据发生改变时所触发的事件，属性 TimeBetweenUpdates 的值决定触发此事件的时间间隔
 *     
 * GyroscopeReading - 陀螺仪传感器数据
 *     RotationRate - 获取围绕设备各轴旋转的旋转速率（单位：弧度/秒）
 *     DateTimeOffset - 从陀螺仪传感器中获取到数据的时间点
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

namespace Demo.Device
{
    public partial class GyroscopeDemo : PhoneApplicationPage
    {
        private Gyroscope _gyroscope;

        public GyroscopeDemo()
        {
            InitializeComponent();

            // 判断设备是否支持陀螺仪
            if (Gyroscope.IsSupported)
            {
                lblGyroscopeStatus.Text = "此设备支持陀螺仪";
            }
            else
            {
                lblGyroscopeStatus.Text = "此设备不支持陀螺仪";

                btnStart.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_gyroscope == null)
            {
                // 实例化 Gyroscope，注册相关事件
                _gyroscope = new Gyroscope();
                _gyroscope.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1);
                _gyroscope.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<GyroscopeReading>>(_gyroscope_CurrentValueChanged);

                lblTimeBetweenUpdates.Text = "TimeBetweenUpdates 设置为 1 毫秒，实际为 " + _gyroscope.TimeBetweenUpdates.TotalMilliseconds.ToString() + " 毫秒";
            }

            try
            {
                // 打开陀螺仪
                _gyroscope.Start();
                lblGyroscopeStatus.Text = "陀螺仪已打开";
            }
            catch (Exception ex)
            {
                lblGyroscopeStatus.Text = "陀螺仪打开失败";
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_gyroscope != null)
            {
                // 关闭陀螺仪
                _gyroscope.Stop();
                lblGyroscopeStatus.Text = "陀螺仪已关闭";
            }
        }

        void _gyroscope_CurrentValueChanged(object sender, SensorReadingEventArgs<GyroscopeReading> e)
        {
            // 注：此方法是在后台线程运行的，所以需要更新 UI 的话注意要调用 UI 线程
            Dispatcher.BeginInvoke(() => UpdateUI(e.SensorReading));
        }

        private DateTimeOffset _lastUpdateTime = DateTimeOffset.MinValue; // 上一次获取陀螺仪数据的时间
        private Vector3 _rotationTotal = Vector3.Zero; // 陀螺仪各轴的累积旋转弧度
        // 更新 UI
        private void UpdateUI(GyroscopeReading gyroscopeReading)
        {
            // 以下用于计算陀螺仪各轴的累积旋转弧度
            if (_lastUpdateTime.Equals(DateTimeOffset.MinValue))
            {
                _lastUpdateTime = gyroscopeReading.Timestamp;
            }
            else
            {
                TimeSpan timeSinceLastUpdate = gyroscopeReading.Timestamp - _lastUpdateTime;

                // 陀螺仪当前旋转速率 * 计算此速率所经过的时间 = 此时间段内旋转的弧度
                _rotationTotal += gyroscopeReading.RotationRate * (float)(timeSinceLastUpdate.TotalSeconds);

                _lastUpdateTime = gyroscopeReading.Timestamp;
            }
            try
            {
                Vector3 rotationRate = gyroscopeReading.RotationRate;

                //var transform = img.RenderTransform;
                //transform.TranslateX += _rotationTotal.X;
                //transform.TranslateY += _rotationTotal.Y;

                var _compositeTransform = new CompositeTransform();
                var _previousTransform = new MatrixTransform() { Matrix = System.Windows.Media.Matrix.Identity };

                var _transformGroup = new TransformGroup();
                _transformGroup.Children.Add(_previousTransform);

                _compositeTransform.TranslateX = 100*_rotationTotal.X;
                _compositeTransform.TranslateY = 100*_rotationTotal.Y;
                _transformGroup.Children.Add(_compositeTransform);

                img.RenderTransform = _transformGroup;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }
    }
}