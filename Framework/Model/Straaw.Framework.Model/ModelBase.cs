using System;

namespace Straaw.Framework.Model
{
    public abstract class ModelBase
    {
        protected static string ModelCopy(string s)
        {
            return s;
        }

        protected static TStruct ModelCopy<TStruct>(TStruct s) where TStruct : struct
        {
            return s;
        }

        protected static TStruct? ModelCopy<TStruct>(TStruct? p) where TStruct : struct
        {
            return p;
        }

        protected static TValue[] ModelCopy<TValue>(TValue[] source) where TValue : struct
        {
            var copy = new TValue[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }

        protected static string[] ModelCopy(string[] source)
        {
            var copy = new string[source.Length];
            Array.Copy(source, copy, source.Length);
            return copy;
        }
    }
}
