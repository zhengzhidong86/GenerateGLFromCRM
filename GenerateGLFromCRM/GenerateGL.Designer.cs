namespace GenerateGLFromCRM
{
    partial class GenerateGL
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
            this.btn_ImportGL = new System.Windows.Forms.Button();
            this.epiUltraGridC1 = new System.Windows.Forms.DataGridView();
            this.txt_GroupPre = new System.Windows.Forms.TextBox();
            this.txt_MaxGLCount = new System.Windows.Forms.TextBox();
            this.txt_FiscalYear = new System.Windows.Forms.TextBox();
            this.txt_FiscalPeriod = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_ImportAll = new System.Windows.Forms.Button();
            this.lb_Msg = new System.Windows.Forms.Label();
            this.lb_Process = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epiUltraGridC1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_ImportGL
            // 
            this.btn_ImportGL.Location = new System.Drawing.Point(464, 14);
            this.btn_ImportGL.Name = "btn_ImportGL";
            this.btn_ImportGL.Size = new System.Drawing.Size(75, 22);
            this.btn_ImportGL.TabIndex = 0;
            this.btn_ImportGL.Text = "提取凭证";
            this.btn_ImportGL.UseVisualStyleBackColor = true;
            this.btn_ImportGL.Click += new System.EventHandler(this.btn_ImportGL_Click);
            // 
            // epiUltraGridC1
            // 
            this.epiUltraGridC1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.epiUltraGridC1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.epiUltraGridC1.Location = new System.Drawing.Point(0, 72);
            this.epiUltraGridC1.Name = "epiUltraGridC1";
            this.epiUltraGridC1.RowTemplate.Height = 23;
            this.epiUltraGridC1.Size = new System.Drawing.Size(693, 396);
            this.epiUltraGridC1.TabIndex = 1;
            // 
            // txt_GroupPre
            // 
            this.txt_GroupPre.Location = new System.Drawing.Point(74, 15);
            this.txt_GroupPre.MaxLength = 2;
            this.txt_GroupPre.Name = "txt_GroupPre";
            this.txt_GroupPre.Size = new System.Drawing.Size(37, 21);
            this.txt_GroupPre.TabIndex = 2;
            this.txt_GroupPre.Text = "T";
            // 
            // txt_MaxGLCount
            // 
            this.txt_MaxGLCount.Location = new System.Drawing.Point(215, 16);
            this.txt_MaxGLCount.Name = "txt_MaxGLCount";
            this.txt_MaxGLCount.Size = new System.Drawing.Size(51, 21);
            this.txt_MaxGLCount.TabIndex = 3;
            this.txt_MaxGLCount.Text = "100";
            // 
            // txt_FiscalYear
            // 
            this.txt_FiscalYear.Location = new System.Drawing.Point(337, 15);
            this.txt_FiscalYear.MaxLength = 4;
            this.txt_FiscalYear.Name = "txt_FiscalYear";
            this.txt_FiscalYear.Size = new System.Drawing.Size(42, 21);
            this.txt_FiscalYear.TabIndex = 4;
            this.txt_FiscalYear.Text = "2017";
            // 
            // txt_FiscalPeriod
            // 
            this.txt_FiscalPeriod.Location = new System.Drawing.Point(420, 15);
            this.txt_FiscalPeriod.MaxLength = 2;
            this.txt_FiscalPeriod.Name = "txt_FiscalPeriod";
            this.txt_FiscalPeriod.Size = new System.Drawing.Size(34, 21);
            this.txt_FiscalPeriod.TabIndex = 5;
            this.txt_FiscalPeriod.Text = "2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "群组前缀";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(120, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "群组最大凭证数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(278, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "会计年度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(385, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "期间";
            // 
            // btn_ImportAll
            // 
            this.btn_ImportAll.Location = new System.Drawing.Point(546, 14);
            this.btn_ImportAll.Name = "btn_ImportAll";
            this.btn_ImportAll.Size = new System.Drawing.Size(87, 22);
            this.btn_ImportAll.TabIndex = 10;
            this.btn_ImportAll.Text = "提取所有公司";
            this.btn_ImportAll.UseVisualStyleBackColor = true;
            this.btn_ImportAll.Click += new System.EventHandler(this.btn_ImportAll_Click);
            // 
            // lb_Msg
            // 
            this.lb_Msg.AutoSize = true;
            this.lb_Msg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lb_Msg.Location = new System.Drawing.Point(18, 47);
            this.lb_Msg.Name = "lb_Msg";
            this.lb_Msg.Size = new System.Drawing.Size(23, 12);
            this.lb_Msg.TabIndex = 11;
            this.lb_Msg.Text = "123";
            // 
            // lb_Process
            // 
            this.lb_Process.AutoSize = true;
            this.lb_Process.Location = new System.Drawing.Point(498, 47);
            this.lb_Process.Name = "lb_Process";
            this.lb_Process.Size = new System.Drawing.Size(77, 12);
            this.lb_Process.TabIndex = 12;
            this.lb_Process.Text = "Process Info";
            // 
            // GenerateGL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 464);
            this.Controls.Add(this.lb_Process);
            this.Controls.Add(this.lb_Msg);
            this.Controls.Add(this.btn_ImportAll);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_FiscalPeriod);
            this.Controls.Add(this.txt_FiscalYear);
            this.Controls.Add(this.txt_MaxGLCount);
            this.Controls.Add(this.txt_GroupPre);
            this.Controls.Add(this.epiUltraGridC1);
            this.Controls.Add(this.btn_ImportGL);
            this.Name = "GenerateGL";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "从CRM提取凭证 V2.4";
            this.Load += new System.EventHandler(this.GenerateGL_Load);
            ((System.ComponentModel.ISupportInitialize)(this.epiUltraGridC1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ImportGL;
        private System.Windows.Forms.DataGridView epiUltraGridC1;
        private System.Windows.Forms.TextBox txt_GroupPre;
        private System.Windows.Forms.TextBox txt_MaxGLCount;
        private System.Windows.Forms.TextBox txt_FiscalYear;
        private System.Windows.Forms.TextBox txt_FiscalPeriod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_ImportAll;
        private System.Windows.Forms.Label lb_Msg;
        private System.Windows.Forms.Label lb_Process;
    }
}