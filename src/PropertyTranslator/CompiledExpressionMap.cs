using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace PropertyTranslator
{
    /// <summary>
    /// Collection of <see cref="CompiledExpression"/>s for different UiCultures.
    /// </summary>
    [Serializable]
    public class CompiledExpressionMap : Dictionary<string, CompiledExpression>
    {
        /// <summary>
        /// Default key for invariant language.
        /// </summary>
        public const string DefaultLanguageKey = "INVARIANT";

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionMap" /> class.
        /// </summary>
        public CompiledExpressionMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionMap" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected CompiledExpressionMap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the compiled expression for current thread ui culture.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">If no <see cref="CompiledExpression"/> for current environment is available.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Might throw an exception")]
        public virtual CompiledExpression GetValue()
        {
            CompiledExpression result;

            if (this.TryGetValue(out result))
            {
                return result;
            }

            throw new InvalidOperationException("No expression registered for specified method.");
        }

        /// <summary>
        /// Tries to get the compiled expression for current thread ui culture.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public virtual bool TryGetValue(out CompiledExpression expression)
        {
            var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant();

            if (this.ContainsKey(language))
            {
                expression = this[language];
            }
            else if (this.ContainsKey(DefaultLanguageKey))
            {
                expression = this[DefaultLanguageKey];
            }
            else if (this.Count > 0)
            {
                expression = this.Values.First();
            }
            else
            {
                expression = null;
            }

            return expression != null;
        }
    }
}
