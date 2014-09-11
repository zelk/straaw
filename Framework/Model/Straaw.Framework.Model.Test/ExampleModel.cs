using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Straaw.Framework.Model;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Straaw.Framework.Model.Test
{
    [DataContract(Name = "TestModel1")]
    public class TestModel1 : ImmutableModel<TestModel1, MutableTestModel1>
    {
        // Non-DataMember
        public string ShouldNotBeTouched { get; private set; }

        // Value Types
        public bool TestBool { get; private set; }
        [DataMember] public int TestInt { get; private set; }
        public long TestLong { get; private set; }
        public float TestFloat { get; private set; }
        public double TestDouble { get; private set; }
        public decimal TestDecimal { get; private set; }

        // Semi-Value Types
        public string TestStringObject { get; private set; }
        public Boolean TestBooleanObject { get; private set; }
        public Double TestDoubleObject { get; private set; }
        public Decimal TestDecimalObject { get; private set; }
        public DateTime TestDateTimeObject { get; private set; }

        // TODO: Find out how much int testing I need to do!!?!??
        public UInt64 TestUint64Object { get; private set; }

        // Reference Types
        public TestModel1 TestInternalReference { get; private set; }
        public TestModel2 TestExternalReference { get; private set; }

        // Lists of Value Types
        // TODO: Find out how much value type lists testing I need to do!!?!??
        public ReadOnlyCollection<int> TestIntList { get; private set; }

        // Lists of Semi-Value Types
        // TODO: Find out how much semi-value type lists testing I need to do!!?!??
        public ReadOnlyCollection<string> TestStringList { get; private set; }
        public ReadOnlyCollection<DateTime> TestDateTimeList { get; private set; }

        // Lists of Reference Types
        public ReadOnlyCollection<TestModel1> TestInternalReferenceList { get; private set; }
        public ReadOnlyCollection<TestModel2> TestExternalReferenceList { get; private set; }

        public TestModel1(MutableTestModel1 mutableTestModel1)
        {
            TestBool = mutableTestModel1.TestBool;
            TestInt = mutableTestModel1.TestInt;
            TestLong = mutableTestModel1.TestLong;
            TestFloat = mutableTestModel1.TestFloat;
            TestDouble = mutableTestModel1.TestDouble;
            TestDecimal = mutableTestModel1.TestDecimal;

			// TODO: Implement the rest of the method!!!
        }

        public override MutableTestModel1 ToMutable()
        {
            return new MutableTestModel1(this);
        }
    }

    [DataContract(Name = "TestModel1")]
    public class MutableTestModel1 : MutableModel<TestModel1, MutableTestModel1>
    {
        // Non-DataMember
        public string ShouldNotBeTouched { get; set; }

        // Value Types
        [DataMember(Name = "testBool")]
        public bool TestBool { get; set; }
        [DataMember(Name = "testInt")]
        public int TestInt { get; set; }
        [DataMember(Name = "testLong")]
        public long TestLong { get; set; }
        [DataMember(Name = "testFloat")]
        public float TestFloat { get; set; }
        [DataMember(Name = "testDouble")]
        public double TestDouble { get; set; }
        [DataMember(Name = "testDecimal")]
        public decimal TestDecimal { get; set; }

        // Semi-Value Types
        [DataMember(Name = "testStringObject")]
        public string TestStringObject { get; set; }
        [DataMember(Name = "testBooleanObject")]
        public Boolean TestBooleanObject { get; set; }
        [DataMember(Name = "testDoubleObject")]
        public Double TestDoubleObject { get; set; }
        [DataMember(Name = "testDecimalObject")]
        public Decimal TestDecimalObject { get; set; }
        [DataMember(Name = "testDateTimeObject")]
        public DateTime TestDateTimeObject { get; set; }

        // TODO: Find out how much int testing I need to do!!?!??
        [DataMember(Name = "testUint64Object")]
        public UInt64 TestUint64Object { get; set; }

        // Reference Types
        [DataMember(Name = "testInternalReference")]
        public MutableTestModel1 TestInternalReference { get; set; }
        [DataMember(Name = "testExternalReference")]
        public MutableTestModel2 TestExternalReference { get; set; }

        // Lists of Value Types
        // TODO: Find out how much value type lists testing I need to do!!?!??
        [DataMember(Name = "testIntList")]
        public List<int> TestIntList { get; set; }

        // Lists of Semi-Value Types
        // TODO: Find out how much semi-value type lists testing I need to do!!?!??
        [DataMember(Name = "testStringList")]
        public List<string> TestStringList { get; set; }
        [DataMember(Name = "testDateTimeList")]
        public List<DateTime> TestDateTimeList { get; set; }

        // Lists of Reference Types
        [DataMember(Name = "testInternalReferenceList")]
        public List<MutableTestModel1> TestInternalReferenceList { get; set; }
        [DataMember(Name = "testExternalReferenceList")]
        public List<MutableTestModel2> TestExternalReferenceList { get; set; }

        public MutableTestModel1() { }
        public MutableTestModel1(TestModel1 testModel1)
        {
            TestBool = testModel1.TestBool;
            TestInt = testModel1.TestInt;
            TestLong = testModel1.TestLong;
            TestFloat = testModel1.TestFloat;
            TestDouble = testModel1.TestDouble;
            TestDecimal = testModel1.TestDecimal;

            // TODO: Add all the other fields!!!

            TestInternalReference = ModelCopy<TestModel1, MutableTestModel1>(testModel1.TestInternalReference);
            TestExternalReference = ModelCopy<TestModel2, MutableTestModel2>(testModel1.TestExternalReference);

            //            TestIntList = ModelCopy(testModel1.TestIntList);
            TestStringList = ModelCopy(testModel1.TestStringList);
            //            TestDateTimeList = ModelCopy(testModel1.TestDateTimeList);

            TestInternalReferenceList = ModelCopy<TestModel1, MutableTestModel1>(testModel1.TestInternalReferenceList);
            TestExternalReferenceList = ModelCopy<TestModel2, MutableTestModel2>(testModel1.TestExternalReferenceList);
        }

        public override TestModel1 ToImmutable()
        {
            return new TestModel1(this);
        }
    }


    public class TestModel2 : ImmutableModel<TestModel2, MutableTestModel2>
    {
        public string TestString { get; private set; }
        public TestModel2(MutableTestModel2 model)
		{
			TestString = model.TestString;
		}
        public override MutableTestModel2 ToMutable()
        {
            return new MutableTestModel2(this);
        }
    }

    [DataContract]
    public class MutableTestModel2 : MutableModel<TestModel2, MutableTestModel2>
    {
        [DataMember]
        public string TestString { get; set; }
        public MutableTestModel2() { }
        public MutableTestModel2(TestModel2 model)
		{
			TestString = model.TestString;
		}
        public override TestModel2 ToImmutable()
        {
            return new TestModel2(this);
        }
    }

    public class TestModel3 : ImmutableModel<TestModel3, MutableTestModel3>
    {
        public TestModel3 InnerTestModel3 { get; private set; }
        public TestModel3(MutableTestModel3 model)
		{
			InnerTestModel3 = model.InnerTestModel3.ToImmutable();
		}
        public override MutableTestModel3 ToMutable()
        {
            return new MutableTestModel3(this);
        }
    }

    [DataContract]
    public class MutableTestModel3 : MutableModel<TestModel3, MutableTestModel3>
    {
        [DataMember]
        public MutableTestModel3 InnerTestModel3 { get; set; }
        public MutableTestModel3() { }
        public MutableTestModel3(TestModel3 model)
		{
			InnerTestModel3 = model.InnerTestModel3.ToMutable();
		}
        public override TestModel3 ToImmutable()
        {
            return new TestModel3(this);
        }
    }

}
