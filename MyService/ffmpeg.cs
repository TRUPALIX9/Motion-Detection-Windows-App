using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MyService
{
    public class ffmpeg
    {
        private IConversion _conversion;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Stopwatch stopwatch = new Stopwatch();
        private Service1 _service1;

        public ffmpeg( Service1 service1 )
        {
            _service1 = service1;
            _ = InitializeFFmpeg();
        }

        public async Task InitializeFFmpeg()
        {
            await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Full);
        }
        public async Task LoadProfiles( string inputFile, string outputFile )
        {
            try
            {
                stopwatch.Start();
                var mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
                _service1.warningEvent(inputFile+"   -- " + outputFile);
                _conversion = FFmpeg.Conversions.New().SetOutputFormat(Format.rtsp).SetInputFormat(Format.rtsp).AddStream(mediaInfo.VideoStreams)
                 .AddParameter("-c copy")
                 .AddParameter(outputFile);

                stopwatch.Stop();
                _service1.warningEvent($"Profile loaded successfully. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds", 9090);
                stopwatch.Reset();
            }
            catch (Exception ex)
            {
                _service1.warningEvent("inputFile: "+ inputFile +Environment.NewLine + " OutputFile: " + outputFile + Environment.NewLine + "Error: "+ ex.Message.ToString() + Environment.NewLine + "StackTrace: " +ex.StackTrace.ToString(),664);
            }
        }

        public async void StartCapture()
        {
            try
            {
                stopwatch.Start();
                _cancellationTokenSource = new CancellationTokenSource();
             await _conversion.Start(_cancellationTokenSource.Token);
                stopwatch.Stop();
                _service1.warningEvent($"Stream capture started. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds", 5700);
                stopwatch.Reset();
            }
            catch (OperationCanceledException)
            {
                _service1.warningEvent("Stream capture was canceled.");
            }
            catch (Exception ex)
            {
                _service1.warningEvent($"Error starting stream capture: {ex.Message}");
            }
        }

        public void StopCapture()
        {
            try
            {
                stopwatch.Start();
                _cancellationTokenSource.Cancel();
                stopwatch.Stop();
                _service1.warningEvent($"Stream capture stopped. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds", 8787);
                stopwatch.Reset();
            }
            catch (Exception ex)
            {
                _service1.warningEvent($"Error stopping stream capture: {ex.Message}");
            }
        }
    }
}
