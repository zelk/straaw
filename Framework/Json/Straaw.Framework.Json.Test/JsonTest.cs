using System;
using System.Collections.Generic;
using NUnit.Framework;
using Straaw.Framework.Json;
// TODO: Add tests for nullable types
using Straaw.Framework.Model;
using Straaw.Framework.Model.Test;
using Straaw.Framework.Logging;

namespace Straaw.Framework.Test.Wpf
{
	[TestFixture]
	public class JsonTest
	{
		static JsonTest()
		{
			L.SetSharedInstance(new LogManager("JsonTest"));
		}

		[Test]
		public void FullSerializeAndDeserialize()
		{
			var mutableTestModel1 = new MutableTestModel1 { TestStringObject = "TestStringObject" };
			var mutableTestModel2 = new MutableTestModel2 { TestString = "TestString"};
			var mutableTestModel1List = new List<MutableTestModel1>() { new MutableTestModel1 { TestStringObject = "TestStringObject" }, new MutableTestModel1 { TestStringObject = "TestStringObject" } };
			var mutableTestModel2List = new List<MutableTestModel2>() { new MutableTestModel2 { TestString = "TestString" }, new MutableTestModel2 { TestString = "TestString" } };
			var testObject = new MutableTestModel1
			{
				TestBool = true,
				TestInt = 42,
				TestLong = 42,
				TestFloat = 42.0f,
				TestDouble = 42.0d,
				TestDecimal = 42,
				TestStringObject = "TestStringObject",
				TestBooleanObject = true,
				TestDoubleObject = 42.0d,
				TestDecimalObject = 42,
				TestDateTimeObject = new DateTime(2012, 08, 13, 13, 25, 30),
				TestUint64Object = 42,
				TestInternalReference = mutableTestModel1,
				TestExternalReference = mutableTestModel2,
				TestIntList = new List<int>() {4, 2},
				TestStringList = new List<string>() {"4", "2"},
				TestDateTimeList = new List<DateTime>() { new DateTime(2012, 08, 13, 13, 25, 30), new DateTime(2012, 08, 13, 13, 25, 30) },
				TestInternalReferenceList = mutableTestModel1List,
				TestExternalReferenceList = mutableTestModel2List
			};

			var jsonString = JsonSerializer.SerializeDataContract(testObject, false);
//            Assert.Fail(jsonString);
			var result = JsonSerializer.DeserializeDataContract<MutableTestModel1>(jsonString, false);

			Assert.AreEqual(true, result.TestBool);
			Assert.AreEqual(42, result.TestInt);
			Assert.AreEqual(42, result.TestLong);
			Assert.AreEqual(42.0f, result.TestFloat);
			Assert.AreEqual(42.0d, result.TestDouble);
			Assert.AreEqual(42, result.TestDecimal);
			Assert.AreEqual("TestStringObject", result.TestStringObject);
			Assert.AreEqual(true, result.TestBooleanObject);
			Assert.AreEqual(42.0d, result.TestDoubleObject);
			Assert.AreEqual(42, result.TestDecimalObject);
			Assert.AreEqual(new DateTime(2012, 08, 13, 13, 25, 30), result.TestDateTimeObject);
			Assert.AreEqual(42UL, result.TestUint64Object);
			Assert.AreEqual("TestStringObject", result.TestInternalReference.TestStringObject);
			Assert.AreEqual("TestString", result.TestExternalReference.TestString);
			Assert.AreEqual(2, result.TestIntList[1]);
			Assert.AreEqual("2", result.TestStringList[1]);
			Assert.AreEqual(new DateTime(2012, 08, 13, 13, 25, 30), result.TestDateTimeList[1]);
			Assert.AreEqual("TestStringObject", result.TestInternalReferenceList[1].TestStringObject);
			Assert.AreEqual("TestString", result.TestExternalReferenceList[1].TestString);

		}

		[Test]
		public void NonDataMembersSkippedWhenDeserializing()
		{
			var testObject = JsonSerializer.DeserializeDataContract<MutableTestModel1>(@"{""testStringObject"":""myTestString"",""ShouldNotBeTouched"":""ShouldNotBeRead""}", false);
			Assert.AreEqual("myTestString", testObject.TestStringObject);
			Assert.IsNull(testObject.ShouldNotBeTouched);
		}

		[Test]
		public void NonDataMembersSkippedWhenSerializing()
		{
			const string myTestString = "myTestString";
			var testObject = new MutableTestModel1();
			testObject.ShouldNotBeTouched = myTestString;
			testObject.TestStringObject = myTestString;
			testObject = JsonSerializer.DeserializeDataContract<MutableTestModel1>(JsonSerializer.SerializeDataContract(testObject, false), false);
			Assert.AreEqual(myTestString, testObject.TestStringObject);
			Assert.IsNull(testObject.ShouldNotBeTouched);
		}

