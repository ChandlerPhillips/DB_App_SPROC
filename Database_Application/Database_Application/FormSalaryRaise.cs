using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace Database_Application
{
    public partial class FormSalaryRaise : Form
    {
        public FormSalaryRaise()
        {
            InitializeComponent();
        }
        //********************************************************
        private void FormSalaryRaise_Load(object sender, EventArgs e)
        {
            //When the form loads, do some initial housekeeping

            //Clear form controls of old data
            Reset();
           

            //Call user-defined external method to populate the combo box from Database
            Fill_Employee_ComboBox();
        }
        //**********************************************************

        private void Fill_Employee_ComboBox()
        {
            //This method fills the employee combo box by connecting to the database

            //********************************************
            //Fills the Employee name combo box with data
            //by calling using SQL 
            //********************************************

            //Step 1: CONNECTION OBJECT
            //*************************
            //Define a SQL Connection Object using my connection string stored in MyClass

            SqlConnection objMyConn;                                //create SQL connection variable
            objMyConn = new SqlConnection();                        //create new SQL connection object instance
            objMyConn.ConnectionString = Singh.myConnectionString;//assign my connection string to property

            //Step 2: SQL COMMAND OBJECT
            //**************************
            //Define a SQL Command object and set properties
            SqlCommand objMyCmd;                                    //create SQL command variable
            objMyCmd = new SqlCommand();                            //create command object
            objMyCmd.CommandType = CommandType.Text;               //assign command type that is SQL text

            String sqlQuery;     //string to store the required query

            //Formulate SQL query to be used piece-by-piece to avoid a long code line
            
            sqlQuery = "SELECT emp_last_name + ', ' + emp_first_name AS [Name], emp_ssn   ";
            sqlQuery += "   FROM employee ";
            sqlQuery += "  ORDER BY [Name]";


            objMyCmd.CommandText = sqlQuery;          //assign formulated SQL Query to Command object
            objMyCmd.Connection = objMyConn;          //assign connection obj to Connection property

            //Step 3: DATA ADAPTER
            //********************
            //Define a Data Adapter object and set its properties

            SqlDataAdapter objMyDataAdapter;                         //create a SqlDataAdapter variable
            objMyDataAdapter = new SqlDataAdapter();                 //create an object instance for SqlDataAdapter
            objMyDataAdapter.SelectCommand = objMyCmd;               //assign the command to run (i.e., SQL Statement)

            try
            {
                //Step 4: OPEN CONNECTION & RUN via the DataAdapter
                //*******************************************************
                //Open the connection to BPA-SQLServer first using connection object
                objMyConn.Open();

                //Next, create the Data Set object to fill the result set with 
                DataSet objMyDataSet = new DataSet();


                //Next, EXECUTE the SPROC using the FILL method of the Data Adapter
                //Fill the returned result data into the data set object
                //Give an arbitrary name like "EmpData" to the result set returned

                objMyDataAdapter.Fill(objMyDataSet, "EmpData");

                //Check if the Result Set returned is empty 
                if (objMyDataSet.Tables["EmpData"].Rows.Count == 0)
                {
                    //Record does not exist; display error message
                    richTextBoxError.Text = "*****Display Error*****";

                }
                else
                {
                    //assign the returned result set as the data source for the Employee Combo Box
                    comboBoxEmployee.DataSource = objMyDataSet.Tables["EmpData"];

                    //set the properties of Combo Box
                    comboBoxEmployee.DisplayMember = "Name";  //display the alias "Name" in the box
                    comboBoxEmployee.ValueMember = "emp_ssn";     //however, the SSN will be used as the value 
                    comboBoxEmployee.SelectedIndex = -1;      //do not select any options yet

                }

            }

            catch (Exception ex)
            {
                richTextBoxError.Text = ex.Message;
            }

            finally
            {
                //Always close the connection here
                //If the connection is currently open, close it.
                //If it is already closed, do not bother to close it again here since it will give an error

                if (Singh.myConnectionString != null && objMyConn.State == ConnectionState.Open)
                {
                    objMyConn.Close();
                }

                else
                {
                    //It is already closed; do not do anything; end the function
                }

            }

        }
        //**********************************************************
        private void buttonClear_Click(object sender, EventArgs e)
        {
            Reset();
        }
        //***********************************************************
        private void pictureBoxHelp_Click(object sender, EventArgs e)
        {
            string msg = null;

            msg += "To Give Salary Raise to an employee:";
            msg += "\r\n\r\n***Select a specific employee from the drop-down list.";
            msg += "\r\n***Enter the percent salary raise amount without the percent symbol, for example 10.";
            msg += "\r\n*** Press the GO button to give a raise.";

            MessageBox.Show(msg, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //**********************************************************
        private void pictureBoxGo_Click(object sender, EventArgs e)
        {
            //This event attempts to give the specified raise to the specified employee

            //VALIDATE employee combo box 
            if (comboBoxEmployee.SelectedIndex == -1)
            {
                MessageBox.Show("No Employee Selected! Try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                comboBoxEmployee.Focus();
                return;
            }

            //VALIDATE raise textbox
            double raise;  //variable to store converted valid raise

            if (double.TryParse(textBoxRaise.Text, out raise) && raise >= 0 )
            {
                //If raise input data is valid, call user-defined method to give raise
                Give_Employee_Raise();
            }
            else
            {
                MessageBox.Show("Invalid Raise Input Data. Try again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                textBoxRaise.Focus();
                return;
            }

        }
        //**********************************************************

        public void Give_Employee_Raise()
        {
            //This method atgtempts to give specified raise to a specified employee
            //It uses SPROC to give the raise
            //It also activates a Trigger to check the business rule automatically

            //Step 1: CONNECTION OBJECT
            //*************************
            //Define a SQL Connection Object using my connection string stored in MyClass

            SqlConnection objMyConn;                                //create SQL connection variable
            objMyConn = new SqlConnection();                        //create new SQL connection object instance
            objMyConn.ConnectionString = Singh.myConnectionString;//assign my connection string to property

            //Step 2: SQL COMMAND OBJECT
            //**************************
            //Define a SQL Command object and set properties
            SqlCommand objMyCmd;                                    //create SQL command variable
            objMyCmd = new SqlCommand();                            //create command object
            objMyCmd.CommandType = CommandType.StoredProcedure;     //assign command type that is a SPROC (not simple SQL Query!!!)
            objMyCmd.CommandText = "sproc_Raise_Salary_Specific_Employee";            //assign actual name of associated SPROC
            objMyCmd.Connection = objMyConn;                        //assign connection obj to Connection property

            //Step 3: DATA ADAPTER
            //********************
            //Define a Data Adapter object and set its properties

            SqlDataAdapter objMyDataAdapter;                         //create a SqlDataAdapter variable
            objMyDataAdapter = new SqlDataAdapter();                 //create an object instance for SqlDataAdapter
            objMyDataAdapter.SelectCommand = objMyCmd;               //assign the command to run (i.e., a SPROC)

            //~~~~~ Assign INPUT Parameters to SPROC ~~~~~

            //ASSIGN both <param names> and <data types> for INPUT parameters
            //Param names must match EXACTLY and data types must match with those used in SPROC AS CLOSELY AS POSSIBLE!!!

            objMyCmd.Parameters.Add(new SqlParameter("@emp_ssn", SqlDbType.Text));  //data type "Text" matches with "CHAR()" used in SPROC
            objMyCmd.Parameters.Add(new SqlParameter("@percent_raise", SqlDbType.Decimal));


            //ASSIGN <Values> for each INPUT parameter by getting values from form controls
            //Make sure to align the form control data types with those of SPROC parameters

            objMyCmd.Parameters["@emp_ssn"].Value = comboBoxEmployee.SelectedValue;
            objMyCmd.Parameters["@percent_raise"].Value = Decimal.Parse(textBoxRaise.Text.Trim());

            try
            {
                //Step 4: OPEN CONNECTION & RUN SPROC via the DataAdapter
                //*******************************************************
                //Open the connection to BPA-SQLServer first using connection object
                objMyConn.Open();

                //Next, create the Data Set object to fill the result set with 
                DataSet objMyDataSet = new DataSet();


                //Next, EXECUTE the SPROC using the FILL method of the Data Adapter
                //Fill the returned result data into the data set object
                //Give an arbitrary name to the result set returned

                objMyDataAdapter.Fill(objMyDataSet, "RaiseData");

                //Check if the Result Set returned is empty 
                if (objMyDataSet.Tables["RaiseData"].Rows.Count == 0)
                {
                    //Record does not exist; display error message
                    richTextBoxError.Text = "Sorry, query resulted in no hits. Please refine your entry and try again.";

                }
                else
                {
                    MessageBox.Show("Salary Update Successful.", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridViewSalaryRaise.DataSource = objMyDataSet.Tables["RaiseData"];

                }

            }

            catch (Exception ex)
            {
                richTextBoxError.Text = ex.Message;
            }

            finally
            {
                //Always close the connection here
                //If the connection is currently open, close it.
                //If it is already closed, do not bother to close it again here since it will give an error

                if (Singh.myConnectionString != null && objMyConn.State == ConnectionState.Open)
                {
                    objMyConn.Close();
                }

                else
                {
                    //It is already closed; do not do anything; end the function
                }

            }

        }
        //************************************************************
        private void Reset()
        {
            //This method resets all the form controls

            //Clear all form controls
            //Clear the DataGridView of ald data
            dataGridViewSalaryRaise.DataSource = null;
            dataGridViewSalaryRaise.Rows.Clear();

            //CLear the ERROR RichTextBoxform control
            richTextBoxError.Text = "";

            //Clear the input controls
            comboBoxEmployee.SelectedIndex = -1;
            textBoxRaise.Text = "";
        }
        //*********************************************************
        private void comboBoxEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            //This event fills the Employee SSN box in synch with the Employee name selected in the combo box
            //Clear the DataGridView of ald data
            dataGridViewSalaryRaise.DataSource = null;
            dataGridViewSalaryRaise.Rows.Clear();
            string empSSN;
            //Clear the error box
            richTextBoxError.Text = "";
            textBoxEmpSSN.Text = "";
                                

            if (comboBoxEmployee.SelectedIndex != -1)
            {
                textBoxEmpSSN.Text = comboBoxEmployee.SelectedValue.ToString();
                empSSN = textBoxEmpSSN.Text;
                textBoxEmpSSN.Text = empSSN.Substring(0, 3) + '-' + empSSN.Substring(3, 2) + '-' + empSSN.Substring(5, 4);
            }
            else
            {
                textBoxEmpSSN.Text = "";
            }
        }
        //*********************************************************
        private void textBoxRaise_TextChanged(object sender, EventArgs e)
        {
            //This event clears the grid view of old result
            //Clear the DataGridView of ald data
            dataGridViewSalaryRaise.DataSource = null;
            dataGridViewSalaryRaise.Rows.Clear();

            //Clear the error box
            richTextBoxError.Text = "";
        }
        //*******************************************************
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            //Returns to FormMainMenu

            //Hide the current form first
            this.Hide();

            //Display the other form from this form
            FormMainMenu formOther = new FormMainMenu();   //must create a new object instance of the another form first
            formOther.Show();    //then show the form
        }

        //*********************************************************


    } //partial class
} //namespace
