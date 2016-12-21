using System;
using System.Diagnostics;
using System.Reflection;
using Xunit.Sdk;

namespace IntegrationTest
{
    public class MethodNameToConsoleAttribute : BeforeAfterTestAttribute
    {
        private Stopwatch watch;
        public override void Before(MethodInfo methodUnderTest)
        {
            watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"Starting : {methodUnderTest.Name}");
        }

        public override void After(MethodInfo methodUnderTest)
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"{methodUnderTest.Name} Completed in {elapsedMs}ms");
        }
    }
}