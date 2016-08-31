//=======================================================================================
// Copyright © daenet GmbH Frankfurt am Main
//
// LICENSED UNDER THE APACHE LICENSE, VERSION 2.0 (THE "LICENSE"); YOU MAY NOT USE THESE
// FILES EXCEPT IN COMPLIANCE WITH THE LICENSE. YOU MAY OBTAIN A COPY OF THE LICENSE AT
// http://www.apache.org/licenses/LICENSE-2.0
// UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING, SOFTWARE DISTRIBUTED UNDER THE
// LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, EITHER EXPRESS OR IMPLIED. SEE THE LICENSE FOR THE SPECIFIC LANGUAGE GOVERNING
// PERMISSIONS AND LIMITATIONS UNDER THE LICENSE.
//=======================================================================================

using System.Runtime.Serialization;

namespace ServiceBus.OpenSdk.UnitTestsCore
{
   [DataContract(Name ="TestClass", Namespace ="ServiceBus.OpenSdk")]
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