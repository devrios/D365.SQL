namespace D365.SQL.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data;
    using DML.Select.Columns;
    using Engine.Storage.InMemory;

    internal static class DataExtensions
    {
        public static void MoveItemsToStart<T, K>(this List<T> list, Func<T, K, bool> compareFunc, params K[] orderedItems)
            where K : class
        {
            var moveIndex = 0;
            var indexes = new int[orderedItems.Length];

            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = -1;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                for (int j = 0; j < orderedItems.Length; j++)
                {
                    var orItem = orderedItems[j];

                    if (compareFunc(item, orItem))
                    {
                        indexes[j] = i;
                    }
                }
            }

            for (int i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];

                if (index == -1)
                {
                    continue;
                }

                if (index == moveIndex - 1) continue;

                var temp = list[index];
                list.RemoveAt(index);
                list.Insert(moveIndex++, temp);

                for (int j = 0; j < indexes.Length; j++)
                {
                    if (indexes[j] < indexes[i])
                    {
                        indexes[j]++;
                    }
                }
            }
        }

        public static void MoveItemsToEnd<T, K>(this List<T> list, Func<T, K> expressionFunc, List<K> orderedItems)
            where K : class
        {
            var indexes = new int[orderedItems.Count];

            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = -1;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                for (int j = 0; j < orderedItems.Count; j++)
                {
                    var orItem = orderedItems[j];

                    if (expressionFunc(item) == orItem)
                    {
                        indexes[j] = i;
                    }
                }
            }

            for (int i = 0; i < indexes.Length; i++)
            {
                var index = indexes[i];

                if (index == -1)
                {
                    continue;
                }

                var temp = list[index];
                list.RemoveAt(index);
                list.Add(temp);

                for (int j = 0; j < indexes.Length; j++)
                {
                    if (indexes[j] > indexes[i])
                    {
                        indexes[j]--;
                    }
                }
            }
        }
        
        public static DataTable ConvertToDataTable(this InMemoryTable table)
        {
            return ConvertToDataTable(table, null);
        }
        
        public static DataTable ConvertToDataTable(this InMemoryTable table, HashSet<string> columns)
        {
            var dt = new DataTable();

            foreach (var column in table.Columns)
            {
                if (column.IsVisible == false)
                {
                    continue;
                }

                if (columns == null || (columns.Contains(column.Name)))
                {
                    var dataColumn = dt.Columns.Add(column.Name, column.Type);

                    dataColumn.Caption = column.Name;
                }
            }

            foreach (var row in table.Rows)
            {
                var dataRow = dt.NewRow();

                foreach (var column in row.Columns)
                {
                    if (dt.Columns.Contains(column.Key))
                    {
                        dataRow[column.Key] = column.Value;
                    }
                }
                
                dt.Rows.Add(dataRow);
            }

            return dt;
        }

        public static DataTable ConvertToDataTable(this List<List<KeyValuePair<string, object>>> crmDataItems, List<SelectColumnBase> columns)
        {
            var dt = new DataTable();

            foreach (var column in columns)
            {
                if (column.Type.In(SelectColumnTypeEnum.All, SelectColumnTypeEnum.System))
                {

                }
                else
                {
                    var name = column.Label;

                    if (column.Type == SelectColumnTypeEnum.Field)
                    {
                        var fieldColumn = (FieldSelectColumn)column;

                        name = fieldColumn.Name;
                    }

                    var dataColumn = dt.Columns.Add(name, column.ValueType);

                    dataColumn.Caption = column.Label;
                }
            }

            var rawColumns = columns.Where(x => x.Type == SelectColumnTypeEnum.Raw).Cast<RawSelectColumn>();

            foreach (var crmDataItem in crmDataItems)
            {
                var row = dt.NewRow();

                foreach (var crmPair in crmDataItem)
                {
                    if (dt.Columns.Contains(crmPair.Key))
                    {
                        row[crmPair.Key] = crmPair.Value;
                    }
                }

                if (rawColumns.Any())
                {
                    foreach (var rawColumn in rawColumns)
                    {
                        var value = rawColumn.Value;

                        row[rawColumn.Label] = value;
                    }
                }

                dt.Rows.Add(row);
            }

            // caption is broken in linqpad so we are always using label as column name
            foreach (DataColumn dataColumn in dt.Columns)
            {
                if (dataColumn.Caption.IsNotEmpty())
                {
                    if (dataColumn.Caption.IsQuoted())
                    {
                        dataColumn.Caption = dataColumn.Caption.CleanRaw();
                    }

                    dataColumn.ColumnName = dataColumn.Caption;
                }
            }

            return dt;
        }
    }
}
