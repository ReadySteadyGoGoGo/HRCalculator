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
using System.Windows.Shapes;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace SalaryCalc
{
    class EditWindow : NewEmployee
    {

        private int _role = 0;
        private string _group = "";
        private int _id = 0;

        public EditWindow(string Name, int Role)
        {
            this.Title = "Edit Employee";
            _role = Role;  
            SetInitial(Name);
            _group = GroupcomboBox.SelectedValue.ToString();

        }
        public void ChiefEditFilter()
        {
            NametextBox.IsReadOnly = true;
            HiredatePicker.IsEnabled = false;

            GroupcomboBox.SelectedIndex = 0;
            GroupcomboBox.IsEnabled = false;
            ChiefcomboBox.IsEnabled = false;

        }

        
        public void SetInitial(string Name)
        {
            try
            {
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {

                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Name);
                    if (Employee == null) return;
                
                    

                    _id = Employee.ID;
                    NametextBox.Text = Name;

                    HiredatePicker.SelectedDate = LogicMethods.DateFromUnixTimestamp(Employee.HireDate);



                    GroupcomboBox.SelectedIndex = (Employee is Manager) ? 1 :
                                                 (Employee is Salesman) ? 2 :
                                                                          0;

                    List<string> Source = new List<string>();

                    foreach (var item in ChiefcomboBox.Items)
                    {
                        if (item.ToString() != Employee.Name)
                            Source.Add(item.ToString());
                    }


                    ChiefcomboBox.ItemsSource = Source;


                    if (Employee.Chief == null) ChiefcomboBox.SelectedIndex = 0;
                    else
                    {
                        foreach (string item in ChiefcomboBox.Items)
                        {
                            if (item == Employee.Chief.Name)
                            {
                                ChiefcomboBox.SelectedItem = item;
                            }

                        }
                    }

                    SalarytextBox.Text = Employee.BaseSalary.ToString();

                    if (_role == 2)
                    {
                        ChiefEditFilter();
                    }

                    CreateBtn.Content = "Edit";

                }

            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
           
        }


        public override void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            SalarytextBox.Text.Trim();
            NametextBox.Text.Trim();

            int Salary = 0;

            if (!int.TryParse(SalarytextBox.Text, out Salary))
            {
                MessageBox.Show("Please provide correct Salary!");
                return;
            }

                switch (_role)
            {
                case 2: //Chief Role - only changing Salary
                    {
                        try
                        {
                            using (EmployeesDBContext _context = new EmployeesDBContext())
                            {

                                var Employee = _context.Employees.SingleOrDefault(x => x.ID == _id);
                                if (Employee == null) return;
                                Employee.BaseSalary = Salary;
                                _context.Employees.SingleOrDefault(x => x.Name == NametextBox.Text).BaseSalary = Employee.BaseSalary;
                                _context.SaveChanges();
                                MessageBox.Show("Salary Has been changed!");

                                ((MainWindow)Application.Current.Windows[0]).Details_4_textboxE1.Text = Salary.ToString();
                                ((MainWindow)Application.Current.Windows[0]).Details_4_datepicker.SelectedDate = DateTime.Now;

                            }
                        }
                        catch(Exception exc)
                        {
                            WriteExceptionToDatabase(exc);
                        }              
                    }
                    break;
                case 3: //Admin Role - Checking All
                    {
                        try
                        {
                            if (NametextBox.Text == "")
                            {
                                MessageBox.Show("Please, provide employee's name");
                                return;
                            }

                            double SelectedDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(HiredatePicker.SelectedDate));
                            double Now = LogicMethods.DateToUnixTimestamp(DateTime.Now);

                            if (SelectedDate - Now > 0)
                            {
                                MessageBox.Show("You selected incorrect date!");
                                return;
                            }

                            using (EmployeesDBContext _context = new EmployeesDBContext())
                            {

                                var Employee = _context.Employees.SingleOrDefault(x => x.ID == _id);
                                if (Employee == null) return;
                                  
                                string BossName = ChiefcomboBox.SelectedValue.ToString();
                                if (BossName == "None")
                                {

                                    Employee.Chief = null;
                                }
                                else
                                {
                                    var Boss = _context.Employees.SingleOrDefault(x => x.Name == BossName);
                                    Employee.Chief = Boss;


                                }
                                Employee.Name = NametextBox.Text;
                                Employee.BaseSalary = Salary;
                                Employee.HireDate = (long)SelectedDate;
                                Employee.ID = _id;

                                if (_group == GroupcomboBox.SelectedValue.ToString())
                                {

                                    _context.SaveChanges();
                                     MessageBox.Show("Employee's Data has been changed!");
                                    ((MainWindow)Application.Current.Windows[0]).Details_4_textboxE1.Text = Salary.ToString();
                                    ((MainWindow)Application.Current.Windows[0]).Details_4_datepicker.SelectedDate = DateTime.Now;

                                }
                                else
                                {

                                    var DeletingEmployee = _context.Employees.SingleOrDefault(x => x.ID == _id);
                                    User Account = DeletingEmployee.User;

                                    if (Account != null)
                                        Account.Role = (GroupcomboBox.SelectedValue.ToString() == "Employee") ? 1 : 2;


                                    _context.Employees.Remove(DeletingEmployee);
                                    _context.SaveChanges();

                                    base.CreateBtn_Click(this, null);

                                    var EditedEmployee = _context.Employees.SingleOrDefault(x => x.Name == NametextBox.Text);

                                    EditedEmployee.User = Account;
                                    _context.SaveChanges();
                                    MessageBox.Show("Employee's Data has been changed!");
                                }
                                this.Close();
                                if (((AdminWindow)Application.Current.Windows[0]).Menucombobox.SelectedValue.ToString() == "Manage Employees")
                                    ((AdminWindow)Application.Current.Windows[0]).WholeList();
                                if (((AdminWindow)Application.Current.Windows[0]).Menucombobox.SelectedValue.ToString() == "My SubOrdinates")
                                    ((AdminWindow)Application.Current.Windows[0]).SubordinateList(((AdminWindow)Application.Current.Windows[0]).Userlabel.Content.ToString());
                            }

                        }
                        catch(Exception exc)
                        {
                            WriteExceptionToDatabase(exc);
                        }
                       
                    }
                    break;
            }
          
            this.Close();                       
        }
    }
}
