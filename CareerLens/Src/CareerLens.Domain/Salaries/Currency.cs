namespace CareerLens.Domain.Salaries
{
    public sealed class Currency
    {
        public string Code { get; private set; } = null!;

        private Currency() { }   

        public Currency(string code)
        {
            Code = code;
        }
    }
}
