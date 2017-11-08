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
    class AdminWindow:ManagerWindow
    {
        private string _name = "";
        public AdminWindow(string Name, int Role) : base(Name, Role)
        {
            _name = Name;
            List<string> Menulist = new List<string> { "My Details", "My Collegues", "Salary Calculator","My SubOrdinates","SubOrdinates Salary","Department Salary", "Manage Employees", "Manage Accounts" };
            SetInitial(Menulist,_name,Role);

            Details_5_buttonA.Click += Details_5_buttonA_Click;
            Details_5_buttonB.Click += Details_5_buttonB_Click;
            Details_7_buttonA.Click += Details_7_button_Click;
            Details_7_buttonB.Click += Details_7_buttonB_Click;
        }


        public void SetNoAccount()
        {
            Details_7_textboxB2.Text = "Has no Account";
            Details_7_textboxB2.Foreground = Brushes.Red;


            Details_7_textboxC2.Text = "Has no Account";
            Details_7_textboxC2.Foreground = Brushes.Red;
            Details_7_buttonA.Content = "Add";
            Details_7_buttonB.Visibility = Visibility.Collapsed;
        }

        public void SetAccount()
        {
            Details_7_buttonA.Content = "Edit";
            Details_7_buttonB.Visibility = Visibility.Visible;
            Details_7_textboxB2.Foreground = Brushes.Black;
            Details_7_textboxC2.Foreground = Brushes.Black;

        }

        private void Details_7_buttonB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var SelectedUser = _context.Users.SingleOrDefault(x => x.Employee.Name == Details_7_textboxA2.Text);
                    if (SelectedUser == null) return;
                    _context.Users.Remove(SelectedUser);
                    _context.SaveChanges();

                    MessageBox.Show("Account " + Details_7_textboxB2.Text + " related to Employee " + Details_7_textboxA2.Text + " has been deleted");

                    SetNoAccount();

                }

            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
        
        }

        private void Details_7_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Details_7_textboxB2.Text == "Has no Account")
                {
                    AccountAddWindow Add = new AccountAddWindow(Details_7_textboxA2.Text);
                    Add.Show();

                }
                else
                {
                    AccountEditWindow Add = new AccountEditWindow(Details_7_textboxA2.Text);
                    Add.Show();

                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
           
        }

        private void Details_5_buttonB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EmployeeslistView.Items.Count > 0)
                {
                    int SelectedIndex = EmployeeslistView.SelectedIndex;
                    string SelectedEmployee = EmployeeslistView.SelectedValue.ToString();
                    using (EmployeesDBContext _context = new EmployeesDBContext())
                    {
                        var EmployeeToDetele = _context.Employees.SingleOrDefault(x => x.Name == SelectedEmployee);
                        _context.Employees.Remove(EmployeeToDetele);
                        _context.SaveChanges();
                    }
                    WholeList();
                }
                else
                {
                    MessageBox.Show("Employee List is Empty. Theres Nothing to Delete.");
                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
                
        }

        public override void Details_2_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditWindow edit = new EditWindow(Details_2_textboxA2.Text, (int)Roles.Admin);
                edit.Show();
            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }

           
        }

        private void Details_5_buttonA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NewEmployee newEmployee = new NewEmployee();
                newEmployee.Show();
            }
               catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
        }

 
        public void WholeList()
        {
            try
            {
                EmployeeslistView.IsEnabled = true;
                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var Employeelist = _context.Employees.ToList();
                    if (Employeelist == null)
                    {
                        EmployeeslistView.Items.Add("No Employees to Display");
                        EmployeeslistView.IsEnabled = false;
                        return;
                    }

                    EmployeeslistView.Items.Clear();

                    foreach (var Employee in Employeelist)
                    {
                        EmployeeslistView.Items.Add(Employee.Name);
                    }

                    if ((EmployeeslistView.Items.Count > 0) && 
                        (Menucombobox.SelectedValue.ToString() != "Department Salary")&&
                        (Menucombobox.SelectedValue.ToString() != "SubOrdinates Salary"))


                        EmployeeslistView.SelectedIndex = 0;
                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
        }


        public override void EmployeeslistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((Menucombobox.SelectedValue.ToString() == "SubOrdinates Salary") ||
                (Menucombobox.SelectedValue.ToString() == "Department Salary")) return;


            if (Menucombobox.SelectedValue.ToString() != "Manage Accounts")
                base.EmployeeslistView_SelectionChanged(this, null);
            else
            {
                try
                {
                    if (EmployeeslistView.SelectedIndex != -1)
                    {
                        Details_7_buttonB.Visibility = Visibility.Collapsed;
                        string SelectedEmployee = EmployeeslistView.SelectedValue.ToString();
                        Details_7_textboxA2.Text = SelectedEmployee;
                        using (EmployeesDBContext _context = new EmployeesDBContext())
                        {
                            var Login = _context.Users.SingleOrDefault(x => x.Employee.Name == SelectedEmployee);

                            if (Login == null)
                            {
                                SetNoAccount();
                            }
                            else
                            {

                                SetAccount();
                                Details_7_textboxB2.Text = Login.Login.ToString();
                                Details_7_textboxC2.Text = (Login.Role == 1) ? "Employee" :
                                                           (Login.Role == 2) ? "Chief" :
                                                                               "Admin";
                            }
                        }


                    }
                }
                catch (Exception exc)
                {
                    WriteExceptionToDatabase(exc);
                }               
            }
        }

        public override void MenuAdminAddons()
        {
            string SelectedMenu = Menucombobox.SelectedValue.ToString();
            switch (SelectedMenu)
            {
                case "Department Salary":
                    {

                        WholeList();
                        Details_6.Visibility = Visibility.Visible;
                        EmployeeslistView.Visibility = Visibility.Visible;
                        Details_6_datePicker.SelectedDate = DateTime.Now;
                        Details_6_label.Content = "Salary Statistics for the Department:";
                        

                        Details_1.Visibility = Visibility.Collapsed;
                        Details_2.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Collapsed;
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Collapsed;
                        Details_7.Visibility = Visibility.Collapsed;

                    }
                    break;


                case "Manage Employees":
                    {

                        WholeList();
                        EmployeeslistView.Visibility = Visibility.Visible;
                        Details_1.Visibility = Visibility.Collapsed;
                        Details_2.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Collapsed;
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Visible;
                        Details_6.Visibility = Visibility.Collapsed;
                        Details_7.Visibility = Visibility.Collapsed;
                    }
                    break;

                case "Manage Accounts":
                    {

                        WholeList();
                        Details_7.Visibility = Visibility.Visible;
                        EmployeeslistView.Visibility = Visibility.Visible;

                        Details_1.Visibility = Visibility.Collapsed;
                        Details_2.Visibility = Visibility.Collapsed;
                        Details_3.Visibility = Visibility.Collapsed;
                        Details_4.Visibility = Visibility.Collapsed;
                        Details_5.Visibility = Visibility.Collapsed;
                        Details_6.Visibility = Visibility.Collapsed;
                        
                    }
                    break;

            }

        }

    }
}
