using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Orienteer.Pages;

namespace Orienteer.Tests.Pages.ControllerLocator
{
    [TestClass]
    public class ControllerLocatorTests
    {
        public class Test1Controller : Controller
        {
        }

        public class Test2Controller : Controller
        {
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenAControllerThatIsntRegisterThenAnExceptionIsThrown()
        {
            var factory = Substitute.For<IControllerFactory>();
            var controllers = new IController[] { new Test1Controller(), new Test2Controller() };

            var locator = new Orienteer.Pages.ControllerLocator(factory, controllers);

            var controller = locator.Create("Test3");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GivenTwoControllersWithTheSamePrefixThenAnExceptionIsThrown()
        {
            var factory = Substitute.For<IControllerFactory>();
            var controllers = new IController[] { new Test1Controller(), new Test2Controller(), new ControllerLocator.Test2Controller()  };

            var locator = new Orienteer.Pages.ControllerLocator(factory, controllers);

            var controller = locator.Create("Test2");
        }

        [TestMethod]
        public void GivenAControllerThatIsRegisterThenThatControllerIsReturned()
        {
            var factory = Substitute.For<IControllerFactory>();
            var controllers = new IController[] { new Test1Controller(), new Test2Controller() };

            var locator = new Orienteer.Pages.ControllerLocator(factory, controllers);

            var controller = locator.Create("Test1");

            factory.Received(1).Create(typeof (Test1Controller));
        }
    }

    public class Test2Controller : Controller
    {
    }
}