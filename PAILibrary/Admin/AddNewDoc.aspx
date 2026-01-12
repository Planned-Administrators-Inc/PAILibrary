<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNewDoc.aspx.cs" Inherits="PAILibrary.AddNewDoc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - Admin - New Document</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript">
		function CheckUpload() {
			var f = document.getElementById("UploadFile");
			if (f.value == "") {
				alert("Please select a file to upload.");
				f.focus();
				return false;
			}
			return true;
		}
	</script>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library - Admin - New Document</h1>
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
			<form id="form1" enctype="multipart/form-data" runat="server">
			<fieldset>
				<legend>Upload Document</legend>
				<span>
					<label for="UploadFile">File to upload:</label>
					<input type="file" class="file" id="UploadFile" size="50" style="width:85% !important;" runat="server" />
					<br />
					<asp:Button ID="btnSubmit" runat="server" CssClass="button" onclick="btnSubmit_Click" OnClientClick="return CheckUpload();" Text="Upload" />
				</span>
				</fieldset>
				<div id="footer">
					<asp:Literal ID="litFooter" runat="server"></asp:Literal>
				</div>
			</form>
		</div>
	</div>
</body>
</html>
