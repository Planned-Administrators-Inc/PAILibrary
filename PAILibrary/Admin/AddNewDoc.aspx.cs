using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Web;

namespace PAILibrary
{
  public partial class AddNewDoc : System.Web.UI.Page
  {
    private string Username = string.Empty;
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
        Username = Helpers.RemoveDomainFromUserName(HttpContext.Current.Request.LogonUserIdentity.Name);
      else
        Username = Helpers.RemoveDomainFromUserName(User.Identity.Name);
      //NEW CODE END

      if (!Helpers.CheckAdminUser(Username))
        Response.Redirect("../NotAdmin.aspx");

      //if (!Helpers.CheckAdminUser(User.Identity.Name))
      //Response.Redirect("../NotAdmin.aspx");

      //Username = Helpers.RemoveDomainFromUserName(User.Identity.Name);
      litFooter.Text = string.Empty;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
      IntPtr token = IntPtr.Zero;
      IntPtr tokenDuplicate = IntPtr.Zero;

      if ((UploadFile.PostedFile != null) && (UploadFile.PostedFile.ContentLength > 0))
      {
        string fileName = Path.GetFileName(UploadFile.PostedFile.FileName);
        string code = string.Empty;
        string SaveLocation = string.Empty;

        try
        {
          // verify file name convention, doc code and date stamp
          if (!VerifyFileName(fileName))
          {
            AddUserMessage("The uploaded file is not in the proper naming convention."
              + "<br />Please fix the name and upload again.");
            return;
          }
          if (!VerifyFileCode(fileName))
          {
            AddUserMessage("The Doc Code in the file is not present in the database."
              + "<br />Please <a href='EditDocCodes.aspx'>add the Doc Code</a> and upload again.");
            return;
          }
          string ret = VerifyFileDate(fileName);
          if (ret == "Error" || ret != string.Empty)
          {
            if (ret != string.Empty)
              AddUserMessage(ret);
            return;
          }

          code = GetFileCode(fileName);
          if (code == "063")  // plan documents
          {
            if (!VerifyPlanDocumentName(fileName))
              return;
          }

          SaveLocation = Path.Combine(Helpers.LibPath, code);
          SaveLocation = Path.Combine(SaveLocation, fileName);

          //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************
          //if (!NativeMethods.LogonUser(
          //    Helpers.Username, Helpers.Domain, Helpers.Password,
          //    NativeMethods.LogonType.NewCredentials,
          //    NativeMethods.LogonProvider.Default,
          //    out token))
          //{
          //    AddUserMessage("Unable to authenticate with network credentials."
          //        + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.");
          //    return;
          //}
          //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - END **************************************************************

          // here we need to authenticate as a real user since the user that ASP.Net runs
          // as is not a valid local user on the share we want to save to nor is it
          // a valid domain user
          //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************


          //              if (!NativeMethods.LogonUser(
          //Helpers.GetInfoFromINI("USERNAME", iniUsername), Helpers.GetInfoFromINI("DOMAIN", iniDomain), Helpers.GetInfoFromINI("PASSWORD", iniPassword),
          //                  NativeMethods.LogonType.NewCredentials,
          //NativeMethods.LogonProvider.Default,
          //out token))
          if (!NativeMethods.LogonUser(iniUsername, iniDomain, iniPassword,
              NativeMethods.LogonType.NewCredentials,
              NativeMethods.LogonProvider.Default,
              out token))
          {
            AddUserMessage("Unable to authenticate with network credentials."
              + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.");
            return;
          }
          //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - END *************************************************************

          if (!NativeMethods.DuplicateToken(
  token,
  NativeMethods.SecurityImpersonationLevel.Impersonation,
  out tokenDuplicate))
          {
            AddUserMessage("Unable to duplicate network credentials."
              + "<br />Please email this error to <a href='mailto://schedjob@paisc.com'>IS Support</a>.");
            return;
          }

          using (WindowsImpersonationContext impersonationContext =
            new WindowsIdentity(tokenDuplicate).Impersonate())
          {
            // make sure this file doesn't already exist
            if (File.Exists(SaveLocation))
            {
              AddUserMessage("This file already exists in the document database."
                + "<br />Please delete the document <a href=\"EditDocList.aspx\">here</a> first.");
              return;
            }

            // Here is were we can finally do our stuff
            // save the file to the document library @ \\pai_server\Public\library\
            UploadFile.PostedFile.SaveAs(SaveLocation);

            // save the file to \\pai_server\Public\library\
            int docID = SaveDocToDatabase(UploadFile.PostedFile.FileName);
            if (docID == 0)
            {
              // remove the file from the doc lib
              File.Delete(SaveLocation);
              AddUserMessage("Unable to save the file to the Doc Database"
                + "<br />Please notify IT with this full message.");
              return;
            }
            else
            {
              // save in audit log
              Helpers.InsertAudit(docID.ToString(), "DL_Docs", "", fileName, Username, "Added");
              // tell the user they did a good job
              AddUserMessage("The file " + fileName + " has been uploaded successfully.");
            }

            // end our impersonation
            impersonationContext.Undo();
          }
        }
        catch (Exception ex)
        {
          AddUserMessage("Error: " + ex.Message);
        }
        finally
        {
          if (tokenDuplicate != IntPtr.Zero)
          {
            NativeMethods.CloseHandle(tokenDuplicate);
          }
          if (token != IntPtr.Zero)
          {
            NativeMethods.CloseHandle(token);
          }
        }
      }
      else
      {
        AddUserMessage("Please select a file to upload.");
      }
    }

