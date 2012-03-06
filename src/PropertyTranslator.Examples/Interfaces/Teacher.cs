namespace PropertyTranslator.Examples.Interfaces
{
    public class Teacher : IPerson
    {
        private static readonly CompiledExpressionMap<Teacher, string> displayNameExpression
            = DefaultTranslationOf<Teacher>.Property(s => s.DisplayName).Is(s => s.Name);

        public string DisplayName
        {
            get { return displayNameExpression.Evaluate(this); }
        }

        public string Name { get; set; }
    }
}
