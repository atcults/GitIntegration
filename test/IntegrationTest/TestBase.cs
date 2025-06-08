using System;
using System.IO;
using GitIntegration;
using GitIntegration.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTest
{
    public class UnitTestCore : IDisposable
    {
        private readonly ILoggerFactory _logFactory;

        public UnitTestCore()
        {
            Console.WriteLine("Starting UnitTests");
            _logFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "HH:mm:ss ";
                });
            });
        }

        public void Dispose()
        {
            _logFactory.Dispose();
            Console.WriteLine("Disposing UnitTests");
        }

        public ILoggerFactory LogFactory => _logFactory;
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

        protected ILogger Logger => _fixture.LogFactory.CreateLogger("IntegrationTest");

        protected string RepositoryLocation
        {
            get
            {
                string baseDir = AppContext.BaseDirectory;
                var root = Path.GetFullPath(Path.Combine(baseDir, "../../../../.."));
                return root;
            }
        }
    }
}