    private bool VerifyFileName(string fileName)
    {
      bool ret = true;

      try
      {
        // make sure the file fits the naming convention
        string[] parts = fileName.Split(new char[] { '-' }, 5);
        if (parts.Length != 5)
        {
          ret = false;
        }

        // don't allow spaces in plan documents
        if (parts[1].Trim() == "063")
        {
          foreach (string s in parts)
          {
            if (s.IndexOf(' ') != -1)
            {
              AddUserMessage("You can't have spaces in files names for plan documents.");
              return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        AddUserMessage("Error: " + ex.Message);
        ret = false;
      }

      return ret;
    }

    private string VerifyFileDate(string fileName)
    {
      string ret = string.Empty;
      int tmp = 0;

      try
      {
        // make sure the file fits the naming convention
        string date = fileName.Split(new char[] { '-' }, 5)[2];
        string year = string.Empty;
        string month = string.Empty;
        string day = string.Empty;

        if (date.Length != 8)
          return "Date stamp needs to be a valid 8 digit value in the form yyyyMMdd.";

        // validate the year
        year = date.Substring(0, 4);
        if (!int.TryParse(year, out tmp))
          return "Please enter a valid 4 digit year for the date stamp.";
        if (tmp < 1900 || tmp > DateTime.Now.AddYears(1).Year)
          return "Please enter a valid year between 1900 and "
            + DateTime.Now.AddYears(1).Year.ToString();

        // validate the month
        month = date.Substring(4, 2);
        if (!int.TryParse(month, out tmp))
          return "Please enter a valid 2 digit month for the date stamp.";
        if (tmp < 1 || tmp > 12)
          return "Please enter a valid month between 01 and 12 for the date stamp.";

        // validate the day
        day = date.Substring(6, 2);
        if (!int.TryParse(month, out tmp))
          return "Please enter a valid 2 digit day for the date stamp.";
        if (tmp < 1 || tmp > 31)
          return "Please enter a valid day between 01 and 31 for the date stamp.";
      }
      catch (Exception ex)
      {
        AddUserMessage("Error: " + ex.Message);
        ret = "Error";
      }

      return ret;
    }

    private string GetFileCode(string fileName)
    {
      return fileName.Split(new char[] { '-' }, 5)[1];
    }

    private bool VerifyFileCode(string fileName)
    {
      SqlConnection con = null;
      SqlCommand cmd = null;
      bool ret = true;

      try
      {
        // make sure the file fits the naming convention
        string code = fileName.Split(new char[] { '-' }, 5)[1];

        // make sure the code in the file name is in our DL_Codes table
        con = new SqlConnection(Helpers.GetSQLConString());
        con.Open();

        cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "usp_DL_CheckDocCode";
        cmd.Parameters.AddWithValue("@Code", code);
        using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
        {
          if (!dr.HasRows)
          {
            ret = false;
          }
        }
      }
      catch (Exception ex)
      {
        AddUserMessage("Error: " + ex.Message);
        ret = false;
      }
      finally
      {
        if (con != null && con.State == ConnectionState.Open)
        {
          con.Close();
        }
      }

      return ret;
    }

    private bool VerifyPlanDocumentName(string fileName)
    {
      bool ret = true;
      string tmpFile = string.Empty;
      string version = string.Empty;
      string desc = string.Empty;
      string plan = string.Empty;
      string type = string.Empty;
      int v = 0;

      try
      {
        FileInfo f = new FileInfo(fileName);
        tmpFile = f.Name.Replace(f.Extension, "");

        // make sure the file fits the naming convention
        string[] parts = tmpFile.Split(new char[] { '-' }, 5);

        // version must be numeric
        version = parts[3];
        if (int.TryParse(version, out v) == false)
        {
          AddUserMessage("The version part of the file name must be numeric.");
          return false;
        }

        // the full description part must be the plan number-{Med,Den,Vis,RX}
        desc = parts[4];
        string[] descParts = desc.Split(new char[] { '-' }, 2);
        if (descParts.GetLength(0) != 2)
        {
          AddUserMessage("The full description part must be in the form: PlanNumber-{Med,Den,Vis,RX}.");
          return false;
        }
        if (descParts[1].ToUpper() != "MED" && descParts[1].ToUpper() != "DEN" && descParts[1].ToUpper() != "VIS" && descParts[1].ToUpper() != "RX")
        {
          AddUserMessage("The full description part must be in the form: PlanNumber-{Med,Den,Vis,RX}.");
          return false;
        }
      }
      catch (Exception ex)
      {
        AddUserMessage("Error: " + ex.Message);
        ret = false;
      }

      return ret;
    }

    private int SaveDocToDatabase(string fileName)
    {
      SqlConnection con = null;
      SqlCommand cmd = null;
      FileInfo fi = null;
      string[] parts = null;
      int ret = 0;

      try
      {
        fi = new FileInfo(fileName);

        // split the file name on the '-' character
        parts = fi.Name.Split(new char[] { '-' }, 5);
        parts[4] = parts[4].Replace(fi.Extension, "");

        // connect and save to DB
        con = new SqlConnection(Helpers.GetSQLConString());
        con.Open();

        cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "usp_DL_InsertDoc";
        cmd.Parameters.AddWithValue("@GroupNo", parts[0]);
        cmd.Parameters.AddWithValue("@Code", parts[1]);
        cmd.Parameters.AddWithValue("@DateStamp", parts[2]);
        cmd.Parameters.AddWithValue("@Version", parts[3]);
        cmd.Parameters.AddWithValue("@FullDesc", parts[4]);
        cmd.Parameters.AddWithValue("@Ext", fi.Extension);
        cmd.Parameters.AddWithValue("@FileName", fi.Name);
        // create our output param
        SqlParameter pOut = new SqlParameter("@DocID", SqlDbType.Int);
        pOut.Direction = ParameterDirection.Output;
        cmd.Parameters.Add(pOut);

        // save
        cmd.ExecuteNonQuery();
        if (int.TryParse(cmd.Parameters["@DocID"].Value.ToString(), out ret) == false)
          ret = 0;
      }
      catch (SqlException se)
      {
        if (se.Number == 2601 || se.Number == 2627)
          AddUserMessage("While this file doesn't exist in the library, it exists in the database.<br />"
            + "Please delete the document <a href=\"EditDocList.aspx\">here</a> first.");
        else
          AddUserMessage("Sql Error Number: " + se.Number.ToString());
        ret = 0;
      }
      catch (Exception ex)
      {
        AddUserMessage("Error: " + ex.Message);
        ret = 0;
      }
      finally
      {
        if (con != null && con.State == ConnectionState.Open)
        {
          con.Close();
        }
      }

      return ret;
    }

    private void AddUserMessage(string msg)
    {
      if (litFooter.Text.Trim() == string.Empty)
        litFooter.Text += msg;
      else
        litFooter.Text += "<br />" + msg;
    }
  }
}
