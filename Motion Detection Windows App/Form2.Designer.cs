namespace zoomtest
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            richTextBox1 = new RichTextBox();
            button2 = new Button();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            labelDateTime = new Label();
            button1 = new Button();
            button3 = new Button();
            comboBox1 = new ComboBox();
            fpsLabel = new Label();
            trackBar1 = new TrackBar();
            label1 = new Label();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(640, 360);
            pictureBox1.TabIndex = 28;
            pictureBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(704, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(333, 360);
            richTextBox1.TabIndex = 27;
            richTextBox1.Text = "";
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(1084, 101);
            button2.Name = "button2";
            button2.Size = new Size(229, 39);
            button2.TabIndex = 26;
            button2.Text = "Start Motion Detection";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(12, 443);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(640, 360);
            pictureBox2.TabIndex = 29;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(704, 443);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(640, 360);
            pictureBox3.TabIndex = 30;
            pictureBox3.TabStop = false;
            // 
            // labelDateTime
            // 
            labelDateTime.AutoSize = true;
            labelDateTime.Location = new Point(1176, 26);
            labelDateTime.Name = "labelDateTime";
            labelDateTime.Size = new Size(38, 15);
            labelDateTime.TabIndex = 31;
            labelDateTime.Text = "label1";
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(1084, 333);
            button1.Name = "button1";
            button1.Size = new Size(229, 39);
            button1.TabIndex = 32;
            button1.Text = "Clear Box";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button3.Location = new Point(1084, 164);
            button3.Name = "button3";
            button3.Size = new Size(229, 39);
            button3.TabIndex = 33;
            button3.Text = "Stop Motion Detection";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // comboBox1
            // 
            comboBox1.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "rtsp://192.168.222.253:8556/mgfmallgurugram", "rtsp://aivid:aivid_2022@192.168.222.50:554/cam/realmonitor?channel=1&subtype=0&unicast=true&proto=Onvif", "rtsp://aivid:aivid_2022@192.168.222.242/media/video1" });
            comboBox1.Location = new Point(1084, 54);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(229, 33);
            comboBox1.TabIndex = 34;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Location = new Point(1084, 239);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(26, 15);
            fpsLabel.TabIndex = 35;
            fpsLabel.Text = "FPS";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(12, 378);
            trackBar1.Maximum = 10000;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(1332, 45);
            trackBar1.TabIndex = 36;
            trackBar1.TickFrequency = 1000;
            trackBar1.Value = 1000;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(140, 408);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 37;
            label1.Text = "1000";
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(1152, 268);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(100, 50);
            pictureBox4.TabIndex = 38;
            pictureBox4.TabStop = false;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1360, 815);
            Controls.Add(pictureBox4);
            Controls.Add(label1);
            Controls.Add(trackBar1);
            Controls.Add(fpsLabel);
            Controls.Add(comboBox1);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(labelDateTime);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(richTextBox1);
            Controls.Add(button2);
            Name = "Form2";
            Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private Button button2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Label labelDateTime;
        private Button button1;
        private Button button3;
        private ComboBox comboBox1;
        private Label fpsLabel;
        private TrackBar trackBar1;
        private Label label1;
        private PictureBox pictureBox4;
    }
}