using System;
using System.Linq.Expressions;

namespace PropertyTranslator
{
    /// <summary>
    /// Generic extension to the <see cref="CompiledExpression"/> class.
    /// </summary>
    /// <typeparam name="T">The object (e.g. entity) type.</typeparam>
    /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
    public class CompiledExpression<T, TResult> : CompiledExpression
    {
        /// <summary>
        /// The base expression.
        /// </summary>
        private readonly Expression<Func<T, TResult>> expression;

        /// <summary>
        /// The compiled expression.
        /// </summary>
        private readonly Func<T, TResult> function;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpression{T, TResult}" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Necessary for evaluation of results.")]
        public CompiledExpression(Expression<Func<T, TResult>> expression)
        {
            this.expression = expression;
            this.function = expression.Compile();
        }

        /// <summary>
        /// Gets the undelying lambda expression.
        /// </summary>
        /// <value>The lambda expression.</value>
        internal override LambdaExpression BaseExpression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Evaluates the compiled expression on the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        public TResult Evaluate(T instance)
        {
            return this.function(instance);
        }
    }
}