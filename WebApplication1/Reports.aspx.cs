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

                int excelRowNum = 1;
                
                //get column headers
                for(int i = 0; i < dt.Columns.Count; i++) {
                    workSheet.Cells[excelRowNum, i + 1].Value = dt.Columns[i].ColumnName;
                    workSheet.Cells[excelRowNum, i + 1].Interior.Color = System.Drawing.Color.Gray;
                    workSheet.Columns[i + 1].AutoFit();
                    //Excel.Range columnHeader = workSheet.Range[workSheet.Cells[excelRowNum, i + 1]];
                    //columnHeader.Interior.Color = System.Drawing.Color.Gray;
                    //Excel.Range rng = workSheet.Columns[i + 1];
                    //rng.AutoFit();
                }

                //Set excelRowNum to 1 as Excel is 1 based, not 0 based.
                excelRowNum = 2;

                int rowNum = 0;
                foreach(DataRow row in dt.Rows) {
                    for(int i = 0; i < dt.Columns.Count; i++) {
                        workSheet.Cells[excelRowNum,i+1].Value = row[i].ToString();
                    }
                    rowNum++;
                    excelRowNum++;
                }

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