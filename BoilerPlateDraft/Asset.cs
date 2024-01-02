using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using ZXing;

namespace BoilerPlateDraft.Code_Repo
{
    public class Asset
    {
        public string barCode { get; }
        public string make { get; }
        public string modelNumber { get; }
        public string assignedUser { get; }
        public string serialNumber { get; }
        public string osVersion { get; }
        public string osLicenseNumber { get; }
        public string machineName { get; }
        public string lastLoggedInUser { get; }
        public string ramInGigabytes { get; }
        public string macAddress { get; }
        public string lastLoggedIPAddress { get; }
        public string cpuId { get; }
        public string cpuSpeed { get; }
        public string numberOfProcessors { get; }
        public string numberOfRAMSlots { get; }
        public string buildingName { get; }
        public string roomNumber { get; }
        public string assetTypeId { get; }
        public bool capitalItem { get; }
        public string purchasedDate { get; }
        public string inventoryDate { get; }
        public string recordGeneratedDate { get; }
        public string notes { get; }
        public string deviceStatus { get; }
        public System.Drawing.Bitmap assetImage { get; set; }
        public string barcodeImageUrl { get; set; }


        public Asset(string make, string assetTypeId, bool capitalItem, string modelNumber = "", string assignedUser = "", 
            string serialNumber= "", string osVersion= "", string osLicenseNumber = "", string machineName = "", 
            string lastLoggedInUser = "", string ramInGigabytes = "", string macAddress = "", string lastLoggedIPAddress = "", 
            string cpuId = "", string cpuSpeed = "", string numberOfProcessors = "", string numberOfRAMSlots = "", 
            string buildingName = "", string roomNumber = "", string purchasedDate = "", string notes = "", string deviceStatus = "")
        {
            this.make = make;
            this.modelNumber = modelNumber;
            this.assignedUser = assignedUser;
            this.serialNumber = serialNumber;
            this.osVersion = osVersion;
            this.osLicenseNumber = osLicenseNumber;
            this.machineName = machineName;
            this.lastLoggedInUser = lastLoggedInUser;
            this.ramInGigabytes = ramInGigabytes;
            this.macAddress = macAddress;
            this.lastLoggedIPAddress = lastLoggedIPAddress;
            this.cpuId = cpuId;
            this.cpuSpeed = cpuSpeed;
            this.numberOfProcessors = numberOfProcessors;
            this.numberOfRAMSlots = numberOfRAMSlots;
            this.buildingName = buildingName;
            this.roomNumber = roomNumber;
            this.assetTypeId = assetTypeId;
            this.capitalItem = capitalItem;
            this.purchasedDate = purchasedDate;
            this.inventoryDate = DateTime.Now.ToString("yyyy-MM-dd");
            this.notes = notes;
            this.deviceStatus = deviceStatus;
            barCode = GetBarcodeInfo();
            GenerateBarcode(barCode);
            barcodeImageUrl = ConvertBitmapToImageUrl(assetImage);
        }

        private void GenerateBarcode(string data)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_93,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 100,
                    Width = 300
                }
            };
            assetImage = writer.Write(data);
        }

        private string ConvertBitmapToImageUrl(System.Drawing.Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = memoryStream.ToArray();
                return "data:image/png;base64," + Convert.ToBase64String(imageBytes);
            }
        }

        private string GetBarcodeInfo()
        {
            BoilerPlateDraft.Code_Repo.DatabaseConnection dbconn = new BoilerPlateDraft.Code_Repo.DatabaseConnection();
            string year = purchasedDate.Substring(2, 2);
            string type = assetTypeId;
            string yearType = year + type;
            string query = "SELECT COUNT(*) FROM tblInventory WHERE InventoryIdentifierBarCode LIKE '" + yearType + "%'";

            int sequenceQuantity = dbconn.GetSequenceNumber(query) + 1;
            string assetNumber = assetNumberGenerator(Int32.Parse(year), Int32.Parse(assetTypeId), sequenceQuantity);
            return assetNumber;
            
        }

        // method to generate an asset number for the barcode
        private static string assetNumberGenerator(int yearPurchased, int deviceType, int purchasedNumber)
        {
            // Make sure the values entered are within range
            if (yearPurchased < 0 || yearPurchased > 99 || deviceType < 1 || deviceType > 9 || purchasedNumber < 1 || purchasedNumber > 999)
            {
                throw new Exception("invalid input");
            }
            else
            {
                // Generate the asset number
                string yearPurchasedString = yearPurchased.ToString();
                string purchasedNumberString = purchasedNumber.ToString();

                //add leading 0s for yearPurchasedString and purchasedNumberString
                while (yearPurchasedString.Length < 2)
                {
                    yearPurchasedString = "0" + yearPurchasedString;
                }

                while (purchasedNumberString.Length < 3)
                {
                    purchasedNumberString = "0" + purchasedNumberString;
                }

                string assetNumber = yearPurchasedString + deviceType.ToString() + purchasedNumberString;

                return assetNumber;
            }
        }

    }
}

