using System;
using GitIntegration;
using GitIntegration.Extensions;
using NLog;
using Xunit;

namespace IntegrationTest
{
    public class UnitTestCore : IDisposable
    {
        public UnitTestCore()
        {
            Console.WriteLine("Starting UnitTests");
            LogHelper.AddLogger("IntegrationTest");
        }

        public void Dispose()
        {
            LogManager.Flush();
            LogManager.Shutdown();
            Console.WriteLine("Disposing UnitTests");
        }
    }

    [CollectionDefinition("UnitTest")]
    public class UnitTestCollection : ICollectionFixture<UnitTestCore>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    [MethodNameToConsole]
    [Collection("UnitTest")]
    public class UnitTestBase : IDisposable
    {
        protected readonly UnitTestCore _fixture;

        public UnitTestBase(UnitTestCore fixture)
        {
            _fixture = fixture;
            ResetSystemTime();
        }

        public void Dispose()
        {

        }

        protected readonly DateTime StartDay = SystemTime.Now().Date.AddMonths(-1);
        protected readonly DateTime EndDay = SystemTime.Now().Date.AddMonths(2);
        protected readonly DateTime Today = SystemTime.Now().Date;
        protected void StubDateTime(DateTime now)
        {
            SystemTime.Now = () => now;
        }

        public static void ResetSystemTime()
        {
            SystemTime.Now = () => DateTime.Now;
        }

        protected ILogger Logger
        {
            get
            {
                return LogManager.GetLogger("IntegrationTest");
            }
        }

        protected string RepositoryLocation
        {
            get
            {
                return "c:/code/GitIntegration";
            }
        }
    }
}