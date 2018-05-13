namespace D365.SQL.Engine.Configuration
{
    using DML.Select;

    internal interface ISqlEngineConfiguration
    {
        ISqlEngineStatementConfiguration<SelectStatement> SelectStatementConfiguration { get; }
    }
}