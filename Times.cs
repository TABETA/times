using RA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace times
{

    public enum ExpenseType
    {
        RefularTime,
        OverTime,
        Midnight,
        TravelExpense,
    }
    public static class ExpenseTypeExtensions
    {
        private static readonly string[] s_names = { "定時内", "定時外", "深夜", "出張費" };
        private static Dictionary<string, ExpenseType> s_map;
        static ExpenseTypeExtensions()
        {
            var vals = Enum.GetValues(typeof(ExpenseType)).OfType<ExpenseType>();
            s_map = s_names.Zip(vals, (n, e) => (n, e)).ToDictionary(v => v.n, v => v.e);
        }
        public static ExpenseType Parse(string name)
        {
            return s_map[name];
        }
        public static string GetName(this ExpenseType settlement)
        {
            return s_names[(int)settlement];
        }
    }
    public enum SettlementType
    {
        Tenth,
        Twentieth,
        EndOfMonth,
    }
    public static class SettlementTypeExtensions
    {
        private static readonly string[] s_names = { "10日", "20日", "末日" };
        private static Dictionary<string, SettlementType> s_map;
        static SettlementTypeExtensions()
        {
            var vals = Enum.GetValues(typeof(SettlementType)).OfType<SettlementType>();
            s_map = s_names.Zip(vals, (n, e) => (n, e)).ToDictionary(v => v.n, v => v.e);
        }
        public static string GetName(this SettlementType settlement)
        {
            return s_names[(int)settlement];
        }
        public static SettlementType Parse(string name)
        {
            return s_map[name];
        }
        public static DateTime StartDay(this SettlementType settlement, DateTime acceptanceMonth)
        {
            var baseDate = new DateTime(acceptanceMonth.Year, acceptanceMonth.Month, 1).AddMonths(-1);
            switch (settlement)
            {
                case SettlementType.Tenth:
                    return baseDate.AddDays(10);
                case SettlementType.Twentieth:
                    return baseDate.AddDays(20);
                case SettlementType.EndOfMonth:
                    return baseDate;
                default:
                    throw new NotImplementedException("未定義の締め日タイプです");
            }
        }
        public static DateTime EndDay(this SettlementType settlement, DateTime acceptanceMonth)
        {
            var baseDate = new DateTime(acceptanceMonth.Year, acceptanceMonth.Month, 1);
            switch (settlement)
            {
                case SettlementType.Tenth:
                    return baseDate.AddDays(9);
                case SettlementType.Twentieth:
                    return baseDate.AddDays(19);
                case SettlementType.EndOfMonth:
                    return baseDate.AddDays(-1);
                default:
                    throw new NotImplementedException("未定義の締め日タイプです");
            }
        }
        public static IEnumerable<DateTime> EnumerateWorkDays(this SettlementType settlement, DateTime acceptanceMonth)
        {
            var endDate = settlement.EndDay(acceptanceMonth);
            for (DateTime dt = settlement.StartDay(acceptanceMonth); dt <= endDate; dt = dt.AddDays(1))
            {
                yield return dt;
            }
        }
    }
    class UpdatableSPOListItem : SPOListItem
    {
        public UpdatableSPOListItem(int? id, object item, bool isDirty)
            : base(id, item)
        {
            IsDirty = isDirty;
        }
        public UpdatableSPOListItem(int? id, object item)
            : this(id, item, false)
        {
        }
        public UpdatableSPOListItem()
            : this(null, null, false)
        {
        }
        public bool IsDirty { get; set; }
    }
    public class Attendance
    {
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string AttendanceType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double BreakTime { get; set; }
        public double WorkHours { get => (End - Start).TotalHours - BreakTime; }
    }
    public abstract class Expense
    {
        public string EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string ThemeNumber { get; set; }
        public abstract string GetDisplayValue();
    }
    public class LaborCost : Expense
    {
        public double Duration { get; set; }
        public string WorkType { get; set; }
        public override string GetDisplayValue()
        {
            return Duration.ToString("F2");
        }
    }
    public class TravelExpense : Expense
    {
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double SubTotal { get; set; }
        public override string GetDisplayValue()
        {
            return UnitPrice.ToString();
        }
    }
    class Times
    {
        public string EmployeeID
        {
            get => m_employeeID;
            set => m_employeeID = value;
        }
        public List<string> Themes { get => m_themes; }
        public List<string> SavedThemes
        {
            get => m_savedThemes;
            set
            {
                m_savedThemes = value;
                ConfigurationUtility.UpdateSetting("SavedThemes", m_savedThemes.Aggregate((l, r) => $"{l},{r}"));
            }
        }
        private SharePointListHandler<Attendance> m_listAttendance;
        private SharePointListHandler<LaborCost> m_listWorkloads;
        private SharePointListHandler<TravelExpense> m_listExpenses;
        private Dictionary<DateTime, UpdatableSPOListItem> m_attendanceMap;
        private Dictionary<DateTime, Dictionary<string, UpdatableSPOListItem>> m_workloadMap;
        private string m_employeeID;
        private List<string> m_themes;
        private List<string> m_savedThemes;
        public Times(
            SharePointListHandler<Attendance> listAttendance,
            SharePointListHandler<LaborCost> listWorkloads,
            SharePointListHandler<TravelExpense> listExpenses)
        {
            m_savedThemes = ConfigurationManager.AppSettings["SavedThemes"]?.Split(',')?.ToList() ?? new List<string> { };
            m_listAttendance = listAttendance;
            m_listWorkloads = listWorkloads;
            m_listExpenses = listExpenses;
        }
        public void LoadItems(string account, string password, DateTime[] dataRange = null)
        {
            var camlQuery = ToCamlQuery(dataRange);
            var (attendanceItems, workloadItems, expenseItems) = GetSPOListItems(account, password, camlQuery);
            m_themes = workloadItems
                .Select(v => ((LaborCost)v.Item).ThemeNumber)
                .Union(expenseItems.Select(v => (v.Item as LaborCost)?.ThemeNumber ?? (v.Item as TravelExpense)?.ThemeNumber))
                .Union(SavedThemes)
                .Distinct()
                .ToList();
            m_attendanceMap = attendanceItems.GroupBy(x => ((Attendance)x.Item).Date).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            m_workloadMap = new Dictionary<DateTime, Dictionary<string, UpdatableSPOListItem>>();
            foreach (var x in workloadItems)
            {
                var v = (LaborCost)x.Item;
                if (!m_workloadMap.ContainsKey(v.Date))
                {
                    m_workloadMap[v.Date] = new Dictionary<string, UpdatableSPOListItem>();
                }
                var key = v.ThemeNumber + ":" + v.WorkType;
                m_workloadMap[v.Date][key] = x;
            }
            foreach (var x in expenseItems)
            {
                var v = (TravelExpense)x.Item;
                if (!m_workloadMap.ContainsKey(v.Date))
                {
                    m_workloadMap[v.Date] = new Dictionary<string, UpdatableSPOListItem>();
                }
                var key = v.ThemeNumber + ":出張費";
                m_workloadMap[v.Date][key] = x;
            }
        }
        public bool Push(DateTime date, string fieldName, object value)
        {
            var result = false;
            result |= PushAttendance(date, fieldName, value);
            result |= PushExpense(date, fieldName, value);
            return result;
        }
        public object Inquire(DateTime date, string fieldName)
        {
            var attendance = m_attendanceMap.GetOrDefault(date)?.Item as Attendance;
            switch (fieldName)
            {
                case "日":
                    return date.ToString("MM/dd(ddd)");
                case "予実":
                    return attendance?.Type ?? "";
                case "勤務形態":
                    return attendance?.AttendanceType ?? "";
                case "開始":
                    return attendance?.Start.ToString("HH:mm") ?? "";
                case "終了":
                    return attendance?.End.ToString("HH:mm") ?? "";
                case "休憩[h]":
                    return attendance?.BreakTime.ToString("F2") ?? "";
                case "勤務時間":
                    return attendance?.WorkHours.ToString("F2") ?? "";
                default:
                    {
                        var v = m_workloadMap.GetOrDefault(date)?.GetOrDefault(fieldName);
                        return (v?.Item as Expense)?.GetDisplayValue() ?? "";
                    }
            }
        }
        public void Save()
        {
            var atttendanceDirties = m_attendanceMap.Where(v => v.Value?.IsDirty ?? false).Select(v => v.Value);
            var attendanceUpdates = new List<SPOListItem>();
            var attendanceInserts = new List<Attendance>();
            foreach (var item in atttendanceDirties)
            {
                var v = (Attendance)item.Item;
                if (item.ID.HasValue)
                {
                    attendanceUpdates.Add(new SPOListItem(item.ID.Value, v));
                }
                else
                {
                    attendanceInserts.Add(v);
                }
            }
            m_listAttendance.Update(attendanceUpdates);
            m_listAttendance.Insert(attendanceInserts);
            var dict = new Dictionary<Type, (List<SPOListItem> Updates, List<object> Inserts)>();
            dict[typeof(LaborCost)] = (new List<SPOListItem>(), new List<object>());
            dict[typeof(TravelExpense)] = (new List<SPOListItem>(), new List<object>());
            foreach (var item in m_workloadMap.SelectMany(v => v.Value.Select(x => x.Value)).Where(v => v.IsDirty))
            {
                var t = item.Item.GetType();
                var l = dict[t];
                SortAdd(item, ref l);
            }
            m_listWorkloads.Update(dict[typeof(LaborCost)].Updates);
            m_listWorkloads.Insert(dict[typeof(LaborCost)].Inserts.Cast<LaborCost>());
            m_listExpenses.Update(dict[typeof(TravelExpense)].Updates);
            m_listExpenses.Insert(dict[typeof(TravelExpense)].Inserts.Cast<TravelExpense>());
        }
        public static bool TryToDateTime(object value, DateTime refDate, out DateTime val)
        {
            switch (value)
            {
                case DateTime @_:
                    val = @_;
                    return true;
                case string @_:
                    val = refDate.AddHours(@_.ToHours());
                    return true;
                default:
                    val = new DateTime();
                    return false;
            }
        }
        private (
            IEnumerable<UpdatableSPOListItem> attendanceItems,
            IEnumerable<UpdatableSPOListItem> workloadItems,
            IEnumerable<UpdatableSPOListItem> expenseItems)
            GetSPOListItems(string account, string password, string camlQuery)
        {
            m_listAttendance.SetCredentials(account, password);
            var attendanceItems = m_listAttendance.GetAll(camlQuery).Select(v => new UpdatableSPOListItem(v.ID, v.Item));
            m_listWorkloads.SetCredentials(account, password);
            var workloadItems = m_listWorkloads.GetAll(camlQuery).Select(v => new UpdatableSPOListItem(v.ID, v.Item));
            m_listExpenses.SetCredentials(account, password);
            var expenseItems = m_listExpenses.GetAll(camlQuery).Select(v => new UpdatableSPOListItem(v.ID, v.Item));
            return (attendanceItems, workloadItems, expenseItems);
        }
        private string ToCamlQuery(DateTime[] dataRange = null)
        {
            var employeeIDField = new CamlQueryField("EmployeeID");
            var employeeQuery = employeeIDField == m_employeeID;
            var orderByQuery = new CamlQueryOrderBy("Date", true);
            CamlQuery condition = employeeQuery;
            if (dataRange != null)
            {
                var minDate = dataRange[0];
                var maxDate = dataRange[dataRange.Length - 1];

                var dateField = new CamlQueryField("Date");
                var dateQuery = (dateField >= minDate) & (dateField <= maxDate);
                condition = condition & dateQuery;
            }
            return new CamlQuery(new CamlQueryWhere(condition), orderByQuery);
        }
        private static DateTime ToDateTime(object value, DateTime refDate)
        {
            switch (value)
            {
                case DateTime @_: return @_;
                case string @_:
                    return refDate.AddHours(@_.ToHours());
                default:
                    throw new NotImplementedException($"ToDateTime is not implemented for {value.GetType()} type.");
            }
        }
        private Expense CreateExpense(string expenseType, DateTime date, string theme, double dval, string t, string employeeID)
        {
            switch (expenseType)
            {
                case "出張費":
                    return new TravelExpense
                    {
                        Date = date,
                        ThemeNumber = theme,
                        UnitPrice = dval,
                        Quantity = 1,
                        Type = t,
                        EmployeeID = employeeID,
                    };
                case "定時内":
                case "定時外":
                case "深夜":
                    return new LaborCost
                    {
                        Date = date,
                        ThemeNumber = theme,
                        Duration = dval,
                        WorkType = expenseType,
                        Type = t,
                        EmployeeID = employeeID,
                    };
                default:
                    throw new NotImplementedException($"出張費, 定時内, 定時外, 深夜 以外の値が入力されました {expenseType}");
            }
        }
        private bool PushExpense(DateTime date, string fieldName, object value)
        {
            var workloads = m_workloadMap.GetOrDefault(date, new Dictionary<string, UpdatableSPOListItem>());
            switch (fieldName)
            {
                case "予実":
                    {
                        var str = value as string;
                        foreach (var v in workloads)
                        {
                            var w = v.Value;
                            if (w.Item is Expense e)
                            {
                                e.Type = str;
                            }
                            else
                            {
                                throw new NotImplementedException("Expense型以外のデータが入っている");
                            }
                            w.IsDirty = true;
                        }
                    }
                    return true;
                default:
                    {
                        var v = workloads.GetOrDefault(fieldName);
                        if (v == null)
                        {
                            v = workloads[fieldName] = new UpdatableSPOListItem();
                        }
                        if (!fieldName.Contains(':'))
                        {
                            return false;
                        }
                        var (theme, expenseType, _) = fieldName.Split(':');
                        var dval = value.ToDouble();
                        var t = Inquire(date, "予実").ToString();
                        v.Item = CreateExpense(expenseType, date, theme, dval, t, m_employeeID);
                        v.IsDirty = true;
                        return true;
                    }
            }
        }
        private bool PushAttendance(DateTime date, string fieldName, object value)
        {
            var attendance = m_attendanceMap.GetOrDefault(date);
            if (attendance == null)
            {
                attendance = new UpdatableSPOListItem
                {
                    ID = null,
                    Item = new Attendance
                    {
                        EmployeeID = m_employeeID,
                        Date = date,
                    },
                    IsDirty = true
                };
                m_attendanceMap[date] = attendance;
            }
            var item = attendance.Item as Attendance;
            if (item == null)
            {
                throw new Exception("");
            }
            switch (fieldName)
            {
                case "日":
                case "勤務時間":
                    //自動計算セルのため編集不可
                    return false;
                case "休憩[h]":
                    item.BreakTime = value.ToDouble();
                    attendance.IsDirty = true;
                    return true;
                case "開始":
                    item.Start = ToDateTime(value, date);
                    attendance.IsDirty = true;
                    return true;
                case "終了":
                    item.End = ToDateTime(value, date);
                    attendance.IsDirty = true;
                    return true;
                case "予実":
                    var str = value as string;
                    item.Type = str;
                    attendance.IsDirty = true;
                    return true;
                case "勤務形態":
                    item.AttendanceType = (string)value;
                    attendance.IsDirty = true;
                    return true;
                default:
                    return false;
            }
        }
        private void SortAdd(UpdatableSPOListItem item, ref (List<SPOListItem> Updates, List<object> Inserts) lists)
        {
            if (item.ID is int id)
            {
                lists.Updates.Add(new SPOListItem(id, item.Item));
            }
            else
            {
                lists.Inserts.Add(item.Item);
            }
        }
    }
}
