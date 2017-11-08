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
using System.Security.Cryptography;

namespace SalaryCalc
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string hash = BitConverter.ToString(md5.ComputeHash(utf8.GetBytes(UserpasswordBox.Password.Trim()))).Replace("-", "");

                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    if ((_context.Users.Count(x => x.Login == UsertextBox.Text) > 0) &&
                       (_context.Users.Count(x => x.Password == hash) > 0))
                    {

                        var User = _context.Users.Include(c => c.Employee).FirstOrDefault(x => x.Login == UsertextBox.Text);

                        switch (User.Role)
                        {
                            case 1:
                                {
                                    MessageBox.Show("Employee Role");
                                    EmployeeWindow eMain = new EmployeeWindow(User.Employee.Name, User.Role);
                                    eMain.Show();
                                    this.Close();

                                }
                                break;
                            case 2:
                                {
                                    MessageBox.Show("Manager Role");
                                    ManagerWindow mMain = new ManagerWindow(User.Employee.Name, User.Role);

                                    mMain.Show();
                                    this.Close();

                                }
                                break;
                            case 3:
                                {
                                    MessageBox.Show("Admin Role");
                                    AdminWindow aMain = new AdminWindow(User.Employee.Name, User.Role);
                                    aMain.Show();
                                    this.Close();
                                }
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Incorrect login or password");
                    }

                }


            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
                WriteExceptionToDatabase(exc);
            }

           
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
