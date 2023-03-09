using System;
using System.Collections.Generic;
using SP = Microsoft.SharePoint.Client;
using System.Security;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using RA;

namespace times
{
    public struct PropertyBinder
    {
        public string ColumnName;
        public string PropertyName;
    }
    public class SPOListItem
    {
        public SPOListItem(int? iD, object item)
        {
            ID = iD;
            Item = item;
        }
        public SPOListItem(object item) : this(null, item)
        {
        }

        public int? ID { get; set; }
        public object Item { get; set; }
    }
    public interface SharePointListHandler<T> where T: new()
    {
        bool Insert(T w);
        bool Insert(IEnumerable<T> w);
        bool Update(SPOListItem item);
        bool Update(IEnumerable<SPOListItem> items);
        IEnumerable<SPOListItem> GetAll(string query = "");
        void SetCredentials(string account, string password);
    }
    public class SharePointListByCSOM<T>: SharePointListHandler<T> where T: new()
    {
        private string m_url;
        private string m_listName;
        private PropertyBinder[] m_pbs;
        private string m_account;
        private SecureString m_password = new SecureString();

        public SharePointListByCSOM(string url, string listName, PropertyBinder[] pbs)
        {
            m_url = url;
            m_listName = listName;
            m_pbs = pbs;
        }
        public SharePointListByCSOM(string url, string listName, PropertyBinder[] pbs, string account, string password)
            :this(url,listName,pbs)
        {
            SetCredentials(account, password);
        }
        ~SharePointListByCSOM()
        {
            m_password.Dispose();
        }
        public void SetCredentials(string account, string password)
        {
            m_account = account;
            m_password.Dispose();
            m_password = new SecureString();
            foreach (char c in password)
            {
                m_password.AppendChar(c);
            }
            m_password.MakeReadOnly();
        }
        private SP.ListItem SetItem(SP.ListItem item, T w)
        {
            foreach (var pb in m_pbs)
            {
                var property = typeof(T).GetProperty(pb.PropertyName);
                var val = property.GetValue(w);
                switch (val)
                {
                    case null:
                        break;
                    case DateTime @_:
                        if (@_ != new DateTime())
                        {
                            //SharePointはUTC時刻を入れてあげないといけない
                            var dateUTC = TimeZoneInfo.ConvertTimeToUtc(@_);
                            item[pb.ColumnName] = dateUTC.ToString();
                        }
                        break;
                    case string @_:
                        if (!string.IsNullOrEmpty(@_))
                        {
                            item[pb.ColumnName] = @_;
                        }
                        break;
                    default:
                        item[pb.ColumnName] = val;
                        break;
                }
            }
            return item;
        }
        private SPOListItem GetItem(SP.ListItem item)
        {
            var result = new T();
            foreach (var pb in m_pbs)
            {
                var property = typeof(T).GetProperty(pb.PropertyName);
                switch (item[pb.ColumnName])
                {
                    case null:
                        break;
                    case DateTime @_:
                        var localTime = TimeZoneInfo.ConvertTimeFromUtc(@_, TimeZoneInfo.Local);
                        property.SetValue(result, localTime);
                        break;
                    default:
                        property.SetValue(result, item[pb.ColumnName]);
                        break;
                }
            }
            var ID = (int)item["ID"];
            return new SPOListItem(ID, result);
        }
        public bool Update(SPOListItem item)
        {
            try
            {
                int id = item.ID.Value;
                T w = (T)item.Item;
                var context = new SP.ClientContext(m_url);
                context.Credentials = new SP.SharePointOnlineCredentials(m_account, m_password);
                var oList = context.Web.Lists.GetByTitle(m_listName);
                var oListItem = oList.GetItemById(id);
                context.Load(oListItem);
                context.ExecuteQuery();
                oListItem = SetItem(oListItem, w);
                oListItem.Update();
                context.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        public bool Update(IEnumerable<SPOListItem> items)
        {
            try
            {
                var context = new SP.ClientContext(m_url);
                context.Credentials = new SP.SharePointOnlineCredentials(m_account, m_password);
                var oList = context.Web.Lists.GetByTitle(m_listName);
                foreach (var item in items)
                {
                    var oListItem = oList.GetItemById(item.ID.Value);
                    oListItem = SetItem(oListItem, (T)item.Item);
                    oListItem.Update();
                }
                context.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public bool Insert(T w)
        {
            try
            {
                var context = new SP.ClientContext(m_url);
                context.Credentials = new SP.SharePointOnlineCredentials(m_account, m_password);
                var oList = context.Web.Lists.GetByTitle(m_listName);
                var itemCreateInfo = new SP.ListItemCreationInformation();
                var oListItem = oList.AddItem(itemCreateInfo);
                oListItem = SetItem(oListItem, w);
                oListItem.Update();
                context.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        public bool Insert(IEnumerable<T> items)
        {
            try
            {
                var context = new SP.ClientContext(m_url);
                context.Credentials = new SP.SharePointOnlineCredentials(m_account, m_password);
                var oList = context.Web.Lists.GetByTitle(m_listName);
                var itemCreateInfo = new SP.ListItemCreationInformation();
                foreach (var item in items)
                {
                    var oListItem = oList.AddItem(itemCreateInfo);
                    oListItem = SetItem(oListItem, item);
                    oListItem.Update();
                }
                context.ExecuteQuery();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
        public IEnumerable<SPOListItem> GetAll(string query = "")
        {
            var context = new SP.ClientContext(m_url);
            context.Credentials = new SP.SharePointOnlineCredentials(m_account, m_password);
            var oList = context.Web.Lists.GetByTitle(m_listName);
            var camlQuery = new SP.CamlQuery();
            camlQuery.ViewXml = query;
            if (string.IsNullOrEmpty(camlQuery.ViewXml))
            {
                camlQuery = SP.CamlQuery.CreateAllItemsQuery();
            }
            var collListItem = oList.GetItems(camlQuery);
            context.Load(collListItem);
            try
            {
                context.ExecuteQuery();
            }

            catch (SP.IdcrlException ex)
            {
                throw new ArgumentException("認証失敗", ex);
            }
            catch (Exception)
            {
                throw;
            }
            foreach (var item in collListItem)
            {
                var result = GetItem(item);
                yield return result;
            }
        }
    }

    public class CamlQuery
    {
        public CamlQuery(XElement elem)
        {
            m_elem = elem;
        }
        public CamlQuery(CamlQueryWhere where, CamlQueryOrderBy orderby)
        {
            m_elem = new XElement("View", new XAttribute("Scope", "RecursiveAll"),
                new XElement("Query",
                    where,
                    orderby
                )
                );
        }
        protected XElement m_elem;
        public static CamlQuery operator & (CamlQuery l, CamlQuery r)
        {
            return new CamlQuery(new XElement("And", l, r));
        }
        public static CamlQuery operator | (CamlQuery l, CamlQuery r)
        {
            return new CamlQuery(new XElement("Or", l, r));
        }

        public static implicit operator XElement(CamlQuery me)
        {
            return me.m_elem;
        }
        public static implicit operator string(CamlQuery me)
        {
            return me.ToString();
        }
        public override string ToString()
        {
            return m_elem.ToString();
        }
    }

    public class CamlQueryWhere : CamlQuery
    {
        public CamlQueryWhere(CamlQuery v)
            :base(new XElement("Where", v))
        {
        }
    }
    public class CamlQueryOrderBy : CamlQuery
    {
        public CamlQueryOrderBy(string fieldName, bool isAscending)
            :base(new XElement("OrderBy",
                new XElement("FieldRef", new XAttribute("Name", fieldName), new XAttribute("Ascending", isAscending))))
        {
        }
    }
    public class CamlQueryField : CamlQuery
    {
        public CamlQueryField(XElement elem)
            :base(elem)
        {
        }
        public CamlQueryField(string name)
            :base(new XElement("FieldRef", new XAttribute("Name", name)))
        {
        }
        public CamlQuery In<T>(IEnumerable<T> range)
        {
            return (this <= range.Min()) & (this >= range.Max());
        }
        public static CamlQueryField operator >=(CamlQueryField l, object r) => new CamlQueryField(new XElement("Geq", l, r.ToValueElement()));
        public static CamlQueryField operator <=(CamlQueryField l, object r) => new CamlQueryField(new XElement("Leq", l, r.ToValueElement()));
        public static CamlQueryField operator ==(CamlQueryField l, object r) => new CamlQueryField(new XElement("Eq", l, r.ToValueElement()));
        public static CamlQueryField operator !=(CamlQueryField l, object r) => new CamlQueryField(new XElement("Neq", l, r.ToValueElement()));
    }
    public static class CamlQueryExtensions
    {
        public static XElement ToValueElement(this object value)
        {
            switch (value)
            {
                case DateTime @_:
                    return
                        new XElement("Value",
                            new XAttribute("Type", "DateTime"),
                            new XAttribute("IncludeTimeValue", "True"),
                            @_.ToISO8601()
                            );
                default:
                    return
                        new XElement("Value",
                            new XAttribute("Type", "Text"),
                            value
                            );
            }
        }
    }
}
