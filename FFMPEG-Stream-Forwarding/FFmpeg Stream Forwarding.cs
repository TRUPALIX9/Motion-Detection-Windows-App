
namespace FFMPEG_Stream_Forwarding
{
    public partial class Form1 : Form
    {
        private readonly ffmpeg _class1;
        public Form1()
        {
            InitializeComponent();
            _class1 = new ffmpeg(this);
            // No camera or relay URLs in code: pre-fill from the environment only.
            txt_input.Text = Environment.GetEnvironmentVariable("ZONEWATCH_RTSP_URL") ?? string.Empty;
            txt_output.Text = Environment.GetEnvironmentVariable("ZONEWATCH_RELAY_URL") ?? string.Empty;
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