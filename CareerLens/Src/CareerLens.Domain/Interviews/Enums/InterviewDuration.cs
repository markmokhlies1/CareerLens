using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Interviews.Enums
{
    public enum InterviewDurationUnit
    {
        Days,
        Weeks,
        Months
    }
    public sealed record InterviewDuration
    {
        public int Value { get; }
        public InterviewDurationUnit Unit { get; }

        private InterviewDuration(int value, InterviewDurationUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Result<InterviewDuration> Create(int value, InterviewDurationUnit unit)
        {
            if (value <= 0)
                return InterviewErrors.DurationInvalid;

            if (!Enum.IsDefined(unit))
                return InterviewErrors.DurationUnitInvalid;

            return new InterviewDuration(value, unit);
        }
    }

}
