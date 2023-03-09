namespace times
{
    partial class TimesForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.tbCompany = new System.Windows.Forms.TextBox();
            this.tbEmployeeID = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbMonth = new System.Windows.Forms.TextBox();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.dgvAttendance = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOpenThemeDialog = new System.Windows.Forms.Button();
            this.cmbSettlement = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttendance)).BeginInit();
            this.SuspendLayout();
            // 
            // tbCompany
            // 
            this.tbCompany.Location = new System.Drawing.Point(12, 12);
            this.tbCompany.Name = "tbCompany";
            this.tbCompany.Size = new System.Drawing.Size(100, 19);
            this.tbCompany.TabIndex = 0;
            this.tbCompany.Text = "会社名";
            // 
            // tbEmployeeID
            // 
            this.tbEmployeeID.Location = new System.Drawing.Point(128, 12);
            this.tbEmployeeID.Name = "tbEmployeeID";
            this.tbEmployeeID.Size = new System.Drawing.Size(100, 19);
            this.tbEmployeeID.TabIndex = 1;
            this.tbEmployeeID.Text = "人員コード";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(234, 12);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(100, 19);
            this.tbName.TabIndex = 2;
            this.tbName.Text = "氏名";
            // 
            // tbMonth
            // 
            this.tbMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMonth.Location = new System.Drawing.Point(699, 12);
            this.tbMonth.Name = "tbMonth";
            this.tbMonth.Size = new System.Drawing.Size(72, 19);
            this.tbMonth.TabIndex = 3;
            this.tbMonth.Validating += new System.ComponentModel.CancelEventHandler(this.tbMonth_Validating);
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrev.Location = new System.Drawing.Point(682, 12);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(17, 19);
            this.btnPrev.TabIndex = 4;
            this.btnPrev.Text = "<";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(771, 12);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(17, 19);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // dgvAttendance
            // 
            this.dgvAttendance.AllowUserToAddRows = false;
            this.dgvAttendance.AllowUserToDeleteRows = false;
            this.dgvAttendance.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAttendance.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttendance.Location = new System.Drawing.Point(12, 37);
            this.dgvAttendance.Name = "dgvAttendance";
            this.dgvAttendance.RowTemplate.Height = 21;
            this.dgvAttendance.Size = new System.Drawing.Size(776, 700);
            this.dgvAttendance.TabIndex = 6;
            this.dgvAttendance.VirtualMode = true;
            this.dgvAttendance.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvAttendance_CellValidating);
            this.dgvAttendance.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvAttendance_CellValueNeeded);
            this.dgvAttendance.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvAttendance_CellValuePushed);
            this.dgvAttendance.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvAttendance_KeyDown);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(350, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpenThemeDialog
            // 
            this.btnOpenThemeDialog.Location = new System.Drawing.Point(431, 10);
            this.btnOpenThemeDialog.Name = "btnOpenThemeDialog";
            this.btnOpenThemeDialog.Size = new System.Drawing.Size(75, 23);
            this.btnOpenThemeDialog.TabIndex = 9;
            this.btnOpenThemeDialog.Text = "テーマ管理";
            this.btnOpenThemeDialog.UseVisualStyleBackColor = true;
            this.btnOpenThemeDialog.Click += new System.EventHandler(this.btnOpenThemeDialog_Click);
            // 
            // cmbSettlement
            // 
            this.cmbSettlement.FormattingEnabled = true;
            this.cmbSettlement.Location = new System.Drawing.Point(567, 11);
            this.cmbSettlement.Name = "cmbSettlement";
            this.cmbSettlement.Size = new System.Drawing.Size(52, 20);
            this.cmbSettlement.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(530, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "締日:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(633, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "対象月:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 749);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSettlement);
            this.Controls.Add(this.btnOpenThemeDialog);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvAttendance);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.tbMonth);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbEmployeeID);
            this.Controls.Add(this.tbCompany);
            this.Name = "Form1";
            this.Text = "Times";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttendance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCompany;
        private System.Windows.Forms.TextBox tbEmployeeID;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbMonth;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.DataGridView dgvAttendance;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpenThemeDialog;
        private System.Windows.Forms.ComboBox cmbSettlement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

