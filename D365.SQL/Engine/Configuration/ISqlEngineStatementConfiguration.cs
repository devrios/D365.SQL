namespace D365.SQL.Engine.Configuration
{
    using System.Collections.Generic;
    using Parsers;

    internal interface ISqlEngineStatementConfiguration<T>
        where T : IStatement
    {
        IEnumerable<IStatementTokenParser<T>> Parsers();

        IEnumerable<SqlStatementRule> Rules();
    }
}