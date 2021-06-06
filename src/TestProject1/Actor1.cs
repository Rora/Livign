using System;

namespace TestProject1
{
    public class Actor1
    {
        private readonly Actor2 _actor2 = new Actor2();

        internal void TwoDifferentCallsToOtherActor()
        {
            _actor2.EmptyMethod1();
            _actor2.EmptyMethod2();
        }

        internal void OneCallToOtherActorViaPrivateMethod()
        {
            OneCallToOtherActorViaPrivateMethod_Private();
        }

        private void OneCallToOtherActorViaPrivateMethod_Private()
        {
            _actor2.EmptyMethod1();
        }
        public void OneCallToOtherActorWithAssignment()
        { 
            var result = _actor2.MethodWithReturnValue();
        }

        public void OneStaticCallToOtherActor()
        {
            TestProject1.Actor2.StaticMethod();
        }
        public void OneCallToAClassFromAssemblyWithoutSymbols()
        {  
            Newtonsoft.Json.JsonConvert.DeserializeObject("{}");
        }
        public void SelfCallingRecursiveMethod()
        {
            this.SelfCallingRecursiveMethod();
        }
        public void MethodThatRecursIn3Steps_1()
        {
            _actor2.MethodThatRecursIn3Steps_2();
        }

        internal void MethodThatRecursIn3Steps_3()
        {
            MethodThatRecursIn3Steps_1();
        }

    }
}
