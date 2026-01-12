<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDocSec.aspx.cs" Inherits="PAILibrary.EditDocSec" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - Admin - Document Security</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript">
		function CheckForm() {
			var code = document.getElementById("Code");
			var uid = document.getElementById("UserID");
			if (code.value == "" || code.value.length > 5) {
				alert("Please enter a Code with 1 to 5 characters.");
				code.focus();
				return false;
			}
			if (uid.value == "") {
				alert("Please enter a user ID.");
				uid.focus();
				return false;
			}
			return true;
		}
	</script>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library - Admin - Document Security</h1>
			</div>
			<div id="menucontainer">
				<ul id="menu">
					<li><a id="active" href="../Default.aspx" title="Search the document library"><img src="../images/home_22x22.png" alt="home" />Home</a></li>
					<li><a href="AddNewDoc.aspx" title="Perform administration tasks"><img src="../images/admin_22x22.png" alt="admin" />Admin</a></li>
					<li><a href="../LibraryHelp_05-22-2014.pdf" target="_blank" style="font-weight: bold;" title="View help document."><img src="../images/help_22x22.png" alt="admin" />Help</a></li>
				</ul>
			</div>
		</div>
		<div id="main">
			<a class="menuLink" href="AddNewDoc.aspx" title="Adds a new document to the library">New Document</a>
			<a class="menuLink"	href="EditDocList.aspx" title="Edit a document in the library">Edit Document</a>
			<a class="menuLink" href="EditDocCodes.aspx" title="Edit a document code">Document Codes</a>
			<a class="menuLink" href="EditDocSec.aspx" title="Edit existing document security">Document Security</a>
			<a class="menuLink" href="AuditSearch.aspx" title="Search audit history">Audit Search</a>
			<br /><br />
			Please enter your search criteria below.
			<form id="form1" runat="server">
			<fieldset>
				<legend>Search</legend>
				<div class="editor-label">
					<label for="Code">Code</label>
					<input class="mediumbox" id="Code" type="text" runat="server" value="" />
					<label for="UserID">User ID</label>
					<input class="largebox" id="UserID" type="text" runat="server" value="" />
					<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" OnClick="btnSearch_Click" />
					<asp:Button ID="btnAdd" runat="server" CssClass="button" Text="Add" OnClick="btnAdd_Click" OnClientClick="return CheckForm();" />
					<div id="footer">
						<asp:Literal ID="litFooter" runat="server"></asp:Literal>
					</div>
					<div id="results">
						<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
							DataKeyNames="SID" DataSourceID="sdsPAILib" onrowcancelingedit="gvResults_RowCancelingEdit"
							onrowediting="gvResults_RowEditing" onprerender="gvResults_PreRender"
							onrowdatabound="gvResults_RowDataBound" onrowdeleted="gvResults_RowDeleted"
							onrowupdating="gvResults_RowUpdating" OnRowDeleting="gvResults_RowDeleting" onrowupdated="gvResults_RowUpdated">
							<Columns>
								<asp:templatefield headertext="Edit/Delete">
									<itemtemplate>
										<asp:linkbutton id="lbEdit" runat="server" commandname="Edit" forecolor="#333333" text="Edit"></asp:linkbutton>
										<asp:linkbutton id="lbCancel" runat="server" commandname="Cancel" forecolor="#333333" text="Cancel" visible="false"></asp:linkbutton>
										<asp:linkbutton id="lbUpdate" runat="server" commandname="Update" forecolor="#333333" text="Update" visible="false"></asp:linkbutton>
										<asp:linkbutton id="lbDelete" runat="server" commandname="Delete" forecolor="#333333" text="Delete" onclientclick="javascript : return confirm('Do you really want to\ndelete the item?');"></asp:linkbutton>
									</itemtemplate>
									<ItemStyle Width="110px" />
								</asp:templatefield>
								<asp:BoundField DataField="SID" HeaderText="ID" ReadOnly="True" />
								<asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" >
									<ItemStyle Width="75px" />
								</asp:BoundField>
								<asp:BoundField DataField="UserID" HeaderText="User ID" SortExpression="UserID" />
							</Columns>
						</asp:GridView>
					</div>
					
				</div>
			</fieldset>
			</form>
		</div>
	</div>
	<asp:SqlDataSource ID="sdsPAILib" runat="server"
		UpdateCommand="Update DL_Security Set Code = @Code, UserID = @UserID Where SID = @SID" DeleteCommand="delete DL_Security where SID = 0" ondeleting="sdsPAILib_Deleting" ConnectionString="Data Source=dbServer1;Initial Catalog=PAILibraryCI;Persist Security Info=True;User ID=PAILibUser;Password=Lu$443!;Asynchronous Processing=True;Application Name=PAILibrary" ProviderName="System.Data.SqlClient">
		<UpdateParameters>
			<asp:Parameter Name="@SID" Type="Int32" />
			<asp:parameter Name="@Code" Type="String" />
			<asp:parameter Name="@UserID" Type="String" />
		</UpdateParameters>
	</asp:SqlDataSource>
</body>
</html>
