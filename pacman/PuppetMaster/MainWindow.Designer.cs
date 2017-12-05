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
            this.SuspendLayout();
            // 
            // MainWindow
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load_1);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button exit;
        private System.Windows.Forms.TextBox logs;
        private System.Windows.Forms.Button start_op;
        private System.Windows.Forms.Button interval;
        private System.Windows.Forms.Button status;
        private System.Windows.Forms.Button crash;
        private System.Windows.Forms.Button freeze;
        private System.Windows.Forms.Button unfreeze;
        private System.Windows.Forms.Label Intro;
        private System.Windows.Forms.OpenFileDialog open_file;
        private System.Windows.Forms.Button load_config;
        private System.Windows.Forms.Button step_button;
        private System.Windows.Forms.Button all_button;
    }
}

