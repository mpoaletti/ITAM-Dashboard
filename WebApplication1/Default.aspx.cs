using BoilerPlateDraft.Code_Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using BoilerPlateDraft;
using AjaxControlToolkit;

namespace WebApplication1
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If Not logged in, redirect to login page
            if (!Request.IsAuthenticated)
            {
                Session["Username"] = null;
                Session["logInStatus"] = false;
                Session["CanUpdateTable"] = false;
                Response.Redirect("LoginPage.aspx", false);
            }


            if (!IsPostBack)
            {
                //sets the purchased date to default to the current date
                PurchasedDateTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        
        protected void SearchResultsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            SearchResultsGridView.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void SearchResultsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //get the values in the grid view
            string assignedUser = ((TextBox)SearchResultsGridView.Rows[e.RowIndex].FindControl("AssignedUserTextBox")).Text;
            string buildingName = ((TextBox)SearchResultsGridView.Rows[e.RowIndex].FindControl("BuildingNameTextBox")).Text;
            string roomNumber = ((TextBox)SearchResultsGridView.Rows[e.RowIndex].FindControl("RoomNumberTextBox")).Text;
            string notes = ((TextBox)SearchResultsGridView.Rows[e.RowIndex].FindControl("NotesTextBox")).Text;
            string deviceStatus = ((AjaxControlToolkit.ComboBox)SearchResultsGridView.Rows[e.RowIndex].FindControl("DeviceStatusComboBox")).Text;
            int inventoryItemId = Convert.ToInt32(SearchResultsGridView.DataKeys[e.RowIndex].Value);

            DatabaseConnection dc = new DatabaseConnection();
            //call method in DatabaseConnection to update the database
            dc.UpdateInventoryItem(inventoryItemId, assignedUser, buildingName, roomNumber, notes, deviceStatus);

            SearchResultsGridView.EditIndex = -1;
            BindGridView();
        }

        protected void SearchResultsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            SearchResultsGridView.EditIndex = -1;
            BindGridView();
        }


        private void BindGridView()
        {
            DatabaseConnection dbconn = new DatabaseConnection();
            string searchValue = Session["SearchValue"] as string;
            string searchType = Session["SearchType"] as string;
            DataTable dt;

            if (searchType == "purchasedDate")
            {
                string endDate = Session["EndDate"] as string;
                dt = dbconn.SearchInventoryItem(searchValue, searchType, endDate);
            }
            else
            {
                dt = dbconn.SearchInventoryItem(searchValue, searchType);
            }

            SearchResultsGridView.DataSource = dt;
            SearchResultsGridView.DataBind();
        }



    }
}



