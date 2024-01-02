using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace BoilerPlateDraft.Code_Repo
{
    public class DatabaseConnection
    {
        private string connectionString = "Server=LAPTOP-GUKAUC7S\\SQLEXPRESS;Database=ITAM;User ID=arice11;Password=Dyeflame2!";
        //create a database connection and pass a sql query
        BoilerPlateDraft.Code_Repo.AzureSQLServerConnection dbconn = new BoilerPlateDraft.Code_Repo.AzureSQLServerConnection();

        public int GetSequenceNumber(string query)
        {
            int count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                count = (int)cmd.ExecuteScalar();
                conn.Close();
            }
            return count;
        }

        public DataTable SearchInventoryItem(string searchValue, string searchType, string endDate = "")
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                string queryBegining = "SELECT * FROM tblInventory WHERE ";

                if (searchType == "barcode")
                {
                    cmd.CommandText = queryBegining + "InventoryIdentifierBarCode = @SearchValue";
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue);
                }
                else if (searchType == "assignedUser")
                {
                    cmd.CommandText = queryBegining + "AssignedUser LIKE @SearchValue";
                    cmd.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");
                }
                else if (searchType == "serialNumber")
                {
                    cmd.CommandText = queryBegining + "SerialNumber = @SearchValue";
                    cmd.Parameters.AddWithValue("@SearchValue", searchValue);
                }
                else if (searchType == "purchasedDate")
                {
                    cmd.CommandText = queryBegining + "PurchasedDate BETWEEN @StartDate AND @EndDate";
                    cmd.Parameters.AddWithValue("@StartDate", searchValue);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM tblInventory";
                }


                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                    conn.Close();
                }
            }
            return dt;
        }



        public void UpdateInventoryItem(int inventoryItemId, string assignedUser, string buildingName, string roomNumber, string notes, string deviceStatus)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE tblInventory SET AssignedUser = @AssignedUser, BuildingName = @BuildingName, RoomNumber = @RoomNumber, Notes = @Notes, DeviceStatus = @DeviceStatus WHERE InventoryItemID = @InventoryItemID", conn);

                cmd.Parameters.AddWithValue("@AssignedUser", assignedUser);
                cmd.Parameters.AddWithValue("@BuildingName", buildingName);
                cmd.Parameters.AddWithValue("@RoomNumber", roomNumber);
                cmd.Parameters.AddWithValue("@Notes", notes);
                cmd.Parameters.AddWithValue("@DeviceStatus", deviceStatus);
                cmd.Parameters.AddWithValue("@InventoryItemID", inventoryItemId);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public string InsertItemIntoInventory(Asset asset)
        {
            string aString = "";
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;

                        cmd.CommandText = "INSERT INTO tblInventory(InventoryIdentifierBarCode, Make, ModelNumber, AssignedUser, SerialNumber, OSVersion, OSLicenseNumber, MachineName, " +
                            "LastLoggedInUser, RAMInGigabytes, MACAddress, LastLoggedIPAddress, CPUID, CPUSpeed, NumberOfProcessors, NumberOfRAMSlots, BuildingName, RoomNumber, " +
                            "AssetTypeID, CapitalItem, PurchasedDate, InventoryDate, Notes, DeviceStatus) " +
                                          "VALUES(@InventoryIdentifierBarCode, @Make, @ModelNumber, @AssignedUser, @SerialNumber, @OSVersion, @OSLicenseNumber, @MachineName, " +
                                          "@LastLoggedInUser, @RAMInGigabytes, @MACAddress, @LastLoggedIPAddress, @CPUID, @CPUSpeed, @NumberOfProcessors, @NumberOfRAMSlots, " +
                                          "@BuildingName, @RoomNumber, @AssetTypeID, @CapitalItem, @PurchasedDate, @InventoryDate, @Notes, @DeviceStatus)";

                        cmd.Parameters.AddWithValue("@InventoryIdentifierBarCode", asset.barCode);
                        cmd.Parameters.AddWithValue("@Make", asset.make);
                        cmd.Parameters.AddWithValue("@ModelNumber", asset.modelNumber);
                        cmd.Parameters.AddWithValue("@AssignedUser", asset.assignedUser);
                        cmd.Parameters.AddWithValue("@SerialNumber", asset.serialNumber);
                        cmd.Parameters.AddWithValue("@OSVersion", asset.osVersion);
                        cmd.Parameters.AddWithValue("@OSLicenseNumber", asset.osLicenseNumber);
                        cmd.Parameters.AddWithValue("@MachineName", asset.machineName);
                        cmd.Parameters.AddWithValue("@LastLoggedInUser", asset.lastLoggedInUser);
                        cmd.Parameters.AddWithValue("@RAMInGigabytes", asset.ramInGigabytes);
                        cmd.Parameters.AddWithValue("@MACAddress", asset.macAddress);
                        cmd.Parameters.AddWithValue("@LastLoggedIPAddress", asset.lastLoggedIPAddress);
                        cmd.Parameters.AddWithValue("@CPUID", asset.cpuId);
                        cmd.Parameters.AddWithValue("@CPUSpeed", asset.cpuSpeed);
                        cmd.Parameters.AddWithValue("@NumberOfProcessors", asset.numberOfProcessors);
                        cmd.Parameters.AddWithValue("@NumberOfRAMSlots", asset.numberOfRAMSlots);
                        cmd.Parameters.AddWithValue("@BuildingName", asset.buildingName);
                        cmd.Parameters.AddWithValue("@RoomNumber", asset.roomNumber);
                        cmd.Parameters.AddWithValue("@AssetTypeID", asset.assetTypeId);
                        cmd.Parameters.AddWithValue("@CapitalItem", asset.capitalItem);
                        cmd.Parameters.AddWithValue("@PurchasedDate", asset.purchasedDate);
                        cmd.Parameters.AddWithValue("@InventoryDate", asset.inventoryDate);
                        cmd.Parameters.AddWithValue("@Notes", asset.notes);
                        cmd.Parameters.AddWithValue("@DeviceStatus", asset.deviceStatus);


                        Boolean FoundRecords = false; //this logic assumes the user enters the correct value to search the database for. 


                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            FoundRecords = true;

                        }

                        conn.Close();
                        if (!FoundRecords) //If the lookup was not found let the user know.
                        {
                            conn.Close();

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


                }
            }
            return aString;
        }


    }


}

