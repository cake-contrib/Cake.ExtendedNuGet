using System;
using System.Collections.Generic;
using Cake.Core;

namespace Cake.ExtendedNuGet.Tests.Fakes
{
    internal sealed class FakeCakeArguments : ICakeArguments
    {
        private readonly Dictionary<string, ICollection<string>> _arguments;

        /// <summary>
        /// Initializes a new instance of the <see cref="CakeArguments"/> class.
        /// </summary>
        public FakeCakeArguments()
        {
            _arguments = new Dictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether or not the specified argument exist.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <returns>
        ///   <c>true</c> if the argument exist; otherwise <c>false</c>.
        /// </returns>
        public bool HasArgument(string name)
        {
            return _arguments.ContainsKey(name);
        }

        public ICollection<string> GetArguments(string name)
        {
            return _arguments.TryGetValue(name, out var val) ? val : new List<string>();
        }
        
        public IDictionary<string, ICollection<string>> GetArguments() => _arguments;
    }
}