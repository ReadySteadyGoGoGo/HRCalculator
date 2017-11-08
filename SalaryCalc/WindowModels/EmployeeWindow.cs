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
    class EmployeeWindow : MainWindow
    {
        private string _name = "";
        public EmployeeWindow(string Name, int Role)
        {
            _name = Name;
            List<string> Menulist = new List<string> { "My Details", "My Collegues", "Salary Calculator"};
            SetInitial(Menulist, _name, Role);

            Menucombobox.SelectionChanged += Menucombobox_SelectionChanged;
            EmployeeslistView.SelectionChanged += EmployeeslistView_SelectionChanged;
            Details_3_datepickerA4.SelectedDateChanged += Details_3_datepickerA4_SelectedDateChanged;
            Details_1_button.Click += Details_1_button_Click;
        }

        private void Details_1_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangePasswordWindow ChangeWindow = new ChangePasswordWindow(Details_1_textboxB1.Text, Details_1_textboxB7.Text);
                ChangeWindow.Show();
            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
            
        }


        public void SetInitial(List<string> MenuList, string Name, int Role)
        {

            Menucombobox.ItemsSource = MenuList;
            Menucombobox.SelectedIndex = 0;
            Userlabel.Content = _name;
            Rolelabel.Content = (Role == 1) ? "Employee" :
                                    (Role == 2) ? "Chief" :
                                                  "Admin";


            Userlabel.Foreground = (Role == 1) ? Brushes.Beige :
                                      (Role == 2) ? Brushes.DarkRed :
                                                    Brushes.Indigo;

            Rolelabel.Foreground = Userlabel.Foreground;
            SetMyDetails(Name);


            Details_1.Visibility = Visibility.Visible;
            EmployeeslistView.Visibility = Visibility.Collapsed;
            Details_2.Visibility = Visibility.Collapsed;
            Details_3.Visibility = Visibility.Collapsed;
            Details_4.Visibility = Visibility.Collapsed;
            Details_5.Visibility = Visibility.Collapsed;
            Details_6.Visibility = Visibility.Collapsed;
            Details_7.Visibility = Visibility.Collapsed;

        }

        public void SetMyDetails(string Name)
        {
            try
            {

                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Name);
                    if (Employee != null)
                    {
                        Details_1_textboxB1.Text = Employee.Name;
                        Details_3_textboxA1.Text = Employee.Name;

                        Details_1_textboxB2.Text = LogicMethods.DateFromUnixTimestamp(Employee.HireDate).ToShortDateString();
                        Details_3_textboxA2.Text = Details_1_textboxB2.Text;


                        Details_1_textboxB3.Text = (Employee is Manager) ? ("Manager") : (Employee is Salesman) ? ("Salesman") : ("Employee");
                        Details_3_textboxA3.Text = Details_1_textboxB3.Text;


                        Details_1_textboxB4.Text = Employee.BaseSalary.ToString();
                        Details_1_textboxB5.Text = (Employee is Manager) ? ((Manager)Employee).FullSalary(DateTime.Now).ToString() :
                                                 (Employee is Salesman) ? ((Salesman)Employee).FullSalary(DateTime.Now).ToString() :
                                                 ((Employee)Employee).FullSalary(DateTime.Now).ToString();



                        if (Employee.ChiefID != null) Details_1_textboxB6.Text = Employee.Chief.Name;
                        else Details_1_textboxB6.Text = "None";

                        Details_1_textboxB7.Text = Employee.User.Login;

                    }
                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }


        }

        private void Details_3_datepickerA4_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DateTime SelectedDateNormal = Convert.ToDateTime(Details_3_datepickerA4.SelectedDate);

                double SelectedDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(SelectedDateNormal));
                double HireDate = LogicMethods.DateToUnixTimestamp(Convert.ToDateTime(Details_3_textboxA2.Text));

                if (HireDate - SelectedDate > 0)
                {
                    MessageBox.Show("You selected a date prior your hire date. Please chose correct one. ");
                    Details_3_textboxA5.Text = "";
                    Details_3_textboxA6.Text = "";
                    Details_3_textboxA7.Text = "";
                    return;
                }

                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Details_3_textboxA1.Text);
                    if (Employee != null)
                    {
                        Details_3_textboxA5.Text = Employee.BaseSalary.ToString();

                        int FullSalary = (Employee is Manager) ? ((Manager)Employee).FullSalary(SelectedDateNormal) :
                                                   (Employee is Salesman) ? ((Salesman)Employee).FullSalary(SelectedDateNormal) :
                                                   ((Employee)Employee).FullSalary(SelectedDateNormal);

                        Details_3_textboxA7.Text = FullSalary.ToString();
                        Details_3_textboxA6.Text = (FullSalary - Employee.BaseSalary).ToString();
                    }
                }
            }

            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
    
        }

        //ListView Selection Changed

        public virtual void EmployeeslistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (EmployeeslistView.SelectedIndex != -1)
                {


                    string SearchName = EmployeeslistView.SelectedValue.ToString();
                    using (EmployeesDBContext _context = new EmployeesDBContext())
                    {
                        var Employee = _context.Employees.SingleOrDefault(x => x.Name == SearchName);
                        if (Employee == null) return;

                        string Type = (Employee is Manager) ? ("Manager") : (Employee is Salesman) ? ("Salesman") : ("Employee");
                        Details_2_textboxA2.Text = Employee.Name;
                        Details_2_textboxB2.Text = LogicMethods.DateFromUnixTimestamp(Employee.HireDate).ToShortDateString();
                        Details_2_textboxC2.Text = Type;

                        if (Employee.Chief != null) Details_2_textboxD2.Text = Employee.Chief.Name;
                        else Details_2_textboxD2.Text = "None";

                        Details_2.Visibility = Visibility.Visible;
                        ShowExtraDetails(Name, Employee.BaseSalary);
                    }
                }
            }

            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
          
        }
  

        public void CollegueList(string Name)
        {
            try
            {
                EmployeeslistView.Items.Clear();
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employee = _context.Employees.SingleOrDefault(x => x.Name == Name);
                    if (Employee != null)
                    {

                        var CollegueList = _context.Employees.Where(x => x.ChiefID == Employee.ChiefID && x.ChiefID != null).Select(x => x.Name).ToList();

                        if (CollegueList.Count > 1)
                        {
                            EmployeeslistView.IsEnabled = true;

                            foreach (var collegue in CollegueList)
                            {
                                if ((collegue != Employee.Name) && (Employee.ChiefID != null))
                                {
                                    EmployeeslistView.Items.Add(collegue);
                                }
                            }
                            EmployeeslistView.SelectedIndex = 0;
                        }
                        else
                        {

                            EmployeeslistView.Items.Add("No collegues to display");
                            Details_2.Visibility = Visibility.Collapsed;
                            EmployeeslistView.IsEnabled = false;
                            EmployeeslistView.SelectedIndex = -1;
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
        }
 

        public  void Menucombobox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            string SelectedMenu = Menucombobox.SelectedValue.ToString();
            switch (SelectedMenu)
            {
                case "My Details":
                    {

                        SetMyDetails(_name);
                        Details_1.Visibility = Visibility.Visible;
                        EmployeeslistView.Visibility = Visibility.Collapsed;
                        Details_2.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Collapsed;
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Collapsed;
                        Details_6.Visibility = Visibility.Collapsed;
                        Details_7.Visibility = Visibility.Collapsed;
                    }

                break;

                case "My Collegues":
                    {
                        CollegueList(_name);
                        EmployeeslistView.Visibility = Visibility.Visible;
                        Details_1.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Collapsed;
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Collapsed;
                        Details_6.Visibility = Visibility.Collapsed;
                        Details_7.Visibility = Visibility.Collapsed;

                    }
                break;

                case "Salary Calculator":
                    {
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Visible;
                        Details_2.Visibility = Visibility.Collapsed;
                        Details_1.Visibility = Visibility.Collapsed;
                        EmployeeslistView.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Collapsed;
                        Details_6.Visibility = Visibility.Collapsed;
                        Details_7.Visibility = Visibility.Collapsed;

                    }
                break;

            }
            MenuManagerAddons();
        }


        public virtual void MenuManagerAddons()
        {

        }

        public virtual void ShowExtraDetails(string Name, int Salary)
        {


        }






    }





}
