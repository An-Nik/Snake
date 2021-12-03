
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
      this.form_timer = new System.Windows.Forms.Timer(this.components);
      this.lblScoreLabel = new System.Windows.Forms.Label();
      this.lblScore = new System.Windows.Forms.Label();
      this.grParams = new System.Windows.Forms.GroupBox();
      this.btnGenerateMap = new System.Windows.Forms.Button();
      this.grParams.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblScoreLabel
      // 
      this.lblScoreLabel.AutoSize = true;
      this.lblScoreLabel.Location = new System.Drawing.Point(6, 21);
      this.lblScoreLabel.Name = "lblScoreLabel";
      this.lblScoreLabel.Size = new System.Drawing.Size(39, 15);
      this.lblScoreLabel.TabIndex = 1;
      this.lblScoreLabel.Text = "Score:";
      // 
      // lblScore
      // 
      this.lblScore.AutoSize = true;
      this.lblScore.Location = new System.Drawing.Point(44, 21);
      this.lblScore.Name = "lblScore";
      this.lblScore.Size = new System.Drawing.Size(13, 15);
      this.lblScore.TabIndex = 2;
      this.lblScore.Text = "0";
      // 
      // grParams
      // 
      this.grParams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grParams.Controls.Add(this.btnGenerateMap);
      this.grParams.Controls.Add(this.lblScore);
      this.grParams.Controls.Add(this.lblScoreLabel);
      this.grParams.Location = new System.Drawing.Point(422, 4);
      this.grParams.Name = "grParams";
      this.grParams.Size = new System.Drawing.Size(110, 527);
      this.grParams.TabIndex = 3;
      this.grParams.TabStop = false;
      this.grParams.Text = "Параметры";
      // 
      // btnGenerateMap
      // 
      this.btnGenerateMap.Location = new System.Drawing.Point(6, 46);
      this.btnGenerateMap.Name = "btnGenerateMap";
      this.btnGenerateMap.Size = new System.Drawing.Size(98, 23);
      this.btnGenerateMap.TabIndex = 0;
      this.btnGenerateMap.Text = "Generate map";
      this.btnGenerateMap.UseVisualStyleBackColor = true;
      this.btnGenerateMap.Click += new System.EventHandler(this.btnStart_Click);
      this.btnGenerateMap.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btnStart_PreviewKeyDown);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(532, 531);
      this.Controls.Add(this.grParams);
      this.KeyPreview = true;
      this.Name = "Form1";
      this.Text = "Form1";
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
      this.grParams.ResumeLayout(false);
      this.grParams.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Timer form_timer;
    private System.Windows.Forms.Label lblScoreLabel;
    private System.Windows.Forms.Label lblScore;
    private System.Windows.Forms.GroupBox grParams;
    private System.Windows.Forms.Button btnGenerateMap;
  }
}

