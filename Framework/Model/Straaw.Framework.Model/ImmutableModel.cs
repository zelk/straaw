using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Straaw.Framework.Model
{
	public abstract class ImmutableModel<TImmutableModel, TMutableModel> : ModelBase, IImmutableModel
		where TImmutableModel : ImmutableModel<TImmutableModel, TMutableModel>
		where TMutableModel : MutableModel<TImmutableModel, TMutableModel>
	{
		public abstract TMutableModel ToMutable();

		public IMutableModel ToIMutableModel()
		{
			return ToMutable();
		}

		public Type ImmutableModelType()
		{
			return typeof(TMutableModel);
		}

		public Type MutableModelType()
		{
			return typeof(TMutableModel);
		}

		protected IImmutableDictionary<string, T> ModelCopy<T>(IDictionary<string, T> dictionary)
		{
			if (dictionary == null)
			{
				return new Dictionary<string, T>().ToImmutableDictionary();
			}

			return dictionary.ToImmutableDictionary();
		}

		protected ReadOnlyCollection<T> ModelCopy<T>(List<T> mutableList)
		{
			return mutableList == null ? new ReadOnlyCollection<T>(new List<T>()) : new ReadOnlyCollection<T>(new List<T>(mutableList));
		}

		protected TOtherImmutableModel ModelCopy<TOtherImmutableModel, TOtherMutableModel> (TOtherMutableModel mutableModel)
			where TOtherImmutableModel : ImmutableModel<TOtherImmutableModel, TOtherMutableModel>
			where TOtherMutableModel : MutableModel<TOtherImmutableModel, TOtherMutableModel>
		{
			return mutableModel == null ? null : mutableModel.ToImmutable();
		}

		protected ReadOnlyCollection<TOtherImmutableModel> ModelCopy<TOtherImmutableModel, TOtherMutableModel>(List<TOtherMutableModel> mutableList)
			where TOtherImmutableModel : ImmutableModel<TOtherImmutableModel, TOtherMutableModel>
			where TOtherMutableModel : MutableModel<TOtherImmutableModel, TOtherMutableModel>
		{
			if (mutableList == null)
				return null;

			var immutableList = new List<TOtherImmutableModel>(mutableList.Count);
			foreach (TOtherMutableModel mutableModel in mutableList)
			{
				immutableList.Add(mutableModel.ToImmutable());
			}
			return new ReadOnlyCollection<TOtherImmutableModel>(immutableList);
		}
	}
}
