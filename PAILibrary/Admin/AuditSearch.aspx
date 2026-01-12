<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditSearch.aspx.cs" Inherits="PAILibrary.Admin.AuditSearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>PAI Document Library - Admin - Audit Search</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
	<link href="../css/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
	<link href="../css/tip-paiblue.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript" src="../js/jquery-1.8.0.min.js"></script>
	<script type="text/javascript" src="../js/jquery-ui-1.8.23.custom.min.js"></script>
	<script type="text/javascript" src="../js/jquery.poshytip.js"></script>
	<script type="text/javascript">
		function DocReady() {
			var dpTargetDate = $('#<%= txtTargetDate.ClientID %>').datepicker();
			// load poshytips
			$('#txtDoc').poshytip({
				className: 'tip-yellowsimple',
				showOn: 'focus',
				alignTo: 'target',
				alignX: 'inner-left',
				offsetX: 0,
				offsetY: 15,
				showTimeout: 100
			});
			dpTargetDate.datepicker('option',
				{
					firstDay: 1,
					changeMonth: true,
					changeYear: true,
					minDate: '01/01/1900',
					maxDate: '12/31/2050',
					yearRange: '1900:2050'
				});
		}
		// setup the jquery-ui datepickers
		$(document).ready(DocReady);
	</script>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library - Admin - Audit Search</h1>
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
				
				<label for="d">Group</label>
				<asp:TextBox ID="txtGroup" runat="server" CssClass="mediumbox"></asp:TextBox>
				
				<label for="txtCode">Code</label>
				<asp:TextBox ID="txtCode" runat="server" CssClass="smallbox"></asp:TextBox>
				
				<label for="txtDoc">Doc Name(*)</label>
				<asp:TextBox ID="txtDoc" runat="server" CssClass="very-largebox" ToolTip="Enter any part of the document name"></asp:TextBox>
				
				<label for="txtUserID">User ID</label>
				<asp:TextBox ID="txtUserID" runat="server" CssClass="mediumbox"></asp:TextBox>
				
				<label for="txtCode">Date</label>
				<asp:TextBox ID="txtTargetDate" runat="server" CssClass="mediumbox datebox"></asp:TextBox>
				
				<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" onclick="btnSearch_Click" />
				
				<div id="footer">
					<asp:Literal ID="litFooter" runat="server"></asp:Literal>
				</div>
				<div id="results">
					<asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
						DataKeyNames="AID" Width="100%">
						<Columns>
							<asp:BoundField DataField="AID" HeaderText="AID" InsertVisible="False" Visible="false" />
							<asp:BoundField DataField="OldValue" HeaderText="Orig Val." SortExpression="OldValue" />
							<asp:BoundField DataField="NewValue" HeaderText="New Val." SortExpression="NewValue" />
							<asp:BoundField DataField="UserID" HeaderText="User ID" SortExpression="UserID" />
							<asp:BoundField DataField="AuditDate" HeaderText="Audit Date" SortExpression="AuditDate"
								DataFormatString="{0:MM/dd/yy HH:mm}" ItemStyle-CssClass="dateMed" />
							<asp:BoundField DataField="AuditNote" HeaderText="Action" SortExpression="AuditNote" />
						</Columns>
					</asp:GridView>
				</div>
			</fieldset>
			</form>
		</div>
	</div>
</body>
</html>
