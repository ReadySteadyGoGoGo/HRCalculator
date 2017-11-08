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

namespace SalaryCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public enum Roles
        {
            Employee = 1,
            Chief = 2,
            Admin = 3
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
                        Source = exc.StackTrace,
                        Details = exc.Message
                    };
                    _exceptioncontext.ExceptionNotes.Add(excNote);
                    _exceptioncontext.SaveChanges();
                }
            }
            catch (Exception exc1)
            {
                MessageBox.Show(exc1.Message);
                MessageBox.Show("Something went out of control");
            }
        }

    }
}
