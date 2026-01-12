using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAILibrary.Admin
{
	public partial class AuditSearch : System.Web.UI.Page
	{
		private string userName = string.Empty;

		protected void Page_Load(object sender, EventArgs e)
		{
            if (Helpers.Offline == true)
                Response.Redirect("../Offline.aspx");

            //NEW CODE START
            if (User.Identity.Name == "")
                userName = Helpers.RemoveDomainFromUserName(HttpContext.Current.Request.LogonUserIdentity.Name);
            else
                userName = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            //NEW CODE END

            if (!Helpers.CheckAdminUser(userName))
                Response.Redirect("../NotAdmin.aspx");

            //if (!Helpers.CheckAdminUser(User.Identity.Name))
            //Response.Redirect("../NotAdmin.aspx");

            //Username = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            litFooter.Text = string.Empty;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtCode.Text) && string.IsNullOrEmpty(txtGroup.Text)
				&& string.IsNullOrEmpty(txtDoc.Text) && string.IsNullOrEmpty(txtUserID.Text)
				&& string.IsNullOrEmpty(txtTargetDate.Text))
			{
				litFooter.Text = "Please enter at least one search criteria";
				return;
			}
			else
			{
				BindData();
			}
		}
		
		private void BindData()
		{
			string group = txtGroup.Text.Trim();
			string code = txtCode.Text.Trim();
			string docSearch = txtDoc.Text.Trim();
			string user = txtUserID.Text.Trim();
			string date = txtTargetDate.Text.Trim();

			try
			{
				using (SqlConnection con = new SqlConnection(Helpers.ConString))
				using (SqlCommand cmd = new SqlCommand())
				{
					// get messages from the server
					con.InfoMessage += new SqlInfoMessageEventHandler(con_InfoMessage);
					con.Open();
					cmd.Connection = con;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "usp_DL_SearchAudit";

					// add parameters
					if (group + code != string.Empty)
						cmd.Parameters.AddWithValue("@GroupCode", group + "-" + code);
					if (docSearch != string.Empty)
						cmd.Parameters.AddWithValue("@txtSearch", docSearch);
					if (user != string.Empty)
						cmd.Parameters.AddWithValue("@UserID", user);
					if (date != string.Empty)
						cmd.Parameters.AddWithValue("@AuditDate", date);

					SqlDataAdapter da = new SqlDataAdapter(cmd);
					DataSet ds = new DataSet();
					da.Fill(ds);
					gvResults.DataSource = ds;
					gvResults.DataBind();
				}
			}
			catch (Exception ex)
			{
				litFooter.Text = ex.Message;
			}
		}

		void con_InfoMessage(object sender, SqlInfoMessageEventArgs e)
		{
			litFooter.Text = e.Message;
		}
	}
}
