using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel.Channels;
using System.Threading;
using devicemgmt;
using DateTime = System.DateTime;
using System.Xml.Schema;
using Rectangle = System.Drawing.Rectangle;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using System.Text.RegularExpressions;
using Emgu.CV.Features2D;
using System.Diagnostics;
using Color = System.Drawing.Color;

namespace zoomtest
{
    public partial class Form2 : Form
    {
        private List<PointF> roiPoints;

        public Form2()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        #region Global Variables
        private CancellationTokenSource cancellationTokenSource;
        private DateTime lastMotionDetectionTime = System.DateTime.MinValue;
        private TimeSpan noMotionPrintInterval = TimeSpan.FromSeconds(10);
        private static Stopwatch stopwatch = new Stopwatch();
        private static int frameCount = 0;
        private int consecutiveMotionFrames = 0;
        private int consecutiveNoMotionFrames = 0;
        #endregion


        #region Motion Detection


        private List<List<Point>> FindContours( Mat dilated, int minContourSize = 50 )
        {
            List<List<Point>> contours = new List<List<Point>>();

            using (VectorOfVectorOfPoint contourVector = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contourVector.Size; i++)
                {
                    using (VectorOfPoint contour = contourVector[i])
                    {
                        double area = CvInvoke.ContourArea(contour);
                        if (area >= minContourSize)
                        {
                            contours.Add(new List<Point>(contour.ToArray()));
                        }
                    }
                }
            }

            return contours;
        }

