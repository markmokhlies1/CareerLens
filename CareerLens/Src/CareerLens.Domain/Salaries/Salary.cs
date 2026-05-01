using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Constants;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Companies;
using CareerLens.Domain.Reviews.Enums;
using CareerLens.Domain.Salaries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CareerLens.Domain.Salaries
{
    public sealed class Salary : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company? Company { get; private set; }
        public string? JobTitle { get; private set; }
        public EmployeeType EmployeeType { get; private set; }
        public EmploymentStatus EmploymentStatus { get; private set; }
        public LengthOfEmployment LengthOfEmployment { get; private set; }
        public string? Location { get; private set; }
        public Money? BasePay { get; private set; }
        public Money? Bonus { get; private set; }
        public Money? Stock { get; private set; }
        public Money? ProfitSharing { get; private set; }
        public Money? Tips { get; private set; }
        public Money? Commission { get; private set; }
        public SalaryPeriod SalaryPeriod { get; private set; }
        public int Year { get; private set; }
        public SalaryStatus Status { get; private set; }
        private Salary()
        {
        }
        private Salary(Guid id, Guid userId,Guid companyId,string jobTitle,EmployeeType employeeType,
            EmploymentStatus employmentStatus,LengthOfEmployment lengthOfEmployment,string location,
            Money basePay,SalaryPeriod salaryPeriod,int year,Money? bonus,Money? stock,Money? profitSharing,
            Money? tips,Money? commission)
            : base(id)
        {
            UserId = userId;
            CompanyId = companyId;
            JobTitle = jobTitle;
            EmployeeType = employeeType;
            EmploymentStatus = employmentStatus;
            LengthOfEmployment = lengthOfEmployment;
            Location = location;
            BasePay = basePay;
            SalaryPeriod = salaryPeriod;
            Year = year;
            Bonus = bonus;
            Stock = stock;
            ProfitSharing = profitSharing;
            Tips = tips;
            Commission = commission;
            Status = SalaryStatus.Pending;
        }

        public static Result<Salary> Create(Guid id,Guid userId,Guid companyId,string jobTitle,EmployeeType employeeType,
            EmploymentStatus employmentStatus,LengthOfEmployment lengthOfEmployment,Money basePay,SalaryPeriod salaryPeriod,
            int year,Money? bonus = null,Money? stock = null,Money? profitSharing = null,Money? tips = null,
            Money? commission = null,string? location = null)
        {
            if (id == Guid.Empty)
            {
                return SalaryErrors.IdRequired;
            }

            if (userId == Guid.Empty)
            {
                return SalaryErrors.UserIdRequired;
            }

            if (companyId == Guid.Empty)
            {
                return SalaryErrors.CompanyIdRequired;
            }

            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                return SalaryErrors.JobTitleRequired;
            }

            if (jobTitle.Length > CareerLensConstants.SalaryJobTitleLenght)
            {
                return SalaryErrors.JobTitleTooLong;
            }

            if (!Enum.IsDefined(employeeType))
            {
                return SalaryErrors.EmployeeTypeInvalid;
            }

            if (!Enum.IsDefined(employmentStatus))
            {
                return SalaryErrors.EmploymentStatusInvalid;
            }

            if (!Enum.IsDefined(lengthOfEmployment))
            {
                return SalaryErrors.LengthOfEmploymentInvalid;
            }

            if (!Enum.IsDefined(salaryPeriod))
            {
                return SalaryErrors.SalaryPeriodInvalid;
            }

            if (salaryPeriod == SalaryPeriod.Yearly && basePay.Amount < CareerLensConstants.SalaryBasePayAmountForYear)
            {
                return SalaryErrors.BasePayUnrealistic;
            }

            if (salaryPeriod == SalaryPeriod.Monthly && basePay.Amount < CareerLensConstants.SalaryBasePayAmountForMonth)
            {
                return SalaryErrors.BasePayUnrealistic;
            }

            var currentYear = DateTime.UtcNow.Year;
            if (year < 2000 || year > currentYear + 1)
            {
                return SalaryErrors.InvalidYear;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return SalaryErrors.LocationRequired;
            }
            if (location.Length > CareerLensConstants.SalaryLocationLength)
            {
                return SalaryErrors.LocationTooLong;
            }
            return new Salary(id,userId,companyId,jobTitle,employeeType,employmentStatus,
                    lengthOfEmployment,location,basePay,salaryPeriod,year,bonus,stock,profitSharing,tips,commission);
        }


        public Result<Updated> Update(string jobTitle,
                                      EmployeeType employeeType,
                                      EmploymentStatus employmentStatus,
                                      LengthOfEmployment lengthOfEmployment,
                                      string location,
                                      Money basePay,
                                      SalaryPeriod salaryPeriod,
                                      int year,
                                      Money? bonus = null,
                                      Money? stock = null,
                                      Money? profitSharing = null,
                                      Money? tips = null,
                                      Money? commission = null)
        {
            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                return SalaryErrors.JobTitleRequired;
            }

            if (jobTitle.Length > CareerLensConstants.SalaryJobTitleLenght)
            {
                return SalaryErrors.JobTitleTooLong;
            }

            if (!Enum.IsDefined(employeeType))
            {
                return SalaryErrors.EmployeeTypeInvalid;
            }

            if (!Enum.IsDefined(employmentStatus))
            {
                return SalaryErrors.EmploymentStatusInvalid;
            }

            if (!Enum.IsDefined(lengthOfEmployment))
            {
                return SalaryErrors.LengthOfEmploymentInvalid;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                return SalaryErrors.LocationRequired;
            }

            if (location.Length > CareerLensConstants.SalaryLocationLength)
            {
                return SalaryErrors.LocationTooLong;
            }

            if (!Enum.IsDefined(salaryPeriod))
            {
                return SalaryErrors.SalaryPeriodInvalid;
            }

            if (salaryPeriod == SalaryPeriod.Yearly && basePay.Amount < CareerLensConstants.SalaryBasePayAmountForYear)
            {
                return SalaryErrors.BasePayUnrealistic;
            }

            if (salaryPeriod == SalaryPeriod.Monthly && basePay.Amount < CareerLensConstants.SalaryBasePayAmountForMonth)
            {
                return SalaryErrors.BasePayUnrealistic;
            }

            var currentYear = DateTime.UtcNow.Year;
            if (year < 2000 || year > currentYear + 1)
            {
                return SalaryErrors.InvalidYear;
            }

            JobTitle = jobTitle;
            EmployeeType = employeeType;
            EmploymentStatus = employmentStatus;
            LengthOfEmployment = lengthOfEmployment;
            Location = location;
            BasePay = basePay;
            SalaryPeriod = salaryPeriod;
            Year = year;
            Bonus = bonus;
            Stock = stock;
            ProfitSharing = profitSharing;
            Tips = tips;
            Commission = commission;

            return Result.Updated;
        }
        public Result<Updated> UpdateStatus(SalaryStatus newStatus)
        {
            if (newStatus == SalaryStatus.Pending)
            {
                return SalaryErrors.CannotSetStatusToPending;
            }

            if (newStatus == Status)
            {
                return SalaryErrors.StatusAlreadySet;
            }

            switch (Status, newStatus)
            {
                case (SalaryStatus.Pending, SalaryStatus.Approved):
                case (SalaryStatus.Pending, SalaryStatus.Rejected):
                    break;

                case (SalaryStatus.Approved, _):
                    return SalaryErrors.CannotModifyApprovedSalary;

                case (SalaryStatus.Rejected, _):
                    return SalaryErrors.CannotModifyRejectedSalary;

                default:
                    return SalaryErrors.InvalidStatusTransition;
            }

            Status = newStatus;
            return Result.Updated;
        }
    }
}
