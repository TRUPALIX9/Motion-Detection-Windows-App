using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using DateTime = System.DateTime;
using Rectangle = System.Drawing.Rectangle;
using System.Diagnostics;
using Color = System.Drawing.Color;
using MyService;

namespace Motion_Dection
{
    public partial class MotionDetectionForm : Form
    {
        private List<PointF> roiPoints;
        private ffmpeg _class1;

        public MotionDetectionForm()
        {
            InitializeComponent();
            _class1 = new ffmpeg(this);
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
        List<Point> motionZone = new List<Point>
{
    new Point(100, 200),
    new Point(150, 1000),
    new Point(1000, 1000),
    new Point(1000, 200)
};

        #endregion


        #region Motion Detection

        private List<List<Point>> FindContours( Mat dilated, out bool motionInZone, out Point point, int minContourSize = 10000 )
        {
            List<List<Point>> contours = new List<List<Point>>();

            motionInZone = false;
            point = default;

            using (VectorOfVectorOfPoint contourVector = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contourVector.Size; i++)
                {
                    VectorOfPoint c = contourVector[i];

                    double area = CvInvoke.ContourArea(c);
                    if (area >= minContourSize)
                    {
                        contours.Add(new List<Point>(c.ToArray()));
                        Rectangle boundingRect = CvInvoke.BoundingRectangle(c);

                        int center_x = boundingRect.X + boundingRect.Width / 2;
                        int center_y = boundingRect.Y + boundingRect.Height / 2;

                        Point[] pts = motionZone.ToArray();
                        point = new Point(center_x, center_y);

                        if (CvInvoke.PointPolygonTest(new VectorOfPoint(pts), point, false) >= 0)
                        {
                            motionInZone = true;
                            break;
                        }
                    }
                }
            }

            return contours;
        }

        public async Task ShowRtsp( CancellationToken whileLoopCondition, string rtsp )
        {

            try
            {
                while (!whileLoopCondition.IsCancellationRequested)
                {

                    Mat currentFrame = new Mat();
                    using (var capture = new VideoCapture(rtsp))
                    {
                        if (!capture.IsOpened)
                        {
                               printMe("Failed to open the RTSP stream. Retrying in 1 second...");
                            continue;
                        }
                        stopwatch.Start();

                        bool confirm = capture.Read(currentFrame);
                        stopwatch.Stop();
                        if (confirm && !currentFrame.IsEmpty)
                        {
                            originalImage = currentFrame;
                            UpdateUI(ResizeImage(currentFrame, 640, 360));
                            CalculateFPS(); // Make sure this method is defined

                            break;
                        }



                    }
                }
            }
            catch (Exception e)
            {
                printMe($"Error: {e.Message}");
            }
            finally
            {

            }
        }

