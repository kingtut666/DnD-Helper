namespace DnDHelper
{
    partial class BatchSaveEncAs
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
            this.clEncounters = new System.Windows.Forms.CheckedListBox();
            this.butSave = new System.Windows.Forms.Button();
            this.butCheckAll = new System.Windows.Forms.Button();
            this.butUnCheckAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // clEncounters
            // 
            this.clEncounters.FormattingEnabled = true;
            this.clEncounters.Location = new System.Drawing.Point(12, 12);
            this.clEncounters.Name = "clEncounters";
            this.clEncounters.Size = new System.Drawing.Size(193, 484);
            this.clEncounters.TabIndex = 0;
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(256, 172);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(75, 23);
            this.butSave.TabIndex = 1;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCheckAll
            // 
            this.butCheckAll.Location = new System.Drawing.Point(256, 12);
            this.butCheckAll.Name = "butCheckAll";
            this.butCheckAll.Size = new System.Drawing.Size(94, 23);
            this.butCheckAll.TabIndex = 2;
            this.butCheckAll.Text = "Check All";
            this.butCheckAll.UseVisualStyleBackColor = true;
            this.butCheckAll.Click += new System.EventHandler(this.butCheckAll_Click);
            // 
            // butUnCheckAll
            // 
            this.butUnCheckAll.Location = new System.Drawing.Point(256, 42);
            this.butUnCheckAll.Name = "butUnCheckAll";
            this.butUnCheckAll.Size = new System.Drawing.Size(94, 23);
            this.butUnCheckAll.TabIndex = 3;
            this.butUnCheckAll.Text = "UnCheck All";
            this.butUnCheckAll.UseVisualStyleBackColor = true;
            this.butUnCheckAll.Click += new System.EventHandler(this.butUnCheckAll_Click);
            // 
            // BatchSaveEncAs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 518);
            this.Controls.Add(this.butUnCheckAll);
            this.Controls.Add(this.butCheckAll);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.clEncounters);
            this.Name = "BatchSaveEncAs";
            this.Text = "BatchSaveEncAs";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox clEncounters;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Button butCheckAll;
        private System.Windows.Forms.Button butUnCheckAll;
    }
}