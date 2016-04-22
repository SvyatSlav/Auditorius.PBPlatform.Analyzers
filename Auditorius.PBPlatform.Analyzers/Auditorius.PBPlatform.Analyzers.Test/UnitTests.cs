using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Auditorius.PBPlatform.Analyzers;

namespace Auditorius.PBPlatform.Analyzers.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void SimpleVerifyTest()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void Code_WithUtc_WithoutError()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.UtcNow;
            }                  
        }
    }";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void DiagnosticAndCodeFixTests()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.Now;
            }                  
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DomainDateTimeNewFormatAnalyzer.DiagnosticId,
                Message = DomainDateTimeNewFormatAnalyzer.MessageFormat.ToString(),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 30)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.UtcNow;
            }                  
        }
    }";
            VerifyCSharpFix(test, fixtest);
        }

        [TestMethod]
        public void Diagnostic_WithInnerField_ExpectedDiagnosticResult()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.Now.Date;
            }                  
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = DomainDateTimeNewFormatAnalyzer.DiagnosticId,
                Message = DomainDateTimeNewFormatAnalyzer.MessageFormat.ToString(),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[]
                    {
                        new DiagnosticResultLocation("Test0.cs", 14, 30)
                    }
            };

            VerifyCSharpDiagnostic(test, expected);
           
        }

        [TestMethod]
        public void TwiceDateTimeNow_FixAll_Expected2Utc()
        {
            var test = @"    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.Now;
var empty = String.Empty;
var Now = DateTime.Now;
            }                  
        }
    }";

            var fixtest = @"    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            public static void Run() {
                  var date = DateTime.UtcNow;
var empty = String.Empty;
var Now = DateTime.UtcNow;
            }                  
        }
    }";

            VerifyCSharpFix(test, fixtest);
        }


        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new DomainDateTimeNewFormatCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DomainDateTimeNewFormatAnalyzer();
        }
    }
}