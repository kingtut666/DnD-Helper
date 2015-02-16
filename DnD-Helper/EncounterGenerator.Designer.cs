namespace DnDMonsters
{
    partial class EncounterGenerator
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
            this.label1 = new System.Windows.Forms.Label();
            this.treeMonsters = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblRemainingXP = new System.Windows.Forms.Label();
            this.lblCurrentXP = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblThresholdXP = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboDifficulty = new System.Windows.Forms.ComboBox();
            this.numLevel = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numNPlayers = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numMonsters = new System.Windows.Forms.NumericUpDown();
            this.butRollMonsters = new System.Windows.Forms.Button();
            this.checkListMonsters = new System.Windows.Forms.CheckedListBox();
            this.butAddSelected = new System.Windows.Forms.Button();
            this.butClear = new System.Windows.Forms.Button();
            this.numTargetXP = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.radioMaxGroup = new System.Windows.Forms.RadioButton();
            this.radioMaxSingle = new System.Windows.Forms.RadioButton();
            this.radioBoss50 = new System.Windows.Forms.RadioButton();
            this.radioBoss25 = new System.Windows.Forms.RadioButton();
            this.radioBoss75 = new System.Windows.Forms.RadioButton();
            this.radioRandom = new System.Windows.Forms.RadioButton();
            this.checkResetTree = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNPlayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonsters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetXP)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 1;
            // 
            // treeMonsters
            // 
            this.treeMonsters.CheckBoxes = true;
            this.treeMonsters.Location = new System.Drawing.Point(15, 25);
            this.treeMonsters.Name = "treeMonsters";
            this.treeMonsters.Size = new System.Drawing.Size(226, 548);
            this.treeMonsters.TabIndex = 2;
            this.treeMonsters.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeMonsters_AfterCheck);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Monsters";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(268, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "No of Monsters";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblRemainingXP);
            this.groupBox1.Controls.Add(this.lblCurrentXP);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.lblThresholdXP);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.comboDifficulty);
            this.groupBox1.Controls.Add(this.numLevel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numNPlayers);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(261, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 180);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Party";
            // 
            // lblRemainingXP
            // 
            this.lblRemainingXP.AutoSize = true;
            this.lblRemainingXP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRemainingXP.Location = new System.Drawing.Point(125, 152);
            this.lblRemainingXP.Name = "lblRemainingXP";
            this.lblRemainingXP.Size = new System.Drawing.Size(14, 13);
            this.lblRemainingXP.TabIndex = 11;
            this.lblRemainingXP.Text = "0";
            // 
            // lblCurrentXP
            // 
            this.lblCurrentXP.AutoSize = true;
            this.lblCurrentXP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentXP.Location = new System.Drawing.Point(125, 129);
            this.lblCurrentXP.Name = "lblCurrentXP";
            this.lblCurrentXP.Size = new System.Drawing.Size(14, 13);
            this.lblCurrentXP.TabIndex = 10;
            this.lblCurrentXP.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(28, 152);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 9;
            this.label10.Text = "Remaining XP = ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(28, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Current XP =";
            // 
            // lblThresholdXP
            // 
            this.lblThresholdXP.AutoSize = true;
            this.lblThresholdXP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThresholdXP.Location = new System.Drawing.Point(125, 106);
            this.lblThresholdXP.Name = "lblThresholdXP";
            this.lblThresholdXP.Size = new System.Drawing.Size(14, 13);
            this.lblThresholdXP.TabIndex = 7;
            this.lblThresholdXP.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "XP Threshold =";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Difficulty";
            // 
            // comboDifficulty
            // 
            this.comboDifficulty.FormattingEnabled = true;
            this.comboDifficulty.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard",
            "Deadly"});
            this.comboDifficulty.Location = new System.Drawing.Point(79, 71);
            this.comboDifficulty.Name = "comboDifficulty";
            this.comboDifficulty.Size = new System.Drawing.Size(121, 21);
            this.comboDifficulty.TabIndex = 4;
            this.comboDifficulty.SelectedIndexChanged += new System.EventHandler(this.comboDifficulty_SelectedIndexChanged);
            // 
            // numLevel
            // 
            this.numLevel.Location = new System.Drawing.Point(80, 45);
            this.numLevel.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLevel.Name = "numLevel";
            this.numLevel.Size = new System.Drawing.Size(59, 20);
            this.numLevel.TabIndex = 3;
            this.numLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numLevel.ValueChanged += new System.EventHandler(this.numLevel_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Player Level";
            // 
            // numNPlayers
            // 
            this.numNPlayers.Location = new System.Drawing.Point(80, 19);
            this.numNPlayers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNPlayers.Name = "numNPlayers";
            this.numNPlayers.Size = new System.Drawing.Size(59, 20);
            this.numNPlayers.TabIndex = 1;
            this.numNPlayers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numNPlayers.ValueChanged += new System.EventHandler(this.numNPlayers_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "# Players";
            // 
            // numMonsters
            // 
            this.numMonsters.Location = new System.Drawing.Point(353, 202);
            this.numMonsters.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMonsters.Name = "numMonsters";
            this.numMonsters.Size = new System.Drawing.Size(65, 20);
            this.numMonsters.TabIndex = 7;
            this.numMonsters.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // butRollMonsters
            // 
            this.butRollMonsters.Location = new System.Drawing.Point(441, 199);
            this.butRollMonsters.Name = "butRollMonsters";
            this.butRollMonsters.Size = new System.Drawing.Size(94, 23);
            this.butRollMonsters.TabIndex = 8;
            this.butRollMonsters.Text = "Roll Monsters";
            this.butRollMonsters.UseVisualStyleBackColor = true;
            this.butRollMonsters.Click += new System.EventHandler(this.butRollMonsters_Click);
            // 
            // checkListMonsters
            // 
            this.checkListMonsters.FormattingEnabled = true;
            this.checkListMonsters.Location = new System.Drawing.Point(261, 364);
            this.checkListMonsters.Name = "checkListMonsters";
            this.checkListMonsters.Size = new System.Drawing.Size(249, 169);
            this.checkListMonsters.TabIndex = 9;
            // 
            // butAddSelected
            // 
            this.butAddSelected.Location = new System.Drawing.Point(261, 548);
            this.butAddSelected.Name = "butAddSelected";
            this.butAddSelected.Size = new System.Drawing.Size(89, 23);
            this.butAddSelected.TabIndex = 10;
            this.butAddSelected.Text = "Add Selected";
            this.butAddSelected.UseVisualStyleBackColor = true;
            this.butAddSelected.Click += new System.EventHandler(this.butAddSelected_Click);
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(356, 548);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(75, 23);
            this.butClear.TabIndex = 11;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // numTargetXP
            // 
            this.numTargetXP.Location = new System.Drawing.Point(353, 228);
            this.numTargetXP.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTargetXP.Name = "numTargetXP";
            this.numTargetXP.Size = new System.Drawing.Size(120, 20);
            this.numTargetXP.TabIndex = 12;
            this.numTargetXP.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTargetXP.ValueChanged += new System.EventHandler(this.numTargetXP_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(268, 230);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Target XP";
            // 
            // radioMaxGroup
            // 
            this.radioMaxGroup.AutoSize = true;
            this.radioMaxGroup.Location = new System.Drawing.Point(261, 290);
            this.radioMaxGroup.Name = "radioMaxGroup";
            this.radioMaxGroup.Size = new System.Drawing.Size(75, 17);
            this.radioMaxGroup.TabIndex = 14;
            this.radioMaxGroup.Text = "Max group";
            this.radioMaxGroup.UseVisualStyleBackColor = true;
            // 
            // radioMaxSingle
            // 
            this.radioMaxSingle.AutoSize = true;
            this.radioMaxSingle.Location = new System.Drawing.Point(261, 314);
            this.radioMaxSingle.Name = "radioMaxSingle";
            this.radioMaxSingle.Size = new System.Drawing.Size(75, 17);
            this.radioMaxSingle.TabIndex = 15;
            this.radioMaxSingle.Text = "Max single";
            this.radioMaxSingle.UseVisualStyleBackColor = true;
            // 
            // radioBoss50
            // 
            this.radioBoss50.AutoSize = true;
            this.radioBoss50.Checked = true;
            this.radioBoss50.Location = new System.Drawing.Point(407, 313);
            this.radioBoss50.Name = "radioBoss50";
            this.radioBoss50.Size = new System.Drawing.Size(83, 17);
            this.radioBoss50.TabIndex = 16;
            this.radioBoss50.TabStop = true;
            this.radioBoss50.Text = "Boss at 50%";
            this.radioBoss50.UseVisualStyleBackColor = true;
            // 
            // radioBoss25
            // 
            this.radioBoss25.AutoSize = true;
            this.radioBoss25.Location = new System.Drawing.Point(407, 290);
            this.radioBoss25.Name = "radioBoss25";
            this.radioBoss25.Size = new System.Drawing.Size(83, 17);
            this.radioBoss25.TabIndex = 17;
            this.radioBoss25.Text = "Boss at 25%";
            this.radioBoss25.UseVisualStyleBackColor = true;
            // 
            // radioBoss75
            // 
            this.radioBoss75.AutoSize = true;
            this.radioBoss75.Location = new System.Drawing.Point(407, 337);
            this.radioBoss75.Name = "radioBoss75";
            this.radioBoss75.Size = new System.Drawing.Size(83, 17);
            this.radioBoss75.TabIndex = 18;
            this.radioBoss75.Text = "Boss at 75%";
            this.radioBoss75.UseVisualStyleBackColor = true;
            // 
            // radioRandom
            // 
            this.radioRandom.AutoSize = true;
            this.radioRandom.Location = new System.Drawing.Point(261, 337);
            this.radioRandom.Name = "radioRandom";
            this.radioRandom.Size = new System.Drawing.Size(65, 17);
            this.radioRandom.TabIndex = 19;
            this.radioRandom.Text = "Random";
            this.radioRandom.UseVisualStyleBackColor = true;
            // 
            // checkResetTree
            // 
            this.checkResetTree.AutoSize = true;
            this.checkResetTree.Checked = true;
            this.checkResetTree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkResetTree.Location = new System.Drawing.Point(353, 254);
            this.checkResetTree.Name = "checkResetTree";
            this.checkResetTree.Size = new System.Drawing.Size(151, 17);
            this.checkResetTree.TabIndex = 12;
            this.checkResetTree.Text = "Reset Tree on XP Change";
            this.checkResetTree.UseVisualStyleBackColor = true;
            // 
            // Encounter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 585);
            this.Controls.Add(this.checkResetTree);
            this.Controls.Add(this.radioRandom);
            this.Controls.Add(this.radioBoss75);
            this.Controls.Add(this.radioBoss25);
            this.Controls.Add(this.radioBoss50);
            this.Controls.Add(this.radioMaxSingle);
            this.Controls.Add(this.radioMaxGroup);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numTargetXP);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.butAddSelected);
            this.Controls.Add(this.checkListMonsters);
            this.Controls.Add(this.butRollMonsters);
            this.Controls.Add(this.numMonsters);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.treeMonsters);
            this.Controls.Add(this.label1);
            this.Name = "Encounter";
            this.Text = "Encounter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numNPlayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMonsters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetXP)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView treeMonsters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblRemainingXP;
        private System.Windows.Forms.Label lblCurrentXP;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblThresholdXP;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboDifficulty;
        private System.Windows.Forms.NumericUpDown numLevel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numNPlayers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numMonsters;
        private System.Windows.Forms.Button butRollMonsters;
        private System.Windows.Forms.CheckedListBox checkListMonsters;
        private System.Windows.Forms.Button butAddSelected;
        private System.Windows.Forms.Button butClear;
        private System.Windows.Forms.NumericUpDown numTargetXP;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioMaxGroup;
        private System.Windows.Forms.RadioButton radioMaxSingle;
        private System.Windows.Forms.RadioButton radioBoss50;
        private System.Windows.Forms.RadioButton radioBoss25;
        private System.Windows.Forms.RadioButton radioBoss75;
        private System.Windows.Forms.RadioButton radioRandom;
        private System.Windows.Forms.CheckBox checkResetTree;
    }
}