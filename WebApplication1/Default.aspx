<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="ZXing" %>
<%@ Import Namespace="System.IO" %>


<script runat="server">
    void SaveButton_Click(object sender, System.EventArgs e)
    {
        // Check if AssetTypeIDComboBox has a valid int between 0 and 9
        int assetTypeId;
        bool isValidAssetType = int.TryParse(AssetTypeIDComboBox.SelectedValue, out assetTypeId) && assetTypeId >= 0 && assetTypeId <= 9;

        // if not exit and display a message to fill in required fields
        if (!isValidAssetType)
        {
            MessageLabel1.Visible = true;
            MessageLabel2.Visible = true;
            return;
        }

        BoilerPlateDraft.Code_Repo.DatabaseConnection dbconn = new BoilerPlateDraft.Code_Repo.DatabaseConnection();

        BoilerPlateDraft.Code_Repo.Asset asset = new BoilerPlateDraft.Code_Repo.Asset(
            make: MakeComboBox.SelectedValue,
            assetTypeId: AssetTypeIDComboBox.SelectedValue,
            capitalItem: CapitalItemCheckBox.Checked,
            modelNumber: ModelNumberComboBox.Text,
            assignedUser: AssignedUserTextBox.Text,
            serialNumber: SerialNumberTextBox.Text,
            osVersion: OSVersionTextBox.Text,
            osLicenseNumber: OSLicenseNumberTextBox.Text,
            machineName: MachineNameTextBox.Text,
            lastLoggedInUser: LastLoggedInUserTextBox.Text,
            ramInGigabytes: RAMInGigabytesTextBox.Text,
            macAddress: MACAddressTextBox.Text,
            lastLoggedIPAddress: LastLoggedIPAddressTextBox.Text,
            cpuId: CPUIDTextBox.Text,
            cpuSpeed: CPUSpeedTextBox.Text,
            numberOfProcessors: NumberOfProcessorsTextBox.Text,
            numberOfRAMSlots: NumberOfRAMSlotsTextBox.Text,
            buildingName: BuildingNameTextBox.Text,
            roomNumber: RoomNumberTextBox.Text,
            purchasedDate: PurchasedDateTextBox.Text,
            notes: NotesTextBox.Text,
            deviceStatus: DeviceStatusComboBox.SelectedValue
        );

        dbconn.InsertItemIntoInventory(asset);

        BarcodeImage.ImageUrl = asset.barcodeImageUrl;

        //Make the print button visible after save button is hit after barcode is visible
        PrintButton.Visible = true;

    }


    protected void SearchButton_Click(object sender, EventArgs e)
    {
        BoilerPlateDraft.Code_Repo.DatabaseConnection dbconn = new BoilerPlateDraft.Code_Repo.DatabaseConnection();
        string searchValue = SearchTextBox.Text;
        string searchType;

        DataTable dt;

        if (RadioBarcode.Checked)
        {
            searchType = "barcode";
            dt = dbconn.SearchInventoryItem(searchValue, searchType);
        }
        else if (RadioAssignedUser.Checked)
        {
            searchType = "assignedUser";
            dt = dbconn.SearchInventoryItem(searchValue, searchType);
        }
        else if (RadioSerialNumber.Checked)
        {
            searchType = "serialNumber";
            dt = dbconn.SearchInventoryItem(searchValue, searchType);
        }
        else if (RadioPurchasedDate.Checked)
        {
            searchType = "purchasedDate";
            string endDate = EndDateTextBox.Text;
            dt = dbconn.SearchInventoryItem(searchValue, searchType, endDate);
        }
        else
        {
            searchType = "barcode";
            dt = dbconn.SearchInventoryItem(searchValue, searchType);
        }

        SearchResultsGridView.DataSource = dt;
        try{
            SearchResultsGridView.DataBind();
        }
        catch (Exception ex)
            {
               
            }


        Session["SearchValue"] = searchValue;
        Session["SearchType"] = searchType;
        if (RadioPurchasedDate.Checked)
        {
            Session["StartDate"] = searchValue;
            Session["EndDate"] = EndDateTextBox.Text;
        }
        //Clear new entry text boxes, 
        ClearTextBoxes(this);

        //uncomment the following code to hide the print button and barcode on search button click
        //PrintButton.Visible = false;
        //BarcodeImage.ImageUrl = "";
    }


    private void ClearTextBoxes(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is TextBox && c.ID != "SearchTextBox")
            {
                ((TextBox)c).Text = "";
            }
            else if (c is AjaxControlToolkit.ComboBox)
            {
                ((AjaxControlToolkit.ComboBox)c).SelectedIndex = 0;
            }
            else if (c.HasControls())
            {
                ClearTextBoxes(c);
            }
        }
        PurchasedDateTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
    }



