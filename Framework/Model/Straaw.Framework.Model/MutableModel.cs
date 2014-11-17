using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System;
using System.Linq;

namespace Straaw.Framework.Model
{
// TODO: Try to find a way to move the implementation of ToMutable() and ToImmutable() to the base classes.
// TODO: Maybe: Try to find a way to implement the constructors in the base classes, using reflection to copy the data (less fast, less control in implementation but simpler data model classes).

	public abstract class MutableModel<TImmutableModel, TMutableModel> : IMutableModel
		where TImmutableModel : ImmutableModel<TImmutableModel, TMutableModel>
		where TMutableModel : MutableModel<TImmutableModel, TMutableModel>
	{
		public abstract TImmutableModel ToImmutable();

		public IImmutableModel ToIImmutableModel()
		{
			return ToImmutable();
		}

		public Type ImmutableModelType()
		{
			return typeof(TImmutableModel);
		}

		public Type MutableModelType()
		{
			return typeof(TMutableModel);
		}

		protected string ModelCopy(string s)
		{
			return s;
		}

		protected TStruct ModelCopy<TStruct>(TStruct s) where TStruct : struct
		{
			return s;
		}

		protected TStruct? ModelCopy<TStruct>(TStruct? p) where TStruct : struct
		{
			return p;
		}

		protected IDictionary<string, T> ModelCopy<T>(IImmutableDictionary<string, T> dictionary)
		{
			if (dictionary == null)
			{
				return new Dictionary<string, T>();
			}

			return dictionary.ToDictionary(r => r.Key, r => r.Value);
		}

		protected List<T> ModelCopy<T>(ReadOnlyCollection<T> immutableList)
		{
			return immutableList == null ? new List<T>() : new List<T>(immutableList);
		}

		protected
			TOtherMutableModel
			ModelCopy<TOtherImmutableModel, TOtherMutableModel>
			(TOtherImmutableModel immutableModel)
			where TOtherImmutableModel : ImmutableModel<TOtherImmutableModel, TOtherMutableModel>
			where TOtherMutableModel : MutableModel<TOtherImmutableModel, TOtherMutableModel>
		{
			if (immutableModel == null)
				return null;

			return immutableModel.ToMutable();
		}

		protected List<TOtherMutableModel>
			ModelCopy<TOtherImmutableModel, TOtherMutableModel>
			(ReadOnlyCollection<TOtherImmutableModel> immutableList)
			where TOtherImmutableModel : ImmutableModel<TOtherImmutableModel, TOtherMutableModel>
			where TOtherMutableModel : MutableModel<TOtherImmutableModel, TOtherMutableModel>
		{
			if (immutableList == null)
				return null;

			var mutableList = new List<TOtherMutableModel>(immutableList.Count);
			foreach (TOtherImmutableModel immutableModel in immutableList)
				mutableList.Add(immutableModel.ToMutable());
			return mutableList;
		}

	}
}
