namespace DnDHelper
{
    partial class EditSpell
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
            this.comboSpellList = new System.Windows.Forms.ComboBox();
            this.comboSchool = new System.Windows.Forms.ComboBox();
            this.numericLevel = new System.Windows.Forms.NumericUpDown();
            this.checkVerbal = new System.Windows.Forms.CheckBox();
            this.checkSomatic = new System.Windows.Forms.CheckBox();
            this.checkMaterial = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.richTextMaterial = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textDuration = new System.Windows.Forms.TextBox();
            this.textRange = new System.Windows.Forms.TextBox();
            this.textCastingTime = new System.Windows.Forms.TextBox();
            this.checkRitual = new System.Windows.Forms.CheckBox();
            this.checkBard = new System.Windows.Forms.CheckBox();
            this.checkCleric = new System.Windows.Forms.CheckBox();
            this.checkDruid = new System.Windows.Forms.CheckBox();
            this.checkPaladin = new System.Windows.Forms.CheckBox();
            this.checkRanger = new System.Windows.Forms.CheckBox();
            this.checkSorcerer = new System.Windows.Forms.CheckBox();
            this.checkWarlock = new System.Windows.Forms.CheckBox();
            this.checkWizard = new System.Windows.Forms.CheckBox();
            this.richDescr = new System.Windows.Forms.RichTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.butSave = new System.Windows.Forms.Button();
            this.butAddNew = new System.Windows.Forms.Button();
            this.butDelete = new System.Windows.Forms.Button();
            this.checkShowMissingOnly = new System.Windows.Forms.CheckBox();
            this.checkMissingMat = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericLevel)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboSpellList
            // 
            this.comboSpellList.FormattingEnabled = true;
            this.comboSpellList.Location = new System.Drawing.Point(13, 13);
            this.comboSpellList.Name = "comboSpellList";
            this.comboSpellList.Size = new System.Drawing.Size(251, 21);
            this.comboSpellList.TabIndex = 0;
            this.comboSpellList.SelectedValueChanged += new System.EventHandler(this.comboSpellList_SelectedValueChanged);
            // 
            // comboSchool
            // 
            this.comboSchool.FormattingEnabled = true;
            this.comboSchool.Location = new System.Drawing.Point(73, 66);
            this.comboSchool.Name = "comboSchool";
            this.comboSchool.Size = new System.Drawing.Size(191, 21);
            this.comboSchool.TabIndex = 1;
            // 
            // numericLevel
            // 
            this.numericLevel.Location = new System.Drawing.Point(73, 40);
            this.numericLevel.Name = "numericLevel";
            this.numericLevel.Size = new System.Drawing.Size(54, 20);
            this.numericLevel.TabIndex = 2;
            // 
            // checkVerbal
            // 
            this.checkVerbal.AutoSize = true;
            this.checkVerbal.Location = new System.Drawing.Point(6, 22);
            this.checkVerbal.Name = "checkVerbal";
            this.checkVerbal.Size = new System.Drawing.Size(56, 17);
            this.checkVerbal.TabIndex = 3;
            this.checkVerbal.Text = "Verbal";
            this.checkVerbal.UseVisualStyleBackColor = true;
            // 
            // checkSomatic
            // 
            this.checkSomatic.AutoSize = true;
            this.checkSomatic.Location = new System.Drawing.Point(68, 22);
            this.checkSomatic.Name = "checkSomatic";
            this.checkSomatic.Size = new System.Drawing.Size(64, 17);
            this.checkSomatic.TabIndex = 4;
            this.checkSomatic.Text = "Somatic";
            this.checkSomatic.UseVisualStyleBackColor = true;
            // 
            // checkMaterial
            // 
            this.checkMaterial.AutoSize = true;
            this.checkMaterial.Location = new System.Drawing.Point(138, 22);
            this.checkMaterial.Name = "checkMaterial";
            this.checkMaterial.Size = new System.Drawing.Size(63, 17);
            this.checkMaterial.TabIndex = 5;
            this.checkMaterial.Text = "Material";
            this.checkMaterial.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.richTextMaterial);
            this.groupBox1.Controls.Add(this.checkSomatic);
            this.groupBox1.Controls.Add(this.checkVerbal);
            this.groupBox1.Controls.Add(this.checkMaterial);
            this.groupBox1.Location = new System.Drawing.Point(282, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(278, 123);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Components";
            // 
            // richTextMaterial
            // 
            this.richTextMaterial.Location = new System.Drawing.Point(6, 45);
            this.richTextMaterial.Name = "richTextMaterial";
            this.richTextMaterial.Size = new System.Drawing.Size(258, 66);
            this.richTextMaterial.TabIndex = 6;
            this.richTextMaterial.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Level:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "School:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(285, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Duration:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(293, 174);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Range:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(264, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Casting Time:";
            // 
            // textDuration
            // 
            this.textDuration.Location = new System.Drawing.Point(341, 145);
            this.textDuration.Name = "textDuration";
            this.textDuration.Size = new System.Drawing.Size(205, 20);
            this.textDuration.TabIndex = 14;
            // 
            // textRange
            // 
            this.textRange.Location = new System.Drawing.Point(341, 171);
            this.textRange.Name = "textRange";
            this.textRange.Size = new System.Drawing.Size(205, 20);
            this.textRange.TabIndex = 15;
            // 
            // textCastingTime
            // 
            this.textCastingTime.Location = new System.Drawing.Point(341, 197);
            this.textCastingTime.Name = "textCastingTime";
            this.textCastingTime.Size = new System.Drawing.Size(205, 20);
            this.textCastingTime.TabIndex = 16;
            // 
            // checkRitual
            // 
            this.checkRitual.AutoSize = true;
            this.checkRitual.Location = new System.Drawing.Point(163, 41);
            this.checkRitual.Name = "checkRitual";
            this.checkRitual.Size = new System.Drawing.Size(53, 17);
            this.checkRitual.TabIndex = 17;
            this.checkRitual.Text = "Ritual";
            this.checkRitual.UseVisualStyleBackColor = true;
            // 
            // checkBard
            // 
            this.checkBard.AutoSize = true;
            this.checkBard.Location = new System.Drawing.Point(13, 107);
            this.checkBard.Name = "checkBard";
            this.checkBard.Size = new System.Drawing.Size(48, 17);
            this.checkBard.TabIndex = 18;
            this.checkBard.Text = "Bard";
            this.checkBard.UseVisualStyleBackColor = true;
            // 
            // checkCleric
            // 
            this.checkCleric.AutoSize = true;
            this.checkCleric.Location = new System.Drawing.Point(13, 130);
            this.checkCleric.Name = "checkCleric";
            this.checkCleric.Size = new System.Drawing.Size(52, 17);
            this.checkCleric.TabIndex = 19;
            this.checkCleric.Text = "Cleric";
            this.checkCleric.UseVisualStyleBackColor = true;
            // 
            // checkDruid
            // 
            this.checkDruid.AutoSize = true;
            this.checkDruid.Location = new System.Drawing.Point(13, 153);
            this.checkDruid.Name = "checkDruid";
            this.checkDruid.Size = new System.Drawing.Size(51, 17);
            this.checkDruid.TabIndex = 20;
            this.checkDruid.Text = "Druid";
            this.checkDruid.UseVisualStyleBackColor = true;
            // 
            // checkPaladin
            // 
            this.checkPaladin.AutoSize = true;
            this.checkPaladin.Location = new System.Drawing.Point(88, 107);
            this.checkPaladin.Name = "checkPaladin";
            this.checkPaladin.Size = new System.Drawing.Size(61, 17);
            this.checkPaladin.TabIndex = 21;
            this.checkPaladin.Text = "Paladin";
            this.checkPaladin.UseVisualStyleBackColor = true;
            // 
            // checkRanger
            // 
            this.checkRanger.AutoSize = true;
            this.checkRanger.Location = new System.Drawing.Point(88, 130);
            this.checkRanger.Name = "checkRanger";
            this.checkRanger.Size = new System.Drawing.Size(61, 17);
            this.checkRanger.TabIndex = 22;
            this.checkRanger.Text = "Ranger";
            this.checkRanger.UseVisualStyleBackColor = true;
            // 
            // checkSorcerer
            // 
            this.checkSorcerer.AutoSize = true;
            this.checkSorcerer.Location = new System.Drawing.Point(88, 153);
            this.checkSorcerer.Name = "checkSorcerer";
            this.checkSorcerer.Size = new System.Drawing.Size(66, 17);
            this.checkSorcerer.TabIndex = 23;
            this.checkSorcerer.Text = "Sorcerer";
            this.checkSorcerer.UseVisualStyleBackColor = true;
            // 
            // checkWarlock
            // 
            this.checkWarlock.AutoSize = true;
            this.checkWarlock.Location = new System.Drawing.Point(163, 107);
            this.checkWarlock.Name = "checkWarlock";
            this.checkWarlock.Size = new System.Drawing.Size(66, 17);
            this.checkWarlock.TabIndex = 24;
            this.checkWarlock.Text = "Warlock";
            this.checkWarlock.UseVisualStyleBackColor = true;
            // 
            // checkWizard
            // 
            this.checkWizard.AutoSize = true;
            this.checkWizard.Location = new System.Drawing.Point(163, 130);
            this.checkWizard.Name = "checkWizard";
            this.checkWizard.Size = new System.Drawing.Size(59, 17);
            this.checkWizard.TabIndex = 25;
            this.checkWizard.Text = "Wizard";
            this.checkWizard.UseVisualStyleBackColor = true;
            // 
            // richDescr
            // 
            this.richDescr.Location = new System.Drawing.Point(15, 223);
            this.richDescr.Name = "richDescr";
            this.richDescr.Size = new System.Drawing.Size(531, 267);
            this.richDescr.TabIndex = 26;
            this.richDescr.Text = "";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Description:";
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(12, 496);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(75, 23);
            this.butSave.TabIndex = 28;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butAddNew
            // 
            this.butAddNew.Location = new System.Drawing.Point(106, 496);
            this.butAddNew.Name = "butAddNew";
            this.butAddNew.Size = new System.Drawing.Size(75, 23);
            this.butAddNew.TabIndex = 29;
            this.butAddNew.Text = "Add New";
            this.butAddNew.UseVisualStyleBackColor = true;
            this.butAddNew.Click += new System.EventHandler(this.butAddNew_Click);
            // 
            // butDelete
            // 
            this.butDelete.Location = new System.Drawing.Point(203, 496);
            this.butDelete.Name = "butDelete";
            this.butDelete.Size = new System.Drawing.Size(75, 23);
            this.butDelete.TabIndex = 30;
            this.butDelete.Text = "Delete";
            this.butDelete.UseVisualStyleBackColor = true;
            this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
            // 
            // checkShowMissingOnly
            // 
            this.checkShowMissingOnly.AutoSize = true;
            this.checkShowMissingOnly.Location = new System.Drawing.Point(444, 500);
            this.checkShowMissingOnly.Name = "checkShowMissingOnly";
            this.checkShowMissingOnly.Size = new System.Drawing.Size(116, 17);
            this.checkShowMissingOnly.TabIndex = 31;
            this.checkShowMissingOnly.Text = "Missing Descr Only";
            this.checkShowMissingOnly.UseVisualStyleBackColor = true;
            this.checkShowMissingOnly.CheckedChanged += new System.EventHandler(this.checkShowMissingOnly_CheckedChanged);
            // 
            // checkMissingMat
            // 
            this.checkMissingMat.AutoSize = true;
            this.checkMissingMat.Location = new System.Drawing.Point(338, 500);
            this.checkMissingMat.Name = "checkMissingMat";
            this.checkMissingMat.Size = new System.Drawing.Size(100, 17);
            this.checkMissingMat.TabIndex = 32;
            this.checkMissingMat.Text = "Missing material";
            this.checkMissingMat.UseVisualStyleBackColor = true;
            this.checkMissingMat.CheckedChanged += new System.EventHandler(this.checkMissingMat_CheckedChanged);
            // 
            // EditSpell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 531);
            this.Controls.Add(this.checkMissingMat);
            this.Controls.Add(this.checkShowMissingOnly);
            this.Controls.Add(this.butDelete);
            this.Controls.Add(this.butAddNew);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.richDescr);
            this.Controls.Add(this.checkWizard);
            this.Controls.Add(this.checkWarlock);
            this.Controls.Add(this.checkSorcerer);
            this.Controls.Add(this.checkRanger);
            this.Controls.Add(this.checkPaladin);
            this.Controls.Add(this.checkDruid);
            this.Controls.Add(this.checkCleric);
            this.Controls.Add(this.checkBard);
            this.Controls.Add(this.checkRitual);
            this.Controls.Add(this.textCastingTime);
            this.Controls.Add(this.textRange);
            this.Controls.Add(this.textDuration);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.numericLevel);
            this.Controls.Add(this.comboSchool);
            this.Controls.Add(this.comboSpellList);
            this.Name = "EditSpell";
            this.Text = "EditSpell";
            ((System.ComponentModel.ISupportInitialize)(this.numericLevel)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboSpellList;
        private System.Windows.Forms.ComboBox comboSchool;
        private System.Windows.Forms.NumericUpDown numericLevel;
        private System.Windows.Forms.CheckBox checkVerbal;
        private System.Windows.Forms.CheckBox checkSomatic;
        private System.Windows.Forms.CheckBox checkMaterial;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox richTextMaterial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textDuration;
        private System.Windows.Forms.TextBox textRange;
        private System.Windows.Forms.TextBox textCastingTime;
        private System.Windows.Forms.CheckBox checkRitual;
        private System.Windows.Forms.CheckBox checkBard;
        private System.Windows.Forms.CheckBox checkCleric;
        private System.Windows.Forms.CheckBox checkDruid;
        private System.Windows.Forms.CheckBox checkPaladin;
        private System.Windows.Forms.CheckBox checkRanger;
        private System.Windows.Forms.CheckBox checkSorcerer;
        private System.Windows.Forms.CheckBox checkWarlock;
        private System.Windows.Forms.CheckBox checkWizard;
        private System.Windows.Forms.RichTextBox richDescr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button butSave;
        private System.Windows.Forms.Button butAddNew;
        private System.Windows.Forms.Button butDelete;
        private System.Windows.Forms.CheckBox checkShowMissingOnly;
        private System.Windows.Forms.CheckBox checkMissingMat;
    }
}