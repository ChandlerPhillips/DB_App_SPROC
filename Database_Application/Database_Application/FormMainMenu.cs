using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Database_Application
{
    public partial class FormMainMenu : Form
    {
        public FormMainMenu()
        {
            InitializeComponent();
        }
        //******************************************************
        
        //*******************************************************
        

        private void buttonSalaryRaise_Click(object sender, EventArgs e)
        {
            //Open FormDisplaySpecificEmp when this button is clicked.

            //Hide the current form first
            this.Hide();

            //Display the other form from this form
            FormSalaryRaise formOther = new FormSalaryRaise();   //must create a new object instance of the another form first
            formOther.Show();    //then show the form
        }

        private void updateHours_Click(object sender, EventArgs e)
        {
            FormUpdateHours formUpdateHours1 = new FormUpdateHours();
            this.Hide();
            formUpdateHours1.Show();
        }
        //*******************************************************



    } //partial class
} //namespace
