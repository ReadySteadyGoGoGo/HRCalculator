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
    class ChangePasswordWindow : AccountAddWindow
    {
        Button _checkButton = new Button();
        PasswordBox _passwordBox1 = new PasswordBox();
        PasswordBox _passwordBox2 = new PasswordBox();
        TextBox _newpasswordBox = new TextBox();


        private string _name = "";
        private string _login = "";
        public ChangePasswordWindow(string Name, string Login) :base(Name)
        {
            this.Title = "Change Account Password";
            this.Width = 300;
            this.Height = 400;
            _name = Name;
            _login = Login;
            SetInitial();
       
        }

        public void SetInitial()
        {
            try
            {
                Button Addbutton = (Button)LogicalTreeHelper.FindLogicalNode(this, "Addbutton");
                ComboBox RoleComboBox = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "Rolecombobox");
                Label RoleLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "Rolelabel");
                TextBox Passwordbox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "Passwordbox");


                this.Mainpanel.Children.Remove(Addbutton);
                this.Mainpanel.Children.Remove(RoleComboBox);
                this.Mainpanel.Children.Remove(RoleLabel);
                this.Mainpanel.Children.Remove(Passwordbox);

                TextBox Logintextbox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "Logintextbox");
                Logintextbox.Text = _login;
                Logintextbox.IsEnabled = false;

                Label Passwordlabel_1 = (Label)LogicalTreeHelper.FindLogicalNode(this, "Passwordlabel");
                Passwordlabel_1.Name = "Passwordlabel_1";
                Passwordlabel_1.Content = "Current password";
                Passwordlabel_1.Width = 180;
                Passwordlabel_1.Margin = new Thickness(0, 0, 60, 0);




                Label Passwordlabel_2 = new Label()
                {
                    Name = "Passwordlabel_2",
                    Content = "Repeat Current password",
                    FontSize = 14,
                    Foreground = Brushes.Azure,
                    Width = 180,
                    Margin = new Thickness(0, 0, 60, 0)
                };

                Label NewPasswordlabel = new Label()
                {
                    Name = "NewPasswordlabel",
                    Content = "Type new password",
                    FontSize = 14,
                    Foreground = Brushes.Azure,
                    Width = 180,
                    Margin = new Thickness(0, 0, 60, 0)
                };



                PasswordBox Passwordbox_1 = new PasswordBox()
                {
                    Name = "Passwordbox_1",
                    Foreground = Brushes.SteelBlue,
                    Background = Brushes.White,
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 0)

                };

                PasswordBox Passwordbox_2 = new PasswordBox()
                {
                    Name = "Passwordbox_2",
                    Foreground = Brushes.SteelBlue,
                    Background = Brushes.White,
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 0)

                };

                TextBox NewPasswordbox = new TextBox()
                {
                    Name = "NewPasswordbox",
                    Foreground = Brushes.SteelBlue,
                    Background = Brushes.White,
                    FontSize = 16,
                    IsEnabled = false,
                    Margin = new Thickness(10, 0, 10, 0)
                };

                Button Checkbutton = new Button()
                {
                    Name = "Checkbutton",
                    Content = "Check",
                    Background = Brushes.DarkSlateGray,
                    Foreground = Brushes.Azure,
                    Margin = new Thickness(50, 10, 50, 0)
                };


                Mainpanel.Children.Add(Passwordbox_1);
                Mainpanel.Children.Add(Passwordlabel_2);
                Mainpanel.Children.Add(Passwordbox_2);
                Mainpanel.Children.Add(NewPasswordlabel);
                Mainpanel.Children.Add(NewPasswordbox);
                Mainpanel.Children.Add(Checkbutton);


                _checkButton = Checkbutton;
                _passwordBox1 = Passwordbox_1;
                _passwordBox2 = Passwordbox_2;
                _newpasswordBox = NewPasswordbox;
                _checkButton.Click += _checkButton_Click;
            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }
                     
        }

        private void _checkButton_Click(object sender, RoutedEventArgs e)
        {

            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            if (!_newpasswordBox.IsEnabled)
            {
                try
                {
                    if ((_passwordBox1.Password == "") && (_passwordBox2.Password == ""))
                    {
                        MessageBox.Show("Password field mustn't be empty");
                        return;
                    }

                    if (_passwordBox1.Password != _passwordBox2.Password)
                    {
                        MessageBox.Show("Passwords don't match");
                        return;

                    }

                    if (_passwordBox1.Password.Length < 6)
                    {
                        MessageBox.Show("Password must be at least 6 character long");
                        return;
                    }


                    string Hashedpassword = BitConverter.ToString(md5.ComputeHash(utf8.GetBytes(_passwordBox1.Password.Trim()))).Replace("-", "");

                    using (EmployeesDBContext _context = new EmployeesDBContext())
                    {
                        var User = _context.Users.SingleOrDefault(x => x.Employee.Name == _name);
                        if (User == null) return;

                        if (User.Password == Hashedpassword)
                        {
                            _passwordBox1.IsEnabled = false;
                            _passwordBox2.IsEnabled = false;
                            _newpasswordBox.IsEnabled = true;
                            _checkButton.Content = "Change Password";
                        }
                    }

                }
                catch(Exception exc)
                {
                    WriteExceptionToDatabase(exc);
                }
           
            }
            else
            {
                try
                {
                    if (_newpasswordBox.Text.Length < 6)
                    {
                        MessageBox.Show("New Password must be at least 6 character long");
                        return;
                    }
                    string Hashedpassword = BitConverter.ToString(md5.ComputeHash(utf8.GetBytes(_newpasswordBox.Text.Trim()))).Replace("-", "");
                    using (EmployeesDBContext _context = new EmployeesDBContext())
                    {
                        var User = _context.Users.SingleOrDefault(x => x.Employee.Name == _name);
                        User.Password = Hashedpassword;
                        _context.SaveChanges();
                        MessageBox.Show("Password has been successfuly changed");
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
}
