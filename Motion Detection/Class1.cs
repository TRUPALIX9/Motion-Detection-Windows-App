using Motion_Dection;
using System.Diagnostics;
using Xabe.FFmpeg;

public class Class1
{
    private IConversion _conversion;
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private MotionDetectionForm _motionDetectionForm;
    private Stopwatch stopwatch = new Stopwatch();

    public Class1( MotionDetectionForm motionDetectionForm )
    {
        _motionDetectionForm = motionDetectionForm;
    }

    public async Task LoadProfiles( string inputFile, string outputFile )
    {
        try
        {
            stopwatch.Start();
            var mediaInfo = await FFmpeg.GetMediaInfo(inputFile);
            _conversion = FFmpeg.Conversions.New()
                .AddStream(mediaInfo.Streams)
                .SetOutput(outputFile).AddParameter("-c copy").SetPreset(ConversionPreset.UltraFast).UseHardwareAcceleration(HardwareAccelerator.auto, VideoCodec.h264, VideoCodec.h264)
                .SetOutputFormat(Format.rtsp);
            stopwatch.Stop();
            _motionDetectionForm.printMe($"Profile loaded successfully. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();
        }
        catch (Exception ex)
        {
            _motionDetectionForm.printMe(ex.Message.ToString());
        }
    }

    public  void StartCapture()
    {
        try
        {
            stopwatch.Start();
            _cancellationTokenSource = new CancellationTokenSource();
             _conversion.Start(_cancellationTokenSource.Token);
            stopwatch.Stop();
            _motionDetectionForm.printMe($"Stream capture started. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();
        }
        catch (OperationCanceledException)
        {
            _motionDetectionForm.printMe("Stream capture was canceled.");
        }
        catch (Exception ex)
        {
            _motionDetectionForm.printMe($"Error starting stream capture: {ex.Message}");
        }
    }

    public async Task StopCapture()
    {
        try
        {
            stopwatch.Start();
            _cancellationTokenSource.Cancel();
            stopwatch.Stop();
            _motionDetectionForm.printMe($"Stream capture stopped. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();
        }
        catch (Exception ex)
        {
            _motionDetectionForm.printMe($"Error stopping stream capture: {ex.Message}");
        }
    }
}
