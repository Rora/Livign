using System;

namespace TestProject1
{
    public class Car
    {
        private Engine _engine = new Engine();

        internal void DriveTo(string destination)
        {
            _engine.Start();
            _engine.Stop();
        }
    }
}
