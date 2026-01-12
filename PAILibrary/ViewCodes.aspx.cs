using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAILibrary
{
	public partial class ViewCodes : System.Web.UI.Page
	{
		protected void Page_Load( object sender, EventArgs e )
		{

		}

		protected void gvNotes_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				HyperLink f = e.Row.Cells[1].Controls[0] as HyperLink;
				if (f != null)
				{
					f.NavigateUrl = "javascript:SetParentCode(\"" + e.Row.Cells[0].Text + "\");";
				}
			}
		}
	}
}
