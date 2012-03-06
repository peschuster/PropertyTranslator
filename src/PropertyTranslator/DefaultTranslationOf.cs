using System;
using System.Linq.Expressions;
using System.Reflection;

namespace PropertyTranslator
{
    /// <summary>
    /// Registers property mappings to the default <see cref="TranslationMap"/> and provides access to them.
    /// </summary>
    /// <typeparam name="T">The object (e.g. entity) type.</typeparam>
    public static class DefaultTranslationOf<T>
    {
        /// <summary>
        /// Evaluates the registered expression for specified method and instance.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="method">The method.</param>
        /// <returns>The result of the expression execution.</returns>
        /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
        public static TResult Evaluate<TResult>(T instance, MethodBase method)
        {
            return TranslationMap.DefaultMap.Get<T, TResult>(method).Evaluate(instance);
        }

        /// <summary>
        /// Property wrapper for specified object property.
        /// </summary>
        /// <param name="property">The property wrapper.</param>
        /// <returns></returns>
        /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
        public static IncompletePropertyTranslation<TResult> Property<TResult>(Expression<Func<T, TResult>> property)
        {
            return new IncompletePropertyTranslation<TResult>(property);
        }

        /// <summary>
        /// Registers a mapping for specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="language">The ui culture (e.g. "de", "en", etc.).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">On invalid property expression type (must be of type MemberExpression).</exception>
        /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
        public static CompiledExpressionMap<T, TResult> Property<TResult>(Expression<Func<T, TResult>> property, Expression<Func<T, TResult>> expression, string language = "")
        {
            return TranslationMap.DefaultMap.Add<T, TResult>(property, expression, language);
        }

        /// <summary>
        /// Property wrapper for chained registration.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the expression.</typeparam>
        public class IncompletePropertyTranslation<TResult>
        {
            private readonly Expression<Func<T, TResult>> property;

            /// <summary>
            /// Initializes a new instance of the <see cref="IncompletePropertyTranslation{TResult}" /> class.
            /// </summary>
            /// <param name="property">The property.</param>
            internal IncompletePropertyTranslation(Expression<Func<T, TResult>> property)
            {
                this.property = property;
            }

            /// <summary>
            /// Registers the specified expression for current property and specified language.
            /// </summary>
            /// <param name="expression">The expression.</param>
            /// <param name="language">The language (optional).</param>
            /// <returns></returns>
            /// <exception cref="System.InvalidOperationException">On invalid property expression type (must be of type MemberExpression).</exception>
            public CompiledExpressionMap<T, TResult> Is(Expression<Func<T, TResult>> expression, string language = "")
            {
                try
                {
                    return DefaultTranslationOf<T>.Property<TResult>(this.property, expression, language);
                }
                catch (ArgumentException exception)
                {
                    throw new InvalidOperationException("Invalid expression type of property. Must be of type MemberExpression.", exception);
                }
            }
        }
    }
}
