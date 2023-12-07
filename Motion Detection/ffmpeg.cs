﻿using Motion_Dection;
using System;
using System.Diagnostics;
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
        private MotionDetectionForm _MotionDetectionForm;

        public ffmpeg( MotionDetectionForm MotionDetectionForm )
        {
            _MotionDetectionForm = MotionDetectionForm;
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
                _conversion = FFmpeg.Conversions.New()
                    .AddStream(mediaInfo.Streams)
                    .SetOutput(outputFile).AddParameter("-c copy").SetPreset(ConversionPreset.UltraFast).UseHardwareAcceleration(HardwareAccelerator.auto,VideoCodec.h264,VideoCodec.h264) 
                    .SetOutputFormat(Format.rtsp);
                stopwatch.Stop();
                _MotionDetectionForm.printMe($"Profile loaded successfully. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
                stopwatch.Reset();
            }
            catch (Exception ex)
            {
                _MotionDetectionForm.printMe(ex.Message.ToString());
            }
        }

        public void StartCapture()
        {
            try
            {
                stopwatch.Start();
                _cancellationTokenSource = new CancellationTokenSource();
                _conversion.Start(_cancellationTokenSource.Token);
                stopwatch.Stop();
                _MotionDetectionForm.printMe($"Stream capture started. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
                stopwatch.Reset();
            }
            catch (OperationCanceledException)
            {
                _MotionDetectionForm.printMe("Stream capture was canceled.");
            }
            catch (Exception ex)
            {
                _MotionDetectionForm.printMe($"Error starting stream capture: {ex.Message}");
            }
        }

        public void StopCapture()
        {
            try
            {
                stopwatch.Start();
                _cancellationTokenSource.Cancel();
                stopwatch.Stop();
                _MotionDetectionForm.printMe($"Stream capture stopped. Time taken: {stopwatch.Elapsed.TotalSeconds} seconds");
                stopwatch.Reset();
            }
            catch (Exception ex)
            {
                _MotionDetectionForm.printMe($"Error stopping stream capture: {ex.Message}");
            }
        }
    }
}