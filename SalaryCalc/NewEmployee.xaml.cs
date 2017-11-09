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
    /// <summary>
    /// Interaction logic for NewEmployee.xaml
    /// </summary>
    public partial class NewEmployee : Window
    {
        public NewEmployee()
        {
            InitializeComponent();
            SetInitialEmployee();
        

        }

        public void SetInitialEmployee()
        {
            try
            {
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {

                    var Employeelist = _context.Employees.ToList();
                    List<string> GroupList = new List<string>() { "Employee", "Manager", "Salesman" };
                    ObservableCollection<string> EmployeeCollection = new ObservableCollection<string>();
                    EmployeeCollection.Add("None");

                    foreach (var emp in Employeelist)
                    {
                        if ((emp is Manager) || (emp is Salesman))
                        {
                            EmployeeCollection.Add(emp.Name);
                        }
                    };
                    ChiefcomboBox.ItemsSource = EmployeeCollection;
                    GroupcomboBox.ItemsSource = GroupList;

                    ChiefcomboBox.SelectedIndex = 0;
                    GroupcomboBox.SelectedIndex = 0;
                    HiredatePicker.SelectedDate = DateTime.Now;
                }
            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
           
        }

        public virtual void CreateBtn_Click(object sender, RoutedEventArgs e)
        {

            NametextBox.Text.Trim();
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

            int Salary = 0;

            if (!int.TryParse(SalarytextBox.Text, out Salary))
            {
                MessageBox.Show("Please provide correct Salary!");
                return;
            }

            using (EmployeesDBContext _context = new EmployeesDBContext())
            {

                if (_context.Employees.Count(x => x.Name == NametextBox.Text) > 0)
                {
                    MessageBox.Show("User with the name" + NametextBox.Text + " is already exist in the Database!");
                    return;

                }

                string Chief = ChiefcomboBox.SelectedValue.ToString();


                switch (GroupcomboBox.SelectedValue.ToString())
                {
                    case "Manager":
                        {
                            try
                            {
                                Manager m1 = new Manager()
                                {
                                    ID = _context.Employees.Max(i => i.ID) + 1,
                                    Name = NametextBox.Text,
                                    BaseSalary = Salary,
                                    HireDate = (long)SelectedDate,

                                    Chief = (_context.Employees.Count(x => x.Name == Chief) > 0) ?
                                    _context.Employees.SingleOrDefault(x => x.Name == Chief) : null
                                };

                                _context.Employees.Add(m1);
                                _context.SaveChanges();
                                ((MainWindow)Application.Current.Windows[0]).EmployeeslistView.Items.Add(m1.Name);
                                MessageBox.Show("Employee" + m1.Name + " has been added to the Database!");
                            }
                            catch(Exception exc)
                            {
                                WriteExceptionToDatabase(exc);
                            }
                       

                        }

                        break;

                    case "Salesman":
                        {
                            try
                            {
                                Salesman s1 = new Salesman()
                                {
                                    ID = _context.Employees.Max(i => i.ID) + 1,
                                    Name = NametextBox.Text,
                                    BaseSalary = Salary,
                                    HireDate = (long)SelectedDate,

                                    Chief = (_context.Employees.Count(x => x.Name == Chief) > 0) ?
                                    _context.Employees.SingleOrDefault(x => x.Name == Chief) : null
                                };


                                _context.Employees.Add(s1);
                                _context.SaveChanges();
                                ((MainWindow)Application.Current.Windows[0]).EmployeeslistView.Items.Add(s1.Name);

                                MessageBox.Show("Employee" + s1.Name + " has been added to the Database!");
                            }
                            catch(Exception exc)
                            {
                                WriteExceptionToDatabase(exc);
                            }
                        
                        }
                        break;

                    case "Employee":
                        {
                            try
                            {
                                Employee e1 = new Employee()
                                {
                                    ID = _context.Employees.Max(i => i.ID) + 1,
                                    Name = NametextBox.Text,
                                    BaseSalary = Salary,
                                    HireDate = (long)SelectedDate,

                                    Chief = (_context.Employees.Count(x => x.Name == Chief) > 0) ?
                                   _context.Employees.SingleOrDefault(x => x.Name == Chief) : null
                                };



                                _context.Employees.Add(e1);
                                _context.SaveChanges();
                                ((MainWindow)Application.Current.Windows[0]).EmployeeslistView.Items.Add(e1.Name);
                                 MessageBox.Show("Employee" + e1.Name + " has been added to the Database!");
                            }
                            catch(Exception exc)
                            {
                                WriteExceptionToDatabase(exc);
                            }             
                        }
                        break;
                }
            }
            this.Close();
        }


        public void WriteExceptionToDatabase(Exception exc)
        {
            try
            {
                using (EmployeesDBContext _exceptioncontext = new EmployeesDBContext())
                {

                    ExceptionNote excNote = new ExceptionNote()
                    {
                        Date = (long)LogicMethods.DateToUnixTimestamp(DateTime.Now),
                        Source = exc.Source,
                        Details = exc.Message
                    };
                    _exceptioncontext.ExceptionNotes.Add(excNote);
                    _exceptioncontext.SaveChanges();
                }
            }
            catch
            {
                MessageBox.Show("Something went out of control");
            }
        }

    }
}
