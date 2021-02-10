using System;
using Cake.Core.IO;
using Cake.Core;
using System.Collections.Generic;
using Cake.Core.Tooling;

namespace Cake.ExtendedNuGet.Tests.Fakes
{
    public class FakeCakeContext
    {
        ICakeContext context;
        FakeLog log;
        DirectoryPath testsDir;

        public FakeCakeContext ()
        {
            testsDir = new DirectoryPath (
                System.IO.Path.GetFullPath (AppDomain.CurrentDomain.BaseDirectory));

            var fileSystem = new FileSystem ();
            var environment = new Cake.Testing.FakeEnvironment(PlatformFamily.Windows);
            var globber = new Globber (fileSystem, environment);
            log = new FakeLog ();
            var args = new FakeCakeArguments ();
            var registry = new WindowsRegistry ();
            var toolRepo = new ToolRepository(environment);
            var config = new Core.Configuration.CakeConfigurationProvider(fileSystem, environment).CreateConfiguration(testsDir, new Dictionary<string, string>());
            var toolResolutionStrategy = new ToolResolutionStrategy(fileSystem, environment, globber, config, log);
            var toolLocator = new ToolLocator(environment, toolRepo, toolResolutionStrategy);
            var processRunner = new ProcessRunner(fileSystem, environment, log, toolLocator, config);
            var dataService = new FakeCakeDataService();
            context = new CakeContext(fileSystem, environment, globber, log, args, processRunner, registry, toolLocator, dataService, config);
            context.Environment.WorkingDirectory = testsDir;
        }

        public DirectoryPath WorkingDirectory {
            get { return testsDir; }
        }
            
        public ICakeContext CakeContext {
            get { return context; }
        }

        public string GetLogs ()
        {
            return string.Join(Environment.NewLine, log.Messages);
        }

        public void DumpLogs ()
        {
            foreach (var m in log.Messages)
                Console.WriteLine (m);
        }
    }
}

