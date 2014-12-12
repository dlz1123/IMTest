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
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using System.Windows.Navigation;
using System.Windows.Media.Imaging;

namespace Demo.Device
{
    public partial class CameraDemo : PhoneApplicationPage
    {
        private PhotoCamera _camera;
        private int _currentResolutionIndex = 0;

        public CameraDemo()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary))
            {
                _camera = new Microsoft.Devices.PhotoCamera(CameraType.Primary);

                _camera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(_camera_Initialized);
                _camera.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(_camera_CaptureCompleted);
                _camera.CaptureImageAvailable += new EventHandler<ContentReadyEventArgs>(_camera_CaptureImageAvailable);
                _camera.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(_camera_CaptureThumbnailAvailable);

                CameraButtons.ShutterKeyHalfPressed += OnButtonHalfPress;
                CameraButtons.ShutterKeyPressed += OnButtonFullPress;
                CameraButtons.ShutterKeyReleased += OnButtonRelease;

                videoBrush.SetSource(_camera);
            }
            else
            {
                lblMsg.Text = "A Camera is not available on this device.";
                btnShutter.IsEnabled = false;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_camera != null)
            {
                _camera.Dispose();

                _camera.Initialized -= _camera_Initialized;
                _camera.CaptureCompleted -= _camera_CaptureCompleted;
                _camera.CaptureImageAvailable -= _camera_CaptureImageAvailable;
                _camera.CaptureThumbnailAvailable -= _camera_CaptureThumbnailAvailable;

                CameraButtons.ShutterKeyHalfPressed -= OnButtonHalfPress;
                CameraButtons.ShutterKeyPressed -= OnButtonFullPress;
                CameraButtons.ShutterKeyReleased -= OnButtonRelease;
            }
        }

        void _camera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    _camera.FlashMode = FlashMode.Off;
                    btnFlash.Content = "闪光灯：Off";
                    btnResolution.Content = "分辨率：" + _camera.AvailableResolutions.ElementAt<Size>(_currentResolutionIndex);

                    lblMsg.Text = "Camera initialized.";
                });
            }
        }

        void _camera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                lblMsg.Text = "_camera_CaptureCompleted.";
            });
        }

        void _camera_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.ImageStream);

                imgCapture.Source = bitmapImage;
            });
        }

        public void _camera_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(e.ImageStream);

                imgCaptureThumbnail.Source = bitmapImage;
            });
        }

        private void OnButtonHalfPress(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                lblMsg.Text = "Half Button Press: Auto Focus";
            });
        }

        private void OnButtonFullPress(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                lblMsg.Text = "OnButtonFullPress";
            });
        }

        private void OnButtonRelease(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(delegate()
            {
                lblMsg.Text = "OnButtonRelease";
            });
        }

        private void btnShutter_Click(object sender, RoutedEventArgs e)
        {
            if (_camera != null)
            {
                try
                {
                    _camera.CaptureImage();
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        lblMsg.Text = ex.Message;
                    });
                }
            }
        }

        private void btnFlash_Click(object sender, RoutedEventArgs e)
        {
            switch (_camera.FlashMode)
            {
                case FlashMode.Off:
                    if (_camera.IsFlashModeSupported(FlashMode.On))
                    {
                        _camera.FlashMode = FlashMode.On;
                        btnFlash.Content = "闪光灯：On";
                    }
                    break;
                case FlashMode.On:
                    if (_camera.IsFlashModeSupported(FlashMode.RedEyeReduction))
                    {
                        _camera.FlashMode = FlashMode.RedEyeReduction;
                        btnFlash.Content = "闪光灯：RedEyeReduction";
                    }
                    else if (_camera.IsFlashModeSupported(FlashMode.Auto))
                    {
                        _camera.FlashMode = FlashMode.Auto;
                        btnFlash.Content = "闪光灯：Auto";
                    }
                    else
                    {
                        _camera.FlashMode = FlashMode.Off;
                        btnFlash.Content = "闪光灯：Off";
                    }
                    break;
                case FlashMode.RedEyeReduction:
                    if (_camera.IsFlashModeSupported(FlashMode.Auto))
                    {
                        _camera.FlashMode = FlashMode.Auto;
                        btnFlash.Content = "闪光灯：Auto";
                    }
                    else
                    {
                        _camera.FlashMode = FlashMode.Off;
                        btnFlash.Content = "闪光灯：Off";
                    }
                    break;
                case FlashMode.Auto:
                    if (_camera.IsFlashModeSupported(FlashMode.Off))
                    {
                        _camera.FlashMode = FlashMode.Off;
                        btnFlash.Content = "闪光灯：Off";
                    }
                    break;
            }
        }

       
        private void btnResolution_Click(object sender, RoutedEventArgs e)
        {
            if (++_currentResolutionIndex >= _camera.AvailableResolutions.Count())
                _currentResolutionIndex = 0;

            _camera.Resolution = _camera.AvailableResolutions.ElementAt<Size>(_currentResolutionIndex);
            btnResolution.Content = "分辨率：" + _camera.AvailableResolutions.ElementAt<Size>(_currentResolutionIndex);
        }
    }
}



/*
（可选）如果要使应用程序要求正面相机，请额外将正面相机功能添加到应用程序清单文件中的 Capabilities 元素。

<Capability Name="ID_HW_FRONTCAMERA"/>
*/

/*
 * 事件

说明

ShutterKeyHalfPressed
当按下快门按钮并保持大约 800 毫秒时。短于该时间的半按压将不会触发该事件。
ShutterKeyPressed
当快门按钮收到一个完全按压时。
ShutterKeyReleased
当松开快门按钮时。
 * 
 * 
这些事件可以用于拍摄照片或视频。拍摄视频时，必须在按硬件快门按钮之前调用 CaptureSource.Start 方法。如果未启动捕获源，则不会引发这些事件。有关在 Windows Phone 应用程序中拍摄视频的更多信息，请参阅如何在 Windows Phone 的相机应用程序中录制视频。


*/