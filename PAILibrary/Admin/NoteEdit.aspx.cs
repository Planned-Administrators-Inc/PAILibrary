using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PAILibrary.Admin;

namespace PAILibrary.Admin
{
	public partial class NoteEdit : System.Web.UI.Page
	{
		private NameValueCollection q = null;
		private string _noteID = string.Empty;
		private string user = string.Empty;

		protected void Page_Load( object sender, EventArgs e )
		{
            if (Helpers.Offline == true)
                Response.Redirect("../Offline.aspx");

            //NEW CODE START
            if (User.Identity.Name == "")
                user = Helpers.RemoveDomainFromUserName(HttpContext.Current.Request.LogonUserIdentity.Name);
            else
                user = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            //NEW CODE END

            if (!Helpers.CheckAdminUser(user))
                Response.Redirect("../NotAdmin.aspx");

            //if (!Helpers.CheckAdminUser(User.Identity.Name))
            //Response.Redirect("../NotAdmin.aspx");

            //user = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            litFooter.Text = string.Empty;

            if ( !Page.IsPostBack )
			{
				if (Request.QueryString.Count > 0 && CKNotes.Text == string.Empty)
				{
					q = Request.QueryString;
					LoadNote();
				}
				else if (Request.QueryString.Count == 0)
					litFooter.Text = "Invalid Note ID.";
			}
			else
			{
				BtnSave_Click( null, null );
			}
		}

		protected void BtnSave_Click( object sender, EventArgs e )
		{
			if ( HidNID.Value.Trim() != string.Empty )
			{
				using ( SqlConnection con = new SqlConnection() )
				using ( SqlCommand cmd = new SqlCommand() )
				{
					try
					{
						con.ConnectionString = Helpers.GetSQLConString();
						con.Open();

						cmd.Connection = con;
						cmd.CommandTimeout = 0;
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.CommandText = "usp_DL_UpdateNotes";

						// add our parameters and execute
						cmd.Parameters.AddWithValue( "@DocID", HidNID.Value );
						string replaceWith = " ";
						string removedBreaks = CKNotes.Text.Replace( "\n", replaceWith ).Replace( "\r", replaceWith );
						cmd.Parameters.AddWithValue( "@Notes", removedBreaks.Trim() );
						cmd.ExecuteNonQuery();
						litFooter.Text = "Note saved.";

						// add an audit that the note was saved/edited
						Helpers.InsertAudit(HidNID.Value, "DL_Docs", "Notes", "", user, "Edit");
					}
					catch ( Exception ex )
					{
						litFooter.Text = ex.Message;
					}
				}
			}
			else
			{
				litFooter.Text = "Invalid Note ID.";
			}
		}
		
		private void LoadNote()
		{
			if ( q["nid"] != null )
			{
				string nid = q["nid"];
				using ( SqlConnection con = new SqlConnection() )
				using ( SqlCommand cmd = new SqlCommand() )
				{
					con.ConnectionString = Helpers.GetSQLConString();
					con.Open();

					cmd.Connection = con;
					cmd.CommandTimeout = 0;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "usp_DL_GetNotes";
					cmd.Parameters.AddWithValue( "@DocID", nid );
					using ( SqlDataReader dr = cmd.ExecuteReader() )
					{
						if ( dr.HasRows )
						{
							dr.Read();
							CKNotes.Text = dr["Notes"].ToString();
							HidNID.Value = nid;
						}
					}
				}
			}
		}
	}
}
