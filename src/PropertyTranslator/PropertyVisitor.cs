using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace PropertyTranslator
{
    /// <summary>
    /// Expression visitor resolving and performing registered property translations.
    /// </summary>
    public class PropertyVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Stack of bindings to visit.
        /// </summary>
        private readonly Stack<KeyValuePair<ParameterExpression, Expression>> bindings = new Stack<KeyValuePair<ParameterExpression, Expression>>();

        /// <summary>
        /// Used <see cref="TranslationMap"/> for property mapping.
        /// </summary>
        private readonly TranslationMap map;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyVisitor" /> class with default <see cref="TranslationMap"/>.
        /// </summary>
        public PropertyVisitor()
            : this(TranslationMap.DefaultMap)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyVisitor" /> class.
        /// </summary>
        /// <param name="map">The translation map.</param>
        public PropertyVisitor(TranslationMap map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            this.map = map;
        }

        /// <summary>
        /// Visits the member.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            CompiledExpression expression;

            // Ensure that all property mappings are registered (the usual way -> on object type)
            EnsureTypeInitialized(node.Member.DeclaringType);

            // Ensure that all property mappings are registered (when the linq query was built towards an interface)
            if (IsBuildOnInterface(node))
            {
                EnsureTypeInitialized(node.Expression.Type);
            }

            if ((IsBuildOnInterface(node) && this.map.TryGetValue(node.Member, node.Expression.Type, out expression))
                 || this.map.TryGetValue(node.Member, out expression))
            {
                return this.VisitCompiledExpression(expression, node.Expression);
            }

            if (typeof(CompiledExpression).IsAssignableFrom(node.Member.DeclaringType))
            {
                return this.VisitCompiledExpression(expression, node.Expression);
            }

            return base.VisitMember(node);
        }

        /// <summary>
        /// Visits the parameter.
        /// </summary>
        /// <param name="p">The parameter expression.</param>
        /// <returns></returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            var pair = this.bindings.FirstOrDefault(b => b.Key == p);

            if (pair.Value != null)
            {
                return this.Visit(pair.Value);
            }

            return base.VisitParameter(p);
        }

        /// <summary>
        /// Ensures that the static fields on the reflected type are initialized.
        /// </summary>
        /// <param name="type">The reflected type.</param>
        private static void EnsureTypeInitialized(Type type)
        {
            try
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
            catch (TypeInitializationException)
            {
            }
        }

        /// <summary>
        /// Determines whether the query was build towards an interface.
        /// </summary>
        /// <param name="node">The current node.</param>
        /// <returns></returns>
        private static bool IsBuildOnInterface(MemberExpression node)
        {
            return node.Expression != null 
                && node.Expression.Type != null 
                && node.Expression.Type != node.Member.DeclaringType;
        }

        private Expression VisitCompiledExpression(CompiledExpression compiledExpression, Expression expression)
        {
            this.bindings.Push(new KeyValuePair<ParameterExpression, Expression>(compiledExpression.BaseExpression.Parameters.Single(), expression));

            var result = this.Visit(compiledExpression.BaseExpression.Body);
            this.bindings.Pop();

            return result;
        }
    }
}
