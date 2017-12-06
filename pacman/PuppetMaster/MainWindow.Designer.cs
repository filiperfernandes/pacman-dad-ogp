namespace pacman
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
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
            this.btn_start_server = new System.Windows.Forms.Button();
            this.btn_start_client = new System.Windows.Forms.Button();
            this.btn_freeze = new System.Windows.Forms.Button();
            this.btn_crash = new System.Windows.Forms.Button();
            this.btn_globalstate = new System.Windows.Forms.Button();
            this.btn_delay = new System.Windows.Forms.Button();
            this.btn_unfreeze = new System.Windows.Forms.Button();
            this.btn_wait = new System.Windows.Forms.Button();
            this.btn_localstate = new System.Windows.Forms.Button();
            this.btn_run_command = new System.Windows.Forms.Button();
            this.btn_load = new System.Windows.Forms.Button();
            this.input_box = new System.Windows.Forms.TextBox();
            this.file_box = new System.Windows.Forms.TextBox();
            this.output_box = new System.Windows.Forms.TextBox();
            this.btn_next_command = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_start_server
            // 
            this.btn_start_server.Location = new System.Drawing.Point(12, 12);
            this.btn_start_server.Name = "btn_start_server";
            this.btn_start_server.Size = new System.Drawing.Size(75, 23);
            this.btn_start_server.TabIndex = 0;
            this.btn_start_server.Text = "Start Server";
            this.btn_start_server.UseVisualStyleBackColor = true;
            this.btn_start_server.Click += new System.EventHandler(this.btn_start_server_Click);
            // 
            // btn_start_client
            // 
            this.btn_start_client.Location = new System.Drawing.Point(108, 12);
            this.btn_start_client.Name = "btn_start_client";
            this.btn_start_client.Size = new System.Drawing.Size(75, 23);
            this.btn_start_client.TabIndex = 1;
            this.btn_start_client.Text = "Start Client";
            this.btn_start_client.UseVisualStyleBackColor = true;
            this.btn_start_client.Click += new System.EventHandler(this.btn_start_client_Click);
            // 
            // btn_freeze
            // 
            this.btn_freeze.Location = new System.Drawing.Point(204, 12);
            this.btn_freeze.Name = "btn_freeze";
            this.btn_freeze.Size = new System.Drawing.Size(75, 23);
            this.btn_freeze.TabIndex = 2;
            this.btn_freeze.Text = "Freeze";
            this.btn_freeze.UseVisualStyleBackColor = true;
            this.btn_freeze.Click += new System.EventHandler(this.btn_freeze_Click);
            // 
            // btn_crash
            // 
            this.btn_crash.Location = new System.Drawing.Point(304, 51);
            this.btn_crash.Name = "btn_crash";
            this.btn_crash.Size = new System.Drawing.Size(75, 23);
            this.btn_crash.TabIndex = 3;
            this.btn_crash.Text = "Crash";
            this.btn_crash.UseVisualStyleBackColor = true;
            this.btn_crash.Click += new System.EventHandler(this.btn_crash_Click);
            // 
            // btn_globalstate
            // 
            this.btn_globalstate.Location = new System.Drawing.Point(153, 95);
            this.btn_globalstate.Name = "btn_globalstate";
            this.btn_globalstate.Size = new System.Drawing.Size(97, 23);
            this.btn_globalstate.TabIndex = 4;
            this.btn_globalstate.Text = "Global Status";
            this.btn_globalstate.UseVisualStyleBackColor = true;
            this.btn_globalstate.Click += new System.EventHandler(this.btn_globalstate_Click);
            // 
            // btn_delay
            // 
            this.btn_delay.Location = new System.Drawing.Point(12, 51);
            this.btn_delay.Name = "btn_delay";
            this.btn_delay.Size = new System.Drawing.Size(75, 23);
            this.btn_delay.TabIndex = 5;
            this.btn_delay.Text = "Delay";
            this.btn_delay.UseVisualStyleBackColor = true;
            this.btn_delay.Click += new System.EventHandler(this.btn_delay_Click);
            // 
            // btn_unfreeze
            // 
            this.btn_unfreeze.Location = new System.Drawing.Point(304, 12);
            this.btn_unfreeze.Name = "btn_unfreeze";
            this.btn_unfreeze.Size = new System.Drawing.Size(75, 23);
            this.btn_unfreeze.TabIndex = 6;
            this.btn_unfreeze.Text = "Unfreeze";
            this.btn_unfreeze.UseVisualStyleBackColor = true;
            this.btn_unfreeze.Click += new System.EventHandler(this.btn_unfreeze_Click);
            // 
            // btn_wait
            // 
            this.btn_wait.Location = new System.Drawing.Point(204, 51);
            this.btn_wait.Name = "btn_wait";
            this.btn_wait.Size = new System.Drawing.Size(75, 23);
            this.btn_wait.TabIndex = 7;
            this.btn_wait.Text = "Wait";
            this.btn_wait.UseVisualStyleBackColor = true;
            this.btn_wait.Click += new System.EventHandler(this.btn_wait_Click);
            // 
            // btn_localstate
            // 
            this.btn_localstate.Location = new System.Drawing.Point(108, 51);
            this.btn_localstate.Name = "btn_localstate";
            this.btn_localstate.Size = new System.Drawing.Size(75, 23);
            this.btn_localstate.TabIndex = 8;
            this.btn_localstate.Text = "Local State";
            this.btn_localstate.UseVisualStyleBackColor = true;
            this.btn_localstate.Click += new System.EventHandler(this.btn_localstate_Click);
            // 
            // btn_run_command
            // 
            this.btn_run_command.Location = new System.Drawing.Point(502, 12);
            this.btn_run_command.Name = "btn_run_command";
            this.btn_run_command.Size = new System.Drawing.Size(108, 23);
            this.btn_run_command.TabIndex = 9;
            this.btn_run_command.Text = "Run All Commands";
            this.btn_run_command.UseVisualStyleBackColor = true;
            this.btn_run_command.Click += new System.EventHandler(this.btn_run_command_Click);
            // 
            // btn_load
            // 
            this.btn_load.Location = new System.Drawing.Point(410, 12);
            this.btn_load.Name = "btn_load";
            this.btn_load.Size = new System.Drawing.Size(75, 23);
            this.btn_load.TabIndex = 10;
            this.btn_load.Text = "Load File";
            this.btn_load.UseVisualStyleBackColor = true;
            this.btn_load.Click += new System.EventHandler(this.btn_load_Click);
            // 
            // input_box
            // 
            this.input_box.Location = new System.Drawing.Point(0, 133);
            this.input_box.Name = "input_box";
            this.input_box.Size = new System.Drawing.Size(379, 20);
            this.input_box.TabIndex = 11;
            this.input_box.TextChanged += new System.EventHandler(this.input_box_TextChanged);
            // 
            // file_box
            // 
            this.file_box.Location = new System.Drawing.Point(410, 51);
            this.file_box.Multiline = true;
            this.file_box.Name = "file_box";
            this.file_box.Size = new System.Drawing.Size(321, 235);
            this.file_box.TabIndex = 12;
            this.file_box.TextChanged += new System.EventHandler(this.file_box_TextChanged);
            // 
            // output_box
            // 
            this.output_box.Location = new System.Drawing.Point(0, 175);
            this.output_box.Multiline = true;
            this.output_box.Name = "output_box";
            this.output_box.Size = new System.Drawing.Size(379, 121);
            this.output_box.TabIndex = 13;
            this.output_box.TextChanged += new System.EventHandler(this.output_box_TextChanged);
            // 
            // btn_next_command
            // 
            this.btn_next_command.Location = new System.Drawing.Point(616, 12);
            this.btn_next_command.Name = "btn_next_command";
            this.btn_next_command.Size = new System.Drawing.Size(115, 23);
            this.btn_next_command.TabIndex = 14;
            this.btn_next_command.Text = "Run Next Command";
            this.btn_next_command.UseVisualStyleBackColor = true;
            this.btn_next_command.Click += new System.EventHandler(this.btn_next_command_Click);
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(743, 298);
            this.Controls.Add(this.btn_next_command);
            this.Controls.Add(this.output_box);
            this.Controls.Add(this.file_box);
            this.Controls.Add(this.input_box);
            this.Controls.Add(this.btn_load);
            this.Controls.Add(this.btn_run_command);
            this.Controls.Add(this.btn_localstate);
            this.Controls.Add(this.btn_wait);
            this.Controls.Add(this.btn_unfreeze);
            this.Controls.Add(this.btn_delay);
            this.Controls.Add(this.btn_globalstate);
            this.Controls.Add(this.btn_crash);
            this.Controls.Add(this.btn_freeze);
            this.Controls.Add(this.btn_start_client);
            this.Controls.Add(this.btn_start_server);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load_1);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

     
        private System.Windows.Forms.Button btn_start_server;
        private System.Windows.Forms.Button btn_start_client;
        private System.Windows.Forms.Button btn_freeze;
        private System.Windows.Forms.Button btn_crash;
        private System.Windows.Forms.Button btn_globalstate;
        private System.Windows.Forms.Button btn_delay;
        private System.Windows.Forms.Button btn_unfreeze;
        private System.Windows.Forms.Button btn_wait;
        private System.Windows.Forms.Button btn_localstate;
        private System.Windows.Forms.Button btn_run_command;
        private System.Windows.Forms.Button btn_load;
        private System.Windows.Forms.TextBox input_box;
        private System.Windows.Forms.TextBox file_box;
        private System.Windows.Forms.TextBox output_box;
        private System.Windows.Forms.Button btn_next_command;
    }
}

