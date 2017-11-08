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
    class AccountAddWindow : NewEmployee
    {
       private string _name = "";
       public Button _Addbutton = new Button();
       public TextBox _Logintextbox = new TextBox();
       public TextBox _Passwordtextbox = new TextBox();
       public ComboBox _Rolecombobox = new ComboBox();

        public AccountAddWindow(string Name)
        {
            this.Title = "Add New Account";
            this.Width = 300;
            this.Height = 350;
            _name = Name;
 
            InitializeComponent();
         
            InitialSet();
                 
        }

        public void InitialSet()
        {
            try
            {
                Button CreateBtn = (Button)LogicalTreeHelper.FindLogicalNode(this, "CreateBtn");
                TextBox SalaryTextbox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "SalarytextBox");
                ComboBox ChiefcomboBox = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "ChiefcomboBox");
                ComboBox GroupcomboBox = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "GroupcomboBox");
                DatePicker HiredatePicker = (DatePicker)LogicalTreeHelper.FindLogicalNode(this, "HiredatePicker");
                Label SalaryLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "SalaryLabel");
                Label ChiefLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "ChiefLabel");
                Label GroupLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "GroupLabel");
                Label HireDateLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "HireDateLabel");


                this.Mainpanel.Children.Remove(CreateBtn);
                this.Mainpanel.Children.Remove(SalaryTextbox);
                this.Mainpanel.Children.Remove(ChiefcomboBox);
                this.Mainpanel.Children.Remove(GroupcomboBox);
                this.Mainpanel.Children.Remove(HiredatePicker);
                this.Mainpanel.Children.Remove(SalaryLabel);
                this.Mainpanel.Children.Remove(ChiefLabel);
                this.Mainpanel.Children.Remove(GroupLabel);
                this.Mainpanel.Children.Remove(HireDateLabel);



                Label NameLabel = (Label)LogicalTreeHelper.FindLogicalNode(this, "NameLabel");
                TextBox NametextBox = (TextBox)LogicalTreeHelper.FindLogicalNode(this, "NametextBox");

                TextBox Logintextbox = new TextBox()
                {
                    Name = "Logintextbox",
                    Foreground = Brushes.SteelBlue,
                    Background = Brushes.White,
                    FontSize = 16,

                };

                Thickness Loginmargin = Logintextbox.Margin;
                Loginmargin.Left = 10;
                Loginmargin.Right = 10;
                Loginmargin.Top = 0;
                Logintextbox.Margin = Loginmargin;


                Label Loginlabel = new Label()
                {
                    Name = "Loginlabel",
                    Content = "Login",
                    FontSize = 14,
                    Foreground = Brushes.Azure,
                    Width = 60,
                    Margin = new Thickness(0, 0, 180, 0)
                };


                Label Passwordlabel = new Label()
                {
                    Name = "Passwordlabel",
                    Content = "Password",
                    FontSize = 14,
                    Foreground = Brushes.Azure,
                    Width = 80,
                    Margin = new Thickness(0, 0, 155, 0)
                };

                Label Rolelabel = new Label()
                {
                    Name = "Rolelabel",
                    Content = "Role",
                    FontSize = 14,
                    Foreground = Brushes.Azure,
                    Width = 60,
                    Margin = new Thickness(0, 0, 180, 0)
                };

                TextBox Passwordbox = new TextBox()
                {
                    Name = "Passwordbox",
                    Foreground = Brushes.SteelBlue,
                    Background = Brushes.White,
                    FontSize = 16,
                    Margin = new Thickness(10, 0, 10, 0)

                };

                ComboBox Rolecombobox = new ComboBox()
                {
                    Name = "Rolecombobox",
                    Margin = new Thickness(10, 0, 10, 0)

                };


                List<string> RoleList = new List<string>() { "Employee", "Chief", "Admin" };
                Rolecombobox.ItemsSource = RoleList;
                Rolecombobox.SelectedIndex = 0;


                Button Addbutton = new Button()
                {
                    Name = "Addbutton",
                    Content = "Add",
                    Background = Brushes.DarkSlateGray,
                    Foreground = Brushes.Azure,
                    Margin = new Thickness(50, 10, 50, 0)

                };


                Mainpanel.Children.Add(Loginlabel);
                Mainpanel.Children.Add(Logintextbox);
                Mainpanel.Children.Add(Passwordlabel);
                Mainpanel.Children.Add(Passwordbox);
                Mainpanel.Children.Add(Rolelabel);
                Mainpanel.Children.Add(Rolecombobox);
                Mainpanel.Children.Add(Addbutton);


                NametextBox.IsEnabled = false;
                NametextBox.Text = _name;
                Passwordbox.Text = "Qq123456";


                _Addbutton = Addbutton;
                _Logintextbox = Logintextbox;
                _Passwordtextbox = Passwordbox;
                _Rolecombobox = Rolecombobox;
                _Addbutton.Click += Addbutton_Click;

            }
            catch(Exception exc)
            {
                WriteExceptionToDatabase(exc);
            }      
        }

        public virtual void Addbutton_Click(object sender, RoutedEventArgs e)
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

                if (_Passwordtextbox.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 character long");
                    return;
                }

                string SelectedRole = _Rolecombobox.SelectedValue.ToString();

                UTF8Encoding utf8 = new UTF8Encoding();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string hash = BitConverter.ToString(md5.ComputeHash(utf8.GetBytes(_Passwordtextbox.Text.Trim()))).Replace("-", "");


                using (EmployeesDBContext _context = new EmployeesDBContext())
                {
                    User NewUser = new User()
                    {
                        Login = _Logintextbox.Text,
                        Password = hash,
                        Role = (SelectedRole == "Employee") ? 1 :
                                  (SelectedRole == "Chief") ? 2 :
                                                              3,
                        Employee = _context.Employees.Single(x => x.Name == _name)
                    };

                    _context.Users.Add(NewUser);
                    _context.SaveChanges();


                    ((AdminWindow)Application.Current.Windows[0]).SetAccount();
                    ((AdminWindow)Application.Current.Windows[0]).Details_7_textboxB2.Text = _Logintextbox.Text;
                    ((AdminWindow)Application.Current.Windows[0]).Details_7_textboxC2.Text = SelectedRole;

                    MessageBox.Show("User " + _Logintextbox.Text + " was added to the database");
                    this.Close();
                }

            }
            catch (Exception exc)
            {
                WriteExceptionToDatabase(exc);
            } 
        }

       

    }
}
