<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDocCodes.aspx.cs" Inherits="PAILibrary.EditDocCodes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - Admin - Document Codes</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript">
		function CheckForm()
		{
			var code = document.getElementById("Code").value;
			var docType = document.getElementById("DocType").value;
			if (code == "" || code.length > 5)
			{
				alert("Please enter a Code with 1 to 5 characters.");
				return false;
			}
			if (docType == "")
			{
				alert("Please enter a document type.");
				return false;
			}
			return true;
		}
	</script>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library - Admin - Document Codes</h1>
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
					<label for="DocType">Doc Type</label>
					<input class="very-largebox" id="DocType" type="text" runat="server" value="" />
					<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" OnClick="btnSearch_Click" />
					<asp:Button ID="btnAdd" runat="server" CssClass="button" Text="Add" OnClick="btnAdd_Click" OnClientClick="return CheckForm();" />
					<div id="footer">
						<asp:Literal ID="litFooter" runat="server"></asp:Literal>
					</div>
					<div id="results">
						<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
							DataKeyNames="Code" DataSourceID="sdsPAILib" onrowcancelingedit="gvResults_RowCancelingEdit"
							onrowediting="gvResults_RowEditing" onprerender="gvResults_PreRender"
							onrowdatabound="gvResults_RowDataBound" onrowupdating="gvResults_RowUpdating"
							OnRowDeleted="gvResults_RowDeleted" OnRowDeleting="gvResults_RowDeleting"
							Width="100%" onrowupdated="gvResults_RowUpdated">
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
								<asp:BoundField DataField="Code" HeaderText="Code" InsertVisible="False" 
									ReadOnly="True" SortExpression="Code" >
								<ItemStyle Width="75px" />
								</asp:BoundField>
								<asp:BoundField DataField="DocType" HeaderText="Doc Type" SortExpression="DocType" />
							</Columns>
						</asp:GridView>
					</div>
				</div>
			</fieldset>
			</form>
		</div>
	</div>
	<asp:SqlDataSource ID="sdsPAILib" runat="server" 
		updatecommand="Update DL_Codes Set DocType = @DocType Where Code = @Code"
		deletecommand="Delete DL_Codes Where Code = 'numnum'" ondeleting="sdsPAILib_Deleting" ConnectionString="Data Source=dbServer1;Initial Catalog=PAILibraryCI;Persist Security Info=True;User ID=PAILibUser;Password=Lu$443!;Asynchronous Processing=True;Application Name=PAILibrary" ProviderName="System.Data.SqlClient">
		<updateparameters>
			<asp:parameter name="@Code" type="String" />
			<asp:parameter name="@DocType" type="String" />
		</updateparameters>
	</asp:SqlDataSource>
</body>
</html>
