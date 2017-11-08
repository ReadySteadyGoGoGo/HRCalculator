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
using System.Security.Cryptography;

namespace SalaryCalc
{
    class AccountEditWindow : AccountAddWindow
    {
        string _name = "";
        Button _Resetpasswordbutton = new Button();
        public AccountEditWindow(string Name) : base(Name)
        {
            _name = Name;
            this.Title = "Edit Account";
            this.Width = 300;
            this.Height = 375;

            SetControls();


        }

         
        public void SetControls()
        {
            try
            {
                TextBox LogintextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "Logintextbox");
                TextBox PasswordBox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "Passwordbox");
                ComboBox RoleComboBox = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "Rolecombobox");
                Button Addbutton = (Button)LogicalTreeHelper.FindLogicalNode(this, "Addbutton");



                Button Resetpasswordbutton = new Button()
                {
                    Name = "Resetpasswordbutton",
                    Content = "Reset Password",
                    Background = Brushes.DarkSlateGray,
                    Foreground = Brushes.Azure,
                    Margin = new Thickness(50, 10, 50, 0)

                };

                this.Mainpanel.Children.Add(Resetpasswordbutton);


                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var User = _context.Users.SingleOrDefault(x => x.Employee.Name == _name);
                    if (User == null) return;

                    string Login = User.Login;
                    string Password = User.Password;


                    LogintextBox.Text = Login;
                    _Logintextbox = LogintextBox;

                    PasswordBox.Text = "";
                    PasswordBox.IsEnabled = false;

                    _Passwordtextbox = PasswordBox;


                    RoleComboBox.SelectedIndex = User.Role - 1;
                    _Rolecombobox = RoleComboBox;

                    Addbutton.Content = "Edit";

                    _Resetpasswordbutton = Resetpasswordbutton;
                    _Resetpasswordbutton.Click += _Resetpasswordbutton_Click;

                }

            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
       
        }

        private void _Resetpasswordbutton_Click(object sender, RoutedEventArgs e)
        {
            _Passwordtextbox.Text = "Qq123456";
        }

        public override void Addbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Logintextbox.Text.Trim();
                _Passwordtextbox.Text.Trim();

                if (_Logintextbox.Text.Length < 4)
                {
                    MessageBox.Show("Login must be at least 5 character long");
                    return;

                }


                UTF8Encoding utf8 = new UTF8Encoding();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string Hashedpassword = BitConverter.ToString(md5.ComputeHash(utf8.GetBytes(_Passwordtextbox.Text.Trim()))).Replace("-", "");


                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    var EditedUser = _context.Users.SingleOrDefault(x => x.Employee.Name == _name);
                    if (EditedUser == null) return;

                    EditedUser.Login = _Logintextbox.Text;
                    EditedUser.Password = Hashedpassword;

                    EditedUser.Role = _Rolecombobox.SelectedIndex + 1;

                    _context.SaveChanges();


                    ((AdminWindow)Application.Current.Windows[0]).SetAccount();
                    ((AdminWindow)Application.Current.Windows[0]).Details_7_textboxB2.Text = _Logintextbox.Text;
                    ((AdminWindow)Application.Current.Windows[0]).Details_7_textboxC2.Text = _Rolecombobox.SelectedValue.ToString();
                    MessageBox.Show("Account information has been updated");
                    this.Close();

                }
            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
                      
        }
    }
}