		[Test]
		public void SimpleModelInModel()
		{
			var mutableTestModel3 = new MutableTestModel3() { InnerTestModel3 = new MutableTestModel3() };
			var jsonString = JsonSerializer.SerializeDataContract(mutableTestModel3, false);
			var newMutableTestModel3 = JsonSerializer.DeserializeDataContract<MutableTestModel3>(jsonString, false);
			Assert.NotNull(newMutableTestModel3.InnerTestModel3);
		}

		[Test]
		public void ListsWithNullValues()
		{
			var mutableTestModel1List = new List<MutableTestModel1>() { null, null };
			var mutableTestModel2List = new List<MutableTestModel2>() { null, null };
			var testObject = new MutableTestModel1
			{
				TestStringList = new List<string>() { null, null },
				TestInternalReferenceList = mutableTestModel1List,
				TestExternalReferenceList = mutableTestModel2List
			};

			var jsonString = JsonSerializer.SerializeDataContract(testObject, false);
			var result = JsonSerializer.DeserializeDataContract<MutableTestModel1>(jsonString, false);

			Assert.AreEqual(2, result.TestStringList.Count);
			Assert.AreEqual(2, result.TestInternalReferenceList.Count);
			Assert.AreEqual(2, result.TestExternalReferenceList.Count);
		}

		[Test]
		public void ContactTest()
		{
			string jsonString = @"
			{
				""contact"": {
					""addresses"": [
					],
					""dates"": [
					],
					""emailAddresses"": [
					],
					""groups"": [
					],
					""images"": [
					],
					""mainPhoneNumber"": ""+46-666474"",
					""names"": {
						""firstName"": null,
						""firstNamePhonetic"": null,
						""lastName"": null,
						""lastNamePhonetic"": null,
						""middleName"": null,
						""middleNamePhonetic"": null,
						""namePrefix"": null,
						""nameSuffix"": null,
						""nickName"": null,
						""reveal"": null
					},
					""phoneNumbers"": [
					],
					""services"": [
					],
					""urls"": [
					],
					""work"": {
						""departmentName"": null,
						""jobTitle"": null,
						""organizationName"": null,
						""reveal"": null
					}
				}
			}";

			var contact = JsonSerializer.DeserializeDataContract<MutableContact>(jsonString, true);
			Assert.IsNotNull(contact);

			var jsonString2 = JsonSerializer.SerializeDataContract(contact, true);
			var contact2 = JsonSerializer.DeserializeDataContract<MutableContact>(jsonString2, true);
			var jsonString3 = JsonSerializer.SerializeDataContract(contact2, true);
			Assert.AreEqual(jsonString2, jsonString3);
		}

        [Test]
        public void ModelListTest()
        {
			var mutableModelList = new MutableModelList<TestModel1, MutableTestModel1>
			{
				new MutableTestModel1()
				{
					TestInt = 666
				}
			};
			Assert.AreEqual(666, mutableModelList[0].TestInt);
			var serializedData = JsonSerializer.SerializeDataContract(mutableModelList, false);
			Assert.NotNull(serializedData);
			var immutableModelList = JsonSerializer.DeserializeDataContract<MutableModelList<TestModel1, MutableTestModel1>>(serializedData, false);
			Assert.AreEqual(666, immutableModelList[0].TestInt);
        }

		[Test]
		public void DictionaryModelTest()
		{
			var mutableDictionaryModel = new MutableDictionaryTestModel();
			mutableDictionaryModel.Dict.Add("test1", "test1");
			mutableDictionaryModel.Dict.Add("test2", null);
			mutableDictionaryModel.Dict.Add("test3", "");
			mutableDictionaryModel.Dict.Add("", "test4");

			string json = JsonSerializer.SerializeDataContract(mutableDictionaryModel, false);
			Assert.AreEqual("{\"Dict\": {\"test1\": \"test1\", \"test2\": null, \"test3\": \"\", \"\": \"test4\"}}", json);

			var newMutableDictionaryModel = JsonSerializer.DeserializeDataContract<MutableDictionaryTestModel>(json);

			Assert.AreEqual(4, newMutableDictionaryModel.Dict.Count);
			Assert.IsTrue(newMutableDictionaryModel.Dict.ContainsKey("test1"));
			Assert.IsTrue(newMutableDictionaryModel.Dict.ContainsKey("test2"));
			Assert.IsTrue(newMutableDictionaryModel.Dict.ContainsKey("test3"));
			Assert.IsTrue(newMutableDictionaryModel.Dict.ContainsKey(""));
			Assert.AreEqual("test1", newMutableDictionaryModel.Dict["test1"]);
			Assert.AreEqual(null, newMutableDictionaryModel.Dict["test2"]);
			Assert.AreEqual("", newMutableDictionaryModel.Dict["test3"]);
			Assert.AreEqual("test4", newMutableDictionaryModel.Dict[""]);
		}
	}
}
