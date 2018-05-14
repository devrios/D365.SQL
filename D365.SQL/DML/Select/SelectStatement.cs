namespace D365.SQL.DML.Select
{
    using System.Collections.Generic;
    using Columns;
    using Engine.Parsers;
    using From;
    using Joins;
    using Order;
    using Where;

    internal class SelectStatement : IStatement
    {
        public SelectStatement()
        {
            Columns = new List<SelectColumnBase>();
            From = new List<SelectFromBase>();
            Joins = new List<SelectJoinBase>();
            Where = new List<SelectWhereBase>();
            Order = new List<SelectOrderBase>();
        }

        public int? Top { get; set; }

        public List<SelectColumnBase> Columns { get; set; }

        public List<SelectFromBase> From { get; set; }

        public List<SelectJoinBase> Joins { get; set; }

        public List<SelectWhereBase> Where { get; set; }

        public List<SelectOrderBase> Order { get; set; }
    }
}