</script>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>

    <script>
        $(document).ready(function () {
            $("#SearchTextBox").focus();
            $("#<%= PurchasedDateTextBox.ClientID %>").datepicker({
                onSelect: function (dateText, inst) {
                    var date = $(this).datepicker('getDate');
                    var formattedDate = $.datepicker.formatDate('yy-mm-dd', date);
                    $(this).val(formattedDate);
                }
            });
        });
    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            //toggle date pickers
            function toggleDatePickers() {
                var isPurchasedDateSelected = $('[id$=RadioPurchasedDate]').is(':checked');

                $('[id$=SearchTextBox]').datepicker('destroy');

                //iif RadioPurchasedDate is selected
                if (isPurchasedDateSelected) {
                    var currentDate = new Date().toISOString().split('T')[0];
                    $('[id$=SearchTextBox]').val(currentDate).datepicker({ dateFormat: 'yy-mm-dd' });
                    $('[id$=EndDateTextBox]').val(currentDate).datepicker({ dateFormat: 'yy-mm-dd' }).show();
                } else {
                    $('[id$=EndDateTextBox]').datepicker('destroy').hide();
                }
            }

            toggleDatePickers();

            $('input:radio[name*="SearchOptions"]').change(function () {
                toggleDatePickers();
            });
        });
    </script>

    <script type="text/javascript">
        function printBarcode() {
            var barcode = document.getElementById('<%= BarcodeImage.ClientID %>');
            var newWin = window.open('');
            newWin.document.write('<html><body onload="window.print()">');
            newWin.document.write('<img src="' + barcode.src + '" />');
            newWin.document.write('</body></html>');
            newWin.document.close();
        }
    </script>

    <style type="text/css">
        /* Put spaces between columns of data table */
        .gridViewHeader th {
            padding-right: 20px;
        }

        /* Put space between the radio button and the search parameter */
        input[type='radio'] {
            margin-right: 2px;
        }

        /* Put space between search parameters */
        .search-radio {
            margin-right: 10px;
        }

        .search-by-label {
            font-size: larger;
            margin-bottom: 5px;
        }

        .button-margin {
            margin-bottom: 20px;
        }

        .form-group {
            display: flex;
            align-items: center;
            margin-bottom: 10px;
        }

        .form-label {
            flex-basis: 10%;
            text-align: right;
            margin-right: 10px;
        }

        .form-textbox {
            flex-basis: 60%;
            text-align: left;
        }
        .required-asterisk {
            color: red;
        }

        .save-button-and-message {
            display: flex;
            align-items: center;
            justify-content: flex-start;
        }

        .message-label {
            color: red;
            font-weight: bold;
            margin-left: 10px; /* Adjust spacing as needed */
        }

    </style>


    <main>

        <img src="images/UW-Superior.svg" alt="UW-Superior logo" style="margin: 15px" />

        <h1>Welcome to the IT Asset Management Dashboard</h1>

        <p class="search-by-label">Search by:</p>

        <p>
            <asp:RadioButton ID="RadioBarcode" runat="server" ClientIDMode="Static" CssClass="search-radio" GroupName="SearchOptions" Text="Barcode" Checked="true" />
            <asp:RadioButton ID="RadioAssignedUser" runat="server" ClientIDMode="Static" CssClass="search-radio" GroupName="SearchOptions" Text="Assigned User" />
            <asp:RadioButton ID="RadioSerialNumber" runat="server" ClientIDMode="Static" CssClass="search-radio" GroupName="SearchOptions" Text="Serial Number" />
            <asp:RadioButton ID="RadioPurchasedDate" runat="server" ClientIDMode="Static" CssClass="search-radio" GroupName="SearchOptions" Text="Purchased Date (date range)" />

            <asp:TextBox ID="SearchTextBox" runat="server" ClientIDMode="Static"></asp:TextBox>
            <asp:TextBox ID="EndDateTextBox" runat="server" ClientIDMode="Static" Style="display: none;"></asp:TextBox>
            <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" />
        </p>


        <asp:GridView ID="SearchResultsGridView" runat="server" AutoGenerateColumns="False" AutoGenerateEditButton="True" DataKeyNames="InventoryItemID" OnRowEditing="SearchResultsGridView_RowEditing" OnRowUpdating="SearchResultsGridView_RowUpdating" OnRowCancelingEdit="SearchResultsGridView_RowCancelingEdit" HeaderStyle-CssClass="gridViewHeader">
            <Columns>

                <asp:BoundField DataField="InventoryItemID" HeaderText="Inventory Item ID" ReadOnly="True" />
                <asp:BoundField DataField="InventoryIdentifierBarCode" HeaderText="Bar Code" ReadOnly="True" />
                <asp:BoundField DataField="Make" HeaderText="Make" ReadOnly="True" />
                <asp:BoundField DataField="ModelNumber" HeaderText="Model Number" ReadOnly="True" />
                <asp:TemplateField HeaderText="Assigned User">
                    <EditItemTemplate>
                        <asp:TextBox ID="AssignedUserTextBox" runat="server" Text='<%# Bind("AssignedUser") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="AssignedUserLabel" runat="server" Text='<%# Bind("AssignedUser") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SerialNumber" HeaderText="Serial Number" ReadOnly="True" />
                <asp:BoundField DataField="OSVersion" HeaderText="OS Version" ReadOnly="True" />
                <asp:BoundField DataField="OSLicenseNumber" HeaderText="OS License Number" ReadOnly="True" />
                <asp:BoundField DataField="MachineName" HeaderText="Machine Name" ReadOnly="True" />
                <asp:BoundField DataField="LastLoggedInUser" HeaderText="Last Logged In User" ReadOnly="True" />
                <asp:BoundField DataField="RAMInGigabytes" HeaderText="RAM (GB)" ReadOnly="True" />
                <asp:BoundField DataField="MACAddress" HeaderText="MAC Address" ReadOnly="True" />
                <asp:BoundField DataField="LastLoggedIPAddress" HeaderText="Last Logged IP Address" ReadOnly="True" />
                <asp:BoundField DataField="CPUID" HeaderText="CPU ID" ReadOnly="True" />
                <asp:BoundField DataField="CPUSpeed" HeaderText="CPU Speed" ReadOnly="True" />
                <asp:BoundField DataField="NumberOfProcessors" HeaderText="Number Of Processors" ReadOnly="True" />
                <asp:BoundField DataField="NumberOfRAMSlots" HeaderText="Number Of RAM Slots" ReadOnly="True" />
                <asp:TemplateField HeaderText="Building Name">
                    <EditItemTemplate>
                        <asp:TextBox ID="BuildingNameTextBox" runat="server" Text='<%# Bind("BuildingName") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="BuildingNameLabel" runat="server" Text='<%# Bind("BuildingName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Room Number">
                    <EditItemTemplate>
                        <asp:TextBox ID="RoomNumberTextBox" runat="server" Text='<%# Bind("RoomNumber") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="RoomNumberLabel" runat="server" Text='<%# Bind("RoomNumber") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="AssetTypeID" HeaderText="Asset Type ID" ReadOnly="True" />
                <asp:BoundField DataField="CapitalItem" HeaderText="Capital Item" ReadOnly="True" />
                <asp:BoundField DataField="PurchasedDate" HeaderText="Purchased Date" DataFormatString="{0:yyyy-MM-dd}" ReadOnly="True" />
                <asp:BoundField DataField="InventoryDate" HeaderText="Inventory Date" DataFormatString="{0:yyyy-MM-dd}" ReadOnly="True" />
                <asp:BoundField DataField="RecordGeneratedDate" HeaderText="Record Generated Date" DataFormatString="{0:yyyy-MM-dd}" ReadOnly="True" />
                <asp:TemplateField HeaderText="Notes">
                    <EditItemTemplate>
                        <asp:TextBox ID="NotesTextBox" runat="server" Text='<%# Bind("Notes") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="NotesLabel" runat="server" Text='<%# Bind("Notes") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Device Status">
                    <EditItemTemplate>
                        <ajaxToolkit:ComboBox ID="DeviceStatusComboBox" runat="server" CssClass="form-input" AutoCompleteMode="SuggestAppend">
                            <asp:ListItem Text="" Value="" />
                            <asp:ListItem Text="In Shop" Value="In Shop" />
                            <asp:ListItem Text="Deployed" Value="Deployed" />
                            <asp:ListItem Text="Missing/Unknown" Value="Missing/Unknown" />
                            <asp:ListItem Text="Sent to Auction" Value="Sent to Auction" />
                            <asp:ListItem Text="Closed (Auctioned or Recycled)" Value="Closed (Auctioned or Recycled)" />
                        </ajaxToolkit:ComboBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="DeviceStatusLabel" runat="server" Text='<%# Bind("DeviceStatus") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </asp:GridView>

        <p></p>
        <h5>Fill the form below to add an item to the inventory.  </h5>
        <div class="save-button-and-message">
            <asp:Label ID="MessageLabel1" runat="server" CssClass="message-label" Text="Must fill in all required fields" Visible="false"></asp:Label>
        </div>
        <p>
            <asp:Image ID="BarcodeImage" runat="server" />
        </p>

        <asp:Button ID="PrintButton" runat="server" CssClass="button-margin" Text="Print Barcode" OnClientClick="printBarcode(); return false;" Visible="false" />


        <!-- Make -->
        <div class="form-group">
            <asp:Label ID="MakeLabel" runat="server" CssClass="form-label" Text="Make:"></asp:Label>
            <ajaxToolkit:ComboBox ID="MakeComboBox" runat="server" CssClass="form-input" AutoCompleteMode="SuggestAppend">
                <asp:ListItem Text="" Value="" Selected="True" />
                <asp:ListItem Text="Alienware" Value="Alienware" />
                <asp:ListItem Text="Amazon" Value="Amazon" />
                <asp:ListItem Text="Apple" Value="Apple" />
                <asp:ListItem Text="Beelink" Value="Beelink" />
                <asp:ListItem Text="Cisco" Value="Cisco" />
                <asp:ListItem Text="Dell" Value="Dell" />
                <asp:ListItem Text="Extron" Value="Extron" />
                <asp:ListItem Text="Google" Value="Google" />
                <asp:ListItem Text="HP" Value="HP" />
                <asp:ListItem Text="Konica Minolta" Value="Konica Minolta" />
                <asp:ListItem Text="Lenovo" Value="Lenovo" />
                <asp:ListItem Text="Microsoft" Value="Microsoft" />
                <asp:ListItem Text="Panasonic" Value="Panasonic" />
                <asp:ListItem Text="Palo Alto" Value="Palo Alto" />
                <asp:ListItem Text="Ricoh" Value="Ricoh" />
                <asp:ListItem Text="Samsung" Value="Samsung" />
                <asp:ListItem Text="Sanyo" Value="Sanyo" />
                <asp:ListItem Text="Sharp" Value="Sharp" />
                <asp:ListItem Text="SMART" Value="SMART" />
                <asp:ListItem Text="Sony" Value="Sony" />
            </ajaxToolkit:ComboBox>
        </div>


        <!-- Model Number -->
        <div class="form-group">
            <asp:Label ID="ModelNumberLabel" runat="server" CssClass="form-label" Text="Model Number:"></asp:Label>
            <ajaxToolkit:ComboBox ID="ModelNumberComboBox" runat="server" CssClass="form-input" AutoCompleteMode="SuggestAppend">
                <asp:ListItem Text="" Value="" Selected="True" />
                <asp:ListItem Text="chromebook" Value="chromebook" />
                <asp:ListItem Text="Elite 8300SFF" Value="Elite 8300SFF" />
                <asp:ListItem Text="Galaxy Tab" Value="Galaxy Tab" />
                <asp:ListItem Text="iMAC" Value="iMAC" />
                <asp:ListItem Text="IN1606" Value="IN1606" />
                <asp:ListItem Text="IN1608" Value="IN1608" />
                <asp:ListItem Text="INSP 1010" Value="INSP 1010" />
                <asp:ListItem Text="INSP 1012" Value="INSP 1012" />
                <asp:ListItem Text="INSP 1018" Value="INSP 1018" />
                <asp:ListItem Text="INSP 1120" Value="INSP 1120" />
                <asp:ListItem Text="INSP 1300" Value="INSP 1300" />
                <asp:ListItem Text="iPad" Value="iPad" />
                <asp:ListItem Text="iPad mini" Value="iPad mini" />
                <asp:ListItem Text="iPad Pro" Value="iPad Pro" />
                <asp:ListItem Text="iPhone" Value="iPhone" />
                <asp:ListItem Text="LAT 14 Rugged" Value="LAT 14 Rugged" />
                <asp:ListItem Text="LAT 3310" Value="LAT 3310" />
                <asp:ListItem Text="LAT 3390 xcto" Value="LAT 3390 xcto" />
                <asp:ListItem Text="LAT 3500" Value="LAT 3500" />
                <asp:ListItem Text="LAT 5285" Value="LAT 5285" />
                <asp:ListItem Text="LAT 5400" Value="LAT 5400" />
                <asp:ListItem Text="LAT 5420 Rugged" Value="LAT 5420 Rugged" />
                <asp:ListItem Text="LAT 5430" Value="LAT 5430" />
                <asp:ListItem Text="LAT 5440" Value="LAT 5440" />
                <asp:ListItem Text="LAT 5580" Value="LAT 5580" />
                <asp:ListItem Text="LAT 5590" Value="LAT 5590" />
                <asp:ListItem Text="LAT 7300" Value="LAT 7300" />
                <asp:ListItem Text="LAT 7400" Value="LAT 7400" />
                <asp:ListItem Text="LAT 7410" Value="LAT 7410" />
                <asp:ListItem Text="LAT 7420" Value="LAT 7420" />
                <asp:ListItem Text="LAT 7430" Value="LAT 7430" />
                <asp:ListItem Text="LAT 7440" Value="LAT 7440" />
                <asp:ListItem Text="LAT 7480" Value="LAT 7480" />
                <asp:ListItem Text="LAT 7490" Value="LAT 7490" />
                <asp:ListItem Text="LAT 9510" Value="LAT 9510" />
                <asp:ListItem Text="LAT D830" Value="LAT D830" />
                <asp:ListItem Text="LAT E3390" Value="LAT E3390" />
                <asp:ListItem Text="LAT E6230" Value="LAT E6230" />
                <asp:ListItem Text="LAT E6410" Value="LAT E6410" />
                <asp:ListItem Text="LAT E6420" Value="LAT E6420" />
                <asp:ListItem Text="LAT E6430" Value="LAT E6430" />
                <asp:ListItem Text="LAT E6440" Value="LAT E6440" />
                <asp:ListItem Text="LAT E6500" Value="LAT E6500" />
                <asp:ListItem Text="LAT E6510" Value="LAT E6510" />
                <asp:ListItem Text="LAT E6520" Value="LAT E6520" />
                <asp:ListItem Text="LAT E6530" Value="LAT E6530" />
                <asp:ListItem Text="LAT E6540" Value="LAT E6540" />
                <asp:ListItem Text="LAT E7285" Value="LAT E7285" />
                <asp:ListItem Text="LAT E7440" Value="LAT E7440" />
                <asp:ListItem Text="LAT E7450" Value="LAT E7450" />
                <asp:ListItem Text="LAT E7470" Value="LAT E7470" />
                <asp:ListItem Text="LAT XT" Value="LAT XT" />
                <asp:ListItem Text="M600" Value="M600" />
                <asp:ListItem Text="M800" Value="M800" />
                <asp:ListItem Text="Mac mini" Value="Mac mini" />
                <asp:ListItem Text="Mac Pro" Value="Mac Pro" />
                <asp:ListItem Text="MacBook" Value="MacBook" />
                <asp:ListItem Text="MacBook Pro" Value="MacBook Pro" />
                <asp:ListItem Text="MackBook Air" Value="MackBook Air" />
                <asp:ListItem Text="MPX423A" Value="MPX423A" />
                <asp:ListItem Text="Nook" Value="Nook" />
                <asp:ListItem Text="Opti 150" Value="Opti 150" />
                <asp:ListItem Text="Opti 280" Value="Opti 280" />
                <asp:ListItem Text="Opti 3010" Value="Opti 3010" />
                <asp:ListItem Text="Opti 3020" Value="Opti 3020" />
                <asp:ListItem Text="Opti 3050" Value="Opti 3050" />
                <asp:ListItem Text="Opti 3080" Value="Opti 3080" />
                <asp:ListItem Text="Opti 330" Value="Opti 330" />
                <asp:ListItem Text="Opti 5000" Value="Opti 5000" />
                <asp:ListItem Text="Opti 5040" Value="Opti 5040" />
                <asp:ListItem Text="Opti 5050" Value="Opti 5050" />
                <asp:ListItem Text="Opti 5060" Value="Opti 5060" />
                <asp:ListItem Text="Opti 5070" Value="Opti 5070" />
                <asp:ListItem Text="Opti 5080" Value="Opti 5080" />
                <asp:ListItem Text="Opti 5090" Value="Opti 5090" />
                <asp:ListItem Text="Opti 7010" Value="Opti 7010" />
                <asp:ListItem Text="Opti 7040" Value="Opti 7040" />
                <asp:ListItem Text="Opti 7050" Value="Opti 7050" />
                <asp:ListItem Text="Opti 7060" Value="Opti 7060" />
                <asp:ListItem Text="Opti 7070 Ultra" Value="Opti 7070 Ultra" />
                <asp:ListItem Text="Opti 7071" Value="Opti 7071" />
                <asp:ListItem Text="Opti 7090" Value="Opti 7090" />
                <asp:ListItem Text="Opti 745" Value="Opti 745" />
                <asp:ListItem Text="Opti 755" Value="Opti 755" />
                <asp:ListItem Text="Opti 760" Value="Opti 760" />
                <asp:ListItem Text="Opti 780" Value="Opti 780" />
                <asp:ListItem Text="Opti 790" Value="Opti 790" />
                <asp:ListItem Text="Opti 9020" Value="Opti 9020" />
                <asp:ListItem Text="Opti 990" Value="Opti 990" />
                <asp:ListItem Text="Other" Value="Other" />
                <asp:ListItem Text="P2223HC" Value="P2223HC" />
                <asp:ListItem Text="Pixel" Value="Pixel" />
                <asp:ListItem Text="PLV-HD2000" Value="PLV-HD2000" />
                <asp:ListItem Text="Poweredge" Value="Poweredge" />
                <asp:ListItem Text="PRE 3551 CTO" Value="PRE 3551 CTO" />
                <asp:ListItem Text="PRE 3581" Value="PRE 3581" />
                <asp:ListItem Text="PRE 5470" Value="PRE 5470" />
                <asp:ListItem Text="PRE 5540" Value="PRE 5540" />
                <asp:ListItem Text="PRE 5760" Value="PRE 5760" />
                <asp:ListItem Text="PRE 5810" Value="PRE 5810" />
                <asp:ListItem Text="PRE 7500" Value="PRE 7500" />
                <asp:ListItem Text="PRE 7710" Value="PRE 7710" />
                <asp:ListItem Text="PRE 7920" Value="PRE 7920" />
                <asp:ListItem Text="PRE M3520" Value="PRE M3520" />
                <asp:ListItem Text="PRE M4800" Value="PRE M4800" />
                <asp:ListItem Text="PRE T3500" Value="PRE T3500" />
                <asp:ListItem Text="PRE T3610" Value="PRE T3610" />
                <asp:ListItem Text="PRE T5500" Value="PRE T5500" />
                <asp:ListItem Text="PRE T7610" Value="PRE T7610" />
                <asp:ListItem Text="PT-EW630" Value="PT-EW630" />
                <asp:ListItem Text="PT-EW640" Value="PT-EW640" />
                <asp:ListItem Text="PT-EW650" Value="PT-EW650" />
                <asp:ListItem Text="PT-EW730" Value="PT-EW730" />
                <asp:ListItem Text="PT-FRZ50WU7" Value="PT-FRZ50WU7" />
                <asp:ListItem Text="PTMW530U" Value="PTMW530U" />
                <asp:ListItem Text="PTMW630U" Value="PTMW630U" />
                <asp:ListItem Text="PTRZ570" Value="PTRZ570" />
                <asp:ListItem Text="R16" Value="R16" />
                <asp:ListItem Text="Surface Pro" Value="Surface Pro" />
                <asp:ListItem Text="Surface Pro 3" Value="Surface Pro 3" />
                <asp:ListItem Text="Surface Pro 4" Value="Surface Pro 4" />
                <asp:ListItem Text="Toughbook" Value="Toughbook" />
                <asp:ListItem Text="Toughbook CF-33" Value="Toughbook CF-33" />
                <asp:ListItem Text="TV" Value="TV" />
                <asp:ListItem Text="U55" Value="U55" />
                <asp:ListItem Text="UX-60" Value="UX-60" />
                <asp:ListItem Text="VPL-PHZ10" Value="VPL-PHZ10" />
                <asp:ListItem Text="WM5500" Value="WM5500" />
                <asp:ListItem Text="XPS 15" Value="XPS 15" />
                <asp:ListItem Text="XPS 15z" Value="XPS 15z" />
                <asp:ListItem Text="XPS 17" Value="XPS 17" />
            </ajaxToolkit:ComboBox>
        </div>


        <!-- Assigned User -->
        <div class="form-group">
            <asp:Label ID="AssignedUserLabel" runat="server" CssClass="form-label" Text="Assigned User:"></asp:Label>
            <asp:TextBox ID="AssignedUserTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Serial Number -->
        <div class="form-group">
            <asp:Label ID="SerialNumberLabel" runat="server" CssClass="form-label" Text="Serial Number:"></asp:Label>
            <asp:TextBox ID="SerialNumberTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- OS Version -->
        <div class="form-group">
            <asp:Label ID="OSVersionLabel" runat="server" CssClass="form-label" Text="OS Version:"></asp:Label>
            <asp:TextBox ID="OSVersionTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- OS License Number -->
        <div class="form-group">
            <asp:Label ID="OSLicenseNumberLabel" runat="server" CssClass="form-label" Text="OS License Number:"></asp:Label>
            <asp:TextBox ID="OSLicenseNumberTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Machine Name -->
        <div class="form-group">
            <asp:Label ID="MachineNameLabel" runat="server" CssClass="form-label" Text="Machine Name:"></asp:Label>
            <asp:TextBox ID="MachineNameTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Last Logged In User -->
        <div class="form-group">
            <asp:Label ID="LastLoggedInUserLabel" runat="server" CssClass="form-label" Text="Last Logged In User:"></asp:Label>
            <asp:TextBox ID="LastLoggedInUserTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- RAM in Gigabytes -->
        <div class="form-group">
            <asp:Label ID="RAMInGigabytesLabel" runat="server" CssClass="form-label" Text="RAM in Gigabytes:"></asp:Label>
            <asp:TextBox ID="RAMInGigabytesTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- MAC Address -->
        <div class="form-group">
            <asp:Label ID="MACAddressLabel" runat="server" CssClass="form-label" Text="MAC Address:"></asp:Label>
            <asp:TextBox ID="MACAddressTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Last Logged IP Address -->
        <div class="form-group">
            <asp:Label ID="LastLoggedIPAddressLabel" runat="server" CssClass="form-label" Text="Last Logged IP Address:"></asp:Label>
            <asp:TextBox ID="LastLoggedIPAddressTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- CPU ID -->
        <div class="form-group">
            <asp:Label ID="CPUIDLabel" runat="server" CssClass="form-label" Text="CPU ID:"></asp:Label>
            <asp:TextBox ID="CPUIDTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- CPU Speed -->
        <div class="form-group">
            <asp:Label ID="CPUSpeedLabel" runat="server" CssClass="form-label" Text="CPU Speed:"></asp:Label>
            <asp:TextBox ID="CPUSpeedTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Number Of Processors -->
        <div class="form-group">
            <asp:Label ID="NumberOfProcessorsLabel" runat="server" CssClass="form-label" Text="Number Of Processors:"></asp:Label>
            <asp:TextBox ID="NumberOfProcessorsTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Number Of RAM Slots -->
        <div class="form-group">
            <asp:Label ID="NumberOfRAMSlotsLabel" runat="server" CssClass="form-label" Text="Number Of RAM Slots:"></asp:Label>
            <asp:TextBox ID="NumberOfRAMSlotsTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Building Name -->
        <div class="form-group">
            <asp:Label ID="BuildingNameLabel" runat="server" CssClass="form-label" Text="Building Name:"></asp:Label>
            <asp:TextBox ID="BuildingNameTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Room Number -->
        <div class="form-group">
            <asp:Label ID="RoomNumberLabel" runat="server" CssClass="form-label" Text="Room Number:"></asp:Label>
            <asp:TextBox ID="RoomNumberTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Asset Type ID -->
        <div class="form-group">
            <asp:Label ID="AssetTypeIDLabel" runat="server" CssClass="form-label" Text="Asset Type ID: <span class='required-asterisk'>*</span>"></asp:Label>
            <ajaxToolkit:ComboBox ID="AssetTypeIDComboBox" runat="server" CssClass="form-input" AutoCompleteMode="SuggestAppend">
                <asp:ListItem Text="" Value="" Selected="True" />
                <asp:ListItem Text="1 = Desktop" Value="1" />
                <asp:ListItem Text="2 = Laptop" Value="2" />
                <asp:ListItem Text="3 = Mobile" Value="3" />
                <asp:ListItem Text="4 = AV" Value="4" />
                <asp:ListItem Text="5 = MFP" Value="5" />
                <asp:ListItem Text="6 = Networking (switches, WAPs, firewalls, DNS, DHCP)" Value="6" />
                <asp:ListItem Text="7 = Enterprise (servers, security appliances, Network Storage)" Value="7" />
                <asp:ListItem Text="8 = Peripherals" Value="8" />
            </ajaxToolkit:ComboBox>
        </div>


        <!-- Capital Item -->
        <div class="form-group">
            <asp:Label ID="CapitalItemLabel" runat="server" CssClass="form-label" Text="Capital Item: "></asp:Label>
            <asp:CheckBox ID="CapitalItemCheckBox" runat="server" CssClass="form-input"></asp:CheckBox>
        </div>

        <!-- Purchased Date -->
        <div class="form-group">
            <asp:Label ID="PurchasedDateLabel" runat="server" CssClass="form-label" Text="Purchased Date:"></asp:Label>
            <asp:TextBox ID="PurchasedDateTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Notes -->
        <div class="form-group">
            <asp:Label ID="NotesLabel" runat="server" CssClass="form-label" Text="Notes: "></asp:Label>
            <asp:TextBox ID="NotesTextBox" runat="server" CssClass="form-input"></asp:TextBox>
        </div>

        <!-- Device Status -->
        <div class="form-group">
            <asp:Label ID="DeviceStatusLabel" runat="server" CssClass="form-label" Text="Device Status:"></asp:Label>
            <ajaxToolkit:ComboBox ID="DeviceStatusComboBox" runat="server" CssClass="form-input" AutoCompleteMode="SuggestAppend">
                <asp:ListItem Text="" Value="" Selected="True" />
                <asp:ListItem Text="In Shop" Value="In Shop" />
                <asp:ListItem Text="Deployed" Value="Deployed" />
                <asp:ListItem Text="Missing/Unknown" Value="Missing/Unknown" />
                <asp:ListItem Text="Sent to Auction" Value="Sent to Auction" />
                <asp:ListItem Text="Closed (Auctioned or Recycled)" Value="Closed (Auctioned or Recycled)" />
            </ajaxToolkit:ComboBox>
        </div>

        <!-- Required Field Indicator -->
        <div class="form-group">
            <asp:Label ID="RequiredFieldLabel" runat="server" CssClass="form-label" Text="<span class='required-asterisk'>*</span> Required Field"></asp:Label>
        </div>

        <p>
            <!-- Save Button and Message Label -->
            <div class="save-button-and-message">
                <asp:Button ID="Button1" runat="server" Text="Save" OnClick="SaveButton_Click" />
                <asp:Label ID="MessageLabel2" runat="server" CssClass="message-label" Text="Must fill in all required fields" Visible="false"></asp:Label>
            </div>

        </p>
    </main>
</asp:Content>

