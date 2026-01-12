<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewCodes.aspx.cs" Inherits="PAILibrary.ViewCodes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - View Codes</title>
	<link href="css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<script type="text/javascript">
		function SetParentCode(code) {
			window.opener.document.getElementById("Code").value = code;
			window.close();
		}
	</script>
	
	<div id="main">
		<form id="form1" runat="server">
			<div><asp:Label ID="lblMsg" runat="server"></asp:Label></div>
			<asp:GridView ID="gvNotes" runat="server" Width="100%" DataKeyNames="Code" DataSourceID="sdsPAILib"
				AutoGenerateColumns="false" onrowdatabound="gvNotes_RowDataBound">
				<Columns>
					<asp:BoundField DataField="Code" HeaderText="Code" SortExpression="Code" />
					<asp:HyperLinkField DataTextField="DocType" HeaderText="DocType" NavigateUrl="javascript:SetParentCode(this);" />
				</Columns>
			</asp:GridView>
		</form>
	</div>
	<asp:SqlDataSource ID="sdsPAILib" runat="server" SelectCommand="usp_DL_GetDocCodes" SelectCommandType="StoredProcedure" ConnectionString="<%$ ConnectionStrings:conLib %>" ProviderName="<%$ ConnectionStrings:conLib.ProviderName %>">
	</asp:SqlDataSource>
</body>
</html>
