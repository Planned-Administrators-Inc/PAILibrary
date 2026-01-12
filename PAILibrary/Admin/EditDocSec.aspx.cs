using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAILibrary
{
	public partial class EditDocSec : System.Web.UI.Page
	{
		int editRow = 0;
		string user = string.Empty;

		protected void Page_Load(object sender, EventArgs e)
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

            sdsPAILib.ConnectionString = Helpers.GetSQLConString();
            //user = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            litFooter.Text = string.Empty;

			if (!Page.IsPostBack)
				editRow = 0;
		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			BindData();
		}

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			string code = Code.Value.Trim();
			string uid = UserID.Value.Trim();
			int ret = 0;
			
			try
			{
				if (code == string.Empty)
				{
					litFooter.Text = "Please enter a code.";
					Code.Focus();
					return;
				}
				if (uid == string.Empty)
				{
					litFooter.Text = "Please enter a User ID.";
					UserID.Focus();
					return;
				}

				using (SqlConnection  con = new SqlConnection(Helpers.GetSQLConString()))
				using (SqlCommand cmd = new SqlCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "usp_DL_InsertSecurityCode";
					cmd.CommandTimeout = 0;

					cmd.Parameters.AddWithValue("@Code", code);
					cmd.Parameters.AddWithValue("@UserID", uid);
					// create our output param
					SqlParameter pOut = new SqlParameter("@SID", SqlDbType.Int);
					pOut.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(pOut);
					
					// execute and get our return param
					cmd.ExecuteNonQuery();
					int.TryParse(cmd.Parameters["@SID"].Value.ToString(), out ret);

					litFooter.Text = "New document security added.<br />";
					UserID.Value = "";
					
					// add audit record
					Helpers.InsertAudit(ret.ToString(), "DL_Security", "", code + ":" + uid, user, "Added");
					
					BindData();
				}
			}
			catch (SqlException se)
			{
				if (se.Number == 547) // FK violation
				{
					AddMessage("The code entered does not exists.<br />" +
						"Please enter the code <a href=\"EditDocCodes.aspx\">here</a> and try again.<br />");
				}
			}
			catch (Exception ex)
			{
				litFooter.Text = "Error: " + ex.Message;
			}
		}

		private void BindData()
		{
			string code = Code.Value.Trim();
			string userID = UserID.Value.Trim();
			string sql = "";

			// build our sql string
			sql = "SELECT SID, Code, UserID FROM DL_Security WHERE 1=1";
			if (code != string.Empty)
				sql += string.Format(" AND Code = '{0}'", code);
			if (userID != string.Empty)
				sql += string.Format(" AND UserID LIKE '%{0}%'", userID);

			sdsPAILib.SelectCommand = sql;
			sdsPAILib.DataBind();
		}

		private void AddMessage(string msg)
		{
			litFooter.Text += msg + "<br />";
		}

		protected void gvResults_PreRender(object sender, EventArgs e)
		{
			if ((editRow + 3) < gvResults.Rows.Count)
			{
				gvResults.Rows[editRow + 3].Focus();
				gvResults.Rows[editRow].Focus();
			}
			if (Page.IsPostBack)
				AddMessage(gvResults.Rows.Count.ToString() + " row(s) found.");
		}

		protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			gvResults.EditIndex = -1;
			BindData();
		}

		protected void gvResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			try
			{
				using (SqlConnection con = new SqlConnection())
				using (SqlCommand cmd = new SqlCommand())
				{
					con.ConnectionString = Helpers.GetSQLConString();
					con.Open();

					cmd.Connection = con;
					cmd.CommandTimeout = 0;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "usp_DL_DeleteSecurityCode";
					cmd.Parameters.AddWithValue("@SID", e.Keys[0].ToString());
					cmd.ExecuteNonQuery();
					
					// add audit record
					Helpers.InsertAudit(e.Keys["SID"].ToString(), "DL_Security",
						e.Values["Code"].ToString() + ":" + e.Values["UserID"].ToString(), "", user, "Deleted");
				}
			}
			catch (SqlException se)
			{
				AddMessage("SQL Err #: " + se.Number.ToString());
				AddMessage(se.Message);
			}
			catch (Exception ex)
			{
				AddMessage(ex.Message);
			}
		}

		protected void gvResults_RowDeleted(object sender, GridViewDeletedEventArgs e)
		{
			AddMessage("Row deleted for " + e.Keys["SID"].ToString());
			BindData();
		}

		protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
		{
			gvResults.EditIndex = e.NewEditIndex;
			BindData();
		}

		protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			gvResults.EditIndex = -1;
			BindData();
		}

		protected void gvResults_RowUpdated(object sender, GridViewUpdatedEventArgs e)
		{
			string _old = string.Empty;
			string _new = string.Empty;

			_old = e.OldValues["Code"].ToString() + ":" + e.OldValues["UserID"].ToString();
			_new = e.NewValues["Code"].ToString() + ":" + e.NewValues["UserID"].ToString();
			if (_old.ToUpper() != _new.ToUpper())
			{
				Helpers.InsertAudit(e.Keys["SID"].ToString(), "DL_Security", _old, _new, user, "Edit");
			}
		}

		protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				if (e.Row.RowState == DataControlRowState.Edit
					|| e.Row.RowState == (DataControlRowState.Alternate | DataControlRowState.Edit))
				{
					if (e.Row.HasControls())
					{
						foreach (Control c in e.Row.Controls)
						{
							DataControlFieldCell dc = (DataControlFieldCell)c;
							if (dc.HasControls())
							{
								foreach (Control c2 in dc.Controls)
								{
									switch (c2.GetType().Name)
									{
									case "LinkButton":
										LinkButton li = (LinkButton)c2;
										switch (li.Text)
										{
										case "Update":
										case "Cancel":
											e.Row.Focus(); // set focus on item being edited
											editRow = e.Row.RowIndex;
											li.Visible = true;
											break;
										case "Delete":
										case "Edit":
											li.Visible = false;
											break;
										}
										break;
									case "TextBox":
										TextBox t = (TextBox)c2;
										t.Text = t.Text.Trim();
										t.MaxLength = 250;
										t.Style.Add("width", "100%");
										t.Focus();
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		protected void sdsPAILib_Deleting(object sender, SqlDataSourceCommandEventArgs e)
		{
			e.Cancel = true;
		}
	}
}
