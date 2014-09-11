using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Straaw.Framework.Model
{
	[DataContract (Name="MutableModelList")]
	public class MutableModelList<TImmutableModel, TMutableModel> : List<TMutableModel>, IMutableModel
		where TImmutableModel : IImmutableModel
		where TMutableModel : IMutableModel
	{
		public MutableModelList()
		{
		}

		public MutableModelList(ImmutableModelList<TImmutableModel, TMutableModel> list)
		{
			foreach (var immutableModel in list)
			{
				Add((TMutableModel)immutableModel.ToIMutableModel());
			}
		}

		public MutableModelList(IEnumerable<TImmutableModel> list)
		{
			foreach (var immutableModel in list)
			{
				Add((TMutableModel)immutableModel.ToIMutableModel());
			}
		}

		public MutableModelList(IEnumerable<TMutableModel> list)
		{
			foreach (var mutableModel in list)
			{
				Add((TMutableModel)mutableModel);
			}
		}

		public IImmutableModel ToIImmutableModel()		{			return ToImmutable();		}

		public Type ImmutableModelType()
		{
			return typeof(ImmutableModelList<TImmutableModel, TMutableModel>);
		}

		public Type MutableModelType()
		{
			return this.GetType();
		}

		public ImmutableModelList<TImmutableModel, TMutableModel> ToImmutable()
		{
			return new ImmutableModelList<TImmutableModel, TMutableModel>(this);
		}
	}
}
