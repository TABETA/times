using RA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace times
{
    public partial class TimesForm : Form , INotifyPropertyChanged
    {
        public Func<string, bool> IsContinueDialog = (msg) => MessageBox.Show(msg, "", MessageBoxButtons.OKCancel) == DialogResult.Cancel;
        public event PropertyChangedEventHandler PropertyChanged;
        public SettlementType SettlementType
        {
            get => m_settlement;
            set
            {
                m_settlement = value;
                ConfigurationUtility.UpdateSetting("SettlementType", m_settlement.GetName());
                InitializeAttendanceView(m_times.Themes, m_targetMonth, m_settlement);
            }
        }
        public DateTime TargetMonth
        {
            get => m_targetMonth;
            set
            {
                m_targetMonth = new DateTime(value.Year, value.Month, 1);
                tbMonth.Text = $"{m_targetMonth: yyyy/MM}";
                InitializeAttendanceView(m_times.Themes, m_targetMonth, m_settlement);
            }
        }
        public string EmployeeID
        {
            get => m_times.EmployeeID;
            set
            {
                m_times.EmployeeID = value;
                OnPropertyChanged("EmployeeID");
            }
        }
        private List<DataGridViewColumn> m_currentColumns;
        private List<string> m_headers;
        private DateTime[] m_dates;
        private SettlementType m_settlement;
        private DateTime m_targetMonth;
        private Times m_times;
        public TimesForm()
        {
            InitializeComponent();
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        private (string Username, string Password) GetCredentials(string account = "", string password = "")
        {
            var f = new SignInDialog();
            return f.ShowDialog(account, password);
        }
        private SharePointListByCSOM<T> InitializeSPOList<T>(string urlKey, string listNameKey, string[] binderProps) where T : new()
        {
            var url = ConfigurationManager.AppSettings[urlKey];
            var listName = ConfigurationManager.AppSettings[listNameKey];
            var binders = binderProps.Select(b => new PropertyBinder { ColumnName = b, PropertyName = b }).ToArray();
            return new SharePointListByCSOM<T>(url, listName, binders);
        }

        private void InitializeAttendanceView(List<string> themeNames, DateTime targetMonth, SettlementType settlement)
        {
            var types = new[] {"定時内","定時外", "深夜", "出張費" }; 
            var defaultColumnBases = new (string Name, bool ReadOnly, Type ValueType, string Format, string[] Choice)[] {
                ("日",       true,  typeof(DateTime), "MM/dd(ddd)", null),
                ("予実",     false, typeof(string  ), "", new[]{ "見込", "実績"}),
                ("勤務形態", false, typeof(string  ), "", new[]{ "出社", "在宅", "出張", "有休", "欠勤"}),
                ("開始",     false, typeof(DateTime), "HH:mm", null),
                ("終了",     false, typeof(DateTime), "HH:mm", null),
                ("休憩[h]",  false, typeof(double), "F2", null),
                ("勤務時間", false, typeof(double), "F2", null),
            };
            var additionalColumnBases = themeNames?.SelectMany(t => types.Select(ts => (Name: $"{t}:{ts}", ReadOnly: false, ValueType: typeof(double), Format: "F2", Choice: null as string[])));
            m_currentColumns = (additionalColumnBases != null ? defaultColumnBases.Union(additionalColumnBases) : defaultColumnBases).ToColumns(60).ToList();

            var from = settlement.StartDay(targetMonth);
            m_dates = Enumerable.Range(0, from.AddMonths(1).Subtract(from).Days).Select(offset => from.AddDays(offset)).ToArray();

            dgvAttendance.Columns.Clear();
            m_currentColumns.ForEach(col => dgvAttendance.Columns.Add(col));
            m_headers = dgvAttendance.Columns.ToEnumerable().Select(v => v.HeaderText).ToList();
            dgvAttendance.Rows.Clear();
            if (m_dates != null)
            {
                dgvAttendance.Rows.AddRange(Enumerable.Range(0, m_dates.Length).Select(_ => new DataGridViewRow()).ToArray());
            }
        }
        private object Inquire((DateTime Date, string FieldName) key)
        {
            return m_times.Inquire(key.Date, key.FieldName);
        }
        private bool Push((DateTime Date, string FieldName) key, object value)
        {
            return m_times.Push(key.Date, key.FieldName, value);
        }
        private (DateTime Date, string FieldName) ToKey(DataGridViewCellValueEventArgs e)
        {
            return (m_dates[e.RowIndex], m_headers[e.ColumnIndex]);
        }
        private bool Validate(string fieldName, object value)
        {
            if (value == null)
            {
                return true;
            }
            if (string.IsNullOrEmpty(value as string))
            {
                return true;
            }
            switch (fieldName)
            {
                case "日":
                case "勤務時間":
                    return true;
                case "休憩[h]":
                    return double.TryParse(value as string, out var _);
                case "開始":
                case "終了":
                    return Times.TryToDateTime(value, DateTime.Now, out _);
                case "予実":
                    switch (value as string)
                    {
                        case "実績":
                        case "見込":
                            return true;
                        default:
                            return false;
                    }
                case "勤務形態":
                    switch (value as string)
                    {
                        case "出社":
                        case "在宅":
                        case "出張":
                        case "有休":
                        case "欠勤":
                            return true;
                        default:
                            return false;
                    }
                default:
                    return double.TryParse(value as string, out var _);
            }
        }
        private void InitializeSettlementType()
        {
            cmbSettlement.Items.Clear();
            cmbSettlement.Items.AddRange(Enum.GetValues(typeof(SettlementType)).OfType<SettlementType>().Select(v => v.GetName()).ToArray());
            var settlementType = ConfigurationManager.AppSettings["SettlementType"];
            if (string.IsNullOrEmpty(settlementType))
            {
                cmbSettlement.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < cmbSettlement.Items.Count; i++)
                {
                    if (cmbSettlement.Items[i].ToString() == settlementType)
                    {
                        cmbSettlement.SelectedIndex = i;
                        m_settlement = SettlementTypeExtensions.Parse(settlementType);
                        break;
                    }
                }
            }
            cmbSettlement.SelectedIndexChanged += new EventHandler(cmbSettlement_SelectedIndexChanged);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            var listAttendance = InitializeSPOList<Attendance>(
                "Attendance_SiteUrl",
                "Attendance_ListName",
                new[] {
                                "EmployeeID",
                                "Date",
                                "Type",
                                "AttendanceType",
                                "Start",
                                "End",
                                "BreakTime",
                }
                );
            var listWorkloads = InitializeSPOList<LaborCost>(
                "Workloads_SiteUrl",
                "Workloads_ListName",
                new[] {
                    "EmployeeID",
                    "Date",
                    "Type",
                    "ThemeNumber",
                    "Duration",
                    "WorkType",
                }
                );
            var listExpenses = InitializeSPOList<TravelExpense>(
                "Expenses_SiteUrl",
                "Expenses_ListName",
                new[] {
                    "EmployeeID",
                    "Date",
                    "Type",
                    "ThemeNumber",
                    "Quantity",
                    "UnitPrice",
                    "SubTotal",
                }
                );
            m_times = new Times(listAttendance, listWorkloads, listExpenses);
            InitializeSettlementType();
            TargetMonth = DateTime.Now;
            tbEmployeeID.DataBindings.Add("Text", this, "EmployeeID");
            EmployeeID = ConfigurationManager.AppSettings["EmployeeID"];

            var account = ConfigurationManager.AppSettings["M365Account"];
            var password = ConfigurationManager.AppSettings["M365Password"];
            var doesNeedInputCredentials = new[] { account, password }.Any(v => string.IsNullOrEmpty(v));
            while (true)
            {
                try
                {
                    if (doesNeedInputCredentials)
                    {
                        (account, password) = GetCredentials(account, password);
                    }
                    m_times.LoadItems(account, password);
                    ConfigurationUtility.UpdateSetting("M365Account", account);
                    ConfigurationUtility.UpdateSetting("M365Password", password);
                    break;
                }
                catch (OperationCanceledException)
                {
                    if (!IsContinueDialog("認証をキャンセルしました。アプリケーションを終了しますか？"))
                    {
                        Close();
                        return;
                    }
                }
                catch (ArgumentException ex)
                {
                    if (!IsContinueDialog("認証に失敗しました。認証情報を再度入力してください。\n" + ex.ToString()))
                    {
                        Close();
                        return;
                    }
                    doesNeedInputCredentials = true;
                }
                catch (Exception ex)
                {
                    if (!IsContinueDialog("予期せぬ例外:\n" + ex.ToString()))
                    {
                        Close();
                        return;
                    }
                    doesNeedInputCredentials = true;
                }
            }
            InitializeAttendanceView(m_times.Themes, m_targetMonth, m_settlement);
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            TargetMonth = TargetMonth.AddMonths(-1);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            TargetMonth = TargetMonth.AddMonths(1);
        }
        private void dgvAttendance_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            e.Value = Inquire(ToKey(e));
        }
        private void dgvAttendance_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            Push(ToKey(e), e.Value);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            m_times.Save();
            MessageBox.Show("サーバに保存しました。");
        }
        private void dgvAttendance_KeyDown(object sender, KeyEventArgs e)
        {
            var dgv = dgvAttendance;
            if (e.Control && e.KeyCode == Keys.V)
            {
                var vals = Clipboard.GetText().Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(s => s.Split('\t')).ToArray();
                var selectedCells = dgv.SelectedCells.ToEnumerable();
                var rows = selectedCells
                    .GroupBy(v => v.RowIndex)
                    .OrderBy(v => v.Key)
                    .Select(g => g.OrderBy(v => v.ColumnIndex).ToList()).ToList();
                switch (vals.Length)
                {
                    case 0:
                        //何もしない
                        break;
                    case 1:
                        {
                            foreach (var val in vals)
                            {
                                var colnum = val.Length;
                                switch (colnum)
                                {
                                    case 0:
                                        //何もしない
                                        break;
                                    case 1:
                                        //1セルペースト
                                        rows.ForEach(row =>
                                            row.ForEach(cell =>
                                                cell.Value = val[0]
                                                ));
                                        break;
                                    default:
                                        //行ペースト
                                        rows.ForEach(row =>
                                            row.Zip(val, (c, v) => (Cell: c, Val: v)).ToList().ForEach(zip => zip.Cell.Value = zip.Val)
                                            );
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        //行列ペースト
                        {
                            var rowsZip = rows.Zip(vals, (row, val) => (Row: row, Vals: val)).ToList();
                            foreach (var zip in rowsZip)
                            {
                                zip.Row.Zip(zip.Vals, (cell, val) => (Cell: cell, Val: val)).ToList().ForEach(v => v.Cell.Value = v.Val);
                            }
                        }
                        break;
                }
            }
        }
        private void dgvAttendance_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var fieldName = m_headers[e.ColumnIndex];
            if (!Validate(fieldName, e.FormattedValue))
            {
                e.Cancel = true;
            }
        }
        private void btnOpenThemeDialog_Click(object sender, EventArgs e)
        {
            var f = new ThemeListDialog();
            m_times.SavedThemes = f.ShowDialog(m_times.SavedThemes);
        }
        private void cmbSettlement_SelectedIndexChanged(object sender, EventArgs e)
        {
            var i = cmbSettlement.SelectedIndex;
            if (cmbSettlement.Items[i] is string v)
            {
                SettlementType = SettlementTypeExtensions.Parse(v);
            }
            else
            {
                throw new Exception("未定義の選択肢");
            }
        }
        private void tbMonth_Validating(object sender, CancelEventArgs e)
        {
            var val = tbMonth.Text + "/1";
            if (DateTime.TryParse(val, out var dtval))
            {
                TargetMonth = dtval;
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show("年月(yyyy/mm)を入力してください");
            }
        }
    }
}