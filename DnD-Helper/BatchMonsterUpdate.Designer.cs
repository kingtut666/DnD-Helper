namespace DnDMonsters
{
    partial class BatchMonsterUpdate
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
            this.comboSetting = new System.Windows.Forms.ComboBox();
            this.comboValue = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.butUpdate = new System.Windows.Forms.Button();
            this.butExit = new System.Windows.Forms.Button();
            this.clMonsters = new System.Windows.Forms.CheckedListBox();
            this.butCheckAll = new System.Windows.Forms.Button();
            this.butUncheckAll = new System.Windows.Forms.Button();
            this.butCheckNone = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboSetting
            // 
            this.comboSetting.FormattingEnabled = true;
            this.comboSetting.Location = new System.Drawing.Point(260, 28);
            this.comboSetting.Name = "comboSetting";
            this.comboSetting.Size = new System.Drawing.Size(121, 21);
            this.comboSetting.TabIndex = 4;
            this.comboSetting.SelectedIndexChanged += new System.EventHandler(this.comboSetting_SelectedIndexChanged);
            // 
            // comboValue
            // 
            this.comboValue.FormattingEnabled = true;
            this.comboValue.Location = new System.Drawing.Point(260, 86);
            this.comboValue.Name = "comboValue";
            this.comboValue.Size = new System.Drawing.Size(121, 21);
            this.comboValue.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(257, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Setting:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Value:";
            // 
            // butUpdate
            // 
            this.butUpdate.Location = new System.Drawing.Point(260, 136);
            this.butUpdate.Name = "butUpdate";
            this.butUpdate.Size = new System.Drawing.Size(121, 23);
            this.butUpdate.TabIndex = 8;
            this.butUpdate.Text = "Update";
            this.butUpdate.UseVisualStyleBackColor = true;
            this.butUpdate.Click += new System.EventHandler(this.butUpdate_Click);
            // 
            // butExit
            // 
            this.butExit.Location = new System.Drawing.Point(260, 225);
            this.butExit.Name = "butExit";
            this.butExit.Size = new System.Drawing.Size(121, 23);
            this.butExit.TabIndex = 10;
            this.butExit.Text = "Exit";
            this.butExit.UseVisualStyleBackColor = true;
            this.butExit.Click += new System.EventHandler(this.butExit_Click);
            // 
            // clMonsters
            // 
            this.clMonsters.FormattingEnabled = true;
            this.clMonsters.Location = new System.Drawing.Point(12, 22);
            this.clMonsters.Name = "clMonsters";
            this.clMonsters.Size = new System.Drawing.Size(190, 484);
            this.clMonsters.TabIndex = 11;
            // 
            // butCheckAll
            // 
            this.butCheckAll.Location = new System.Drawing.Point(93, 514);
            this.butCheckAll.Name = "butCheckAll";
            this.butCheckAll.Size = new System.Drawing.Size(75, 23);
            this.butCheckAll.TabIndex = 12;
            this.butCheckAll.Text = "Check All";
            this.butCheckAll.UseVisualStyleBackColor = true;
            this.butCheckAll.Click += new System.EventHandler(this.butCheckAll_Click);
            // 
            // butUncheckAll
            // 
            this.butUncheckAll.Location = new System.Drawing.Point(12, 514);
            this.butUncheckAll.Name = "butUncheckAll";
            this.butUncheckAll.Size = new System.Drawing.Size(75, 23);
            this.butUncheckAll.TabIndex = 13;
            this.butUncheckAll.Text = "UnCheck All";
            this.butUncheckAll.UseVisualStyleBackColor = true;
            this.butUncheckAll.Click += new System.EventHandler(this.butUncheckAll_Click);
            // 
            // butCheckNone
            // 
            this.butCheckNone.Location = new System.Drawing.Point(12, 543);
            this.butCheckNone.Name = "butCheckNone";
            this.butCheckNone.Size = new System.Drawing.Size(156, 23);
            this.butCheckNone.TabIndex = 14;
            this.butCheckNone.Text = "Check No Environ";
            this.butCheckNone.UseVisualStyleBackColor = true;
            this.butCheckNone.Click += new System.EventHandler(this.butCheckNone_Click);
            // 
            // BatchMonsterUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 578);
            this.Controls.Add(this.butCheckNone);
            this.Controls.Add(this.butUncheckAll);
            this.Controls.Add(this.butCheckAll);
            this.Controls.Add(this.clMonsters);
            this.Controls.Add(this.butExit);
            this.Controls.Add(this.butUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboValue);
            this.Controls.Add(this.comboSetting);
            this.Name = "BatchMonsterUpdate";
            this.Text = "BatchMonsterUpdate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboSetting;
        private System.Windows.Forms.ComboBox comboValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button butUpdate;
        private System.Windows.Forms.Button butExit;
        private System.Windows.Forms.CheckedListBox clMonsters;
        private System.Windows.Forms.Button butCheckAll;
        private System.Windows.Forms.Button butUncheckAll;
        private System.Windows.Forms.Button butCheckNone;
    }
}