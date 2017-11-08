using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;



namespace SalaryCalc
{
   
    public partial class Manager : Employee
    {

        public new int FullSalary(DateTime CheckDate)
        {
            EmployeesDBContext _contextDB = new EmployeesDBContext();

            var Subordinates = _contextDB.Employees.OfType<Employee>().Where(x => x.Chief.Name == this.Name);

            int DependantWorkersSalary = 0;

            if (Subordinates != null)
            {
                foreach (var emp in Subordinates)
                {
                    if  (!(emp is Manager) && !(emp is Salesman))
                    {
                        DependantWorkersSalary += emp.FullSalary(CheckDate);
                    }

                }
                DependantWorkersSalary = DependantWorkersSalary * 5 / 1000;
            }
            
            int ExpInYears = LogicMethods.ExperienceInYears(this.HireDate, CheckDate);

            return
            DependantWorkersSalary + this.BaseSalary + this.BaseSalary * (ExpInYears > 8 ? 8 : ExpInYears) * 5 / 100;
        }

    }
}
