using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalc
{
    public partial class Employee
    {

        public int FullSalary(DateTime CheckDate)
        {

            int ExpInYears = LogicMethods.ExperienceInYears(this.HireDate, CheckDate);
            return this.BaseSalary + this.BaseSalary * (ExpInYears > 10 ? 10 : ExpInYears) * 3 / 100;
        }

    }
}
