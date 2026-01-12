using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// These were removed from the aspx page.
//<!-- <asp:linkbutton id="lbEdit" runat="server" commandname="Edit" forecolor="#333333" text="Edit"></asp:linkbutton> -->
//<!-- <asp:linkbutton id="lbCancel" runat="server" commandname="Cancel" forecolor="#333333" text="Cancel" visible="false"></asp:linkbutton> -->
//<!-- <asp:linkbutton id="lbUpdate" runat="server" commandname="Update" forecolor="#333333" text="Update" visible="false"></asp:linkbutton> -->

namespace PAILibrary
{
  public partial class EditDocList : System.Web.UI.Page
  {
    int editRow = 0;
    string user = string.Empty;
    //private string iniUsername = string.Empty;
    //private string iniDomain = string.Empty;
    //private string iniPassword = string.Empty;

    private string iniUsername = ConfigurationManager.AppSettings["Username"];
    private string iniDomain = ConfigurationManager.AppSettings["Domain"];
    private string iniPassword = ConfigurationManager.AppSettings["Password"];

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

      //user = Helpers.RemoveDomainFromUserName(User.Identity.Name);
      litFooter.Text = string.Empty;

      if (!Page.IsPostBack)
        editRow = 0;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
      BindData();
    }

    protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
    {
      gvResults.EditIndex = e.NewEditIndex;
      BindData();
    }

    protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
      gvResults.EditIndex = -1;
      BindData();
    }

    protected void gvResults_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
      gvResults.EditIndex = -1;
      BindData();
      AddMessage("Row updated.");
    }

    protected void gvResults_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
      BindData();
      AddMessage("Row deleted.");
      // rebuild the docment name
      string fileName = e.Values["GroupNo"].ToString() + "-" + e.Values["Code"].ToString()
        + "-" + e.Values["DateStamp"].ToString() + "-" + e.Values["Version"].ToString()
        + "-" + e.Values["FullDesc"].ToString() + "." + e.Values["Ext"].ToString();
      Helpers.InsertAudit(e.Keys["DocID"].ToString(), "DL_Docs", fileName, "", user, "Deleted");
    }

    protected void gvResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
      IntPtr token = IntPtr.Zero;
      IntPtr tokenDuplicate = IntPtr.Zero;
      string docId = e.Keys["DocID"].ToString();

      try
      {
        using (SqlConnection con = new SqlConnection())
        using (SqlCommand cmd = new SqlCommand())
        {
          con.ConnectionString = Helpers.GetSQLConString();
          con.Open();

          cmd.Connection = con;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandTimeout = 0;
          cmd.CommandText = "usp_DL_GetFilenameAndCode";
          cmd.Parameters.AddWithValue("@DocID", docId);
          using (SqlDataReader dr = cmd.ExecuteReader())
          {
            if (dr.HasRows)
            {
              dr.Read();
              string fileName = dr["FileName"].ToString();
              string code = dr["Code"].ToString();
              string fullFile = Path.Combine(Helpers.LibPath, Path.Combine(code, fileName));

              //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************
              //                     if (!NativeMethods.LogonUser(
              //	Helpers.Username, Helpers.Domain, Helpers.Password,
              //	NativeMethods.LogonType.NewCredentials,
              //	NativeMethods.LogonProvider.Default,
              //	out token))
              //{
              //	litFooter.Text = "Unable to authenticate with network credentials."
              //		+ "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.";
              //	e.Cancel = true;
              //}
              //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - END **************************************************************

              // here we need to authenticate as a real user since the user that ASP.Net runs
              // as is not a valid local user on the share we want to save to nor is it
              // a valid domain user
              //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************
              if (!NativeMethods.LogonUser(iniUsername, iniDomain, iniPassword,
                  NativeMethods.LogonType.NewCredentials,
                  NativeMethods.LogonProvider.Default,
                  out token))


                //if (!NativeMethods.LogonUser(
                //  Helpers.GetInfoFromINI("USERNAME", iniUsername), Helpers.GetInfoFromINI("DOMAIN", iniDomain), Helpers.GetInfoFromINI("PASSWORD", iniPassword),
                //  NativeMethods.LogonType.NewCredentials,
                //  NativeMethods.LogonProvider.Default,
                //  out token))
              {
                litFooter.Text = "Unable to authenticate with network credentials."
                  + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.";
                e.Cancel = true;
              }
              //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - END *************************************************************

              if (!NativeMethods.DuplicateToken(
  token,
  NativeMethods.SecurityImpersonationLevel.Impersonation,
  out tokenDuplicate))
              {
                litFooter.Text = "Unable to authenticate with network credentials."
                  + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.";
                e.Cancel = true;
              }

              using (WindowsImpersonationContext impersonationContext =
                new WindowsIdentity(tokenDuplicate).Impersonate())
              {
                // Here is were we can finally do our stuff
                // delete the file from the document library @ \\pai_server\Public\library\
                if (File.Exists(fullFile))
                {
                  File.Delete(fullFile);
                }

                // end our impersonation
                impersonationContext.Undo();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        litFooter.Text = ex.Message;
        e.Cancel = true;
      }
    }

    protected void gvResults_PreRender(object sender, EventArgs e)
    {
      if ((editRow + 3) < gvResults.Rows.Count)
      {
        gvResults.Rows[editRow + 3].Focus();
        gvResults.Rows[editRow].Focus();
      }
      //if ( Page.IsPostBack )
      //	AddMessage( gvResults.Rows.Count.ToString() + " row(s) found." );
    }

    protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
    {
      if (e.Row.RowType == DataControlRowType.DataRow)
      {
        if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
        {
          string key = gvResults.DataKeys[e.Row.DataItemIndex].Value.ToString();
          HyperLink hl1 = (HyperLink)e.Row.Controls[8].Controls[1];
          hl1.NavigateUrl = string.Format(hl1.NavigateUrl, key);
          HyperLink hl2 = (HyperLink)e.Row.Controls[8].Controls[3];
          hl2.NavigateUrl = string.Format(hl2.NavigateUrl, key);
          HyperLink hl3 = (HyperLink)e.Row.Controls[0].Controls[1];
          hl3.NavigateUrl = string.Format(hl3.NavigateUrl, key);
        }
        if (e.Row.RowState == DataControlRowState.Edit
          || e.Row.RowState == (DataControlRowState.Alternate | DataControlRowState.Edit))
        {
          e.Row.Focus(); // set focus on item being edited
          editRow = e.Row.RowIndex;

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

    protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
      DataSourceSelectArguments args = new DataSourceSelectArguments();
      gvResults.PageIndex = e.NewPageIndex;
      litFooter.Text = "Pages " + gvResults.PageCount.ToString();
      args.StartRowIndex = gvResults.PageIndex;
      DataView dv = (DataView)gvResults.DataSource;

    }

    private void BindData()
    {
      string group = Group.Value.Trim();
      string code = Code.Value.Trim();
      string dateStamp = DateStamp.Value.Trim();
      string version = Version.Value.Trim();
      string desc = Desc.Value.Trim();
      string sql = "";

      // build our sql string
      sql = "SELECT DocID, GroupNo, Code, DateStamp, Version, FullDesc, Ext, Notes FROM DL_Docs WHERE 1=1";
      if (group != string.Empty)
        sql += string.Format(" AND GroupNo = '{0}'", group);
      if (code != string.Empty)
        sql += string.Format(" AND Code = '{0}'", code);
      if (dateStamp != string.Empty)
        sql += string.Format(" AND DateStamp LIKE '{0}%'", dateStamp);
      if (version != string.Empty)
        sql += string.Format(" AND VERSION = '{0}'", version);
      if (desc != string.Empty)
        sql += string.Format(" AND FullDesc LIKE '%{0}%'", desc);
      sql += " ORDER BY GroupNo, Code, DateStamp";

      sdsPAILib.SelectCommand = sql;
      sdsPAILib.DataBind();
    }

    private void AddMessage(string msg)
    {
      litFooter.Text += msg;
    }
  }
}
