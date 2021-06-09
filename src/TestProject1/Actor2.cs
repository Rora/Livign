using System;

namespace TestProject1
{
    internal class Actor2
    {
        private Actor1 _actor1 = new();

        internal void EmptyMethod1()
        {
            throw new NotImplementedException();
        }

        internal void EmptyMethod2()
        {
            throw new NotImplementedException();
        }

        internal object MethodWithReturnValue()
        {
            throw new NotImplementedException();
        }

        internal static void StaticMethod()
        {
            throw new NotImplementedException();
        }

        internal void MethodThatRecursIn3Steps_2()
        {
            _actor1.MethodThatRecursIn3Steps_3();
        }

        internal void MethodThatRecursIn2Steps_2()
        {
            _actor1.MethodThatRecursIn2Steps_1();
            _actor1.EmptyCall();
        }

        internal void CalActor1Twice()
        {
            _actor1.EmptyCall();
            _actor1.EmptyCall();
        }
    }
}