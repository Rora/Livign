using System;

namespace TestProject1
{
    public class Actor1
    {
        private readonly Actor2 _actor2 = new();

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
    }
}
