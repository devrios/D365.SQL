namespace D365.SQL.Engine.Configuration
{
    using DML.Select;

    internal class SqlEngineConfiguration : ISqlEngineConfiguration
    {
        public SqlEngineSettings Settings { get; set; } = new SqlEngineSettings();

        public ISqlEngineStatementConfiguration<SelectStatement> SelectStatementConfiguration { get; } = new SqlEngineStatementConfiguration();
    }
}