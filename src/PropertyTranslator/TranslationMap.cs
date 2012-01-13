using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace PropertyTranslator
{
    /// <summary>
    /// Map for property translations.
    /// </summary>
    [Serializable]
    public class TranslationMap : Dictionary<MemberInfo, CompiledExpressionMap>
    {
        /// <summary>
        /// Instance of the default <see cref="TranslationMap"/>.
        /// </summary>
        private static readonly TranslationMap defaultMap = new TranslationMap();

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMap" /> class.
        /// </summary>
        public TranslationMap()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslationMap" /> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TranslationMap(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Instance of the default <see cref="TranslationMap"/>.
        /// </summary>
        /// <value>The default translation map.</value>
        public static TranslationMap DefaultMap
        {
            get { return defaultMap; }
        }

        /// <summary>
        /// Adds a new expression for specified property to the map.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="compiledExpression">The compiled expression.</param>
        /// <param name="language">The language (e.g. "de", "en", etc.).</param>
        /// <exception cref="System.ArgumentException">On invalid property expression type (must be of type MemberExpression).</exception>
        public void Add<T, TResult>(Expression<Func<T, TResult>> property, CompiledExpression<T, TResult> compiledExpression, string language = "")
        {
            var member = property.Body as MemberExpression;

            if (member == null)
                throw new ArgumentException("property body must be a MemberExpression.", "property");

            this.AddInternal(member.Member, compiledExpression, language);
        }

        /// <summary>
        /// Adds a new expression for specified property to the map.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="language">The language (e.g. "de", "en", etc.).</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">On invalid property expression type (must be of type MemberExpression).</exception>
        public CompiledExpressionMap<T, TResult> Add<T, TResult>(Expression<Func<T, TResult>> property, Expression<Func<T, TResult>> expression, string language = "")
        {
            var member = property.Body as MemberExpression;

            if (member == null)
                throw new ArgumentException("property body must be a MemberExpression.", "property");

            var compiledExpression = new CompiledExpression<T, TResult>(expression);

            return this.AddInternal<T, TResult>(member.Member, compiledExpression, language);
        }

        /// <summary>
        /// Returns the <see cref="CompiledExpression" /> for specified method and the ui culture of the current thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public CompiledExpression<T, TResult> Get<T, TResult>(MethodBase method)
        {
            CompiledExpression result;

            if (this.TryGetValue(method, out result)) 
            {
                return result as CompiledExpression<T, TResult>;
            }

            throw new InvalidOperationException("No expression registered for specified method.");
        }

        /// <summary>
        /// Tries to return the <see cref="CompiledExpression" /> for specified method and the ui culture of the current thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="expression">The compiled expression.</param>
        /// <returns></returns>
        public bool TryGetValue(MemberInfo method, out CompiledExpression expression)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return this.TryGetValue(method, method.DeclaringType, out expression);
        }

        /// <summary>
        /// Tries to return the <see cref="CompiledExpression" /> for specified method and the ui culture of the current thread.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="baseType">When the quey was built against an interface you can specify the concrete type here (retrieved from node.Expression.Type!).</param>
        /// <param name="expression">The compiled expression.</param>
        /// <returns></returns>
        public bool TryGetValue(MemberInfo method, Type baseType, out CompiledExpression expression)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (baseType == null)
                throw new ArgumentNullException("baseType");

            PropertyInfo property = baseType.GetProperty(method.Name.Replace("get_", string.Empty));

            if (property == null || !this.ContainsKey(property))
            {
                expression = null;

                return false;
            }

            var map = base[property];

            return map.TryGetValue(out expression);
        }

        private CompiledExpressionMap<T, TResult> AddInternal<T, TResult>(MemberInfo property, CompiledExpression<T, TResult> compiledExpression, string language)
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                language = CompiledExpressionMap.DefaultLanguageKey;
            }

            if (!this.ContainsKey(property))
            {
                base.Add(property, new CompiledExpressionMap<T, TResult>());
            }

            base[property].Add(language.ToUpperInvariant(), compiledExpression);

            return base[property] as CompiledExpressionMap<T, TResult>;
        }
    }
}