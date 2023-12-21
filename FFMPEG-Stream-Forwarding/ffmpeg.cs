using Microsoft.VisualBasic;
using System.Diagnostics;
using Xabe.FFmpeg;

namespace FFMPEG_Stream_Forwarding
{
    public class ffmpeg
    {
        private IConversion _conversion;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Stopwatch stopwatch = new Stopwatch();
        private Form1 _service1;
        private string input;
        private string output;
        public ffmpeg( Form1 service1 )
        {
            _service1 = service1;
        }

 
        public async Task LoadProfiles( string inputFile, string outputFile )
        {
            try
            {
                input = inputFile; output = outputFile;
                stopwatch.Start();
                var mediaInfo = await Xabe.FFmpeg.FFmpeg.GetMediaInfo(inputFile);
                _service1.warningEvent(inputFile+"   -- " + outputFile);
                _conversion = Xabe.FFmpeg.FFmpeg.Conversions.New()
                  .SetOutput(outputFile)
                  .SetOutputFormat(Format.rtsp)
                  .SetInputFormat(Format.rtsp)
                  .AddStream(mediaInfo.VideoStreams)
                  .AddParameter("-c copy -rtsp_transport tcp -timeout 3000000 -v error");
            
                stopwatch.Stop();
                _service1.warningEvent($"FFmpeg Command: {_conversion.Build()}");
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
                _cancellationTokenSource = new CancellationTokenSource();
                _service1.warningEvent($"Stream capture started. Time taken: {DateTime.Now}", 5700);
                await _conversion.Start(_cancellationTokenSource.Token);
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

        public async void StopCapture()
        {
            try
            {
                stopwatch.Start();
                _cancellationTokenSource.Cancel();
              //  _cancellationTokenSource.TryReset();
           //    await LoadProfiles(input, output);
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