        public async Task DetectMotion( CancellationToken whiltLoopCondition, string rtsp )
        {
            int motionThreshold = 1000;
            bool motionDetected = false;
            try
            {
                using (var capture = new VideoCapture(rtsp))
                {
                    if (!capture.IsOpened)
                    {
                        printMe("Failed to open the RTSP stream.");
                        return;
                    }
                    Mat previousFrame = null;
                    Mat frameDiff = new Mat();
                    Mat threshold = new Mat();

                    stopwatch.Start();
                    while (!whiltLoopCondition.IsCancellationRequested)
                    {
                        try
                        {
                            Mat currentFrame = new Mat();
                            Mat grayScale = new Mat(); ;
                            bool confirm = capture.Read(currentFrame);
                            if (currentFrame.IsEmpty)
                                break;
                            UpdateUI(ResizeImage(currentFrame, 640, 360));
                            CvInvoke.CvtColor(currentFrame, grayScale, ColorConversion.Bgr2Gray);
                            CvInvoke.GaussianBlur(grayScale, grayScale, new Size(3, 3), 0);

                            if (previousFrame != null)
                            {
                                ProcessMotion(previousFrame, currentFrame, grayScale, threshold, motionThreshold, out bool isMotion, out double totalArea);
                                HandleMotionDetection(isMotion, ref motionDetected, totalArea);
                                frameCount++;
                                UpdateLabel(capture.Get(CapProp.Fps), totalArea);
                                UpdateTrackBar(totalArea > 10000 ? 10000 : totalArea);
                            }
                            previousFrame?.Dispose();
                            previousFrame = grayScale.Clone();

                        }
                        catch (Exception e)
                        {
                            capture.Dispose();
                            previousFrame?.Dispose();
                            frameDiff.Dispose();
                            printMe(e.Message + e.StackTrace); break;

                        }
                      
                    }
                    stopwatch.Stop();
                    CalculateFPS();
                    capture.Dispose();
                    previousFrame?.Dispose();
                }
            }
            catch (Exception e)
            {
                printMe(e.Message);
            }

        }
        private void ProcessMotion( Mat previousFrame, Mat currentFrame, Mat grayScale, Mat threshold, int motionThreshold, out bool isMotion, out double totalArea )
        {
            Mat frameDiff = new Mat();
            CvInvoke.AbsDiff(previousFrame, grayScale, frameDiff);
            CvInvoke.Threshold(frameDiff, threshold, 30, 255, ThresholdType.Binary);
            UpdateABSDiffFrameImage(ResizeImage(frameDiff, 640, 360));
            Mat dilated = new Mat();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));
            List<List<Point>> contours = FindContours(dilated, 10000);
            List<Rectangle> boundingBoxes = new List<Rectangle>();
            foreach (var contour in contours)
            {
                Rectangle boundingBox = CvInvoke.BoundingRectangle(new VectorOfPoint(contour.ToArray()));
                boundingBoxes.Add(boundingBox);
            }

            Mat frameWithBoxes = currentFrame.Clone();
            foreach (var boundingBox in boundingBoxes)
            {
                CvInvoke.Rectangle(frameWithBoxes, boundingBox, new MCvScalar(0, 0, 255), 3);

            }
            UpdateProcessedImage(ResizeImage(frameWithBoxes, 640, 360));
            totalArea = CalculateTotalContourArea(contours);
            isMotion = totalArea > motionThreshold;
        }
      

   
  
        private void HandleMotionDetection( bool isMotion, ref bool motionDetected, double totalMotionArea= 0 )
        {
            if (isMotion)
            {
                consecutiveMotionFrames++;

                if (!motionDetected && consecutiveMotionFrames >= 5)
                {
                    printMe($"Motion Detected...Area: {totalMotionArea} ++++++++");
                    motionDetected = true;
                    pictureBox1.Invoke((MethodInvoker)delegate
                    {
                        pictureBox4.BackColor = System.Drawing.Color.Green;
                    });
                }
            }
            else
            {
                consecutiveNoMotionFrames++;

                if (motionDetected && (DateTime.Now - lastMotionDetectionTime) > noMotionPrintInterval && consecutiveNoMotionFrames >=5)
                {
                    printMe($"No Motion Detected! --------------");
                    motionDetected = false;
                    lastMotionDetectionTime = DateTime.Now;
                    pictureBox1.Invoke((MethodInvoker)delegate
                    {
                        pictureBox4.BackColor = System.Drawing.Color.Red;
                    });
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
              await Task.Run(() => DetectMotion(cancellationTokenSource.Token, rtsp));
           // await Task.Run(() => DetectMotionMinimal(cancellationTokenSource.Token, rtsp));
        }

        private void StopMotionDetection()
        {
            cancellationTokenSource?.Cancel();
            button2.Enabled = true;
            cancellationTokenSource = null;
            pictureBox1.Image?.Dispose();
            pictureBox2.Image?.Dispose();
            pictureBox3.Image?.Dispose();
        }
        #endregion

        #region Helpers
        private double CalculateTotalContourArea( List<List<Point>> contours )
        {
            double totalArea = 0;

            for (int i = 0; i < contours.Count; i++)
            {
                var contourArea = CvInvoke.ContourArea(new VectorOfPoint(contours[i].ToArray()));
                totalArea += contourArea;
            }
            return totalArea;
        }


        private Mat ResizeImage( Mat frame, int maxWidth, int maxHeight )
        {
            double ratioX = (double)maxWidth / frame.Width;
            double ratioY = (double)maxHeight / frame.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(frame.Width * ratio);
            int newHeight = (int)(frame.Height * ratio);

            Mat resizedFrame = new Mat();
            CvInvoke.Resize(frame, resizedFrame, new Size(newWidth, newHeight));

            return resizedFrame;
        }
        private void CalculateFPS()
        {
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            double fps = frameCount / elapsedSeconds;

            printMe($"Frames: {frameCount}, Elapsed Time: {elapsedSeconds} s, FPS: {fps}");

            // Optionally, reset counters for the next iteration
            frameCount = 0;
            stopwatch.Reset();
        }

        public void printMe( string Message )
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => printMe(Message)));
            }
            else
            {
                richTextBox1.Text += Environment.NewLine + System.DateTime.Now.ToString() + Environment.NewLine + Message + Environment.NewLine;
            }
        }
        private void StartDateTimeUpdater()
        {
            System.Windows.Forms.Timer dateTimeUpdateTimer = new System.Windows.Forms.Timer();
            dateTimeUpdateTimer.Interval = 1000;
            dateTimeUpdateTimer.Tick += ( s, e ) => UpdateCurrentDateTimeLabel();
            dateTimeUpdateTimer.Start();
        }


        #endregion

        #region UI Invoke Update

        private void UpdateCurrentDateTimeLabel()
        {
            if (labelDateTime.InvokeRequired)
            {
                labelDateTime.Invoke(new Action(() => UpdateCurrentDateTimeLabel()));
            }
            else
            {
                labelDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        private void UpdateUI( Mat frame )
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new Action(() => UpdateUI(frame)));
            }
            else
            {
                pictureBox1.Width = frame.Width;
                pictureBox1.Height = frame.Height;
                pictureBox1.Image = frame.ToBitmap() ?? new Bitmap(10, 10);
            }
        }
        private void UpdateABSDiffFrameImage( Mat frame )
        {
            if (pictureBox2.InvokeRequired)
            {
                pictureBox2.Invoke(new Action(() => UpdateABSDiffFrameImage(frame)));
            }
            else
            {
                pictureBox2.Width = frame.Width;
                pictureBox2.Height = frame.Height;
                pictureBox2.Image = frame.ToBitmap() ?? new Bitmap(10, 10);
            }
        }

        private void UpdateLabel( double fps, double totalArea )
        {
            if (fpsLabel.InvokeRequired)
            {

                fpsLabel.Invoke(new Action(() => UpdateLabel(fps, totalArea)));
            }
            else
            {
                fpsLabel.Text = $"FPS: {fps} Area : {totalArea}";
            }
        }
        private void UpdateTrackBar( double area )
        {
            if (trackBar1.InvokeRequired)
            {
                trackBar1.Invoke(new Action(() => UpdateTrackBar(area)));
            }
            else
            {
                // Assuming the range of the TrackBar is suitable for your area values
                int trackBarValue = (int)area;
                trackBar1.Value = Math.Min(trackBar1.Maximum, trackBarValue);
            }
        }
        private void UpdateProcessedImage( Mat frame )
        {
            if (pictureBox3.InvokeRequired)
            {
                pictureBox3.Invoke(new Action(() => UpdateProcessedImage(frame)));
            }
            else
            {
                pictureBox3.Width = frame.Width;
                pictureBox3.Height = frame.Height;
                pictureBox3.Image = frame.ToBitmap() ?? new Bitmap(10, 10);
            }
        }


        #endregion

        #region FormUi
        private void button1_Click( object sender, EventArgs e )
        {
            richTextBox1.Text = string.Empty;
        }

        private void button3_Click( object sender, EventArgs e )
        {
            StopMotionDetection();
        }
        private void button2_Click( object sender, EventArgs e )
        {
            // getconfiguration(textBox1.Text, textBox2.Text, textBox3.Text, "80", 0);
            printMe("Detection Started for" + Environment.NewLine + comboBox1.SelectedItem.ToString());
            _ = StartMotionDetection(comboBox1.SelectedItem.ToString());
            StartDateTimeUpdater();
            button2.Enabled = false;
            pictureBox4.BackColor = Color.White;
        }
        #endregion

        #region MINIMAL FUNCTIONS
        public void DetectMotionMinimal( CancellationToken whiltLoopCondition, string rtsp )
        {
            const int motionThreshold = 10000;
            bool motionDetected = false;

            using (var capture = new VideoCapture(rtsp))
            {
                if (!capture.IsOpened)
                {
                    printMe("Failed to open the RTSP stream.");
                    return;
                }
                Mat previousFrame = null;
                
                try
                {
                  capture.Set(CapProp.Fps, 10);
                    stopwatch.Start();
                    while (!whiltLoopCondition.IsCancellationRequested)
                    {

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
                                        frameCount++;
                                        CvInvoke.AbsDiff(previousFrame, grayScale, frameDiff);
                                        previousFrame?.Dispose();
                                        CvInvoke.Threshold(frameDiff, threshold, 25, 255, ThresholdType.Binary);
                                        UpdateUI(ResizeImage(frameDiff, 640, 360));
                                        frameDiff.Dispose();
                                        CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));
                                        threshold.Dispose();
                                        List<List<Point>> contours = new List<List<Point>>();
                                        bool isMotion = false;
                                        using (var contourVector = new VectorOfVectorOfPoint())
                                        {
                                            CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                                            dilated.Dispose();
                                            for (int i = 0; i < contourVector.Size; i++)
                                            {
                                                using (var contour = contourVector[i])
                                                {
                                                    double area = CvInvoke.ContourArea(contour);
                                                    if (area >= motionThreshold)
                                                    {
                                                        isMotion = true;
                                                        break;
                                                    }
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
                                printMe(e.Message + e.StackTrace);
                                break;
                            }
                        }
                     
                    }
                    stopwatch.Stop();
                    CalculateFPS();
                    capture.Dispose();
                }
                catch (Exception e)
                {
                    printMe(e.Message);
                    printMe(e.Message + e.StackTrace);
                }
            }
        }
        #endregion
    }
}
