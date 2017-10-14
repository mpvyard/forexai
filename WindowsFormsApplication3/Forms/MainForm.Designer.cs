namespace WindowsFormsApplication3
{
   partial class Form1
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
         System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
         System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
         System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
         this.loadPricesButton = new System.Windows.Forms.Button();
         this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
         this.checkBox1 = new System.Windows.Forms.CheckBox();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabPage1 = new System.Windows.Forms.TabPage();
         this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
         this.button3 = new System.Windows.Forms.Button();
         this.button2 = new System.Windows.Forms.Button();
         this.button1 = new System.Windows.Forms.Button();
         this.loadTAButton = new System.Windows.Forms.Button();
         this.tabPage2 = new System.Windows.Forms.TabPage();
         this.tabPage3 = new System.Windows.Forms.TabPage();
         this.timeFast = new System.Windows.Forms.Timer(this.components);
         this.tabControl1.SuspendLayout();
         this.tabPage1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
         this.tabPage2.SuspendLayout();
         this.SuspendLayout();
         // 
         // loadPricesButton
         // 
         this.loadPricesButton.AllowDrop = true;
         this.loadPricesButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.loadPricesButton.BackColor = System.Drawing.SystemColors.Info;
         this.loadPricesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
         this.loadPricesButton.Cursor = System.Windows.Forms.Cursors.Arrow;
         this.loadPricesButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
         this.loadPricesButton.FlatAppearance.BorderSize = 2;
         this.loadPricesButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.loadPricesButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Cyan;
         this.loadPricesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.loadPricesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.loadPricesButton.Font = new System.Drawing.Font("Sitka Small", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.loadPricesButton.ForeColor = System.Drawing.SystemColors.Highlight;
         this.loadPricesButton.Location = new System.Drawing.Point(6, 7);
         this.loadPricesButton.Margin = new System.Windows.Forms.Padding(7);
         this.loadPricesButton.Name = "loadPricesButton";
         this.loadPricesButton.Size = new System.Drawing.Size(103, 32);
         this.loadPricesButton.TabIndex = 0;
         this.loadPricesButton.Text = "load prices";
         this.loadPricesButton.UseVisualStyleBackColor = false;
         this.loadPricesButton.Click += new System.EventHandler(this.LoadClick);
         // 
         // notifyIcon1
         // 
         this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
         this.notifyIcon1.BalloonTipText = "ef vf df dd fg";
         this.notifyIcon1.BalloonTipTitle = "d gfhth rhgh";
         this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
         this.notifyIcon1.Text = "notifyIcon1";
         this.notifyIcon1.Visible = true;
         this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayClick);
         this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIcon1MouseDoubleClick);
         // 
         // checkBox1
         // 
         this.checkBox1.AutoSize = true;
         this.checkBox1.BackColor = System.Drawing.Color.Azure;
         this.checkBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
         this.checkBox1.Cursor = System.Windows.Forms.Cursors.Hand;
         this.checkBox1.ForeColor = System.Drawing.Color.CadetBlue;
         this.checkBox1.Location = new System.Drawing.Point(8, 57);
         this.checkBox1.Margin = new System.Windows.Forms.Padding(0);
         this.checkBox1.Name = "checkBox1";
         this.checkBox1.Size = new System.Drawing.Size(80, 17);
         this.checkBox1.TabIndex = 1;
         this.checkBox1.Text = "checkBox1";
         this.checkBox1.UseVisualStyleBackColor = false;
         this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1CheckedChanged);
         // 
         // tabControl1
         // 
         this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
         this.tabControl1.Controls.Add(this.tabPage1);
         this.tabControl1.Controls.Add(this.tabPage2);
         this.tabControl1.Controls.Add(this.tabPage3);
         this.tabControl1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.tabControl1.Location = new System.Drawing.Point(0, 0);
         this.tabControl1.Margin = new System.Windows.Forms.Padding(9);
         this.tabControl1.Name = "tabControl1";
         this.tabControl1.Padding = new System.Drawing.Point(9, 3);
         this.tabControl1.SelectedIndex = 0;
         this.tabControl1.Size = new System.Drawing.Size(736, 513);
         this.tabControl1.TabIndex = 2;
         // 
         // tabPage1
         // 
         this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
         this.tabPage1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
         this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.tabPage1.Controls.Add(this.chart1);
         this.tabPage1.Controls.Add(this.button3);
         this.tabPage1.Controls.Add(this.button2);
         this.tabPage1.Controls.Add(this.button1);
         this.tabPage1.Controls.Add(this.loadTAButton);
         this.tabPage1.Controls.Add(this.loadPricesButton);
         this.tabPage1.Location = new System.Drawing.Point(4, 22);
         this.tabPage1.Name = "tabPage1";
         this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage1.Size = new System.Drawing.Size(728, 487);
         this.tabPage1.TabIndex = 0;
         this.tabPage1.Text = "Preload";
         // 
         // chart1
         // 
         this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
         this.chart1.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.BackwardDiagonal;
         this.chart1.BorderlineColor = System.Drawing.Color.Maroon;
         this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
         chartArea1.Name = "ChartArea1";
         this.chart1.ChartAreas.Add(chartArea1);
         this.chart1.IsSoftShadows = false;
         legend1.Name = "xxx";
         this.chart1.Legends.Add(legend1);
         this.chart1.Location = new System.Drawing.Point(115, 7);
         this.chart1.Name = "chart1";
         this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
         series1.ChartArea = "ChartArea1";
         series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
         series1.Legend = "xxx";
         series1.Name = "Series1";
         series1.YValuesPerPoint = 6;
         this.chart1.Series.Add(series1);
         this.chart1.Size = new System.Drawing.Size(601, 451);
         this.chart1.TabIndex = 5;
         this.chart1.Text = "chart1";
         this.chart1.TextAntiAliasingQuality = System.Windows.Forms.DataVisualization.Charting.TextAntiAliasingQuality.Normal;
         title1.Name = "Title1";
         this.chart1.Titles.Add(title1);
         this.chart1.Click += new System.EventHandler(this.Chart1Click);
         // 
         // button3
         // 
         this.button3.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
         this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.button3.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
         this.button3.FlatAppearance.BorderSize = 2;
         this.button3.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.button3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Cyan;
         this.button3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.button3.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.button3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
         this.button3.Location = new System.Drawing.Point(7, 155);
         this.button3.Name = "button3";
         this.button3.Size = new System.Drawing.Size(103, 29);
         this.button3.TabIndex = 4;
         this.button3.Text = "xxx";
         this.button3.UseVisualStyleBackColor = false;
         this.button3.Click += new System.EventHandler(this.ExecuteClick);
         // 
         // button2
         // 
         this.button2.BackColor = System.Drawing.SystemColors.ButtonFace;
         this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
         this.button2.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
         this.button2.FlatAppearance.BorderSize = 2;
         this.button2.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.button2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Cyan;
         this.button2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.button2.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.button2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
         this.button2.Location = new System.Drawing.Point(7, 120);
         this.button2.Name = "button2";
         this.button2.Size = new System.Drawing.Size(103, 29);
         this.button2.TabIndex = 3;
         this.button2.Text = "yyy";
         this.button2.UseVisualStyleBackColor = false;
         this.button2.Click += new System.EventHandler(this.CreateNetworkClick);
         this.button2.Paint += new System.Windows.Forms.PaintEventHandler(this.Button2Paint);
         // 
         // button1
         // 
         this.button1.BackColor = System.Drawing.SystemColors.MenuHighlight;
         this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
         this.button1.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
         this.button1.FlatAppearance.BorderSize = 2;
         this.button1.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Cyan;
         this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.button1.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.button1.ForeColor = System.Drawing.Color.LightGray;
         this.button1.Location = new System.Drawing.Point(7, 86);
         this.button1.Name = "button1";
         this.button1.Size = new System.Drawing.Size(103, 28);
         this.button1.TabIndex = 2;
         this.button1.Text = "execute";
         this.button1.UseVisualStyleBackColor = false;
         this.button1.Click += new System.EventHandler(this.GenerateTrainClick);
         // 
         // loadTAButton
         // 
         this.loadTAButton.AllowDrop = true;
         this.loadTAButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.loadTAButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
         this.loadTAButton.Cursor = System.Windows.Forms.Cursors.Hand;
         this.loadTAButton.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
         this.loadTAButton.FlatAppearance.BorderSize = 2;
         this.loadTAButton.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.loadTAButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Cyan;
         this.loadTAButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
         this.loadTAButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
         this.loadTAButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.loadTAButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
         this.loadTAButton.Location = new System.Drawing.Point(6, 44);
         this.loadTAButton.Margin = new System.Windows.Forms.Padding(7);
         this.loadTAButton.Name = "loadTAButton";
         this.loadTAButton.Size = new System.Drawing.Size(103, 32);
         this.loadTAButton.TabIndex = 1;
         this.loadTAButton.Text = "load talib";
         this.loadTAButton.UseVisualStyleBackColor = false;
         this.loadTAButton.Click += new System.EventHandler(this.LoadTaLib);
         // 
         // tabPage2
         // 
         this.tabPage2.BackColor = System.Drawing.SystemColors.ButtonShadow;
         this.tabPage2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
         this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
         this.tabPage2.Controls.Add(this.checkBox1);
         this.tabPage2.Location = new System.Drawing.Point(4, 22);
         this.tabPage2.Name = "tabPage2";
         this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage2.Size = new System.Drawing.Size(728, 487);
         this.tabPage2.TabIndex = 1;
         this.tabPage2.Text = "Train";
         // 
         // tabPage3
         // 
         this.tabPage3.Location = new System.Drawing.Point(4, 22);
         this.tabPage3.Name = "tabPage3";
         this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
         this.tabPage3.Size = new System.Drawing.Size(728, 487);
         this.tabPage3.TabIndex = 2;
         this.tabPage3.Text = "tabPage3";
         this.tabPage3.UseVisualStyleBackColor = true;
         // 
         // timeFast
         // 
         this.timeFast.Enabled = true;
         this.timeFast.Tick += new System.EventHandler(this.TimeFastTick);
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoSize = true;
         this.BackgroundImage = global::WindowsFormsApplication3.Properties.Resources.bg3;
         this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
         this.ClientSize = new System.Drawing.Size(733, 500);
         this.Controls.Add(this.tabControl1);
         this.DoubleBuffered = true;
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Name = "Form1";
         this.ShowIcon = false;
         this.Text = "Form1";
         this.Resize += new System.EventHandler(this.Form1Resize);
         this.tabControl1.ResumeLayout(false);
         this.tabPage1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
         this.tabPage2.ResumeLayout(false);
         this.tabPage2.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      public System.Windows.Forms.Button loadPricesButton;
      public System.Windows.Forms.NotifyIcon notifyIcon1;
      public System.Windows.Forms.CheckBox checkBox1;
      public System.Windows.Forms.TabControl tabControl1;
      public System.Windows.Forms.TabPage tabPage1;
      public System.Windows.Forms.TabPage tabPage2;
      public System.Windows.Forms.Button loadTAButton;
      public System.Windows.Forms.Timer timeFast;
      public System.Windows.Forms.Button button2;
      public System.Windows.Forms.Button button1;
      public System.Windows.Forms.Button button3;
      public System.Windows.Forms.DataVisualization.Charting.Chart chart1;
      private System.Windows.Forms.TabPage tabPage3;
   }
}

