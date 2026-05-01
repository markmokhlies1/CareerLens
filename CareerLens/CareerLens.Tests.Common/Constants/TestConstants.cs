using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Tests.Common.Constants
{
    public static class TestConstants
    {
        
        public static readonly Guid DefaultUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static readonly Guid DefaultCompanyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static readonly Guid DefaultSalaryId = Guid.Parse("00000000-0000-0000-0000-000000000003");
        public static readonly Guid DefaultReviewId = Guid.Parse("00000000-0000-0000-0000-000000000004");

        public const string DefaultJobTitle = "Software Engineer";
        public const string DefaultLocation = "Cairo, Egypt";

        
        public const string DefaultInterviewDescription =
            "The interview process was very smooth and well organized. " +
            "The hiring team was professional and friendly throughout the entire process. " +
            "They asked relevant technical questions and were transparent about the role expectations.";

       
        public const string DefaultPros =
            "Great work life balance and very supportive management team. " +
            "The company culture is inclusive and employees are treated with respect. " +
            "There are many opportunities for career growth and skill development.";

        
        public const string DefaultCons =
            "The salary increments are not very competitive compared to the market. " +
            "Some internal processes are slow and could be improved with better tooling. " +
            "Remote work policy could be more flexible for senior team members.";

        public const string DefaultHeadline = "A great place to grow your career";
        public const string DefaultAdviceToManagement = "Keep investing in your people";

       
        public const string DefaultJobDescription = "We are looking for a talented software engineer.";
        public const string DefaultApplyUrl = "https://careers.example.com/apply";

        
        public const string DefaultCurrencyCode = "USD";
        public const decimal DefaultBasePayAmount = 5000m;
        public const int DefaultSalaryYear = 2024;
    }
}