        public async Task DetectMotion( CancellationToken whiltLoopCondition, string rtsp )
        {
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
                    int FPS = (int)capture.Get(CapProp.Fps);
                    while (!whiltLoopCondition.IsCancellationRequested)
                    {
                        try
                        {
                            Mat currentFrame = new Mat();
                            Mat grayScale = new Mat(); ;
                            bool confirm = capture.Read(currentFrame);
                            if (currentFrame.IsEmpty)
                                break;
                            originalImage = currentFrame;
                            CvInvoke.CvtColor(currentFrame, grayScale, ColorConversion.Bgr2Gray);
                            UpdateUI(ResizeImage(currentFrame, 640, 360));
                            CvInvoke.GaussianBlur(grayScale, grayScale, new Size(9, 9), 0);
                            UpdateExtraViewFrameImage(ResizeImage(grayScale, 640, 360));

                            if (previousFrame != null)
                            {
                                ProcessMotion(previousFrame, currentFrame, grayScale, threshold, out bool isMotion, out double totalArea);
                                HandleMotionDetection(isMotion, ref motionDetected, totalArea);
                                frameCount++;
                                UpdateLabel(capture.Get(CapProp.Fps), totalArea);
                                UpdateTrackBar(totalArea);
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
        private void ProcessMotion( Mat previousFrame, Mat currentFrame, Mat grayScale, Mat threshold, out bool isMotion, out double totalArea )
        {
            Mat frameDiff = new Mat();
            CvInvoke.AbsDiff(previousFrame, grayScale, frameDiff);
            CvInvoke.Threshold(frameDiff, threshold, 30, 255, ThresholdType.Binary);
            UpdateABSDiffFrameImage(ResizeImage(frameDiff, 640, 360));
            Mat dilated = new Mat();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar(0));

            List<List<Point>> contours = FindContours(dilated, out bool motionInZone, out Point point, 10000);

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
            Point[] pts = motionZone.ToArray();
            CvInvoke.Polylines(frameWithBoxes, new VectorOfPoint(pts), true, new MCvScalar(255, 0, 0), 2);
            CvInvoke.Circle(frameWithBoxes, point, 5, new MCvScalar(0, 0, 255), -1);
            UpdateProcessedImage(ResizeImage(frameWithBoxes, 640, 360));
            totalArea = CalculateTotalContourArea(contours);
            isMotion = motionInZone;
        }




        private void HandleMotionDetection( bool isMotion, ref bool motionDetected, double totalMotionArea = 0 )
        {
            if (isMotion)
            {
                consecutiveMotionFrames++;

                if (!motionDetected && consecutiveMotionFrames >= 3)
                {
                    //printMe($"Motion Detected...Area: {totalMotionArea} ++++++++");
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

                if (motionDetected && (DateTime.Now - lastMotionDetectionTime) > noMotionPrintInterval && consecutiveNoMotionFrames >= 3)
                {
                    // printMe($"No Motion Detected! --------------");
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
            //await Task.Run(() => DetectMotion(cancellationTokenSource.Token, rtsp));
            // await Task.Run(() => DetectMotionMinimal(cancellationTokenSource.Token, rtsp));
            await Task.Run(() => ShowRtsp(cancellationTokenSource.Token, rtsp));

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
        private void CalculateFPS( string message = " " )
        {
            double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
            double fps = frameCount / elapsedSeconds;

            printMe($"Frames: {frameCount}, Elapsed Time: {elapsedSeconds} s, FPS: {fps}" + "   " + message);

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
        private void UpdateExtraViewFrameImage( Mat frame )
        {
            if (pictureBox5.InvokeRequired)
            {
                pictureBox5.Invoke(new Action(() => UpdateExtraViewFrameImage(frame)));
            }
            else
            {
                pictureBox5.Width = frame.Width;
                pictureBox5.Height = frame.Height;
                pictureBox5.Image = frame.ToBitmap() ?? new Bitmap(10, 10);
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
            //_class1.StopCapture();
      }
        private async void button2_Click( object sender, EventArgs e )
        {
            /*
            var outputFile = "rtsp://192.168.222.253:8556/aivid242";
           await _class1.LoadProfiles(comboBox1.SelectedItem.ToString(), outputFile);
            stopwatch.Start();
            _class1.StartCapture();
            stopwatch.Stop();
            CalculateFPS();
            */
            
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
                                    frameCount++;
                                    CvInvoke.AbsDiff(previousFrame, grayScale, frameDiff);

                                    previousFrame?.Dispose();
                                    CvInvoke.Threshold(frameDiff, threshold, 50, 255, ThresholdType.Binary);
                                    //   UpdateUI(ResizeImage(frameDiff, 640, 360));
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
                                                Rectangle boundingRect = CvInvoke.BoundingRectangle(c);
                                                int center_x = boundingRect.X + boundingRect.Width / 2;
                                                int center_y = boundingRect.Y + boundingRect.Height / 2;
                                                Point[] pts = motionZone.ToArray();
                                                point = new Point(center_x, center_y);

                                                if (CvInvoke.PointPolygonTest(new VectorOfPoint(pts), point, false) >= 0)
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

                        Thread.Sleep(90);
                    }

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

        private void MotionDetectionForm_Load( object sender, EventArgs e )
        {

        }

        #region ROi Logic
        private Mat originalImage;
        private Mat modifiedImage;
        private List<Point> roiList = new List<Point>();
        private Point lastPoint;

        private void DrawingForm_MouseClick( object sender, MouseEventArgs e )
        {
            if(pictureBox1.Image  != null)
            {
                // On the first click, capture the starting point
                double xScale = originalImage.Width / pictureBox1.Width;
                double yScale = originalImage.Height / pictureBox1.Height;
                lastPoint = new Point((int)(e.X * xScale), (int)(e.Y * yScale));

                roiList.Add(lastPoint);
                if (roiList.Count > 1)
                {
                    drawAndUpdateImage(false);
                }
            }
        }


        #endregion
        private void pictureBox1_MouseDoubleClick( object sender, MouseEventArgs e )
        {
            if (pictureBox1.Image != null)
            {
                drawAndUpdateImage(true);

            }
        }

        public void drawAndUpdateImage( bool isCompleted )
        {


            Point[] roi = roiList.ToArray();
            Mat frameWithBoxes = originalImage;
            CvInvoke.Polylines(frameWithBoxes, new VectorOfPoint(roi), isCompleted, isCompleted ? new MCvScalar(252, 252, 3) : new MCvScalar(3, 252, 240), 4);
            modifiedImage = frameWithBoxes;
            UpdateUI(ResizeImage(modifiedImage, 640, 360));
            if (isCompleted)
            {
                foreach (Point point in roiList)
                {
                    printMe($"Updated X: {point.X}, Updated Y: {point.Y}");
                }
                motionZone = roiList;
                roiList = new List<Point>();
            }


        }

        private void splitContainer1_Panel2_Paint( object sender, PaintEventArgs e )
        {

        }

        private void button4_Click( object sender, EventArgs e )
        {
            if (roiList.Count >1 )
            {
                roiList.Remove(roiList[roiList.Count - 1]);
                drawAndUpdateImage(false);
            }
     
        }
    }
}
