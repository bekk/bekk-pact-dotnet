using System;
using Bekk.Pact.Provider.Web.Setup;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bekk.Pact.Provider.Web.Tests.Setup
{
    public class ProviderStateSetupBaseTests
    {
        [Fact]
        public void ConfigureServices_WithKey_CallsPrivateStaticMethodWithAttributeAndReturnValue()
        {
            var target = new TargetClass();
            target.ConfigureServices("Abc")(null);

            Assert.True(TargetClass.AbcWasCalled);
        }

        [Fact]
        public void ConfigureServices_WithKey_CallsProtectedInstanceMethodWithAttributeAndReturnValue()
        {
            var target = new TargetClass();
            target.ConfigureServices("Def")(null);

            Assert.True(target.DefWasCalled);
        }
        [Fact]
        public void ConfigureServices_WithKeyForMultipleMethods_AllCallbacksAreCalled()
        {
            var target = new TargetClass();
            target.ConfigureServices("Ghi")(null);

            Assert.True(target.Ghi0WasCalled, "Method with attribute");
            Assert.True(target.Ghi1WasCalled, "Method with matching name");
        }

        [Fact]
        public void ConfigureServices_WithKeyForMethodWithoutCallback_IsCalled()
        {
            var target = new TargetClass();
            target.ConfigureServices("Jkl")(null);

            Assert.True(target.JklWasCalled);
        }

        private class TargetClass : ProviderStateSetupBase
        {
            public static bool AbcWasCalled;
            [ProviderState("Abc")]
            private static Action<IServiceCollection> AbcMethod()
            {
                AbcWasCalled = true;
                return svc => {};
            }
            public bool DefWasCalled;
            [ProviderState("Def")]
            protected Action<IServiceCollection> DefMethod()
            {
                DefWasCalled = true;
                return svc => {};
            }
            public bool Ghi0WasCalled;
            [ProviderState("Ghi")]
            public Action<IServiceCollection> Ghi0Method()
            {
                return svc => {Ghi0WasCalled = true;};
            }
            public bool Ghi1WasCalled;
            public Action<IServiceCollection> Ghi()
            {
                return svc => {Ghi1WasCalled = true;};
            }
            public bool JklWasCalled;
            [ProviderState("Jkl")]
            public void JklMethod(IServiceCollection svc)
            {
                JklWasCalled = true;
            }
        }
    }
}