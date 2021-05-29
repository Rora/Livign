using System;

namespace TestProject1
{
    public class Person
    {
        private Car _car = new Car();
        private Gym _gym = new Gym();

        public void Workout()
        {
            _car.DriveTo("Gym");
            _gym.Workout(this);
        }
    }
}
