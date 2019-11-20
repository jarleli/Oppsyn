using Moq;
using NUnit.Framework;
using SlackConnector;
using System;
using System.Collections.Generic;
using System.Text;
using Oppsyn;
using System.Threading.Tasks;
using System.Threading;

namespace Oppsyn.Tests
{
    [TestFixture,Category("Unit"),Ignore("bad idea")]
    public class ConnectionProviderTests
    {
        [Test]
        public void GettingAConnectionWithoutSettingOneTimesOut()
        {
            var provider = new SlackConnectionProvider();
            var indexOfTaskThTFinishes = Task.WaitAny(Task.Delay(50), provider.GetConnection());
            Assert.That(indexOfTaskThTFinishes, Is.EqualTo(0));
        }

        [Test]
        public void GettingAConnection()
        {
            var connection = new Mock<ISlackConnection>().Object;
            var provider = new SlackConnectionProvider();
            provider.SetSlackConnection(connection);
            var indexOfTaskThTFinishes = Task.WaitAny(Task.Delay(100), provider.GetConnection());
            Assert.That(indexOfTaskThTFinishes, Is.EqualTo(1));
        }

        [Test]
        public void AddingAConnectionToProviderReturnsTheTaskWaiting()
        {
            var connection = new Mock<ISlackConnection>().Object;
            var provider = new SlackConnectionProvider();
            var task = provider.GetConnection();
            Thread.Sleep(1);
            provider.SetSlackConnection(connection);
            Thread.Sleep(1);
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
            Assert.IsNotNull(provider.GetConnection().Result);
            Assert.IsNotNull(task.Result);

        }
    }

}
