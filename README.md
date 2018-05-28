# D365.SQL

### SQL for Dynamics 365

This project aims at providing full support of the SQL language to interact with any Dynamics 365 instance by leveraging the standard Dynamics 365 SDK.

The library can leverage fetch XML where possible but implements some magic when not so that the SQL features are not compromised due to fetch xml limitations.

*Example*

```c#

// Dynamics 365 Url: https://instance.crm4.dynamics.com

var credential = new D365Credential("instance", "crm4", "username@company.onmicrosoft.com", "password");

var engine = new SqlEngine(credential);

var sql = @"
select top 5 c.firstname, c.lastname 'Last Name', c.emailaddress1 As Email 
from contact as c 
where emailaddress1 is not null and firstname = 'abc' and emailaddress1 like'%@gmail.com'
order by c.firstname asc, lastname desc";

var dataSet = engine.Execute(sql);

Console.WriteLine(dataSet);
```

***Note**: This is still a work in progress and not ready for use.*
