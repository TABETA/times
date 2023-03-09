namespace times
{
    partial class ThemeListDialog
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
            this.dgvTheme = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTheme)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvTheme
            // 
            this.dgvTheme.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTheme.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dgvTheme.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTheme.Location = new System.Drawing.Point(0, 0);
            this.dgvTheme.Name = "dgvTheme";
            this.dgvTheme.RowTemplate.Height = 21;
            this.dgvTheme.Size = new System.Drawing.Size(191, 259);
            this.dgvTheme.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "ThemeNumber";
            this.Column1.Name = "Column1";
            // 
            // ThemeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(191, 259);
            this.Controls.Add(this.dgvTheme);
            this.Name = "ThemeList";
            this.Text = "ThemeList";
            this.Load += new System.EventHandler(this.ThemeList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTheme)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTheme;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
    }
}