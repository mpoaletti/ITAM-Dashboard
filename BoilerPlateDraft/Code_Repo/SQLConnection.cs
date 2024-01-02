//default usings added by visual studio
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//add the following usings to project
using System.Data.SqlClient;
using System.Windows;

namespace BoilerPlateDraft.Code_Repo
{
	public class SQLConnection
  {
    public void SQLConnect() {
      try {
        string connectionstring = "CONNECTION STRING - STORE ENCRYPTED IN APP.CONFIG AND RETRIEVE AND DECRYPT";

          using (SqlConnection conn = new SqlConnection(connectionstring)) 
          {
              conn.Open();
              using (SqlCommand cmd = new SqlCommand())
              {
                  cmd.Connection = conn;
                  //update with SQL Data Type, update value retrieval and uncomment all 3 lines

                  //var SQLParam = new SqlParameter("[VALUE]", SqlDbType.TYPE);
                  //SQLParam.Value = [VALUE];
                  //cmd.Parameters.Add(SQLParam);
                  cmd.CommandText = "SELECT * FROM [TABLE] WHERE [KEY]=[VALUE]";

                  //Example of above code, txtBarCode is a field from windows form application in below example:

                  //var SQLParam = new SqlParameter("MyBarcode", SqlDbType.Int);
                  //SQLParam.Value = txtBarCode.Text;
                  //cmd.Parameters.Add(SQLParam);
                  //cmd.CommandText = "SELECT * FROM tblInventory WHERE InventoryIdentifierBarCode=@MyBarCode;";

                  SqlDataReader dr = cmd.ExecuteReader();
                  
                  while (dr.Read()) {
                        //DO SOME ACTIONS     
                  }
                
                  //close the connection
                  conn.Close();

                }
           }
      }

      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
	  }
  }
}