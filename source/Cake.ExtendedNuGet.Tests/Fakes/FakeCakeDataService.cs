using Cake.Core;
using System.Collections.Generic;
using System.Linq;

namespace Cake.ExtendedNuGet.Tests.Fakes
{
    public class FakeCakeDataService : ICakeDataService
    {
        List<object> values = new List<object>();

        public void Add<TData>(TData value) where TData : class
        {
            values.RemoveAll(v => v is TData);
            values.Add(value);
        }

        public TData Get<TData>() where TData : class
            => values.FirstOrDefault(v => v is TData) as TData;
    }
}