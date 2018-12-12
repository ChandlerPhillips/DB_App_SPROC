using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;



namespace Database_Application
{
    public partial class FormUpdateHours : Form
    {
        public FormUpdateHours()
        {
            InitializeComponent();
        }
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            this.Hide();
            FormMainMenu formOther = new FormMainMenu();   
            formOther.Show();
            formOther = null;
        }

        private void FormWorkHoursUpdate_Load(object sender, EventArgs e)
        {
            //When the form loads, do some initial housekeeping

            //Clear form controls of old data
            Reset();

            //Call user-defined external method to populate the combo box from Database
            Fill_Employee_ComboBox();
        }

        private void Fill_Project_ComboBox()
        {
            //This method fills the project combo box by connecting to the database

            //********************************************
            //Fills the Project name combo box with data
            //by calling using SQL 
            //********************************************

            //Step 1: CONNECTION OBJECT
            //*************************
            //Define a SQL Connection Object using my connection string stored in MyClass

            SqlConnection objMyConn;                                  //create SQL connection variable
            objMyConn = new SqlConnection();                         //create new SQL connection object instance
            objMyConn.ConnectionString = Singh.myConnectionString;  //assign my connection string to property
            
            //Step 2: SQL COMMAND OBJECT
            //**************************
            //Define a SQL Command object and set properties
            SqlCommand objMyCmd;                                    //create SQL command variable
            objMyCmd = new SqlCommand();                            //create command object
            objMyCmd.CommandType = CommandType.StoredProcedure;     //assign command type this is SQL Procedure
            objMyCmd.CommandText = "sproc_Select_Specific_Project"; //Accessing procedure from database
            objMyCmd.Connection = objMyConn;                        //assign connection obj to Connection property
            
            //Step 3: DATA ADAPTER
            //********************
            //Define a Data Adapter object and set its properties
            SqlDataAdapter objMyDataAdapter;            //create a SqlDataAdapter variable
            objMyDataAdapter = new SqlDataAdapter();    //create an object instance for SqlDataAdapter
            objMyDataAdapter.SelectCommand = objMyCmd;  //assign the command to run (i.e., SQL Statement)
            objMyCmd.Parameters.Add(new SqlParameter("@work_emp_ssn",SqlDbType.Text));  //Obtaining work_emp_ssn
            objMyCmd.Parameters["@work_emp_ssn"].Value = comboBoxEmployee.SelectedValue; //Selecting work_emp_ssn value
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

                objMyDataAdapter.Fill(objMyDataSet, "ProjData");
                //Check if the Result Set returned is empty
                if (objMyDataSet.Tables["ProjData"].Rows.Count == 0)
                {
                    //Record does not exist; display error message
                    richTextBoxError.Text = "*****Display Error*****";
                }
                else
                {
                    //assign the returned result set as the data source for the Employee Combo Box
                    comboBoxProject.DataSource = objMyDataSet.Tables["ProjData"];
                    //set the properties of Combo Box
                    comboBoxProject.DisplayMember = "Project";
                    comboBoxProject.ValueMember = "Hours";
                    comboBoxProject.SelectedIndex = -1;
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

        private void comboBoxProject_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //This event fills the Employee SSN box in synch with the Employee name selected in the combo box
            //Clear the DataGridView of ald data
            textBoxNewHours.Text = "";
            dataGridViewWorkHours.DataSource = null;
            dataGridViewWorkHours.Rows.Clear();
            //Clear the error box
            richTextBoxError.Text = "";

            if (comboBoxProject.SelectedIndex != -1)
            {
                textBoxNewHours.Text = comboBoxProject.SelectedValue.ToString();
            }
            else
            {
                textBoxNewHours.Text = "";
            }
        }

        //*********************************************************

        private void Reset()
        {
            //This method resets all the form controls

            //Clear all form controls
            //Clear the DataGridView of ald data
            dataGridViewWorkHours.DataSource = null;
            dataGridViewWorkHours.Rows.Clear();
            
            //Clear the input controls
            comboBoxEmployee.SelectedIndex = -1;
            comboBoxProject.SelectedIndex = -1;

            //CLear the ERROR RichTextBoxform control
            textBoxEmpSSN.Text = "";
            textBoxNewHours.Text = "";
            richTextBoxError.Text = "";
        }
        

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Reset();    //Resets all information on screen
        }

