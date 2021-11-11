
namespace Snake {
  partial class Form1 {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.timer = new System.Windows.Forms.Timer(this.components);
      this.btnStart = new System.Windows.Forms.Button();
      this.lblScoreLabel = new System.Windows.Forms.Label();
      this.lblScore = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btnStart
      // 
      this.btnStart.Location = new System.Drawing.Point(410, 38);
      this.btnStart.Name = "btnStart";
      this.btnStart.Size = new System.Drawing.Size(60, 23);
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
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(498, 450);
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
  }
}

