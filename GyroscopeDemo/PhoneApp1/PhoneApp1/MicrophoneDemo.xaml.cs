/*
 * 演示如何使用麦克风进行录音
 * 
 * Microphone - 用于捕获麦克风音频的类
 *     Default - 返回默认的 Microphone 实例
 *     All - 返回设备的全部 Microphone 实例集合
 *     SampleRate - 获取音频的采样率
 *     State - Microphone 的状态（Microsoft.Xna.Framework.Audio.MicrophoneState 枚举）
 *         Started - 正在捕获音频
 *         Stopped - 已经停止工作
 *     BufferDuration - 麦克风捕获音频时的缓冲时长
 *     
 *     BufferReady - 当麦克风捕获的音频时长达到 BufferDuration 设置的值后所触发的事件
 *     
 *     GetData(byte[] buffer) - 将麦克风最近捕获到的音频数据写入到指定的缓冲区
 *     GetSampleSizeInBytes(TimeSpan duration) - 麦克风捕获音频，根据音频时长返回音频字节数
 *     GetSampleDuration(int sizeInBytes) - 麦克风捕获音频，根据音频字节数返回音频时长
 *     Start() - 开始捕获
 *     Stop() - 停止捕获
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

using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Demo.Device
{
    public partial class MicrophoneDemo : PhoneApplicationPage
    {
        // silverlight 和 xna 混合编程时，所需要用到的计时器
        private GameTimer _timer;

        private Microphone _microphone = Microphone.Default;
        private SoundEffectInstance _soundInstance; // 用于播放音频数据
        private byte[] _buffer; // 每一片录音数据的缓冲区
        private MemoryStream _stream = new MemoryStream(); // 整个录音数据的内存流

        public MicrophoneDemo()
        {
            InitializeComponent();

            _timer = new GameTimer();
            // 指定计时器每 1/30 秒执行一次，即帧率为 30 fps
            _timer.UpdateInterval = TimeSpan.FromTicks(333333);
            // 每次帧更新时所触发的事件
            _timer.FrameAction += FrameworkDispatcherFrameAction;
            _timer.Start();

            _microphone.BufferReady += new EventHandler<EventArgs>(_microphone_BufferReady);
        }

        private void FrameworkDispatcherFrameAction(object sender, EventArgs e)
        {
            // 当使用 silverlight 和 xna 混合编程时，每次帧更新时都需要调用 FrameworkDispatcher.Update()
            FrameworkDispatcher.Update();
        }

        void _microphone_BufferReady(object sender, EventArgs e)
        {
            // 当录音的缓冲被填满后，将数据写入缓冲区
            _microphone.GetData(_buffer);
            // 将缓冲区中的数据写入内存流
            _stream.Write(_buffer, 0, _buffer.Length);
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (lblMsg.Text != "录音中")
            {
                // 设置录音的缓冲时长为 0.5 秒
                _microphone.BufferDuration = TimeSpan.FromMilliseconds(500);
                // 设置录音用的缓冲区的大小
                _buffer = new byte[_microphone.GetSampleSizeInBytes(_microphone.BufferDuration)];
                // 初始化内存流
                _stream.SetLength(0);

                _microphone.Start();

                lblMsg.Text = "录音中";
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_stream.Length > 0)
            {
                // 播放录音
                Thread soundThread = new Thread(new ThreadStart(playSound));
                soundThread.Start();

                lblMsg.Text = "播放录音";
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_microphone.State == MicrophoneState.Started)
            {
                // 停止录音
                _microphone.Stop();

                lblMsg.Text = "停止录音";
            }
            else if (_soundInstance.State == SoundState.Playing)
            {
                // 停止播放录音
                _soundInstance.Stop();

                lblMsg.Text = "停止播放录音";
            }
        }

        private void playSound()
        {
            // 播放内存流中的音频
            SoundEffect sound = new SoundEffect(_stream.ToArray(), _microphone.SampleRate, AudioChannels.Mono);
            _soundInstance = sound.CreateInstance();
            _soundInstance.Play();
        }
    }
}