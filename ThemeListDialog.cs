using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace times
{
    public partial class ThemeListDialog : Form
    {
        public ThemeListDialog()
        {
            InitializeComponent();
        }

        private void ThemeList_Load(object sender, EventArgs e)
        {

        }
        public List<string> ShowDialog(List<string> themeNumbers)
        {
            foreach (var theme in themeNumbers)
            {
                var i = dgvTheme.Rows.Add();
                dgvTheme.Rows[i].Cells[0].Value = theme;
            }
            this.ShowDialog();
            var result = new List<string>();
            foreach (DataGridViewRow row in dgvTheme.Rows)
            {
                result.Add(row.Cells[0].Value as string ?? "");
            }
            return result.Where(v => !string.IsNullOrEmpty(v)).ToList();
        }
    }
}
