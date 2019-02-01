using System;
using System.Diagnostics;

namespace Bellona.Core
{
    /// <summary>
    /// Provides support for lazy initialization, supposing to be used for mainly debug.
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized.</typeparam>
    [DebuggerDisplay("{Value}")]
    public class Lazy2<T>
    {
        Func<T> _valueFactory;
        T _value;

        /// <summary>
        /// Gets a value that indicates whether a value has been created.
        /// </summary>
        public bool IsValueCreated { get; private set; }

        /// <summary>
        /// Gets the lazily initialized value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!IsValueCreated)
                {
                    IsValueCreated = true;
                    _value = _valueFactory();
                }
                return _value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Lazy2{T}"/> class.
        /// When lazy initialization occurs, the specified initialization function is used.
        /// </summary>
        /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is null.</exception>
        public Lazy2(Func<T> valueFactory)
        {
            if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

            _valueFactory = valueFactory;
        }
    }
}
