using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        [Fact]
        public void ConfigureServices_WithMultipleKeysAndMethodDecoratedWithBoth_MethodIsCalledForBoth()
        {
            var target = new TargetClass();
            target.ConfigureServices("Mno")(null);
            target.ConfigureServices("Pqr")(null);

            Assert.Equal(2, target.MnoPqrWasCalled);
        }

        [Fact] 
        void GetClaims_WithKey_ReturnsClaimsFromAllDecoratedMethods()
        {
            var target = new TargetClass();

            var result = target.GetClaims("Abc").OrderBy(c => c.Value);

            Assert.Collection(result,
                claim => Assert.Equal("0", claim.Value),
                claim => Assert.Equal("1", claim.Value)                
            );
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
            public int MnoPqrWasCalled;
            [ProviderState("Mno")]
            [ProviderState("Pqr")]
            public void MnoPqrMethod(IServiceCollection svc)
            {
                MnoPqrWasCalled++;
            }

            [ProviderState("Abc")]
            public IEnumerable<Claim> Abc0ClaimsMethod()
            {
                yield return new Claim("Abc", "0");
            }
            [ProviderState("Abc")]
            public IEnumerable<Claim> Abc1ClaimsMethod()
            {
                yield return new Claim("Abc", "1");
            }
            [ProviderState("Def")]
            public IEnumerable<Claim> Def1ClaimsMethod()
            {
                yield return new Claim("Def", "0");
            }
        }
    }
}