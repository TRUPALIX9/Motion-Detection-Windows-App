using devicemgmt;
using media;
using Onvif.DeviceManagement;
using Onvif.Media;
using Onvif.PTZ;
using ptz;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using PTZSpeed = ptz.PTZSpeed;
using Vector1D = ptz.Vector1D;
using Vector2D = ptz.Vector2D;
using System;
using System.ServiceProcess;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using System.IO;

namespace Motion_Dection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load( object sender, EventArgs e )
        {
            // No default camera address or credentials in code: pre-fill from the environment only.
            textBox1.Text = Environment.GetEnvironmentVariable("ZONEWATCH_PTZ_HOST") ?? string.Empty;
            textBox2.Text = Environment.GetEnvironmentVariable("ZONEWATCH_PTZ_USERNAME") ?? string.Empty;
            textBox3.Text = Environment.GetEnvironmentVariable("ZONEWATCH_PTZ_PASSWORD") ?? string.Empty;
            textBox4.Text = "0.5";
        }

        // The moves wait for several seconds; disable the move buttons instead of freezing the window.
        private void SetMoveButtonsEnabled( bool enabled )
        {
            button1.Enabled = enabled;
            button3.Enabled = enabled;
            button4.Enabled = enabled;
            button6.Enabled = enabled;
        }

        private async void button1_Click( object sender, EventArgs e )
        {
            SetMoveButtonsEnabled(false);
            try
            {
                await continousMoveL(textBox1.Text, textBox2.Text, textBox3.Text, "80", 0);
            }
            finally
            {
                SetMoveButtonsEnabled(true);
            }
        }

        private async void button3_Click( object sender, EventArgs e )
        {
            SetMoveButtonsEnabled(false);
            try
            {
                await AbsoluteMoveL(textBox1.Text, textBox2.Text, textBox3.Text, "80", 0);
            }
            finally
            {
                SetMoveButtonsEnabled(true);
            }
        }

        private void button4_Click( object sender, EventArgs e )
        {
            PtzAgentMoveL(textBox1.Text, textBox2.Text, textBox3.Text, "80", 0);

        }

        // Accepts "host" or "host:port" in the IP address box; otherwise uses the given port.
        private static string BuildOnvifUrl( string ipAddress, string port, string path )
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException("ipAddress");
            }

            string[] array = ipAddress.Trim().Split(':');
            int portNumber = array.Length == 2 ? Convert.ToInt32(array[1]) : Convert.ToInt32(port);
            UriBuilder uriBuilder = new UriBuilder("http", array[0], portNumber, path);
            return uriBuilder.ToString();
        }

        public static string GetPtzOnvifUrl( string ipAddress, string port )
        {
            return BuildOnvifUrl(ipAddress, port, "/onvif/device_service");
        }

        public void printMe( string Message )
        {
            // Motion test and ONVIF callbacks can run off the UI thread.
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => printMe(Message)));
                return;
            }
            richTextBox1.Text += Environment.NewLine + System.DateTime.Now.ToString() + Environment.NewLine + Message + Environment.NewLine;
        }


        public static string GetOnvisfUrl( string ipAddress, string port )
        {
            return BuildOnvifUrl(ipAddress, port, "/onvif/media_service");
        }
        public CustomBinding GetBindings()
        {
            HttpTransportBindingElement httpTransportBindingElement = new HttpTransportBindingElement();
            httpTransportBindingElement.AuthenticationScheme = AuthenticationSchemes.Digest;
            httpTransportBindingElement.MaxReceivedMessageSize = 2147483647;
            httpTransportBindingElement.MaxBufferSize = 2147483647;
            TextMessageEncodingBindingElement textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            textMessageEncodingBindingElement.MessageVersion = MessageVersion.CreateVersion(EnvelopeVersion.Soap12, AddressingVersion.WSAddressing10);
            textMessageEncodingBindingElement.WriteEncoding = Encoding.UTF8;
            return new CustomBinding(textMessageEncodingBindingElement, httpTransportBindingElement);
        }


        public async Task continousMoveL( string cameraAddress, string userName, string password, string port, decimal profileIndex )
        {
            try
            {
                int index = (int)(profileIndex - 1);
                string OnvifUrl = GetOnvisfUrl(cameraAddress, port);
                string ptzurl = GetPtzOnvifUrl(cameraAddress, port);
                DeviceAgent deviceManagement = new DeviceAgent(cameraAddress, userName, password);
                MediaAgent mediaAgent = new MediaAgent(deviceManagement);
                CustomBinding customBinding = GetBindings();
                customBinding.Elements.Find<HttpTransportBindingElement>().MaxReceivedMessageSize = 2147483647;
                PTZClient ptzClient = new PTZClient(customBinding, new EndpointAddress(ptzurl));
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.Password = password;

                var profileToken = mediaAgent.GetProfiles()[(int)profileIndex].token;
                var profileToken1111 = mediaAgent.GetProfiles()[(int)profileIndex].PTZConfiguration.token;
                if (profileToken != null)
                {
                    var resp = ptzClient.GetStatus(profileToken);
                    printMe("origin Zoom : " + resp.Position.Zoom.x.ToString());
                    float targetZoom = float.Parse(textBox4.Text);
                    PTZSpeed velocity = new() { Zoom = new ptz.Vector1D { x = targetZoom } };
                    PTZSpeed velocity1 = new() { Zoom = new Vector1D { x = resp.Position.Zoom.x } };
                    ptzClient.ContinuousMove(profileToken, velocity, "PT10S");
                    await Task.Delay(10000);
                    ptzClient.Stop(profileToken, false, true);
                    ptzClient.ContinuousMove(profileToken, velocity1, "PT10S");
                    await Task.Delay(10000);
                    ptzClient.Stop(profileToken, false, true);


                }
            }
            catch (System.IndexOutOfRangeException)
            {
                MessageBox.Show("No Channel Found on the Given No.");
            }
            catch (System.ServiceModel.Security.MessageSecurityException)
            {
                MessageBox.Show("Please Enter Correct Username Or Password");
            }

            catch (System.ServiceModel.CommunicationException ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }
        }
        public async Task AbsoluteMoveL( string cameraAddress, string userName, string password, string port, decimal profileIndex )
        {
            try
            {
                int index = (int)(profileIndex - 1);
                string OnvifUrl = GetOnvisfUrl(cameraAddress, port);
                string ptzurl = GetPtzOnvifUrl(cameraAddress, port);
                DeviceAgent deviceManagement = new DeviceAgent(cameraAddress, userName, password);
                MediaAgent mediaAgent = new MediaAgent(deviceManagement);
                CustomBinding customBinding = GetBindings();
                customBinding.Elements.Find<HttpTransportBindingElement>().MaxReceivedMessageSize = 2147483647;
                PTZClient ptzClient = new(customBinding, new EndpointAddress(ptzurl));
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.Password = password;
                var profileToken = mediaAgent.GetProfiles()[(int)profileIndex].token;


                if (profileToken != null)
                {
                    var resp = ptzClient.GetStatus(profileToken);
                    printMe("origin Zoom : " + resp.Position.Zoom.x.ToString());
                    float targetZoom = float.Parse(textBox4.Text);
                    int Seconds = (int)numericUpDown1.Value;
                    PTZVector targetPosition = new PTZVector { Zoom = new Vector1D { x = targetZoom } };
                    PTZVector currentPosition = new PTZVector { Zoom = new Vector1D { x = resp.Position.Zoom.x } };
                    PTZSpeed targetVelocity = new() { Zoom = new Vector1D { x = targetZoom } };
                    PTZSpeed currentVelocity = new() { Zoom = new Vector1D { x = resp.Position.Zoom.x } };
                    printMe("Zooming for value " + resp.Position.Zoom.x.ToString());
                    ptzClient.AbsoluteMove(profileToken, targetPosition, targetVelocity);
                    await Task.Delay(Seconds * 1000);
                    printMe("Reseting to original Zoom: " + targetZoom.ToString());
                    ptzClient.AbsoluteMove(profileToken, currentPosition, currentVelocity);

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }
        public (string, string, string, string, bool) ParseRtspUrl( string rtspUrl )
        {
            Regex regex = new(@"rtsp://([^:]+):([^@]+)@([^:/]+)");
            Match match = regex.Match(rtspUrl);

            if (match.Success)
            {
                string userName = match.Groups[1].Value;
                string password = match.Groups[2].Value;
                string cameraAddress = match.Groups[3].Value;
                return (cameraAddress, userName, password, "80", true);
            }
            else
            {
                return (string.Empty, string.Empty, string.Empty, string.Empty, false);
            }
        }

        public async Task realtiveMove( string cameraAddress, string userName, string password, string port, decimal profileIndex )
        {
            try
            {
                int index = (int)(profileIndex - 1);
                string OnvifUrl = GetOnvisfUrl(cameraAddress, port);
                string ptzurl = GetPtzOnvifUrl(cameraAddress, port);
                DeviceAgent deviceManagement = new DeviceAgent(cameraAddress, userName, password);
                MediaAgent mediaAgent = new MediaAgent(deviceManagement);
                CustomBinding customBinding = GetBindings();
                customBinding.Elements.Find<HttpTransportBindingElement>().MaxReceivedMessageSize = 2147483647;
                PTZClient ptzClient = new PTZClient(customBinding, new EndpointAddress(ptzurl));
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.UserName = userName;
                ptzClient.ClientCredentials.HttpDigest.ClientCredential.Password = password;

                var profileToken = mediaAgent.GetProfiles()[(int)profileIndex].token;
                var profileToken1111 = mediaAgent.GetProfiles()[(int)profileIndex].PTZConfiguration.token;
                if (profileToken != null)
                {
                    var resp = ptzClient.GetStatus(profileToken);
                    printMe("origin Zoom : " + resp.Position.Zoom.x.ToString());
                    float targetZoom = float.Parse(textBox4.Text);
                    PTZSpeed velocity = new() { Zoom = new ptz.Vector1D { x = targetZoom } };
                    PTZSpeed velocity1 = new() { Zoom = new Vector1D { x = resp.Position.Zoom.x } };
                    PTZVector transaction = new PTZVector { Zoom = new ptz.Vector1D { x = 0.3F } };
                    PTZVector transaction1 = new PTZVector { Zoom = new ptz.Vector1D { x = -0.3F } };
                    ptzClient.RelativeMove(profileToken, transaction, velocity);
                    await Task.Delay(10000);
                    ptzClient.Stop(profileToken, false, true);
                    ptzClient.RelativeMove(profileToken, transaction1, velocity1);
                    await Task.Delay(10000);
                    ptzClient.Stop(profileToken, false, true);
                    var s = ptzClient.GetStatus(profileToken);
                    printMe("updated Zoom : " + s.Position.Zoom.x.ToString());

                }
            }
            catch (System.IndexOutOfRangeException)
            {
                MessageBox.Show("No Channel Found on the Given No.");
            }
            catch (System.ServiceModel.Security.MessageSecurityException)
            {
                MessageBox.Show("Please Enter Correct Username Or Password");
            }

            catch (System.ServiceModel.CommunicationException ex)
            {

                MessageBox.Show(ex.Message.ToString());
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());

            }
        }
        public void PtzAgentMoveL( string cameraAddress, string userName, string password, string port, decimal profileIndex )
        {
            try
            {
                DeviceAgent deviceManagement = new(cameraAddress, userName, password);
                PtzAgent ptzAgent = new(deviceManagement);
                ptzAgent.LoadProfiles(deviceManagement);
                ptzAgent.ZoomIn();
                ptzAgent.ZoomOut();
            }
            catch (System.IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message.ToString() + "No Channel Found on the Given No.");
            }
            catch (System.ServiceModel.Security.MessageSecurityException ex)
            {
                MessageBox.Show(ex.Message.ToString() + "Please Enter Correct Username Or Password");
            }

            catch (System.ServiceModel.CommunicationException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void button5_Click( object sender, EventArgs e )
        {
            richTextBox1.Text = string.Empty;
            (string cameraAddress, string userName, string password, string zoomPort, bool isValid) = ParseRtspUrl(textBox1.Text);
            if (isValid)
            {
                // The password is parsed but never written to the log.
                printMe("Host: " + cameraAddress);
                printMe("Username: " + userName);
                printMe("Port: " + zoomPort);

            }
            else
            {
                printMe("The IP address box does not contain an RTSP URL with a username and password.");


            }
        }
        /*
        public void DetectMotion()
        {
            string rtsp = Environment.GetEnvironmentVariable("ZONEWATCH_RTSP_URL");

            using (var capture = new VideoCapture(rtsp))
            {
                if (!capture.IsOpened)
                {
                    printMe("Failed to open the RTSP stream.");
                    return;
                }

                int fps = 25;
                int millisecondsPerFrame = (int)(1000.0 / fps);

                Mat previousFrame = null;
                Mat currentFrame = new Mat();
                Mat frameDiff = new Mat();
                Mat threshold = new Mat();

                int motionThreshold = 20000;
                bool isMotion = false;
                printMe("Stream setup Done");

                System.Threading.Timer timer = null;
                TimerCallback timerCallback = new TimerCallback(_ =>
                {
                    capture.Read(currentFrame);
                    if (currentFrame.IsEmpty)
                    {
                        timer.Dispose(); 
                        return;
                    }

                    CvInvoke.CvtColor(currentFrame, currentFrame, ColorConversion.Bgr2Gray);

                    if (previousFrame != null)
                    {
                        CvInvoke.AbsDiff(previousFrame, currentFrame, frameDiff);

                        CvInvoke.Threshold(frameDiff, threshold, 30, 255, ThresholdType.Binary);

                        Mat dilated = new Mat();
                        Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
                        CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(0));

                        List<List<Point>> contours = new List<List<Point>>();
                        using (VectorOfVectorOfPoint contourVector = new VectorOfVectorOfPoint())
                        {
                            CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                            for (int i = 0; i < contourVector.Size; i++)
                            {
                                using (VectorOfPoint contour = contourVector[i])
                                {
                                    contours.Add(new List<Point>(contour.ToArray()));
                                }
                            }
                        }
                        double totalArea = 0;
                        for (int i = 0; i < contours.Count; i++)
                        {
                            totalArea += CvInvoke.ContourArea(new VectorOfPoint(contours[i].ToArray()));
                        }
                        if (totalArea > motionThreshold)
                        {
                            if (!isMotion)
                            {
                                PrintOnUIThread("Motion Detected.........");
                                isMotion = true;
                                Thread.Sleep(5000);
                            }
                        }
                        else
                        {
                            if (isMotion)
                            {
                                PrintOnUIThread("No Motion Detected!!!!!!!!!!! Stop");
                                isMotion = false;
                            }
                        }
                    }

                    previousFrame = currentFrame.Clone();
                });

                // Start the timer
                timer = new System.Threading.Timer(timerCallback, null, 0, millisecondsPerFrame);

                // Wait for the timer to finish (you can add additional logic to handle this)
                Thread.Sleep(Timeout.Infinite);
            }
        }
        */
        private void ProcessMotion( Mat previousFrame, Mat currentFrame, Mat frameDiff, Mat threshold, int motionThreshold, out bool isMotion, out double totalArea )
        {
            CvInvoke.AbsDiff(previousFrame, currentFrame, frameDiff);
            CvInvoke.Threshold(frameDiff, threshold, 25, 255, ThresholdType.Binary);

            Mat dilated = new Mat();
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.Dilate(threshold, dilated, kernel, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(0));

            List<List<Point>> contours = FindContours(dilated);

            totalArea = CalculateTotalContourArea(contours);
            isMotion = totalArea > motionThreshold;
        }

        private List<List<Point>> FindContours( Mat dilated )
        {
            List<List<Point>> contours = new List<List<Point>>();

            using (VectorOfVectorOfPoint contourVector = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(dilated, contourVector, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contourVector.Size; i++)
                {
                    using (VectorOfPoint contour = contourVector[i])
                    {
                        contours.Add(new List<Point>(contour.ToArray()));
                    }
                }
            }

            return contours;
        }

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
        public async Task DetectMotion()
        {
            string? rtsp = Environment.GetEnvironmentVariable("ZONEWATCH_RTSP_URL");
            if (string.IsNullOrWhiteSpace(rtsp))
            {
                printMe("Set ZONEWATCH_RTSP_URL to the camera's RTSP URL to run the motion detection test.");
                return;
            }
            int motionThreshold = 10000;
            bool motionDetected = false;

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

                while (true)
                {
                    Mat currentFrame = new Mat();
                    capture.Read(currentFrame);

                    if (currentFrame.IsEmpty)
                        break;

                    UpdateUI(currentFrame);

                    CvInvoke.CvtColor(currentFrame, currentFrame, ColorConversion.Bgr2Gray);

                    if (previousFrame != null)
                    {
                        ProcessMotion(previousFrame, currentFrame, frameDiff, threshold, motionThreshold, out bool isMotion, out double totalArea);
                        HandleMotionDetection(isMotion, ref motionDetected, totalArea);
                    }

                    previousFrame?.Dispose();
                    previousFrame = currentFrame.Clone();
                    currentFrame?.Dispose();
                }
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


        private void HandleMotionDetection( bool isMotion, ref bool motionDetected, double totalMotionArea )
        {
            if (isMotion)
            {
                if (!motionDetected)
                {
                    printMe($"Motion Detected... Total Area: {totalMotionArea}");
                    motionDetected = true;
                }
            }
            else
            {
                if (motionDetected)
                {
                    printMe("No Motion Detected! Stopping...");
                    motionDetected = false;
                }
            }
        }


        private void button2_Click( object sender, EventArgs e )
        {
            printMe("button Clicked");
            _ = StartMotionDetection();

        }
        public async Task StartMotionDetection()
        {
            await Task.Run(() => DetectMotion());
        }

        private async void button6_Click( object sender, EventArgs e )
        {
            SetMoveButtonsEnabled(false);
            try
            {
                await realtiveMove(textBox1.Text, textBox2.Text, textBox3.Text, "80", 0);
            }
            finally
            {
                SetMoveButtonsEnabled(true);
            }
        }
    }
}