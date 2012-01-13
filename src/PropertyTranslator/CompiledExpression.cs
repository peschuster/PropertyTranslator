using System.Linq.Expressions;

namespace PropertyTranslator
{
    /// <summary>
    /// Abstract, non-generic compiled expression.
    /// </summary>
    public abstract class CompiledExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpression" /> class.
        /// </summary>
        protected CompiledExpression()
        {
        }

        /// <summary>
        /// Gets the undelying lambda expression.
        /// </summary>
        /// <value>The lambda expression.</value>
        internal abstract LambdaExpression BaseExpression { get; }
    }
}