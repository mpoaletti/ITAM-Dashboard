using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
//using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Excel = Microsoft.Office.Interop.Excel;

namespace WebApplication1
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

		protected void AllItemsReport_Click(object sender, EventArgs e)
		{
            BoilerPlateDraft.Code_Repo.DatabaseConnection dbconn = new BoilerPlateDraft.Code_Repo.DatabaseConnection();
            System.Data.DataTable dt;

            dt = dbconn.SearchInventoryItem("","","");

            //dt.Columns.Count

            Excel.Application objExcel = new Excel.Application();
            Excel.Workbook workBook;
            Excel.Worksheet workSheet;

            if(File.Exists("ITAM Report - All")) {
                workBook = objExcel.Workbooks.Add("ITAM Report - All");
                workSheet = workBook.Sheets["ITAM Report All"];
            }
            else {
                workBook = objExcel.Workbooks.Add();
                workSheet = workBook.Worksheets.Add();
                workSheet.Name = "ITAM Report All";
            }

            try{
                //set excelRowNum variable - set to 1 as Excel is 1 based and database/code is 0 based
                int excelRowNum = 1;
                
                //get column headers
                for(int i = 0; i < dt.Columns.Count; i++) {
                    workSheet.Cells[excelRowNum, i + 1].Value = dt.Columns[i].ColumnName;
                    workSheet.Cells[excelRowNum, i + 1].Interior.Color = System.Drawing.Color.Gray;
                    //workSheet.Columns[i + 1].AutoFit();
                }

                //Set excelRowNum to 2 as we have column headers in row 1, data to start in row 2.
                excelRowNum = 2;

                //set rowNum variable for database - 0 based
                int rowNum = 0;

                //get data from datatable
                foreach(DataRow row in dt.Rows) {
                    for(int i = 0; i < dt.Columns.Count; i++) {
                        workSheet.Cells[excelRowNum,i+1].Value = row[i].ToString();
                    }
                    rowNum++;
                    excelRowNum++;
                }

                //Set column size to autofit data entered
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    workSheet.Columns[i + 1].AutoFit();
                }

            }
            catch (Exception ex)
            {

            }
            try{
                workBook.SaveAs("ITAM Report - All");
                workBook.Close();
                objExcel.Quit();
            }
            catch (Exception ex)
            {

            }
        }
	}
}