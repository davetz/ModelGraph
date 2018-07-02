using System;
using System.Collections.Generic;

namespace ModelGraphSTD
{/*

 */
    public partial class Chef
    {
        #region InitializeChoiceColumns  ======================================
        private void InitializeChoiceColumns()
        {
            foreach (var tbl in _tableXStore.Items) { ValidateTableChoiceColumns(tbl); }
        }
        private void ValidateTableChoiceColumns(TableX tbl)
        {
            var colList = TableX_ColumnX.GetChildren(tbl);
            if (colList != null)
            {
                foreach (var col in colList)
                {
                    if (col.IsChoice)
                    {
                        tbl.HasChoiceColumns = true;
                        return;
                    } 
                }
            }
            tbl.HasChoiceColumns = false;
        }
        #endregion

        #region ChoiceColumns  ================================================
        private bool HasChoiceColumns(TableX tx)
        {
            var columns = TableX_ColumnX.GetChildren(tx);
            if (columns != null)
            {
                foreach (var col in columns)
                {
                    if (col.IsChoice) return true;
                }
            }
            return false;
        }
        private List<ColumnX> GetChoiceColumns(TableX tx)
        {
            var columns = TableX_ColumnX.GetChildren(tx);
            if (columns == null) return null;

            var choiceColumns = new List<ColumnX>();
            foreach (var col in columns)
            {
                if (col.IsChoice) choiceColumns.Add(col);
            }
            return choiceColumns;
        }
        #endregion

        #region SetColumnIsChoice  ============================================
        private bool SetColumnIsChoice(ColumnX cx, bool value)
        {
            cx.IsChoice = value;

            var tbl = TableX_ColumnX.GetParent(cx);
            if (tbl != null) ValidateTableChoiceColumns(tbl);
            return true;
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

            var tbl = TableX_ColumnX.GetParent(col);
            if (tbl == null) return false;

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
