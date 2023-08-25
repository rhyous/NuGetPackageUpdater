using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.NuGetPackageUpdater;
using Rhyous.SimpleArgs;
using Rhyous.UnitTesting;
using System.Threading.Tasks;

namespace Rhyous.NugetPacakgeUpdater.Tests.Business
{
    [TestClass]
    public class StarterTests
    {
        private MockRepository _MockRepository;

        private Mock<IArgsManager> _MockArgsManager;
        private Mock<IProgramRunner> _MockProgramRunner;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockArgsManager = _MockRepository.Create<IArgsManager>();
            _MockProgramRunner = _MockRepository.Create<IProgramRunner>();
        }

        private Starter CreateStarter()
        {
            return new Starter(_MockArgsManager.Object,
                               _MockProgramRunner.Object);
        }

        #region Start
        [TestMethod]
        public async Task Starter_Start_ArgsPassedToArgsManager_Test()
        {
            // Arrange
            var starter = CreateStarter();
            string[] args = null;
            _MockArgsManager.Setup(m => m.Start(args, null));
            _MockProgramRunner.Setup(m => m.Run()).Returns(Task.CompletedTask);

            // Act
            await starter.StartAsync(args);

            // Assert
            _MockRepository.VerifyAll();
        }
        #endregion


    }
}
