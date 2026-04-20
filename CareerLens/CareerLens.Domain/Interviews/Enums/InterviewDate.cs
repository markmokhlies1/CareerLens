using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Interviews.Enums
{
    public sealed record InterviewDate
    {
        public int Year { get; }
        public int Month { get; }

        private InterviewDate(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public static Result<InterviewDate> Create(int year, int month)
        {
            var currentYear = DateTime.UtcNow.Year;

            if (year < 2000 || year > currentYear)
                return InterviewErrors.InterviewDateInvalid;

            if (month < 1 || month > 12)
                return InterviewErrors.InterviewDateInvalid;

            return new InterviewDate(year, month);
        }
    }

}
