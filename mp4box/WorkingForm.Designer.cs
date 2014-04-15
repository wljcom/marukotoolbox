namespace mp4box
{
    partial class WorkingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkingForm));
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
            this.labelworkCount = new System.Windows.Forms.Label();
            this.progressBarX264 = new System.Windows.Forms.ProgressBar();
            this.labelProgress = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(12, 292);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 1;
            this.buttonAbort.Text = "&Abort";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(93, 292);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 2;
            this.buttonSave.Text = "&Save Log";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // richTextBoxOutput
            // 
            this.richTextBoxOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxOutput.Location = new System.Drawing.Point(12, 12);
            this.richTextBoxOutput.Name = "richTextBoxOutput";
            this.richTextBoxOutput.ReadOnly = true;
            this.richTextBoxOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBoxOutput.Size = new System.Drawing.Size(632, 274);
            this.richTextBoxOutput.TabIndex = 4;
            this.richTextBoxOutput.Text = "";
            this.richTextBoxOutput.VScroll += new System.EventHandler(this.richTextBoxOutput_VScroll);
            // 
            // labelworkCount
            // 
            this.labelworkCount.AutoSize = true;
            this.labelworkCount.Location = new System.Drawing.Point(174, 302);
            this.labelworkCount.Name = "labelworkCount";
            this.labelworkCount.Size = new System.Drawing.Size(99, 13);
            this.labelworkCount.TabIndex = 6;
            this.labelworkCount.Text = "MM/NN Completed";
            // 
            // progressBarX264
            // 
            this.progressBarX264.Location = new System.Drawing.Point(328, 303);
            this.progressBarX264.Maximum = 1000;
            this.progressBarX264.Name = "progressBarX264";
            this.progressBarX264.Size = new System.Drawing.Size(262, 13);
            this.progressBarX264.TabIndex = 5;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelProgress.Location = new System.Drawing.Point(596, 302);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(56, 14);
            this.labelProgress.TabIndex = 7;
            this.labelProgress.Text = "188.88%";
            // 
            // WorkingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 324);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.labelworkCount);
            this.Controls.Add(this.progressBarX264);
            this.Controls.Add(this.richTextBoxOutput);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonAbort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WorkingForm";
            this.Text = "<Title is modified by UpdateWorkCountUI()>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WorkingForm_FormClosing);
            this.Load += new System.EventHandler(this.WorkingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonAbort;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.RichTextBox richTextBoxOutput;
        private System.Windows.Forms.ProgressBar progressBarX264;
        private System.Windows.Forms.Label labelworkCount;
        private System.Windows.Forms.Label labelProgress;
    }
}