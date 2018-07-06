using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region ChoiceColumns  ================================================
        private bool HasChoiceColumns(TableX tx)
        {
            if (TableX_ColumnX.TryGetChildren(tx, out IList<ColumnX> lst))
            { 
                foreach (var col in lst)
                {
                    if (col.IsChoice) return true;
                }
            }
            return false;
        }
        private bool TryGetChoiceColumns(TableX tx, out IList<ColumnX> columns)
        {
            if (!TableX_ColumnX.TryGetChildren(tx, out columns)) return false;

            var allColumns = columns;
            columns = new List<ColumnX>(allColumns.Count);

            foreach (var col in allColumns)
            {
                if (col.IsChoice) columns.Add(col);
            }
            return columns.Count > 0;
        }
        #endregion

        #region SetColumnValueType  ===========================================
        private bool SetColumnValueType(ColumnX col, int val)
        {
            if (val < 0 || val >= (int)ValType.MaximumType) return false;

            var type = (ValType)val;
            if (col.Value.ValType == type) return true;

            var newGroup = Value.GetValGroup(type);
            var preGroup = Value.GetValGroup(col.Value.ValType);

            if (!TableX_ColumnX.TryGetParent(col, out TableX tbl)) return false;

            var N = tbl.Count;

            if (N == 0)
            {
                col.Value = Value.Create(type);
                return true;
            }

            if ((newGroup & ValGroup.ScalarGroup) != 0 && (preGroup & ValGroup.ScalarGroup) != 0)
            {
                var rows = tbl.Items;
                var value = Value.Create(type, N);

                switch (newGroup)
                {
                    case ValGroup.Bool:
                        for (int i = 0; i < N; i++)
                        {
                            var key = rows[i];
                            col.Value.GetValue(key, out bool v);
                            if (!value.SetValue(key, v)) return false;
                        }
                        break;
                    case ValGroup.Long:
                        for (int i = 0; i < N; i++)
                        {
                            var key = rows[i];
                            col.Value.GetValue(key, out Int64 v);
                            if (!value.SetValue(key, v)) return false;
                        }
                        break;
                    case ValGroup.String:
                        for (int i = 0; i < N; i++)
                        {
                            var key = rows[i];
                            col.Value.GetValue(key, out string v);
                            if (!value.SetValue(key, v)) return false;
                        }
                        break;
                    case ValGroup.Double:
                        for (int i = 0; i < N; i++)
                        {
                            var key = rows[i];
                            col.Value.GetValue(key, out double v);
                            if (!value.SetValue(key, v)) return false;
                        }
                        break;
                    default:
                        return false;
                }
                col.Value = value;
                return true;
            }
            return false;
        }
        #endregion
    }
}
