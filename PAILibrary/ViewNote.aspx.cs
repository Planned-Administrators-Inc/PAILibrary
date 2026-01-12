using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAILibrary
{
	public partial class ViewNote : System.Web.UI.Page
	{
		private string _noteID = string.Empty;
		private NameValueCollection q = null;
		
		protected void Page_Load( object sender, EventArgs e )
		{
			if (Helpers.Offline == true)
				Response.Redirect("Offline.aspx");
			
			if (Request.QueryString.Count > 0)
			{
				q = Request.QueryString;
				LoadNote();
			}
		}
		
		private void LoadNote()
		{
			try
			{
				if (q["nid"] != null)
				{
					string nid = q["nid"];
					using (SqlConnection con = new SqlConnection())
					using (SqlCommand cmd = new SqlCommand())
					{
						con.ConnectionString = Helpers.GetSQLConString();
						con.Open();

						cmd.Connection = con;
						cmd.CommandTimeout = 0;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "usp_DL_GetNotes";
						cmd.Parameters.AddWithValue("@DocID", nid);
						using (SqlDataReader dr = cmd.ExecuteReader())
						{
							if (dr.HasRows)
							{
								dr.Read();
								litNote.Text = dr["Notes"].ToString();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Helpers.SendEmail("ViewNote.aspx - " + ex.Message);
			}
		}
	}
}
