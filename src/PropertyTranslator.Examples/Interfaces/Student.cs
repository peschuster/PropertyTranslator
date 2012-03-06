namespace PropertyTranslator.Examples.Interfaces
{
    public class Student : IPerson
    {
        private static readonly CompiledExpressionMap<Student, string> displayNameExpression
            = DefaultTranslationOf<Student>.Property(s => s.DisplayName).Is(s => s.Name + " (" + s.MatrNo + ")");

        public string DisplayName
        {
            get { return displayNameExpression.Evaluate(this); }
        }

        public string Name { get; set; }

        public string MatrNo { get; set; }
    }
}
