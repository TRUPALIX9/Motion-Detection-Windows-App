using Emgu.CV;

namespace Motion_Dection
{
    partial class MotionDetectionForm
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
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            trackBar1 = new TrackBar();
            label1 = new Label();
            panel1 = new Panel();
            pictureBox4 = new PictureBox();
            fpsLabel = new Label();
            comboBox1 = new ComboBox();
            button3 = new Button();
            button1 = new Button();
            labelDateTime = new Label();
            button2 = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            pictureBox5 = new PictureBox();
            tabPage4 = new TabPage();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(6, 6);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(640, 360);
            pictureBox1.TabIndex = 28;
            pictureBox1.TabStop = false;
            pictureBox1.MouseClick += DrawingForm_MouseClick;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
            // 
            // richTextBox1
            // 
            richTextBox1.Dock = DockStyle.Left;
            richTextBox1.Location = new Point(0, 0);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(271, 392);
            richTextBox1.TabIndex = 27;
            richTextBox1.Text = "";
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(2, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(640, 360);
            pictureBox2.TabIndex = 29;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(2, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(640, 360);
            pictureBox3.TabIndex = 30;
            pictureBox3.TabStop = false;
            // 
            // trackBar1
            // 
            trackBar1.Dock = DockStyle.Bottom;
            trackBar1.Location = new Point(0, 392);
            trackBar1.Maximum = 10000;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(1181, 45);
            trackBar1.TabIndex = 36;
            trackBar1.TickFrequency = 1000;
            trackBar1.Value = 1000;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(133, 421);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 37;
            label1.Text = "1000";
            // 
            // panel1
            // 
            panel1.Controls.Add(button4);
            panel1.Controls.Add(pictureBox4);
            panel1.Controls.Add(fpsLabel);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(labelDateTime);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(richTextBox1);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(656, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(525, 392);
            panel1.TabIndex = 39;
            // 
            // pictureBox4
            // 
            pictureBox4.Location = new Point(348, 307);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(113, 34);
            pictureBox4.TabIndex = 45;
            pictureBox4.TabStop = false;
            // 
            // fpsLabel
            // 
            fpsLabel.AutoSize = true;
            fpsLabel.Location = new Point(315, 212);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(26, 15);
            fpsLabel.TabIndex = 44;
            fpsLabel.Text = "FPS";
            // 
            // comboBox1
            // 
            comboBox1.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "rtsp://192.168.222.253:8556/mgfmallgurugram", "rtsp://aivid:aivid_2022@192.168.222.50:554/cam/realmonitor?channel=1&subtype=0&unicast=true&proto=Onvif", "rtsp://aivid:aivid_2022@192.168.222.242/media/video1", "rtsp://192.168.222.253:8556/aivid50" });
            comboBox1.Location = new Point(315, 43);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(182, 33);
            comboBox1.TabIndex = 43;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button3.Location = new Point(315, 153);
            button3.Name = "button3";
            button3.Size = new Size(182, 39);
            button3.TabIndex = 42;
            button3.Text = "Stop Motion Detection";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(315, 347);
            button1.Name = "button1";
            button1.Size = new Size(182, 39);
            button1.TabIndex = 41;
            button1.Text = "Clear Box";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // labelDateTime
            // 
            labelDateTime.AutoSize = true;
            labelDateTime.Location = new Point(383, 13);
            labelDateTime.Name = "labelDateTime";
            labelDateTime.Size = new Size(38, 15);
            labelDateTime.TabIndex = 40;
            labelDateTime.Text = "label1";
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(315, 90);
            button2.Name = "button2";
            button2.Size = new Size(182, 39);
            button2.TabIndex = 39;
            button2.Text = "Start Motion Detection";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = DockStyle.Left;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(653, 392);
            tabControl1.TabIndex = 40;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(pictureBox1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(645, 364);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Original Frame";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(pictureBox2);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(645, 364);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Absolute Difference";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(pictureBox5);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(645, 364);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Extra Frame View";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // pictureBox5
            // 
            pictureBox5.Location = new Point(2, 2);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(640, 360);
            pictureBox5.TabIndex = 31;
            pictureBox5.TabStop = false;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(pictureBox3);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(645, 364);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Output Frame";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(361, 248);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 46;
            button4.Text = "Undo";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // MotionDetectionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1181, 437);
            Controls.Add(tabControl1);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(trackBar1);
            Name = "MotionDetectionForm";
            Text = "Form2";
            Load += MotionDetectionForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            tabPage4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private PictureBox pictureBox1;
        private RichTextBox richTextBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private TrackBar trackBar1;
        private Label label1;
        private Panel panel1;
        private PictureBox pictureBox4;
        private Label fpsLabel;
        private ComboBox comboBox1;
        private Button button3;
        private Button button1;
        private Label labelDateTime;
        private Button button2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private PictureBox pictureBox5;
        private Button button4;
    }
}