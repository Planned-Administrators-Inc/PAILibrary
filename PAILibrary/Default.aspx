<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PAILibrary._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>PAI Document Library - Home</title>
	<link href="css/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
	<link href="css/Site.css" rel="stylesheet" type="text/css" />
	<script type="text/javascript" src="js/jquery-1.8.0.min.js"></script>
	<script type="text/javascript" src="js/jquery-ui-1.8.23.custom.min.js"></script>
	<script type="text/javascript" src="js/jquery.tablesorter.min.js"></script></head>

<body class="page">
	<script type="text/javascript">
		var win;

		function ViewCodes() {
			var left = (screen.width / 2) - (640 / 2);
			var top = (screen.height / 2) - (640 / 2);
			win = window.open("ViewCodes.aspx", "_preview", "scrollbars=yes,width=640,height=480,top="+top+",left="+left+",location=no,menubar=no,status=no,toolbar=no", false);
		}

		function SetCode(c) {
			document.getElementById("Code").value = c;
		}

		function CheckSearch() {
			var group = document.getElementById("Group").value;
			var code = document.getElementById("Code").value;
			var date = document.getElementById("DateStamp").value;
			var ver = document.getElementById("Version").value;
			var desc = document.getElementById("Desc").value;
			if (group == "" && code == "" && date == "" && ver == "" && desc == "") {
				alert("Please enter at least one search criteria.");
				return false;
			}
			return true;
		}

		function removeDateHint() {
			var d = document.getElementById("DateStamp");
			if (d.value == "yyyymmdd") {
				d.value = "";
			}
			d.style.color = "#000000";
		}
		
		function setDateHint() {
			var d = document.getElementById("DateStamp");
			if (d.value == "" || d.value == "yyyymmdd") {
				d.value = "yyyymmdd";
				d.style.color = "#A9A9A9";
			}
			else {
				d.style.color = "#000000";
			}
		}
		$(document).ready(function() {
			$("#<%= tblResults.ClientID %>").tablesorter(
				{
					headers: { 0: { sorter: false }, 7: { sorter: false} },
					sortList: [[3, 1]]
				});
		});
	</script>

	<div id="header">
		<div id="title">
			<h1>PAI Document Library</h1>
		</div>
	</div>
	
	<div id="menucontainer">
		<ul id="menu">
			<li><a id="active" href="Default.aspx" title="Search the document library"><img src="images/home_22x22.png" alt="home" />Home</a></li>
			<li><a href="Admin/AddNewDoc.aspx" title="Perform administration tasks"><img src="images/admin_22x22.png" alt="admin" />Admin</a></li>
			<li><a href="LibraryHelp_05-22-2014.pdf" target="_blank" style="font-weight: bold;" title="View help document."><img src="images/help_22x22.png" alt="admin" />Help</a></li>
		</ul>
	</div>
	
	<div id="main">
		<span>Please enter your search criteria below.</span>
		<form id="form1" runat="server" style="margin-top: 5px;">
			<fieldset>
				<legend>Search</legend>
				<div class="editor-label">
					<label id="lblGroup">Group</label>
					<input class="mediumbox" id="Group" type="text" runat="server" value="" />
					
					<label id="lblCode"><a href="#" target="_blank" title="View document codes" onclick="ViewCodes(); return false;">Code</a></label>
					<input class="mediumbox" id="Code" type="text" runat="server" value="" />
					
					<label for="DateStamp">Date Stamp</label>
					<select id="DateSearchType" runat="server" style="width: 40px;">
						<option value="=">=</option>
						<option value="&lt;">&lt;</option>
						<option value="&gt;>">&gt;</option>
						<option value="&lt;=">&lt;=</option>
						<option value="&gt;=">&gt;=</option>
					</select>
					<input class="mediumbox" id="DateStamp" type="text" runat="server" value="yyyymmdd" onfocus="removeDateHint();" onblur="setDateHint();" />
					
					<label for="Version">Version</label>
					<input class="mediumbox" id="Version" type="text" runat="server" value="" />
					
					<label for="Desc">Desc</label>
					<input class="largebox" id="Desc" type="text" runat="server" value="" />
					
					<asp:Button ID="btnSearch" runat="server" CssClass="button" Text="Search" 
						OnClientClick="return CheckSearch(this);" onclick="btnSearch_Click" />
					
					<div id="results">
						<asp:Table ID="tblResults" runat="server" Width="100%">
						</asp:Table>
					</div>
				</div>
			</fieldset>
			<div id="footer">
				<asp:Literal ID="litFooter" runat="server"></asp:Literal>
			</div>
			<div id="footersql">
				<asp:Literal ID="litSql" runat="server"></asp:Literal>
			</div>
		</form>
	</div>
</body>
</html>

