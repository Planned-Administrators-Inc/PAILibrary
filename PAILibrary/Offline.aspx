<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotAdmin.aspx.cs" Inherits="PAILibrary.NotAdmin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title>PAI Document Library - Offline</title>
	<link href="css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<div class="page">
		<div id="header">
			<div id="title">
				<h1>PAI Document Library</h1>
			</div>
			<div id="menucontainer">
				<ul id="menu">
					<li><a id="active" href="Default.aspx" title="Search the document library"><img src="images/home_22x22.png" alt="home" />Home</a></li>
					<li><a href="Admin/AddNewDoc.aspx" title="Perform administration tasks"><img src="images/admin_22x22.png" alt="admin" />Admin</a></li>
					<li><a href="LibraryHelp_05-22-2014.pdf" target="_blank" style="font-weight: bold;" title="View help document."><img src="images/help_22x22.png" alt="admin" />Help</a></li>
				</ul>
			</div>
		</div>
		<div id="main">
			<form id="form1" runat="server">
				<fieldset>
					<legend>Offline</legend>
					<div class="editor-label">
						The PAI Document Library is currently <strong>offline</strong> for maintenance.<br />
					</div>
				</fieldset>
			</form>
		</div>
	</div>
</body>
</html>
