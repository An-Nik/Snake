
namespace nsSnake
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
				 this.components = new System.ComponentModel.Container();
				 this.timer = new System.Windows.Forms.Timer(this.components);
				 this.btnStart = new System.Windows.Forms.Button();
				 this.lblScoreLabel = new System.Windows.Forms.Label();
				 this.lblScore = new System.Windows.Forms.Label();
				 this.lblTreesLabel = new System.Windows.Forms.Label();
				 this.lblStonesLabel = new System.Windows.Forms.Label();
				 this.lblWaterLabel = new System.Windows.Forms.Label();
				 this.tbWater = new System.Windows.Forms.TextBox();
				 this.tbStones = new System.Windows.Forms.TextBox();
				 this.tbTrees = new System.Windows.Forms.TextBox();
				 this.lblSpeedLabel = new System.Windows.Forms.Label();
				 this.tbSpeed = new System.Windows.Forms.TextBox();
				 this.lblMapX = new System.Windows.Forms.Label();
				 this.lblMapY = new System.Windows.Forms.Label();
				 this.lblBrickSize = new System.Windows.Forms.Label();
				 this.tbMapHeight = new System.Windows.Forms.TextBox();
				 this.tbBrickSize = new System.Windows.Forms.TextBox();
				 this.tbMapWidth = new System.Windows.Forms.TextBox();
				 this.SuspendLayout();
				 // 
				 // btnStart
				 // 
				 this.btnStart.Location = new System.Drawing.Point(410, 38);
				 this.btnStart.Name = "btnStart";
				 this.btnStart.Size = new System.Drawing.Size(75, 23);
				 this.btnStart.TabIndex = 0;
				 this.btnStart.Text = "Start";
				 this.btnStart.UseVisualStyleBackColor = true;
				 this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
				 this.btnStart.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnStart_PreviewKeyDown);
				 // 
				 // lblScoreLabel
				 // 
				 this.lblScoreLabel.AutoSize = true;
				 this.lblScoreLabel.Location = new System.Drawing.Point(410, 8);
				 this.lblScoreLabel.Name = "lblScoreLabel";
				 this.lblScoreLabel.Size = new System.Drawing.Size(39, 15);
				 this.lblScoreLabel.TabIndex = 1;
				 this.lblScoreLabel.Text = "Score:";
				 // 
				 // lblScore
				 // 
				 this.lblScore.AutoSize = true;
				 this.lblScore.Location = new System.Drawing.Point(448, 8);
				 this.lblScore.Name = "lblScore";
				 this.lblScore.Size = new System.Drawing.Size(13, 15);
				 this.lblScore.TabIndex = 2;
				 this.lblScore.Text = "0";
				 // 
				 // lblTreesLabel
				 // 
				 this.lblTreesLabel.AutoSize = true;
				 this.lblTreesLabel.Location = new System.Drawing.Point(410, 102);
				 this.lblTreesLabel.Name = "lblTreesLabel";
				 this.lblTreesLabel.Size = new System.Drawing.Size(60, 15);
				 this.lblTreesLabel.TabIndex = 3;
				 this.lblTreesLabel.Text = "Trees  Cnt";
				 // 
				 // lblStonesLabel
				 // 
				 this.lblStonesLabel.AutoSize = true;
				 this.lblStonesLabel.Location = new System.Drawing.Point(410, 79);
				 this.lblStonesLabel.Name = "lblStonesLabel";
				 this.lblStonesLabel.Size = new System.Drawing.Size(61, 15);
				 this.lblStonesLabel.TabIndex = 3;
				 this.lblStonesLabel.Text = "StonesCnt";
				 // 
				 // lblWaterLabel
				 // 
				 this.lblWaterLabel.AutoSize = true;
				 this.lblWaterLabel.Location = new System.Drawing.Point(410, 123);
				 this.lblWaterLabel.Name = "lblWaterLabel";
				 this.lblWaterLabel.Size = new System.Drawing.Size(60, 15);
				 this.lblWaterLabel.TabIndex = 3;
				 this.lblWaterLabel.Text = "Water Cnt";
				 // 
				 // tbWater
				 // 
				 this.tbWater.Location = new System.Drawing.Point(471, 121);
				 this.tbWater.Name = "tbWater";
				 this.tbWater.Size = new System.Drawing.Size(23, 23);
				 this.tbWater.TabIndex = 4;
				 // 
				 // tbStones
				 // 
				 this.tbStones.Location = new System.Drawing.Point(471, 76);
				 this.tbStones.Name = "tbStones";
				 this.tbStones.Size = new System.Drawing.Size(23, 23);
				 this.tbStones.TabIndex = 4;
				 // 
				 // tbTrees
				 // 
				 this.tbTrees.Location = new System.Drawing.Point(471, 99);
				 this.tbTrees.Name = "tbTrees";
				 this.tbTrees.Size = new System.Drawing.Size(23, 23);
				 this.tbTrees.TabIndex = 4;
				 // 
				 // lblSpeedLabel
				 // 
				 this.lblSpeedLabel.AutoSize = true;
				 this.lblSpeedLabel.Location = new System.Drawing.Point(410, 147);
				 this.lblSpeedLabel.Name = "lblSpeedLabel";
				 this.lblSpeedLabel.Size = new System.Drawing.Size(60, 15);
				 this.lblSpeedLabel.TabIndex = 3;
				 this.lblSpeedLabel.Text = "TimerTick";
				 // 
				 // tbSpeed
				 // 
				 this.tbSpeed.Location = new System.Drawing.Point(471, 145);
				 this.tbSpeed.Name = "tbSpeed";
				 this.tbSpeed.Size = new System.Drawing.Size(23, 23);
				 this.tbSpeed.TabIndex = 4;
				 // 
				 // lblMapX
				 // 
				 this.lblMapX.AutoSize = true;
				 this.lblMapX.Location = new System.Drawing.Point(411, 182);
				 this.lblMapX.Name = "lblMapX";
				 this.lblMapX.Size = new System.Drawing.Size(63, 15);
				 this.lblMapX.TabIndex = 3;
				 this.lblMapX.Text = "MapWidth";
				 // 
				 // lblMapY
				 // 
				 this.lblMapY.AutoSize = true;
				 this.lblMapY.Location = new System.Drawing.Point(411, 203);
				 this.lblMapY.Name = "lblMapY";
				 this.lblMapY.Size = new System.Drawing.Size(67, 15);
				 this.lblMapY.TabIndex = 3;
				 this.lblMapY.Text = "MapHeight";
				 // 
				 // lblBrickSize
				 // 
				 this.lblBrickSize.AutoSize = true;
				 this.lblBrickSize.Location = new System.Drawing.Point(411, 227);
				 this.lblBrickSize.Name = "lblBrickSize";
				 this.lblBrickSize.Size = new System.Drawing.Size(53, 15);
				 this.lblBrickSize.TabIndex = 3;
				 this.lblBrickSize.Text = "BrickSize";
				 // 
				 // tbMapHeight
				 // 
				 this.tbMapHeight.Location = new System.Drawing.Point(472, 201);
				 this.tbMapHeight.Name = "tbMapHeight";
				 this.tbMapHeight.Size = new System.Drawing.Size(23, 23);
				 this.tbMapHeight.TabIndex = 4;
				 // 
				 // tbBrickSize
				 // 
				 this.tbBrickSize.Location = new System.Drawing.Point(472, 225);
				 this.tbBrickSize.Name = "tbBrickSize";
				 this.tbBrickSize.Size = new System.Drawing.Size(23, 23);
				 this.tbBrickSize.TabIndex = 4;
				 // 
				 // tbMapWidth
				 // 
				 this.tbMapWidth.Location = new System.Drawing.Point(472, 179);
				 this.tbMapWidth.Name = "tbMapWidth";
				 this.tbMapWidth.Size = new System.Drawing.Size(23, 23);
				 this.tbMapWidth.TabIndex = 4;
				 // 
				 // Form1
				 // 
				 this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
				 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
				 this.ClientSize = new System.Drawing.Size(498, 450);
				 this.Controls.Add(this.tbStones);
				 this.Controls.Add(this.tbMapWidth);
				 this.Controls.Add(this.tbTrees);
				 this.Controls.Add(this.tbBrickSize);
				 this.Controls.Add(this.tbSpeed);
				 this.Controls.Add(this.tbMapHeight);
				 this.Controls.Add(this.tbWater);
				 this.Controls.Add(this.lblStonesLabel);
				 this.Controls.Add(this.lblBrickSize);
				 this.Controls.Add(this.lblSpeedLabel);
				 this.Controls.Add(this.lblMapY);
				 this.Controls.Add(this.lblWaterLabel);
				 this.Controls.Add(this.lblMapX);
				 this.Controls.Add(this.lblTreesLabel);
				 this.Controls.Add(this.lblScore);
				 this.Controls.Add(this.lblScoreLabel);
				 this.Controls.Add(this.btnStart);
				 this.KeyPreview = true;
				 this.Name = "Form1";
				 this.Text = "Form1";
				 this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
				 this.ResumeLayout(false);
				 this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Button btnStart;
			private System.Windows.Forms.Label lblScoreLabel;
			private System.Windows.Forms.Label lblScore;
			private System.Windows.Forms.Label lblTreesLabel;
			private System.Windows.Forms.Label lblStonesLabel;
			private System.Windows.Forms.Label lblWaterLabel;
			private System.Windows.Forms.TextBox tbWater;
			private System.Windows.Forms.TextBox tbStones;
			private System.Windows.Forms.TextBox tbTrees;
			private System.Windows.Forms.Label lblSpeedLabel;
			private System.Windows.Forms.TextBox tbSpeed;
			private System.Windows.Forms.Label lblMapX;
			private System.Windows.Forms.Label lblMapY;
			private System.Windows.Forms.Label lblBrickSize;
			private System.Windows.Forms.TextBox tbMapHeight;
			private System.Windows.Forms.TextBox tbBrickSize;
			private System.Windows.Forms.TextBox tbMapWidth;
	 }
}

