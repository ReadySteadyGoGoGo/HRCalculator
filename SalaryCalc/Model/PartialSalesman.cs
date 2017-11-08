using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace SalaryCalc
{
    public partial class Salesman : Employee
    {
        public new int FullSalary(DateTime CheckDate)
        {
            
            EmployeesDBContext _contextDB = new EmployeesDBContext();
            var Subordinates = _contextDB.Employees.Where(x => x.Chief.Name == this.Name).ToList();
            int DependantWorkersSalary = 0;

            if (Subordinates != null)
            {
                foreach (var emp in Subordinates)
                {

                    int SubFullSalary = (emp is Manager) ? ((Manager)emp).FullSalary(CheckDate) :
                                        (emp is Salesman) ? ((Salesman)emp).FullSalary(CheckDate) :
                                        ((Employee)emp).FullSalary(CheckDate);

                    DependantWorkersSalary += SubFullSalary;

                }
                DependantWorkersSalary = DependantWorkersSalary * 3 / 1000;
            }

            int ExpInYears = LogicMethods.ExperienceInYears(this.HireDate, CheckDate);
            return
            DependantWorkersSalary + this.BaseSalary + this.BaseSalary * (ExpInYears > 35 ? 35 : ExpInYears) / 100;
        }






    }
}
