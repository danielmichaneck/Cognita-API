using Cognita_API;
using IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cognita_Tests;

[CollectionDefinition("DbCollection", DisableParallelization = true)]
public class DbCollectionFixture : ICollectionFixture<CustomWebApplicationFactory>
{
}
}
