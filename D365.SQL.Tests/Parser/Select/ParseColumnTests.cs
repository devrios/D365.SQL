namespace D365.SQL.Tests.Parser.Select
{
    using System;
    using System.Linq;
    using Common;
    using DML.Select.Columns;
    using Engine.Configuration;
    using Engine.Parsers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParseRawColumnTests
    {
        [TestMethod]
        public void StringParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'abc' as Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(string), result.Value.Column.ValueType);

            Assert.AreEqual("abc", result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void IntegerParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123 as Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(int), result.Value.Column.ValueType);

            Assert.AreEqual(123, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DoubleParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123.456 as Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(double), result.Value.Column.ValueType);

            Assert.AreEqual(123.456, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DateParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31' as Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(DateTime), result.Value.Column.ValueType);

            Assert.AreEqual(new DateTime(2015, 12, 31), result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void StringParsingWithLabelNoAs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'abc' Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(string), result.Value.Column.ValueType);

            Assert.AreEqual("abc", result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void IntegerParsingWithLabelNoAs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123 Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(int), result.Value.Column.ValueType);

            Assert.AreEqual(123, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DoubleParsingWithLabelNoAs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123.456 Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(double), result.Value.Column.ValueType);

            Assert.AreEqual(123.456, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DateParsingWithLabelNoAs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31' Col";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("Col", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(DateTime), result.Value.Column.ValueType);

            Assert.AreEqual(new DateTime(2015, 12, 31), result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void StringParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'abc'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(string), result.Value.Column.ValueType);

            Assert.AreEqual("abc", result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void IntegerParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(int), result.Value.Column.ValueType);

            Assert.AreEqual(123, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DoubleParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "123.456";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(double), result.Value.Column.ValueType);

            Assert.AreEqual(123.456, result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DateParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(DateTime), result.Value.Column.ValueType);

            Assert.AreEqual(new DateTime(2015, 12, 31), result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DateTimeParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31 12:59'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(DateTime), result.Value.Column.ValueType);

            Assert.AreEqual(new DateTime(2015, 12, 31, 12, 59, 0), result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void DateTimeSecondsParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31 12:59:55'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(DateTime), result.Value.Column.ValueType);

            Assert.AreEqual(new DateTime(2015, 12, 31, 12, 59, 55), result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void IncorrectDateTimeNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31a 12:59:55'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(string), result.Value.Column.ValueType);

            Assert.AreEqual("2015-12-31a 12:59:55", result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void IncorrectDateTime2NoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "'2015-12-31 12:90:55'";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Raw, result.Value.Column.Type);
            Assert.AreEqual(typeof(string), result.Value.Column.ValueType);

            Assert.AreEqual("2015-12-31 12:90:55", result.Value.Column.As<RawSelectColumn>().Value);
        }

        [TestMethod]
        public void StarParsing()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "*";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.All, result.Value.Column.Type);
        }

        [TestMethod]
        public void StarParsingExtraArgs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "* abc";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsTrue(result.Errors.Any());
        }

        [TestMethod]
        public void SystemColumnsParsing()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "+";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.System, result.Value.Column.Type);
        }

        [TestMethod]
        public void SystemColumnsParsingExtraArgs()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "+ abc";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsTrue(result.Errors.Any());
        }

        [TestMethod]
        public void FieldColumnParsing()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "field";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Field, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            Assert.AreEqual("field", result.Value.Column.As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void FieldColumnParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "field label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Field, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            Assert.AreEqual("field", result.Value.Column.As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void FieldColumnParsingWithLabel2()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "field As label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Field, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            Assert.AreEqual("field", result.Value.Column.As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void FunctionColumnParsingNoLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "upper(field)";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.IsNull(result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Function, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            var function = result.Value.Column.As<FunctionSelectColumn>();

            Assert.AreEqual("upper", function.Name);
            Assert.IsNotNull(function.Args);
            Assert.AreEqual(1, function.Args.Count);
            Assert.IsInstanceOfType(function.Args[0], typeof(FieldSelectColumn));
            Assert.AreEqual("field", function.Args[0].As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void FunctionColumnParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "upper(field) label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Function, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            var function = result.Value.Column.As<FunctionSelectColumn>();

            Assert.AreEqual("upper", function.Name);
            Assert.IsNotNull(function.Args);
            Assert.AreEqual(1, function.Args.Count);
            Assert.IsInstanceOfType(function.Args[0], typeof(FieldSelectColumn));
            Assert.AreEqual("field", function.Args[0].As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void FunctionColumnParsingWithLabel2()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "upper(field) As label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Function, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            var function = result.Value.Column.As<FunctionSelectColumn>();

            Assert.AreEqual("upper", function.Name);
            Assert.IsNotNull(function.Args);
            Assert.AreEqual(1, function.Args.Count);
            Assert.IsInstanceOfType(function.Args[0], typeof(FieldSelectColumn));
            Assert.AreEqual("field", function.Args[0].As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void NestedFunctionColumnParsingWithLabel()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "upper(lower(field)) As label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.Function, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            var function = result.Value.Column.As<FunctionSelectColumn>();

            Assert.AreEqual("upper", function.Name);
            Assert.IsNotNull(function.Args);
            Assert.AreEqual(1, function.Args.Count);
            Assert.IsInstanceOfType(function.Args[0], typeof(FunctionSelectColumn));
            
            var nestedFunction = function.Args[0].As<FunctionSelectColumn>();

            Assert.AreEqual("lower", nestedFunction.Name);
            Assert.IsNotNull(nestedFunction.Args);
            Assert.AreEqual(1, nestedFunction.Args.Count);
            Assert.IsInstanceOfType(nestedFunction.Args[0], typeof(FieldSelectColumn));
            Assert.AreEqual("field", nestedFunction.Args[0].As<FieldSelectColumn>().Name);
        }

        [TestMethod]
        public void InnerSelectColumnParsingWithLabel2()
        {
            var statementParser = new ParseStatements(new SqlEngineConfiguration());
            var engine = new ParserEngine(statementParser);

            var sql = "(select field from table) As label";
            var words = ParserUtils.GetWords(sql).ToList();
            var columnList = ParserUtils.SplitTokens(words).ToList();

            var result = engine.ParseSelectColumn(columnList.First());

            Assert.IsFalse(result.Errors.Any(), "Parsing errors found");
            Assert.AreEqual("label", result.Value.Label);
            Assert.AreEqual(SelectColumnTypeEnum.InnerSelect, result.Value.Column.Type);
            Assert.IsInstanceOfType(result.Value.Column.ValueType, typeof(object));

            var function = result.Value.Column.As<InnerSelectColumn>();

            Assert.IsNotNull(function.SelectStatement);
            Assert.AreEqual(1, function.SelectStatement.Columns.Count);
        }
    }
}