        private void pictureBoxHelp_Click(object sender, EventArgs e)
        {
            String str = "";
            str += "To Update an employee's work hour of a specific project:";
            str += "\r\n\r\n***Select a specific employee from the drop-down list.";
            str += "\r\n\r\n***Select a specific project.";
            str += "\r\n\r\n***Enter the prefered NEW hours for the employee";
            str += "\r\n\r\n***Press the GO button to update.";
            MessageBox.Show(str, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //**********************************************************

        private void pictureBoxGo_Click(object sender, EventArgs e)
        {
            if (comboBoxEmployee.SelectedIndex == -1)
            {
                MessageBox.Show("Invalid employee data!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                comboBoxEmployee.Focus();
                return;
            }
            if (comboBoxProject.SelectedIndex == -1)
            {
                MessageBox.Show("Invalid project data!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                comboBoxProject.Focus();
                return;
            }
            decimal workHours;  
            if (decimal.TryParse(textBoxNewHours.Text, out workHours) && workHours >= 0)
            {
                Update_Work_Hours();
            }
            else
            {
                MessageBox.Show("Invalid new hours data!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                textBoxNewHours.Focus();
                return;
            }
        }

        private void textBoxNewHours_TextChanged(object sender, EventArgs e)
        {
            dataGridViewWorkHours.DataSource = null;
            richTextBoxError.Text = "";
        }

        private void Update_Work_Hours()
        {
            //This method fills the update hours combo box by connecting to the database

            //********************************************
            //Fills the update hours combo box with data
            //by calling using SQL 
            //********************************************

            //Step 1: CONNECTION OBJECT
            //*************************
            //Define a SQL Connection Object using my connection string stored in MyClass
            SqlConnection objConnection = new SqlConnection();
            objConnection.ConnectionString = Singh.myConnectionString;
            //Step 2: SQL COMMAND OBJECT
            //**************************
            //Define a SQL Command object and set properties
            SqlCommand objCommand = new SqlCommand();
            objCommand.CommandType = CommandType.StoredProcedure;
            objCommand.CommandText = "sproc_Update_Workhours_Specific_Employee_Project";
            objCommand.Connection = objConnection;
            //Step 3: DATA ADAPTER
            //********************
            //Define a Data Adapter object and set its properties
            SqlDataAdapter objDataAdapter = new SqlDataAdapter();
            objDataAdapter.SelectCommand = objCommand;
            objCommand.Parameters.Add(new SqlParameter("@empssn",SqlDbType.Text));
            objCommand.Parameters.Add(new SqlParameter("@projnum",SqlDbType.SmallInt));
            objCommand.Parameters.Add(new SqlParameter("@workhours",SqlDbType.Decimal));
            objCommand.Parameters["@empssn"].Value = comboBoxEmployee.SelectedValue;
            objCommand.Parameters["@projnum"].Value = comboBoxProject.Text.Trim();
            objCommand.Parameters["@workhours"].Value = Decimal.Parse(textBoxNewHours.Text.Trim());
            try
            {
                //Step 4: OPEN CONNECTION & RUN via the DataAdapter
                //*******************************************************
                //Open the connection to BPA-SQLServer first using connection object
                objConnection.Open();
                DataSet objDataSet = new DataSet();
                objDataAdapter.Fill(objDataSet, "UpdateData");
                if(objDataSet.Tables["UpdateData"].Rows.Count==0)
                {
                    richTextBoxError.Text = "Incorrect data entered. Please try again!";
                }
                else
                {
                    MessageBox.Show("Work Hours updated!", "CONGRATS!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridViewWorkHours.DataSource = objDataSet.Tables["UpdateData"];
                }
            }
            catch(Exception ex)
            {
                richTextBoxError.Text = ex.Message;
            }
            finally
            {
                if (Singh.myConnectionString != null && objConnection.State == ConnectionState.Open)
                {
                    objConnection.Close();
                }

                else
                {
                    
                }
            }
        }

        private void comboBoxEmployee_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //This event fills the Employee SSN box in synch with the Employee name selected in the combo box
            //Clear the DataGridView of ald data
            string empSSN = "";
            dataGridViewWorkHours.DataSource = null;
            dataGridViewWorkHours.Rows.Clear();
            //Clear the error box
            textBoxEmpSSN.Text = "";
            //Clear the error box
            textBoxNewHours.Text = "";
            comboBoxProject.SelectedIndex = -1;
            //Clear the error box
            richTextBoxError.Text = "";
            if (comboBoxEmployee.SelectedIndex != -1)
            {
                textBoxEmpSSN.Text = comboBoxEmployee.SelectedValue.ToString();
                empSSN = textBoxEmpSSN.Text;
                textBoxEmpSSN.Text = empSSN.Substring(0, 3) + '-' + empSSN.Substring(3, 2) + '-' + empSSN.Substring(5, 4);
                Fill_Project_ComboBox();
            }
            else
            {
                textBoxEmpSSN.Text = "";
                comboBoxProject.SelectedIndex = -1;
            }
        }
        //*********************************************************
    } 
} 
