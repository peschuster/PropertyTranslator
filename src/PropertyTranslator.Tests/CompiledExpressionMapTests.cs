using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace PropertyTranslator.Tests
{
    public class CompiledExpressionMapTests : IDisposable
    {
        private const string ContextUiCulture = "DE";

        private readonly string oldUiCulture;
        
        public CompiledExpressionMapTests()
        {
            this.oldUiCulture = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(ContextUiCulture);
        }

        [Fact]
        public void TryGetValue_ReturnsNullOnEmptyTest()
        {
            var target = new CompiledExpressionMap();

            bool expected = false;
            
            bool actual;
            CompiledExpression outExpression;

            actual = target.TryGetValue(out outExpression);

            Assert.Equal(expected, actual);
            Assert.Null(outExpression);
        }

        [Fact]
        public void TryGetValueTest()
        {
            var target = new CompiledExpressionMap();

            CompiledExpression otherExpression1 = new CompiledExpression<string, string>((string s) => s.ToUpperInvariant());
            CompiledExpression otherExpression2 = new CompiledExpression<string, string>((string s) => s.Trim());
            CompiledExpression expectedExpression = new CompiledExpression<string, string>((string s) => s.ToLowerInvariant());

            target.Add(CompiledExpressionMap.DefaultLanguageKey, otherExpression1);
            target.Add("anyKey", otherExpression2);
            target.Add(ContextUiCulture, expectedExpression);

            bool expected = true;

            bool actual;
            CompiledExpression outExpression;

            actual = target.TryGetValue(out outExpression);

            Assert.Equal(expected, actual);
            Assert.Equal(expectedExpression, outExpression);
        }

        [Fact]
        public void TryGetValue_DefaultsToInvariantTest()
        {
            var target = new CompiledExpressionMap();

            CompiledExpression otherExpression = new CompiledExpression<string, string>((string s) => s.ToUpperInvariant());
            CompiledExpression expectedExpression = new CompiledExpression<string, string>((string s) => s.ToLowerInvariant());

            target.Add(CompiledExpressionMap.DefaultLanguageKey, expectedExpression);
            target.Add("anyKey", otherExpression);

            bool expected = true;

            bool actual;
            CompiledExpression outExpression;

            actual = target.TryGetValue(out outExpression);

            Assert.Equal(expected, actual);
            Assert.Equal(expectedExpression, outExpression);
        }

        [Fact]
        public void TryGetValue_DefaultsToAnyEntryTest()
        {
            var target = new CompiledExpressionMap();

            CompiledExpression expectedExpression = new CompiledExpression<string, string>((string s) => s.ToUpperInvariant());

            target.Add("AnyKey", expectedExpression);

            bool expected = true;

            bool actual;
            CompiledExpression outExpression;

            actual = target.TryGetValue(out outExpression);

            Assert.Equal(expected, actual);
            Assert.Equal(expectedExpression, outExpression);
        }

        public void Dispose()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(this.oldUiCulture);
        }
    }
}
