namespace FFMPEG_Stream_Forwarding
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_start = new Button();
            btn_stop = new Button();
            richTextBox1 = new RichTextBox();
            txt_input = new TextBox();
            txt_output = new TextBox();
            label1 = new Label();
            btn_clearText = new Button();
            btn_LoadProfile = new Button();
            SuspendLayout();
            // 
            // btn_start
            // 
            btn_start.Location = new Point(174, 213);
            btn_start.Name = "btn_start";
            btn_start.Size = new Size(185, 32);
            btn_start.TabIndex = 0;
            btn_start.Text = "Start Forwarding RTSP";
            btn_start.UseVisualStyleBackColor = true;
            btn_start.Click += btn_start_Click;
            // 
            // btn_stop
            // 
            btn_stop.Location = new Point(553, 213);
            btn_stop.Name = "btn_stop";
            btn_stop.Size = new Size(170, 32);
            btn_stop.TabIndex = 1;
            btn_stop.Text = "Stop Forwarding RTSP";
            btn_stop.UseVisualStyleBackColor = true;
            btn_stop.Click += btn_stop_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 60);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(711, 147);
            richTextBox1.TabIndex = 2;
            richTextBox1.Text = "";
            // 
            // txt_input
            // 
            txt_input.Location = new Point(12, 12);
            txt_input.Name = "txt_input";
            txt_input.Size = new Size(328, 23);
            txt_input.TabIndex = 4;
            // 
            // txt_output
            // 
            txt_output.Location = new Point(396, 12);
            txt_output.Name = "txt_output";
            txt_output.Size = new Size(327, 23);
            txt_output.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(346, 14);
            label1.Name = "label1";
            label1.Size = new Size(39, 21);
            label1.TabIndex = 6;
            label1.Text = "--->";
            // 
            // btn_clearText
            // 
            btn_clearText.Location = new Point(382, 213);
            btn_clearText.Name = "btn_clearText";
            btn_clearText.Size = new Size(165, 32);
            btn_clearText.TabIndex = 3;
            btn_clearText.Text = "Clear  Logs";
            btn_clearText.UseVisualStyleBackColor = true;
            btn_clearText.Click += btn_clearText_Click;
            // 
            // btn_LoadProfile
            // 
            btn_LoadProfile.Location = new Point(12, 213);
            btn_LoadProfile.Name = "btn_LoadProfile";
            btn_LoadProfile.Size = new Size(156, 32);
            btn_LoadProfile.TabIndex = 7;
            btn_LoadProfile.Text = "Load Profiles";
            btn_LoadProfile.UseVisualStyleBackColor = true;
            btn_LoadProfile.Click += btn_LoadProfile_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(735, 257);
            Controls.Add(btn_LoadProfile);
            Controls.Add(label1);
            Controls.Add(txt_output);
            Controls.Add(txt_input);
            Controls.Add(btn_clearText);
            Controls.Add(richTextBox1);
            Controls.Add(btn_stop);
            Controls.Add(btn_start);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_start;
        private Button btn_stop;
        private RichTextBox richTextBox1;
        private TextBox txt_input;
        private TextBox txt_output;
        private Label label1;
        private Button btn_clearText;
        private Button btn_LoadProfile;
    }
}