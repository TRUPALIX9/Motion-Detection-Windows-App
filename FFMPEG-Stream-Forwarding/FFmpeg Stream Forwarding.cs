
namespace FFMPEG_Stream_Forwarding
{
    public partial class Form1 : Form
    {
        private readonly ffmpeg _class1;
        public Form1()
        {
            InitializeComponent();
            _class1 = new ffmpeg(this);
            txt_input.Text = "rtsp://aivid:aivid_2022@192.168.111.105:554/cam/realmonitor?channel=1&subtype=0&unicast=true&proto=Onvif";
            txt_output.Text = "rtsp://cloud.aividtechvision.com:8556/aivid50";
            string executablePath = "\"C:\\Users\\Aivid11\\source\\repos\\TRUPALIX9\\Motion-Detection-Windows-App\\FFMPEG-Stream-Forwarding\\bin\\Debug\\net6.0-windows\\FFMPEG-Stream-Forwarding.exe\"";
        }

        private void btn_start_Click( object sender, EventArgs e )
        {
            btn_start.Enabled = false;
            _class1.StartCapture();
        }

        private void btn_stop_Click( object sender, EventArgs e )
        {
            _class1.StopCapture();
            btn_start.Enabled = true;

        }
        private void btn_clearText_Click( object sender, EventArgs e )
        {
            richTextBox1.Text = string.Empty;
        }
        public void warningEvent( string Message, int num = 0 )
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action(() => warningEvent(Message, num)));
            }
            else
            {
                richTextBox1.Text += Environment.NewLine + System.DateTime.Now.ToString() + Environment.NewLine + Message + Environment.NewLine;
            }
        }

        private async void btn_LoadProfile_Click( object sender, EventArgs e )
        {
            await _class1.LoadProfiles(txt_input.Text, txt_output.Text);
        }
    }
}