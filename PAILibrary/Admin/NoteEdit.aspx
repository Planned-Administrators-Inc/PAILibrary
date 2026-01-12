<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoteEdit.aspx.cs" Inherits="PAILibrary.Admin.NoteEdit" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>PAI Document Library - Admin - Edit Note</title>
	<link href="../css/Site.css" rel="stylesheet" type="text/css" />
</head>
<body style="background: #ffffff">
	<script type="text/javascript">
		function previewNote() {
			var n = document.getElementById("HidNID").value;
			window.open("../ViewNote.aspx?nid=" + n, "_preview", "scrollbars=yes,width=640,height=480,location=no,menubar=no,status=no,toolbar=no", false);
		}
	</script>
	<div id="main">
		<form id="form1" enctype="multipart/form-data" runat="server">
		<fieldset>
			<legend>Edit Notes</legend>
			<!--
			<input class="editor-label" type="button" id="BtnPreview" value="Preview Note" onclick="previewNote();" />
			<br /><br />
			-->
			<CKEditor:CKEditorControl ID="CKNotes" runat="server" Height="240px" UIColor="#E8EEF4"
				Toolbar="Full" EnterMode="BR" ShiftEnterMode="P" Wrap="true"
				ToolbarFull="Source|-|Save|Preview|-|Templates
Cut|Copy|Paste|PasteText|PasteFromWord|-|Print|SpellChecker|Scayt
Undo|Redo|-|Find|Replace|-|SelectAll|RemoveFormat
/
Bold|Italic|Underline|Strike|-|Subscript|Superscript
NumberedList|BulletedList|-|Outdent|Indent|Blockquote
JustifyLeft|JustifyCenter|JustifyRight|JustifyBlock
Link|Unlink|Anchor
Table|HorizontalRule|Smiley|SpecialChar|PageBreak
/
Styles|Format|Font|FontSize
TextColor|BGColor
Maximize|ShowBlocks"></CKEditor:CKEditorControl>
			<div id="footer">
				<asp:Literal ID="litFooter" runat="server"></asp:Literal>
				<asp:HiddenField ID="HidNID" runat="server" />
			</div>
		</fieldset>
		</form>
	</div>
</body>
</html>
