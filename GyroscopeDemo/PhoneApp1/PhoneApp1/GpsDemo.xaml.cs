/*
 * 演示如何使用位置服务（GPS 定位）
 * 
 * GeoCoordinateWatcher - 用于提供地理位置数据
 *     Start() - 启动位置服务
 *     TryStart(bool suppressPermissionPrompt, TimeSpan timeout) - 尝试启动位置服务，返回值为位置服务是否启动成功
 *         suppressPermissionPrompt - 是否禁用权限提示对话框。true为禁用，false为启用
 *         timeout - 启动位置服务的超时时间
 *     Stop() - 停止位置服务
 *     
 *     DesiredAccuracy - 指定提供位置服务的精度级别（System.Device.Location.GeoPositionAccuracy 枚举）
 *         Default - 低精度定位
 *         High - 高精度定位
 *     Permission - 位置提供程序的权限，只读（System.Device.Location.GeoPositionPermission 枚举）
 *         Unknown - 权限未知
 *         Granted - 授予定位权限
 *         Denied - 拒绝定位权限
 *     Status - 位置服务的状态（System.Device.Location.GeoPositionStatus 枚举）
 *         Ready - 已经准备好相关数据
 *         Initializing - 位置提供程序初始化中
 *         NoData - 无有效数据
 *         Disabled - 位置服务不可用
 *     Position - 定位的位置数据，只读（Position.Location 是一个 System.Device.Location.GeoCoordinate 类型的对象）
 *     MovementThreshold - 自上次触发 PositionChanged 事件后，位置移动了此属性指定的距离后再次触发 PositionChanged 事件（单位：米）
 *         此属性默认值为 0，即位置的任何改变都会触发 PositionChanged 事件
 *     
 *     PositionChanged - 经纬度数据发生改变时所触发的事件（系统会根据 MovementThreshold 属性的值来决定何时触发 PositionChanged 事件，当位置服务被打开后第一次得到位置数据时也会触发此事件）
 *     StatusChanged - 位置服务的状态发生改变时所触发的事件
 * 
 * 
 * 
 * GeoCoordinate - 地理坐标
 *     Altitude - 海拔高度（单位：米）
 *     VerticalAccuracy - 海拔高度的精度（单位：米）
 *     Longitude - 经度
 *     Latitude - 纬度
 *     IsUnknown - 是否无经纬度数据。true代表无数据，false代表有数据
 *     HorizontalAccuracy - 经纬度的精度（单位：米）
 *     Course - 行进方向（单位：度，正北为 0 度）
 *     Speed - 行进速度（单位：米/秒）
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

using System.Device.Location;
using System.Threading;

namespace Demo.Device
{
    public partial class GpsDemo : PhoneApplicationPage
    {
        private GeoCoordinateWatcher _watcher;

        public GpsDemo()
        {
            InitializeComponent();

            _watcher = new GeoCoordinateWatcher();
        }

        private void btnLow_Click(object sender, RoutedEventArgs e)
        {
            // 开启低精度位置服务
            StartLocationService(GeoPositionAccuracy.Default);
        }

        private void btnHigh_Click(object sender, RoutedEventArgs e)
        {
            // 开启高精度位置服务
            StartLocationService(GeoPositionAccuracy.High);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            StopLocationService();
        }

        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            _watcher = new GeoCoordinateWatcher(accuracy);
            // 位置每移动 20 米触发一次 PositionChanged 事件
            _watcher.MovementThreshold = 20; 

            _watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(_watcher_StatusChanged);
            _watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(_watcher_PositionChanged);

            lblStatus.Text = "GPS 服务启动中...";

            new Thread((x) =>
            {
                // 启动 GPS 服务，会阻塞 UI 线程，所以要在后台线程处理
                if (!_watcher.TryStart(true, TimeSpan.FromSeconds(30)))
                {
                    Dispatcher.BeginInvoke(delegate
                    {
                        lblStatus.Text = "GPS 服务无法启动";
                    });
                }
            }).Start();

        }

        private void StopLocationService()
        {
            if (_watcher != null)
                _watcher.Stop();

            lblStatus.Text = "GPS 服务已关闭";
        }

        void _watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // 在 UI 上显示经纬度信息
            Dispatcher.BeginInvoke(delegate
            {
                lblMsg.Text = "经度: " + e.Position.Location.Longitude.ToString("0.000");
                lblMsg.Text += Environment.NewLine;
                lblMsg.Text += "纬度: " + e.Position.Location.Latitude.ToString("0.000");
            });
        }

        void _watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            // 在 UI 上显示 GPS 服务状态
            Dispatcher.BeginInvoke(delegate
            {
                switch (e.Status)
                {
                    case GeoPositionStatus.Disabled:
                        if (_watcher.Permission == GeoPositionPermission.Denied)
                            lblStatus.Text = "GPS 服务拒绝访问";
                        else
                            lblStatus.Text = "GPS 服务不可用";
                        break;
                    case GeoPositionStatus.Initializing:
                        lblStatus.Text = "GPS 服务初始化";
                        break;
                    case GeoPositionStatus.NoData:
                        lblStatus.Text = "GPS 无有效数据";
                        break;
                    case GeoPositionStatus.Ready:
                        lblStatus.Text = "GPS 接收数据中";
                        break;
                }
            });
        }
    }
}