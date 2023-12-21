using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace MyService
{
    public partial class Service1 : ServiceBase
    {
        private ffmpeg _ffmpeg;

        public Service1()
        {
            InitializeComponent();
            _ffmpeg = new ffmpeg(this);
        }

        protected override void OnStart( string[] args )
        {
           string rtsp = "rtsp://aivid:aivid_2022@192.168.111.105:554/cam/realmonitor?channel=1&subtype=0&unicast=true&proto=Onvif";
          //  string rtsp = " rtsp://192.168.222.253:8556/mgfmallgurugram";
       
            try
            {
                infoEvent("Service Started For " + rtsp);
                _ = StartMotionDetection(rtsp);

            }
            catch
            ( Exception ex )
            {
                infoEvent(ex.Message + ex.StackTrace);
            }
        }

        protected override void OnStop()
        {
            infoEvent("stoping Started");
            StopMotionDetection();

        }
        private DateTime lastMotionDetectionTime = System.DateTime.MinValue;
        private TimeSpan noMotionPrintInterval = TimeSpan.FromSeconds(10);
        private CancellationTokenSource cancellationTokenSource;
        private static Stopwatch stopwatch = new Stopwatch();

        List<Point> motionZone = new List<Point>
        {
            new Point(100, 200),
            new Point(150, 1000),
            new Point(1000, 1000),
            new Point(1000, 200)
        };
        public void infoEvent( string message , int eventId = 100 )
        {

            string eventSource = "MotionDetectionService";

            if (!EventLog.SourceExists(eventSource))
            {
                EventLog.CreateEventSource(eventSource, "Application");
            }

            EventLog.WriteEntry(eventSource, message, EventLogEntryType.Information, eventId);
        }
        public void warningEvent( string message, int eventId = 402 )
        {

            string eventSource = "MotionDetectionService";

            if (!EventLog.SourceExists(eventSource))
            {
                EventLog.CreateEventSource(eventSource, "Application");
            }

            EventLog.WriteEntry(eventSource, message, EventLogEntryType.Warning, eventId);
        }
        public void errorEvent( string message, string Strace )
        {

            string eventSource = "MotionDetectionService";

            string messageWithTrace = message+ Environment.NewLine+Strace;

            if (!EventLog.SourceExists(eventSource))
            {
                EventLog.CreateEventSource(eventSource, "Application");
            }

            EventLog.WriteEntry(eventSource, messageWithTrace, EventLogEntryType.Error);
        }


        #region MINIMAL FUNCTIONS

        public async void DetectMotionMinimal( CancellationToken whiltLoopCondition, string rtsp )
        {
            var outputFile = "rtsp://cloud.aividtechvision.com:8556/aivid50";
            await _ffmpeg.LoadProfiles(rtsp, outputFile);
            const int motionThreshold = 10000;
            bool motionDetected = false;
            try
            {
                using (var capture = new VideoCapture(rtsp))
                {
                    if (!capture.IsOpened)
                    {
                        infoEvent("Failed to open the RTSP stream.");
                        return;
                    }
                    infoEvent("Stream Fps : " +capture.Get(CapProp.Fps).ToString());
                    Mat previousFrame = null;
                    try
                    {
                        while (!whiltLoopCondition.IsCancellationRequested)
                        {
                            try
                            {
                                using (var currentFrame = new Mat())
                                using (var grayScale = new Mat())
                                using (var frameDiff = new Mat())
                                using (var threshold = new Mat())
                                using (var dilated = new Mat())
                                using (var kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1)))
                                {
                                    capture.Read(currentFrame);
                                    if (currentFrame.IsEmpty)
                                        break;
                                    CvInvoke.CvtColor(currentFrame, grayScale, ColorConversion.Bgr2Gray);
                                    CvInvoke.GaussianBlur(grayScale, grayScale, new Size(3, 3), 0);
                                    if (previousFrame != null)
                                    {
                                        CvInvoke.AbsDiff(previousFrame, grayScale, frameDiff);
                                        previousFrame?.Dispose();
                                        CvInvoke.Threshold(frameDiff, threshold, 50, 255, ThresholdType.Binary);
                                        frameDiff.Dispose();
                                        CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));
                                        threshold.Dispose();
                                        bool isMotion = false;
                                        using (var contourVector = new VectorOfVectorOfPoint())
                                        {
                                            Point point = default;
                                            CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                                            dilated.Dispose();
                                            for (int i = 0; i < contourVector.Size; i++)
                                            {
                                                VectorOfPoint c = contourVector[i];
                                                double area = CvInvoke.ContourArea(c);
                                                if (area >= motionThreshold)
                                                {
                                                    isMotion = true;
                                                    break;

                                                    /*
                                                    Rectangle boundingRect = CvInvoke.BoundingRectangle(c);
                                                    int center_x = boundingRect.X + boundingRect.Width / 2;
                                                    int center_y = boundingRect.Y + boundingRect.Height / 2;
                                                    Point[] pts = motionZone.ToArray();
                                                    point = new Point(center_x, center_y);

                                                    if (CvInvoke.PointPolygonTest(new VectorOfPoint(pts), point, false) >= 0)
                                                    {
                                                    }
                                                    */
                                                }
                                            }
                                        }
                                        HandleMotionDetection(isMotion, ref motionDetected);
                                    }
                                    previousFrame?.Dispose();
                                    previousFrame = grayScale.Clone();
                                    grayScale.Dispose();
                                }
                            }
                            catch (Exception e)
                            {
                                capture.Dispose();
                                previousFrame?.Dispose();
                                errorEvent(e.Message , e.StackTrace);
                                break;
                            }
                        }

                        capture.Dispose();
                    }
                    catch (Exception e)
                    {
                        errorEvent(e.Message , e.StackTrace);
                    }
                }
            }
            catch( Exception ex )
            {
                errorEvent(ex.Message, ex.StackTrace );
            }
         
        }

        #endregion


        private void HandleMotionDetection( bool isMotion, ref bool motionDetected )
        {
            if (isMotion)
            {

                if(motionDetected)
                {
                    lastMotionDetectionTime = DateTime.Now;
                 //   warningEvent("Continueing",1111);
                }
                else
                {
                    infoEvent($"Motion Detected ++++++++++++++++++++", 1);
                   // StartService("aivid50");
                    _ffmpeg.StartCapture();
                    motionDetected = true;
                    lastMotionDetectionTime = DateTime.Now;                                  
                }
           

            }
           // else if((DateTime.Now - lastMotionDetectionTime) > noMotionPrintInterval)
            else if(!isMotion && motionDetected && (DateTime.Now - lastMotionDetectionTime) > noMotionPrintInterval)
                    {
                {
                    infoEvent($"No Motion Detected! --------------",0);
                  //  StopService("aivid50");
                    _ffmpeg.StopCapture();
                    motionDetected = false;
                    lastMotionDetectionTime = DateTime.Now;
                }
            }
        }


        #region Start Stop Service Functios
        public  void StartService( string serviceName )
        {
            using (ServiceController serviceController = new ServiceController(serviceName))
            {
                try
                {
                    if (serviceController.Status != ServiceControllerStatus.Running)
                    {
                        stopwatch.Start();
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running);
                        stopwatch.Stop();
                        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                        infoEvent($"Service '{serviceName}' started successfully in {elapsedSeconds}.", 200);
                        stopwatch.Reset();
                    }
                    else
                    {
                        warningEvent($"Service '{serviceName}' is already running.");

                    }

                }
                catch (Exception ex)
                {
                    warningEvent($"Failed to start service '{serviceName}': {ex.Message}");
                }
            }
        }

        public void StopService( string serviceName )
        {
            using (ServiceController serviceController = new ServiceController(serviceName))

            {
                try
                {
                    if (serviceController.Status != ServiceControllerStatus.Stopped)
                    {
                        stopwatch.Start();
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                        stopwatch.Stop();
                        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                        infoEvent($"Service '{serviceName}' stopped successfully in {elapsedSeconds}.", 199);
                        stopwatch.Reset();
                    }
                    else
                    {
                        warningEvent($"Service '{serviceName}' is already stopped.");
                    }
                }
                catch (Exception ex)
                {
                    warningEvent($"Failed to stop service '{serviceName}': {ex.Message}");
                }
            }
        }

        #endregion
        #region Start STop Motion Detection
        private async Task StartMotionDetection( string rtsp )
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => DetectMotionMinimal(cancellationTokenSource.Token, rtsp));
        }

        private void StopMotionDetection()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;

        }
        #endregion
    }
}
