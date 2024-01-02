//use the below imports
using Microsoft.SqlServer;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Windows;
using System.Web.Configuration;
using System.Net.Mail;



//Add this to your procudeure to look up info in a database and return one or more records.  If no records are found, provide a Messagebox to 
//inform the user the search did not produce any records. 

namespace BoilerPlateDraft.Code_Repo
{
    public class AzureSQLServerConnection
    {

        //public string AzureSQLConnect(int InventoryIdentifierBarCode, string Make, int AssetTypeID, int CapitalItem)
        public void AzureSQLConnect()
        {
            //string aString = "";
            try
            {

                //Decrypt ConnectionString - value stored encrypted in App.config file along with unencrypted key for decrypting
                string connectionstring = EncryptionDecryption.DecryptString(WebConfigurationManager.AppSettings.Get("encryptKey"), WebConfigurationManager.AppSettings.Get("connectionstring"));
                //string connectionstring = "Server=LAPTOP-GUKAUC7S\\SQLEXPRESS;Database=ITAM;User ID=arice11;Password=Dyeflame2!";

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    /*
                     * System.Data.SqlClient.SqlException: 'A network-related or instance-specific error occurred while establishing a 
                     * connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct 
                     * and that SQL Server is configured to allow remote connections. (provider: Named Pipes Provider, 
                     * error: 40 - Could not open a connection to SQL Server)'
                     * 
                     */
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        var SQLParam = new SqlParameter("SQLParameterName", SqlDbType.Int);
                        SQLParam.Value = ""; //txtBarCode.Text;      --Get Value from FORM FIELD
                        cmd.Parameters.Add(SQLParam);
                        cmd.CommandText = "SQL Statement=@SQLParameterName;";
                        //cmd.CommandText = "SELECT * FROM tblInventory;";
                        //cmd.CommandText = "INSERT INTO tblInventory(InventoryIdentifierBarCode, Make, AssetTypeID, CapitalItem) VALUES(" + InventoryIdentifierBarCode + "," + Make + "," + AssetTypeID + "," + CapitalItem +");";

                        Boolean FoundRecords = false; //this logic assumes the user enters the correct value to search the database for. 

                        //There are other ways to assign values in a db to variables or text boxes etc. but a datareader allows you to step through
                        //the data retruned and assign it to text box or variable in your code. 

                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FoundRecords = true;
                            //aString = dr["InventoryIdentifierBarCode"].ToString();
                            //VALUES BELOW COME FROM FORM FIELDS THAT DON'T EXIST HERE

                            //txtBarCode.Text = dr["InventoryIdentifierBarCode"].ToString();
                            //txtMake.Text = dr["Make"].ToString();
                            //txtModel.Text = dr["ModelNumber"].ToString();
                            //txtAssignedUser.Text = dr["AssignedUser"].ToString();
                            //txtSerialNumber.Text = dr["SerialNumber"].ToString();
                            //txtOSVersion.Text = dr["OSVersion"].ToString();
                            //txtLicense.Text = dr["OSLicenseNumber"].ToString();
                            //txtMachineName.Text = dr["MachineName"].ToString();
                            //txtRAM.Text = dr["RAMInGigabytes"].ToString();
                            //txtMACAddress.Text = dr["MACAddress"].ToString();
                            //txtIPAddress.Text = dr["LastLoggedIPAddress"].ToString();
                            //txtCPU.Text = dr["CPUID"].ToString();
                            //txtCPUSpeed.Text = dr["CPUSpeed"].ToString();
                            //txtNoProcessors.Text = dr["NumberOfProcessors"].ToString();
                            //txtRAMSlots.Text = dr["NumberOfRAMSlots"].ToString();
                            //cbxBuildingName.Text = dr["BuildingName"].ToString();
                            //txtRoomNumber.Text = dr["RoomNumber"].ToString();
                            //cbxAssetType.Text = dr["AssetTypeID"].ToString();
                            //chkCapitalItem.Text = dr["CapitalItem"].ToString();
                            //txtPurchaseDate.Text = dr["PurchasedDate"].ToString();
                        }

                        conn.Close();
                        if (!FoundRecords) //If the lookup was not found let the user know.
                        {
                            conn.Close();
                            //MessageBox.Show("The barcode number was not found in the database.");

                            //VALUES COME FROM FORM FIELDS    

                            //txtBarCode.Text = "";
                            //txtBarCode.Focus();
                        }
                    }
                }

            }


            catch (Exception ex)
            {
                string connectionstring = EncryptionDecryption.DecryptString(ConfigurationManager.AppSettings.Get("encryptKey"), ConfigurationManager.AppSettings.Get("connectionstring"));

                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Close();

                    //MessageBox.Show(ex.Message);

                }
            }
            //return aString;
        }
        //public string AzureSQLConnect(int InventoryIdentifierBarCode, string Make, int AssetTypeID, int CapitalItem)
        


        //Check if the user is Admin on their local computer and prevent users from running an app intended for endpoint. 
        //If they are not - close the application after letting them know they cannot run the applicaiton.
        //If they feel they should be able to then they can contact the Director of Professional Services for IT/Tech Services 
        private static bool IsAdministrationRules()
        {
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    bool b = (new WindowsPrincipal(identity)).IsInRole(WindowsBuiltInRole.Administrator);
                    if (!b)
                    {
                        //MessageBox.Show("You are not Administrator on your PC.  Contact the help desk and submit a ticket for Professional Services");
                        //FOR USE IN FORM APPLICATION NOT WEB APP
                        //System.Windows.Forms.Application.Exit();

                    }
                    return b;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}