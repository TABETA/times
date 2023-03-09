using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;

namespace RA
{
    public static class DataGridViewExtensions
    {
        public static IEnumerable<DataGridViewColumn> ToEnumerable(this DataGridViewColumnCollection vals)
        {
            foreach (DataGridViewColumn val in vals)
            {
                yield return val;
            }
        }
        public static IEnumerable<DataGridViewCell> ToEnumerable(this DataGridViewSelectedCellCollection vals)
        {
            foreach (DataGridViewCell val in vals)
            {
                yield return val;
            }
        }
        public static DataGridViewComboBoxCell ToComboBoxCellTemplate(this string[] vals)
        {
            if (vals == null)
            {
                return null;
            }
            var dt = new DataTable("_");
            dt.Columns.Add("Value", typeof(string));
            for (int i = 0; i < vals.Length; i++)
            {
                dt.Rows.Add(vals[i]);

            }
            return new DataGridViewComboBoxCell
            {
                DataSource = dt,
                DisplayMember = "Value",
                ValueMember = "Value",
                FlatStyle = FlatStyle.Flat,
            };
        }
        public static IEnumerable<DataGridViewColumn> ToColumns(this IEnumerable<(string Name, bool ReadOnly, Type ValueType, string Format, string[] Choice)> vals, int width)
        {
            return vals.Select(v => v.Choice == null
            ? new DataGridViewColumn
            {
                Name = v.Name,
                HeaderText = v.Name,
                ReadOnly = v.ReadOnly,
                ValueType = v.ValueType,
                CellTemplate = new DataGridViewTextBoxCell(),
                Width = width,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = v.Format,
                },
            }
            : new DataGridViewComboBoxColumn
            {
                Name = v.Name,
                HeaderText = v.Name,
                ReadOnly = v.ReadOnly,
                ValueType = v.ValueType,
                CellTemplate = v.Choice.ToComboBoxCellTemplate(),
                Width = width,
                DropDownWidth = width,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = v.Format,
                },
            }
            );
        }
    }

}
