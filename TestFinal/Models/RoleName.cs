using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestFinal.Models
{
     public static class RoleName
    {

        //Rola admin pristup svim funkcionalnostima
        public const string Admin = "Admin";

        //Rola RWE(Read/Write/Execute), pregled podataka izmene podataka i stampanje
        public const string RWE = "RWE";

        //Rola RE(Read/Execute) pregled podataka i stampanje
        public const string RE = "RE";        

    }
}