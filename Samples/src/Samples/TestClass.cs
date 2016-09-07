using System.Runtime.Serialization;

namespace Samples
{
    [DataContract(Name = "TestClass", Namespace = "ServiceBus.OpenSdk")]
    public class TestClass
    {
        [DataMember]
        public string PropertyOne { get; set; }
        [DataMember]
        public int PropertyTwo { get; set; }
        public TestClass(string propertyOne, int propertyTwo)
        {
            this.PropertyOne = propertyOne;
            this.PropertyTwo = propertyTwo;
        }
        public TestClass() { }
    }
}