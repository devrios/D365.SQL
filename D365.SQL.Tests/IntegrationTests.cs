using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace D365.SQL.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void Example()
        {
            var credential = new D365Credential("instanceName", "crm4", "username", "password");

            var engine = new SqlEngine(credential);

            var sql = @"
select top 5 c.firstname, lastname 'Last Name', emailaddress1 Email 
from contact as c 
where emailaddress1 is not null and firstname = 'abc' and emailaddress1 like'%@gmail.com'
order by c.firstname asc, lastname desc";

            var dsResults = engine.Execute(sql);
            
            Assert.IsNotNull(dsResults);
        }
    }
}
