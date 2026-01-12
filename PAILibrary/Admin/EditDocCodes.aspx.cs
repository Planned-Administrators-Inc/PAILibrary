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

namespace PAILibrary
{
  public partial class EditDocCodes : System.Web.UI.Page
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

    protected void btnAdd_Click(object sender, EventArgs e)
    {
      string code = Code.Value.Trim();
      string docType = DocType.Value.Trim();

      // basic validation
      if (code == string.Empty || docType == string.Empty)
      {
        litFooter.Text = "Please enter a code and Doc Type.";
        return;
      }
      if (code.Length > 5)
      {
        litFooter.Text = "Please enter a code that is 1 to 5 characters.";
        return;
      }
      if (docType.Length > 250)
      {
        litFooter.Text = "Please enter a Doc Type that is 1 to 250 characters.";
        return;
      }

      try
      {
        using (SqlConnection con = new SqlConnection(Helpers.GetSQLConString()))
        using (SqlCommand cmd = new SqlCommand())
        {
          con.Open();

          cmd.Connection = con;
          cmd.CommandTimeout = 0;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = "usp_DL_InsertDocType";
          cmd.Parameters.AddWithValue("@Code", code);
          cmd.Parameters.AddWithValue("@DocType", docType);
          cmd.ExecuteNonQuery();

          // update user
          litFooter.Text = "Document type successfully added.";
          // add audit log
          Helpers.InsertAudit(code, "DL_Codes", "", docType, user, "Added");

          // get new results
          BindData();
        }

        // create the directory if it doesn't exist
        IntPtr token = IntPtr.Zero;
        IntPtr tokenDuplicate = IntPtr.Zero;
        string SaveLocation = Path.Combine(Helpers.LibPath, code);

        //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************
        //  if (!NativeMethods.LogonUser(
        //	Helpers.Username, Helpers.Domain, Helpers.Password,
        //	NativeMethods.LogonType.NewCredentials,
        //	NativeMethods.LogonProvider.Default,
        //	out token))
        //{
        //	AddMessage("Unable to authenticate with network credentials."
        //		+ "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.", false);
        //	return;
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
          AddMessage("Unable to authenticate with network credentials."
            + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.", false);
          return;
        }
        //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - END *************************************************************

        if (!NativeMethods.DuplicateToken(
  token,
  NativeMethods.SecurityImpersonationLevel.Impersonation,
  out tokenDuplicate))
        {
          AddMessage("Unable to duplicate network credentials."
            + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.", false);
          return;
        }

        using (WindowsImpersonationContext impersonationContext =
          new WindowsIdentity(tokenDuplicate).Impersonate())
        {
          // make sure this file doesn't already exist
          if (File.Exists(SaveLocation))
          {
            AddMessage("This file already exists in the document database."
              + "<br />Please delete the document <a href=\"EditDocList.aspx\">here</a> first.", false);
            return;
          }

          // Here is were we can finally do our stuff
          // save the file to the document library @ \\pai_server\Public\library\
          if (Directory.Exists(SaveLocation) == false)
          {
            Directory.CreateDirectory(SaveLocation);
          }

          // end our impersonation
          impersonationContext.Undo();
        }
      }
      catch (SqlException se)
      {
        if (se.Number == 2601 || se.Number == 2627)
        {
          litFooter.Text = "Cannot insert duplicate key.&nbsp; Code " + code + " already exists.";
          Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "SelectText", "window.setTimeout(\"document.getElementById('Code').select();\", 100);", true);
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
      string docType = DocType.Value.Trim();
      string sql = "";

      // build our sql string
      sql = "SELECT Code, DocType FROM DL_Codes WHERE 1=1";
      if (code != string.Empty)
        sql += string.Format(" AND Code = '{0}'", code);
      if (docType != string.Empty)
        sql += string.Format(" AND DocType LIKE '%{0}%'", docType);

      sdsPAILib.SelectCommand = sql;
      sdsPAILib.DataBind();
    }

    private void AddMessage(string msg, bool clear)
    {
      if (clear || litFooter.Text.Trim() == string.Empty)
      {
        litFooter.Text = msg;
      }
      else
      {
        litFooter.Text += "<br />" + msg;
      }
    }

    protected void gvResults_PreRender(object sender, EventArgs e)
    {
      if ((editRow + 3) < gvResults.Rows.Count)
      {
        gvResults.Rows[editRow + 3].Focus();
        gvResults.Rows[editRow].Focus();
      }
      if (Page.IsPostBack)
        AddMessage(gvResults.Rows.Count.ToString() + " row(s) found.", false);
    }

    protected void gvResults_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
      gvResults.EditIndex = -1;
      BindData();
    }

    protected void gvResults_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
      string code = e.Keys[0].ToString();
      SqlDataReader dr = null;

      // we need to check to see if there are documents with this doc Code
      // if so, cancel the delete and let the user know.
      using (SqlConnection con = new SqlConnection(Helpers.GetSQLConString()))
      using (SqlCommand cmd = new SqlCommand())
      {
        con.Open();

        cmd.Connection = con;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "usp_DL_CheckCodeForDocuments";
        cmd.CommandTimeout = 0;
        cmd.Parameters.AddWithValue("@Code", code);
        using (dr = cmd.ExecuteReader())
        {
          if (dr.HasRows)
          {
            // oops, let user know we cannot delete this code yet
            AddMessage("Document(s) exist that are using this code '" + code.ToString() + "'<br />"
              + "You must delete all documents using this code before you can delete it.<br />", true);
            Page.ClientScript.RegisterClientScriptBlock(
              Page.GetType(),
              "SelectText", "window.setTimeout(\"document.getElementById('Code').select();\", 100);",
              true);
            e.Cancel = true;
          }
          else
          {
            if (!dr.IsClosed)
              dr.Close();

            // we need to not only delete this code from DL_Codes, but we have to remove any records
            // with this code in DL_Security
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_DL_DeleteCode";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Code", code);
            cmd.ExecuteNonQuery();

            // default to only the code for search criteria
            Code.Value = "";
          }
        }
      }
    }

    protected void gvResults_RowDeleted(object sender, GridViewDeletedEventArgs e)
    {
      BindData();
      litFooter.Text = "Row deleted successfully.";
      Helpers.InsertAudit(e.Keys["Code"].ToString(), "DL_Codes", e.Values["DocType"].ToString(), "", user, "Deleted");
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
      foreach (DictionaryEntry de in e.NewValues)
      {
        if (e.OldValues.Contains(de.Key))
        {
          if (e.OldValues[de.Key].ToString() != e.NewValues[de.Key].ToString())
          {
            Helpers.InsertAudit(e.Keys["Code"].ToString(), "DL_Codes",
              e.OldValues[de.Key].ToString(), e.NewValues[de.Key].ToString(), user, "Edit");
          }
        }
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
