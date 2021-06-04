using System;

namespace TestProject1
{
    public class Person
    {
        private readonly Car _car = new();
        private readonly Gym _gym = new();

        public void Workout()
        {
            _car.DriveTo("Gym");
            _gym.Workout(this);
        }
    }
}
