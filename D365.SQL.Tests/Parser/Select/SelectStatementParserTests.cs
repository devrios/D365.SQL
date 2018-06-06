using System.Linq;
using D365.SQL.Common;

namespace D365.SQL.Tests.Parser.Select
{
    using DML.Select;
    using DML.Select.Columns;
    using DML.Select.From;
    using Engine.Configuration;
    using Engine.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SelectStatementParserTests 
    {
        [TestMethod]
        public void SimpleSelect()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select * from table";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(1, selectStatement.Columns.Count);
            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(AllSelectColumn));

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("table", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.IsNull(selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }

        [TestMethod]
        public void SelectWithTop()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select top 5 * from table";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(1, selectStatement.Columns.Count); 
            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(AllSelectColumn));
            Assert.AreEqual(5, selectStatement.Top.Value);

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("table", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.IsNull(selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }

        [TestMethod]
        public void SelectMultipleColumnsWithTop()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select top 10 [firstname], lastname from contact";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(10, selectStatement.Top.Value);

            Assert.AreEqual(2, selectStatement.Columns.Count);
            
            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("firstname", selectStatement.Columns[0].Column.As<FieldSelectColumn>().Name);

            Assert.IsInstanceOfType(selectStatement.Columns[1].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("lastname", selectStatement.Columns[1].Column.As<FieldSelectColumn>().Name);

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("contact", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.IsNull(selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }

        [TestMethod]
        public void SelectMultipleColumnsWithAlias()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select a.[firstname], a.lastname, a.fullname as [Name], fullname from contact a";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(4, selectStatement.Columns.Count);

            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("firstname", selectStatement.Columns[0].Column.As<FieldSelectColumn>().Name);
            Assert.AreEqual("a", selectStatement.Columns[0].Column.As<FieldSelectColumn>().Alias);
            Assert.IsNull(selectStatement.Columns[0].Label);

            Assert.IsInstanceOfType(selectStatement.Columns[1].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("lastname", selectStatement.Columns[1].Column.As<FieldSelectColumn>().Name);
            Assert.AreEqual("a", selectStatement.Columns[1].Column.As<FieldSelectColumn>().Alias);
            Assert.IsNull(selectStatement.Columns[1].Label);

            Assert.IsInstanceOfType(selectStatement.Columns[2].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("fullname", selectStatement.Columns[2].Column.As<FieldSelectColumn>().Name);
            Assert.AreEqual("a", selectStatement.Columns[2].Column.As<FieldSelectColumn>().Alias);
            Assert.AreEqual("Name", selectStatement.Columns[2].Label);

            Assert.IsInstanceOfType(selectStatement.Columns[3].Column, typeof(FieldSelectColumn));
            Assert.AreEqual("fullname", selectStatement.Columns[3].Column.As<FieldSelectColumn>().Name);
            Assert.IsNull(selectStatement.Columns[3].Column.As<FieldSelectColumn>().Alias);
            Assert.IsNull(selectStatement.Columns[3].Label);

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("contact", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.AreEqual("a", selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }

        [TestMethod]
        public void SelectFromSimple()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select * from [table] a";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(1, selectStatement.Columns.Count);
            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(AllSelectColumn));

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("table", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.AreEqual("a", selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }

        [TestMethod]
        public void SelectFromSimple2()
        {
            var parserManager = new ParserManager(new SqlEngineConfiguration());

            var sql = "select * from [table] as a";

            var result = parserManager.ParseStatements(sql);

            Assert.IsFalse(result.Errors.Any());

            var statement = result.Value;

            Assert.AreEqual(1, statement.Count); // 1 statement
            Assert.IsInstanceOfType(statement.First(), typeof(SelectStatement));

            var selectStatement = statement.First().As<SelectStatement>();

            Assert.AreEqual(1, selectStatement.Columns.Count);
            Assert.IsInstanceOfType(selectStatement.Columns[0].Column, typeof(AllSelectColumn));

            Assert.AreEqual(1, selectStatement.From.Count);
            Assert.IsInstanceOfType(selectStatement.From.First(), typeof(SelectFrom));
            Assert.AreEqual("table", selectStatement.From.First().As<SelectFrom>().Name);
            Assert.AreEqual("a", selectStatement.From.First().As<SelectFrom>().Alias);
            Assert.AreEqual(SelectFromTypeEnum.Table, selectStatement.From.First().As<SelectFrom>().Type);
        }
    }
}
