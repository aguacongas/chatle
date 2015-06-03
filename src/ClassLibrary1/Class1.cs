using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ClassLibrary1
{
    public class User:IdentityUser
    {

    }
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1: IdentityDbContext<User>
    {
        public Class1()
        {
        }
    }
}
