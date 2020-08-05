namespace ComplexPlotter
{
    partial class ComplexPlotter
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
            this.components = new System.ComponentModel.Container();
            this.lstLogger = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.butSinX = new System.Windows.Forms.Button();
            this.picDomain = new System.Windows.Forms.PictureBox();
            this.picCoDomain = new System.Windows.Forms.PictureBox();
            this.txtCoords = new System.Windows.Forms.TextBox();
            this.butCircle = new System.Windows.Forms.Button();
            this.plotTimer = new System.Windows.Forms.Timer(this.components);
            this.butDomCodomSame = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picDomain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCoDomain)).BeginInit();
            this.SuspendLayout();
            // 
            // lstLogger
            // 
            this.lstLogger.FormattingEnabled = true;
            this.lstLogger.ItemHeight = 20;
            this.lstLogger.Location = new System.Drawing.Point(932, 38);
            this.lstLogger.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lstLogger.Name = "lstLogger";
            this.lstLogger.Size = new System.Drawing.Size(738, 164);
            this.lstLogger.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(98, 49);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 75);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // butSinX
            // 
            this.butSinX.Location = new System.Drawing.Point(262, 49);
            this.butSinX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.butSinX.Name = "butSinX";
            this.butSinX.Size = new System.Drawing.Size(115, 49);
            this.butSinX.TabIndex = 4;
            this.butSinX.Text = "SinX";
            this.butSinX.UseVisualStyleBackColor = true;
            this.butSinX.Click += new System.EventHandler(this.butSinX_Click);
            // 
            // picDomain
            // 
            this.picDomain.Location = new System.Drawing.Point(45, 351);
            this.picDomain.Name = "picDomain";
            this.picDomain.Size = new System.Drawing.Size(792, 890);
            this.picDomain.TabIndex = 5;
            this.picDomain.TabStop = false;
            // 
            // picCoDomain
            // 
            this.picCoDomain.Location = new System.Drawing.Point(891, 360);
            this.picCoDomain.Name = "picCoDomain";
            this.picCoDomain.Size = new System.Drawing.Size(844, 880);
            this.picCoDomain.TabIndex = 6;
            this.picCoDomain.TabStop = false;
            // 
            // txtCoords
            // 
            this.txtCoords.Location = new System.Drawing.Point(88, 215);
            this.txtCoords.Name = "txtCoords";
            this.txtCoords.Size = new System.Drawing.Size(356, 26);
            this.txtCoords.TabIndex = 7;
            // 
            // butCircle
            // 
            this.butCircle.Location = new System.Drawing.Point(505, 53);
            this.butCircle.Name = "butCircle";
            this.butCircle.Size = new System.Drawing.Size(152, 54);
            this.butCircle.TabIndex = 8;
            this.butCircle.Text = "Circle at origin";
            this.butCircle.UseVisualStyleBackColor = true;
            this.butCircle.Click += new System.EventHandler(this.butCircle_Click);
            // 
            // plotTimer
            // 
            this.plotTimer.Interval = 50;
            this.plotTimer.Tick += new System.EventHandler(this.plotTimer_Tick);
            // 
            // butDomCodomSame
            // 
            this.butDomCodomSame.Location = new System.Drawing.Point(520, 148);
            this.butDomCodomSame.Name = "butDomCodomSame";
            this.butDomCodomSame.Size = new System.Drawing.Size(136, 42);
            this.butDomCodomSame.TabIndex = 9;
            this.butDomCodomSame.Text = "Match Axis";
            this.butDomCodomSame.UseVisualStyleBackColor = true;
            this.butDomCodomSame.Click += new System.EventHandler(this.butDomCodomSame_Click);
            // 
            // ComplexPlotter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1792, 1367);
            this.Controls.Add(this.butDomCodomSame);
            this.Controls.Add(this.butCircle);
            this.Controls.Add(this.txtCoords);
            this.Controls.Add(this.picCoDomain);
            this.Controls.Add(this.picDomain);
            this.Controls.Add(this.butSinX);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lstLogger);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ComplexPlotter";
            this.Text = "Complex Plotter";
            this.Load += new System.EventHandler(this.ComplexPlotter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picDomain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCoDomain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox lstLogger;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button butSinX;
        private System.Windows.Forms.PictureBox picDomain;
        private System.Windows.Forms.PictureBox picCoDomain;
        private System.Windows.Forms.TextBox txtCoords;
        private System.Windows.Forms.Button butCircle;
        private System.Windows.Forms.Timer plotTimer;
        private System.Windows.Forms.Button butDomCodomSame;
    }
}

