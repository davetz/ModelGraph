using System.Collections.Generic;

namespace ModelGraphLibrary
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
        private bool TryGetChoiceColumns(Item tbl, out ColumnX[] columns)
        {
            var colList = TableX_ColumnX.GetChildren(tbl);
            if (colList != null)
            {
                var choiceColumns = new List<ColumnX>();
                foreach (var col in colList)
                {
                    if (col.IsChoice) choiceColumns.Add(col);
                }
                columns = choiceColumns.ToArray();
                return choiceColumns.Count > 0;
            }
            columns = null;
            return false;
        }
        #endregion


    }
}
