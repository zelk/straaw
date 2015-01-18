using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Straaw.Framework.Model
{
	[DataContract (Name="ImmutableModelList")]
	public class ImmutableModelList<TImmutableModel, TMutableModel> : ReadOnlyCollection<TImmutableModel>, IImmutableModel
		where TImmutableModel : IImmutableModel
		where TMutableModel : IMutableModel
	{
		public ImmutableModelList(MutableModelList<TImmutableModel, TMutableModel> list)
			: base(CopyList(list))
		{
		}

        public ImmutableModelList(IEnumerable<TImmutableModel> models)
            : this(new MutableModelList<TImmutableModel, TMutableModel>(models))
        {
        }

		public IMutableModel ToIMutableModel()
		{
			return ToMutable();
		}

		public Type ImmutableModelType()
		{
			return this.GetType();
		}

		public Type MutableModelType()
		{
			return typeof(MutableModelList<TImmutableModel, TMutableModel>);
		}

		public MutableModelList<TImmutableModel, TMutableModel> ToMutable()
		{
			return new MutableModelList<TImmutableModel, TMutableModel>(this);
		}


		// PRIVATE
		/////////////////////////////////////////////////////////////////////////////////////////////////

		private static List<TImmutableModel> CopyList(MutableModelList<TImmutableModel, TMutableModel> mutableModelList)
		{
			var immutableList = new List<TImmutableModel>();
			foreach(var mutableModel in mutableModelList)
			{
				immutableList.Add((TImmutableModel)mutableModel.ToIImmutableModel());
			}
			return immutableList;
		}
	}
}
