using System;

namespace TestProject1
{
    public class Car
    {
        private readonly Engine _engine = new();

        internal void DriveTo(string destination)
        {   
            _engine.Start();
            _engine.Stop();
        }
    }
}
