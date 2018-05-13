namespace D365.SQL.DML.Select.Where
{
    using System.Collections.Generic;

    internal class SelectWhereGroup : SelectWhereBase
    {
        public SelectWhereGroup()
        {
            Clauses = new List<SelectWhereBase>();
        }

        public List<SelectWhereBase> Clauses { get; set; }

        public override SelectWhereTypeEnum Type => SelectWhereTypeEnum.Group;
    }
}