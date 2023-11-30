using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace Service.AddressBook.Tests
{
    public class TestExample
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Console.WriteLine("Debug output");
            ClassicAssert.Pass();
        }
    }
}
