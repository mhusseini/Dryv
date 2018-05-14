using System;
using Dryv.Demo.Models;

namespace Dryv.Demo
{
    internal class Program
    {
        private static void Main()
        {
            var model = new Model5
            {
                Name = "Hello",
                Child = new Model6
                {
                    Name = "World",
                    Child = new Model7()
                },
                Children = new[]
                {
                    new Model8()
                }
            };

            var validator = new DryvValidator();
            var errors = validator.Validate(model);

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}