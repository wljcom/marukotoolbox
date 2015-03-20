namespace mp4box
{
    partial class FeedbackForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeedbackForm));
            this.PostButton = new ControlExs.QQButton();
            this.UserNameTextBox = new ControlExs.QQTextBox();
            this.QQTextBox = new ControlExs.QQTextBox();
            this.EmailTextBox = new ControlExs.QQTextBox();
            this.TitleTextBox = new ControlExs.QQTextBox();
            this.MessageTextBox = new ControlExs.QQTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PostButton
            // 
            this.PostButton.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.PostButton.Location = new System.Drawing.Point(184, 431);
            this.PostButton.Name = "PostButton";
            this.PostButton.Size = new System.Drawing.Size(130, 36);
            this.PostButton.TabIndex = 0;
            this.PostButton.Text = "提交";
            this.PostButton.UseVisualStyleBackColor = true;
            this.PostButton.Click += new System.EventHandler(this.PostButton_Click);
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.AllowDrop = true;
            this.UserNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UserNameTextBox.EmptyTextTip = "反馈者的名字（必填）";
            this.UserNameTextBox.EmptyTextTipColor = System.Drawing.Color.DarkGray;
            this.UserNameTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.UserNameTextBox.Location = new System.Drawing.Point(48, 12);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.Size = new System.Drawing.Size(437, 23);
            this.UserNameTextBox.TabIndex = 1;
            // 
            // QQTextBox
            // 
            this.QQTextBox.AllowDrop = true;
            this.QQTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.QQTextBox.EmptyTextTip = "用于联系反馈人（选填）";
            this.QQTextBox.EmptyTextTipColor = System.Drawing.Color.DarkGray;
            this.QQTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.QQTextBox.Location = new System.Drawing.Point(48, 43);
            this.QQTextBox.Name = "QQTextBox";
            this.QQTextBox.Size = new System.Drawing.Size(437, 23);
            this.QQTextBox.TabIndex = 2;
            // 
            // EmailTextBox
            // 
            this.EmailTextBox.AllowDrop = true;
            this.EmailTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EmailTextBox.EmptyTextTip = "用于回复反馈的问题（选填）";
            this.EmailTextBox.EmptyTextTipColor = System.Drawing.Color.DarkGray;
            this.EmailTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.EmailTextBox.Location = new System.Drawing.Point(48, 74);
            this.EmailTextBox.Name = "EmailTextBox";
            this.EmailTextBox.Size = new System.Drawing.Size(437, 23);
            this.EmailTextBox.TabIndex = 3;
            // 
            // TitleTextBox
            // 
            this.TitleTextBox.AllowDrop = true;
            this.TitleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TitleTextBox.EmptyTextTip = "（必填）";
            this.TitleTextBox.EmptyTextTipColor = System.Drawing.Color.DarkGray;
            this.TitleTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.TitleTextBox.Location = new System.Drawing.Point(48, 105);
            this.TitleTextBox.Name = "TitleTextBox";
            this.TitleTextBox.Size = new System.Drawing.Size(437, 23);
            this.TitleTextBox.TabIndex = 4;
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.AllowDrop = true;
            this.MessageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MessageTextBox.EmptyTextTip = "（必填）";
            this.MessageTextBox.EmptyTextTipColor = System.Drawing.Color.DarkGray;
            this.MessageTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9F);
            this.MessageTextBox.Location = new System.Drawing.Point(48, 139);
            this.MessageTextBox.Multiline = true;
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(437, 283);
            this.MessageTextBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "昵称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "QQ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "邮箱";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "标题";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 140);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "内容";
            // 
            // FeedbackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 474);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MessageTextBox);
            this.Controls.Add(this.TitleTextBox);
            this.Controls.Add(this.EmailTextBox);
            this.Controls.Add(this.QQTextBox);
            this.Controls.Add(this.UserNameTextBox);
            this.Controls.Add(this.PostButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FeedbackForm";
            this.Text = "反馈";
            this.Load += new System.EventHandler(this.FeedbackForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ControlExs.QQButton PostButton;
        private ControlExs.QQTextBox UserNameTextBox;
        private ControlExs.QQTextBox QQTextBox;
        private ControlExs.QQTextBox EmailTextBox;
        private ControlExs.QQTextBox TitleTextBox;
        private ControlExs.QQTextBox MessageTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}