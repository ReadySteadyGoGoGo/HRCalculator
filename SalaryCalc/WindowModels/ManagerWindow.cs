using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;

namespace SalaryCalc
{
    class ManagerWindow : EmployeeWindow
    {
 
        private string _name= "";
        public ManagerWindow(string Name,int Role) :base(Name,Role)
        {
            _name = Name;
            List<string> Menulist = new List<string> { "My Details", "My Collegues", "Salary Calculator", "My SubOrdinates", "SubOrdinates Salary" };
            SetInitial(Menulist,_name,Role);

            Details_4_datepicker.SelectedDateChanged += Details_4_datepicker_SelectedDateChanged;
            Details_2_button.Click += Details_2_button_Click;
            Details_6_datePicker.SelectedDateChanged += Details_6_datePicker_SelectedDateChanged;

        }


        public override void ShowExtraDetails(string Name, int Salary)
        {
            if (Menucombobox.SelectedValue.ToString() != "My Collegues")
            {
                Details_4.Visibility = Visibility.Visible;
                Details_4_textboxE1.Text = Salary.ToString();

                Details_4_datepicker.SelectedDate = DateTime.Now;
            }


        }

        public virtual void Details_2_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditWindow edit = new EditWindow(Details_2_textboxA2.Text, (int)Roles.Chief);
                edit.Show();

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }


        }
        private void Details_4_datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {
                DateTime SelectedDateNormal = Convert.ToDateTime(Details_4_datepicker.SelectedDate);

                double SelectedDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(SelectedDateNormal));
                double HireDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(Details_2_textboxB2.Text));

                if (HireDate - SelectedDate > 0)
                {
                    MessageBox.Show("You selected a date prior the hire date. Please chose correct one. ");
                    Details_4_textboxS1.Text = "";
                    Details_4_textboxS2.Text = "";
                    Details_4_textboxS3.Text = "";
                    return;
                }
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Details_2_textboxA2.Text);
                    if (Employee != null)
                    {
                        Details_4_textboxS1.Text = Employee.BaseSalary.ToString();

                        int FullSalary = (Employee is Manager) ? ((Manager)Employee).FullSalary(SelectedDateNormal) :
                                                   (Employee is Salesman) ? ((Salesman)Employee).FullSalary(SelectedDateNormal) :
                                                   ((Employee)Employee).FullSalary(SelectedDateNormal);

                        Details_4_textboxS3.Text = FullSalary.ToString();
                        Details_4_textboxS2.Text = (FullSalary - Employee.BaseSalary).ToString();
                    }
                }
            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }

        }

        public void SetNoEmployees()
        {
            Details_6_textboxA2.Text = "No Employees";
            Details_6_textboxA3.Text = "n/a";
            Details_6_textboxA4.Text = "n/a";
            Details_6_textboxB4.Text = "n/a";
            Details_6_textboxA5.Text = "n/a";
            Details_6_textboxA6.Text = "n/a";
            Details_6_textboxB6.Text = "n/a";
        }



        private void Details_6_datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            
            try
            {

                DateTime SelectedDateNormal = Convert.ToDateTime(Details_6_datePicker.SelectedDate);
                double SelectedDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(SelectedDateNormal));

                List<int> Wages = new List<int>();
                List<string> Employees = new List<string>();

            
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {


                    foreach (string item in EmployeeslistView.Items)
                    {
                        var Worker = _context.Employees.SingleOrDefault(x => x.Name == item );
                        if (Worker == null) continue;

                        if (Worker.HireDate - SelectedDate <= 0)
                        {
                            
                            int WorkerSalary = (Worker is Manager) ? ((Manager)Worker).FullSalary(SelectedDateNormal) :
                                                   (Worker is Salesman) ? ((Salesman)Worker).FullSalary(SelectedDateNormal) :
                                                   ((Employee)Worker).FullSalary(SelectedDateNormal);

                            Wages.Add(WorkerSalary);
                            Employees.Add(Worker.Name);
                        }
                    }

                    if ((Wages.Count > 0) && (Employees.Count > 0))
                    {
                        Details_6_textboxA2.Text = Wages.Count.ToString();
                        Details_6_textboxA3.Text = Wages.Sum().ToString();
                        Details_6_textboxA4.Text = Wages.Max().ToString();
                        Details_6_textboxB4.Text = Employees[Wages.IndexOf(Wages.Max())];
                        Details_6_textboxA5.Text = Math.Floor(Wages.Average()).ToString();
                        Details_6_textboxA6.Text = Wages.Min().ToString();
                        Details_6_textboxB6.Text = Employees[Wages.IndexOf(Wages.Min())];
                    }
                    else
                    {
                        Details_6_textboxA2.Text = "No Employees";
                        Details_6_textboxA3.Text = "n/a";
                        Details_6_textboxA4.Text = "n/a";
                        Details_6_textboxB4.Text = "n/a";
                        Details_6_textboxA5.Text = "n/a";
                        Details_6_textboxA6.Text = "n/a";
                        Details_6_textboxB6.Text = "n/a";
                    }

                }
            }
            catch (Exception exc)
            {

                WriteExceptionToDatabase(exc);
            }

        }

        //SubOrdinators List

        public void SubordinateList(string Name)
        {

            try
            {
                EmployeeslistView.IsEnabled = true;
                EmployeeslistView.Items.Clear();
           
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Name);
                    if (Employee == null) return;


                    var SubordinatesList = _context.Employees.Where(x => x.Chief.Name == Employee.Name).ToList();

                    if (SubordinatesList == null || SubordinatesList.Count() == 0)
                    {
                        EmployeeslistView.Items.Add("No SubOrdinates");
                        EmployeeslistView.IsEnabled = false;
                        EmployeeslistView.SelectedIndex = -1;
                        if (Menucombobox.SelectedValue.ToString() == "My SubOrdinates") Details_2.Visibility = Visibility.Collapsed;
                        return;
                    }

                    foreach (var Sub in SubordinatesList)
                    {
                        EmployeeslistView.Items.Add(Sub.Name);
                    }
                    EmployeeslistView.SelectedIndex = -1;

                }
            }
            catch (Exception exc)
            {

                WriteExceptionToDatabase(exc);
            }
                         
        }

        public override void EmployeeslistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Menucombobox.SelectedValue.ToString() != "SubOrdinates Salary")
                base.EmployeeslistView_SelectionChanged(this, null);

        }

        public override void MenuManagerAddons()
        {

            string SelectedMenu = Menucombobox.SelectedValue.ToString();
            switch (SelectedMenu)
            {
                case "My SubOrdinates":
                    {
                     
                            SubordinateList(_name);
                            EmployeeslistView.Visibility = Visibility.Visible;
                            Details_1.Visibility = Visibility.Collapsed;
                            Details_3.Visibility = Visibility.Collapsed;
                            Details_4.Visibility = Visibility.Collapsed;
                            Details_5.Visibility = Visibility.Collapsed;
                            Details_6.Visibility = Visibility.Collapsed;
                            Details_7.Visibility = Visibility.Collapsed;
                  

                    }
                    break;

                case "SubOrdinates Salary":
                    {
                        
                            SubordinateList(_name);                   
                            Details_6_datePicker.SelectedDate = DateTime.Now;
                            Details_6_label.Content = "Salary Statistics for my SubOrdinates:";
                            Details_6.Visibility = Visibility.Visible;


                            EmployeeslistView.Visibility = Visibility.Visible;

                            Details_1.Visibility = Visibility.Collapsed;
                            Details_2.Visibility = Visibility.Collapsed;
                            Details_3.Visibility = Visibility.Collapsed;
                            Details_4.Visibility = Visibility.Collapsed;
                            Details_5.Visibility = Visibility.Collapsed;
                            Details_7.Visibility = Visibility.Collapsed;
                  
                   
                    }
                    break;
                    
            }
            MenuAdminAddons();

        }

        public virtual void MenuAdminAddons()
        {

        }




    }
}
