using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubSonic.Configuration
{
    using NamedOptionsType = IDictionary<string, object>;

    public abstract class SubSonicOptions
        : ISubSonicOptions
    {
        protected readonly IDictionary<string, NamedOptionsType> _options;

        protected SubSonicOptions()
        {
            _options = new Dictionary<string, NamedOptionsType>();
        }

        public bool Enabled { get; set; }

        protected static string OptionSetName<TOptions>() => typeof(TOptions).Name;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <returns></returns>
        public Action<TOptions> SetActionOptions<TOptions>()
            where TOptions : class, new() => SetActionOptions<TOptions>(Options.DefaultName);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="name">named option initializer to use.</param>
        /// <returns></returns>
        public Action<TOptions> SetActionOptions<TOptions>(string name)
            where TOptions : class, new() => (options) =>
            {
                GetOptions(name, options);
            };
        /// <summary>
        /// Set option initializer of type TOptions with a default name
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public SubSonicOptions SetOptions<TOptions>(Action<TOptions> options)
            where TOptions : class, new() => SetOptions(Options.DefaultName, options);
        /// <summary>
        /// Set option initializer of type TOptions with a set name
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public SubSonicOptions SetOptions<TOptions>(string name, Action<TOptions> options)
            where TOptions : class, new()
        {
            name = name ?? Options.DefaultName;

            var set = OptionSetName<TOptions>();

            NamedOptionsType named_options;

            if (_options.ContainsKey(set))
            {
                named_options = _options[set];
            }
            else
            {
                named_options = new Dictionary<string, object>();
            }

            named_options[name] = options;

            _options[set] = named_options;

            return this;
        }

        public void GetOptions<TOptions>(TOptions options)
            where TOptions : class, new() => GetOptions(Options.DefaultName, options);

        public void GetOptions<TOptions>(string name, TOptions options)
            where TOptions : class, new()
        {
            var set = OptionSetName<TOptions>();

            if (_options.ContainsKey(set) &&
                _options[set] is NamedOptionsType named_options &&
                named_options.ContainsKey(name) &&
                named_options[name] is Action<TOptions> initializer)
            {
                initializer.Invoke(options);
            }
        }

        public TOptions GetOptions<TOptions>()
            where TOptions : class, new() => GetOptions<TOptions>(Options.DefaultName);        

        public TOptions GetOptions<TOptions>(string name)
            where TOptions : class, new()
        {
            var set = OptionSetName<TOptions>();

            var options = new TOptions();

            if (_options.ContainsKey(set) &&
                _options[set] is NamedOptionsType named_options &&
                named_options.ContainsKey(name) &&
                named_options[name] is Action<TOptions> initializer)
            {
                initializer.Invoke(options);
            }

            return options;
        }

        public object GetOptions(Type type) => GetOptions(type, Options.DefaultName);

        public object GetOptions(Type type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var options = Activator.CreateInstance(type);

            if (_options.ContainsKey(type.Name) &&
                _options[type.Name] is NamedOptionsType named_options &&
                named_options.ContainsKey(name) &&
                named_options[name] is object initializer)
            {
                var initType = initializer.GetType();

                initType.InvokeMember(nameof(Action.Invoke), System.Reflection.BindingFlags.Public, null, null, new object[] { options! });
            }

            return options!;
        }

        object[] ISubSonicOptions.GetOptions(params Type[] types)
        {
            return types.Select(type => GetOptions(type)).ToArray();
        }

        public void SetExtendedOptions(SubSonicOptions options)
        {
            foreach (var key in options._options.Keys)
            {
                _options[key] = options._options[key];
            }
        }
    }
}
