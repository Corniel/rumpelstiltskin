using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rumpelstiltskin.Shared.Refelection
{
    /// <summary>Registers a <see cref="Type"/>/<see cref="string"/> mapping.</summary>
    public sealed class TypeMap : IEnumerable<KeyValuePair<Type, string>>
    {
        private readonly Dictionary<string, Type> types = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> names = new Dictionary<Type, string>();

        /// <summary>Gets the total of registered <see cref="Type"/> type name combinations.</summary>
        public int Count => types.Count;

        /// <summary>Adds all <see cref="Type"/>s of the assembly.</summary>
        public TypeMap Add(Assembly assembly)
        {
            Guard.NotNull(assembly, nameof(assembly));
            return AddRange(assembly.GetTypes().Where(tp => !names.ContainsKey(tp)).ToArray());
        }

        /// <summary>Adds a <see cref="Type"/> and its name.</summary>
        public TypeMap Add(Type type, string name)
        {
            Guard.NotNull(type, nameof(type));
            Guard.NotNullOrEmpty(name, nameof(name));

            lock (locker)
            {
                var newName = !names.ContainsKey(type);
                var newType = !types.ContainsKey(name);

                if (newName ^ newType)
                {
                    throw new ArgumentException($"The type {type} and/or the name '{name}' has already been added as another pair.");
                }
                if (newName /* && newType */)
                {
                    names[type] = name;
                    types[name] = type;
                }
            }
            return this;
        }

        /// <summary>Adds a <see cref="Type"/> and its <see cref="Type.FullName"/>.</summary>
        public TypeMap Add(Type type) => Add(type, type?.FullName);

        /// <summary>Adds a range of types.</summary>
        public TypeMap AddRange(params Type[] types)
        {
            Guard.NotNull(types, nameof(types));

            foreach (var type in types.Where(t => !t.IsAbstract && t.IsPublic))
            {
                Add(type);
            }
            return this;
        }

        /// <summary>Tries to get the name of a type.</summary>
        public string TryGetName(Type type)
        {
            if (type is null)
            {
                return null;
            }
            names.TryGetValue(type, out string name);
            return name ?? type.FullName;
        }

        /// <summary>Tries to get a type based on its name.</summary>
        public Type TryGetType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            types.TryGetValue(name, out Type type);
            return type ?? Type.GetType(name, throwOnError: false);
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator() => names.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly object locker = new object();
    }
}
