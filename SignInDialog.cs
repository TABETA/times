using System;
using System.Windows.Forms;

namespace times
{
    public partial class SignInDialog : Form
    {
        public SignInDialog()
        {
            InitializeComponent();
        }
        string m_user;
        string m_pswd;

        public (string User, string Password) ShowDialog(string user, string pswd)
        {
            tbMail.Text = m_user = user;
            tbPswd.Text = m_pswd = pswd;
            var result = this.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                throw new OperationCanceledException("ユーザによって認証がキャンセルされました");
            }
            return (m_user, m_pswd);
        }

        private void Submit()
        {
            this.DialogResult = DialogResult.OK;
            m_user = tbMail.Text;
            m_pswd = tbPswd.Text;
            this.Close();

        }
        private void tbPswd_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                Submit();
            }
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Submit();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}
