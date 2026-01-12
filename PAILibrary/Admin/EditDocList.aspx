<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDocList.aspx.cs" Inherits="PAILibrary.EditDocList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - Admin - Edit Documents</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript">
		function ViewCodes() {
			window.open("../ViewCodes.aspx", "_preview", "scrollbars=yes,width=640,height=480,location=no,menubar=no,status=no,toolbar=no", false);
		}
	</script>
</head>
<body>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library - Admin - Edit Documents</h1>
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
					<span id="lblGroup">Group</span>
					<input class="mediumbox" id="Group" type="text" runat="server" value="" />
					<span id="lblCode"><a href="#" target="_blank" title="View document codes" onclick="ViewCodes(); return false;">Code</a></span>
					<input class="mediumbox" id="Code" type="text" runat="server" value="" />
					<label for="DateStamp">DateStamp</label>
					<input class="mediumbox" id="DateStamp" type="text" runat="server" value="" />
					<label for="Version">Version</label>
					<input class="mediumbox" id="Version" type="text" runat="server" value="" />
					<label for="FF_Desc">Desc</label>
					<input class="largebox" id="Desc" type="text" runat="server" value="" />
					<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" OnClick="btnSearch_Click" />
					<div id="results">
						<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
							DataKeyNames="DocID" DataSourceID="sdsPAILib" onrowdatabound="gvResults_RowDataBound"
							onrowdeleted="gvResults_RowDeleted" onrowupdating="gvResults_RowUpdating"
							Width="100%" onrowdeleting="gvResults_RowDeleting" EmptyDataText="No records found.">
							<Columns>
								<asp:templatefield headertext="Action">
									<itemtemplate>
										<asp:HyperLink ID="hlView" NavigateUrl="~/ViewFile.aspx?docid={0}" Target="_blank" runat="server" Text="View"></asp:HyperLink>
										<asp:linkbutton id="lbDelete" runat="server" commandname="Delete" forecolor="#333333" text="Delete" onclientclick="javascript : return confirm('Do you really want to\ndelete the item?');"></asp:linkbutton>
									</itemtemplate>
									<ItemStyle Width="110px" />
								</asp:templatefield>
								<asp:BoundField DataField="DocID" HeaderText="DocID" InsertVisible="False" ReadOnly="True" 
									SortExpression="DocID" Visible="False" />
								<asp:BoundField DataField="GroupNo" HeaderText="GroupNo" ReadOnly="True" SortExpression="GroupNo" />
								<asp:BoundField DataField="Code" HeaderText="Code" ReadOnly="True" SortExpression="Code" />
								<asp:BoundField DataField="DateStamp" HeaderText="DateStamp" ReadOnly="True" SortExpression="DateStamp" />
								<asp:BoundField DataField="Version" HeaderText="Version" ReadOnly="True" SortExpression="Version" />
								<asp:BoundField DataField="FullDesc" HeaderText="FullDesc" ReadOnly="True" SortExpression="FullDesc" />
								<asp:BoundField DataField="Ext" HeaderText="Ext" ReadOnly="True" SortExpression="Ext" 
									ControlStyle-CssClass="smallInput" >
									<ControlStyle CssClass="smallInput"></ControlStyle>
								</asp:BoundField>
								<asp:templatefield ShowHeader="true" headertext="Notes">
									<ItemTemplate>
										<table border="0" style="border: none; padding: 0px">
											<tr>
												<td style="border: none; padding: 0px;"><asp:HyperLink ID="lnkNoteEdit" NavigateUrl="~/Admin/NoteEdit.aspx?nid={0}" Target="_blank" runat="server" Text="Edit"></asp:HyperLink></td>
												<td style="border: none; padding: 0px; width: 8px;"></td>
												<td style="border: none; padding: 0px;"><asp:HyperLink ID="lnkNoteView" NavigateUrl="~/ViewNote.aspx?nid={0}" Target="_blank" runat="server" Text="View"></asp:HyperLink></td>
											</tr>
										</table>
									</ItemTemplate>
									<ItemStyle Width="90px" />
								</asp:templatefield>
							</Columns>
						</asp:GridView>
					</div>
				</div>
				<div id="footer">
					<asp:Literal ID="litFooter" runat="server"></asp:Literal>
				</div>
			</fieldset>
			</form>
		</div>
	</div>
	<asp:SqlDataSource ID="sdsPAILib" runat="server" 
		updatecommand="Update DL_Docs Set Notes=@Notes Where DocID = @DocID"
		deletecommand="Delete DL_Docs Where DocID = @DocID" ConnectionString="Data Source=dbServer1;Initial Catalog=PAILibraryCI;Persist Security Info=True;User ID=PAILibUser;Password=Lu$443!;Asynchronous Processing=True;Application Name=PAILibrary" ProviderName="System.Data.SqlClient">
		<updateparameters>
			<asp:parameter name="@DocID" type="Int32" />
			<asp:parameter name="@Notes" type="String" />
		</updateparameters>
		<deleteparameters>
			<asp:parameter name="@DocID" type="Int32" />
		</deleteparameters>
	</asp:SqlDataSource>
</body>
</html>
