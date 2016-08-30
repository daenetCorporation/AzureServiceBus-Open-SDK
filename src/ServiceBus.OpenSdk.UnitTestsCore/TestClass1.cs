using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
    public class TestClass1
    {
        public string PropertyOne { get; set; }
        public int PropertyTwo { get; set; }
        public TestClass1(string proOne, int ProTwo)
        {
            this.PropertyOne = proOne;
            this.PropertyTwo = ProTwo;
        }
        public TestClass1() { }
    }
}
