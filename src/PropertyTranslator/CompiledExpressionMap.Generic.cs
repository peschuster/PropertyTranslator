using System;
using System.Runtime.Serialization;

namespace PropertyTranslator
{
    /// <summary>
    /// Generic extension to the <see cref="CompiledExpressionMap"/>
    /// </summary>
    /// <typeparam name="T">The object (e.g. entity) type.</typeparam>
    /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
    [Serializable]
    public class CompiledExpressionMap<T, TResult> : CompiledExpressionMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionMap{T, TResult}" /> class.
        /// </summary>
        public CompiledExpressionMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionMap{T, TResult}" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected CompiledExpressionMap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Evaluates the compiled expression for current thread ui culture on the specified instance.
        /// </summary>
        /// <param name="instance">Object instance to be evaluated.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">If no <see cref="CompiledExpression"/> for current environment is available.</exception>
        public TResult Evaluate(T instance)
        {
            return this.GetValue().Evaluate(instance);
        }

        /// <summary>
        /// Gets the compiled expression for current thread ui culture.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">If no <see cref="CompiledExpression"/> for current envirnoment is available.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Throws an exception")]
        public new CompiledExpression<T, TResult> GetValue()
        {
            CompiledExpression result;

            if (this.TryGetValue(out result))
            {
                return result as CompiledExpression<T, TResult>;
            }

            throw new InvalidOperationException("No expression registered for specified method.");
        }

        /// <summary>
        /// Tries to get the compiled expression for current thread ui culture.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public bool TryGetValue(out CompiledExpression<T, TResult> expression)
        {
            CompiledExpression result;
            expression = null;

            if (base.TryGetValue(out result))
            {
                expression = result as CompiledExpression<T, TResult>;
            }

            return expression != null;
        }
    }
}
