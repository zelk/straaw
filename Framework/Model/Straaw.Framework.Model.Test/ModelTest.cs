using System;
using System.Collections.Immutable;
using System.Runtime.Serialization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Straaw.Framework.Model.Test
{
	[DataContract]
	public class DictionaryTestModel : ImmutableModel<DictionaryTestModel, MutableDictionaryTestModel>
	{
		public DictionaryTestModel(MutableDictionaryTestModel dictionary)
		{
			Dict = ModelCopy(dictionary.Dict);
		}

		public override MutableDictionaryTestModel ToMutable()
		{
			return new MutableDictionaryTestModel(this);
		}

		[DataMember] public IImmutableDictionary<string, string> Dict { get; private set; }
	}

	[DataContract]
	public class MutableDictionaryTestModel : MutableModel<DictionaryTestModel, MutableDictionaryTestModel>
	{
		public MutableDictionaryTestModel()
		{
			Dict = new Dictionary<string, string>();
		}

		public MutableDictionaryTestModel(DictionaryTestModel dictionary)
		{
			Dict = ModelCopy(dictionary.Dict);
		}

		public override DictionaryTestModel ToImmutable()
		{
			return new DictionaryTestModel(this);
		}

		[DataMember] public IDictionary<string, string> Dict { get; set; }
	}

	[TestFixture]
	public class ModelTest
	{
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
			var immutableModelList = mutableModelList.ToImmutable();
			Assert.AreEqual(666, immutableModelList[0].TestInt);
		}

		[Test]
		public void DictionaryTest()
		{
			var mutableDictionaryModel = new MutableDictionaryTestModel();
			mutableDictionaryModel.Dict.Add("test1", "test1");
			mutableDictionaryModel.Dict.Add("test2", null);
			mutableDictionaryModel.Dict.Add("test3", "");
			mutableDictionaryModel.Dict.Add("", "test4");

			var dictionaryModel = mutableDictionaryModel.ToImmutable();
			Assert.AreEqual(4, dictionaryModel.Dict.Count);
			Assert.AreEqual("test1", dictionaryModel.Dict.GetValueOrDefault("test1"));
			Assert.AreEqual(null, dictionaryModel.Dict.GetValueOrDefault("test2"));
			Assert.AreEqual("", dictionaryModel.Dict.GetValueOrDefault("test3"));
			Assert.AreEqual("test4", dictionaryModel.Dict.GetValueOrDefault(""));
		}
	}
}
