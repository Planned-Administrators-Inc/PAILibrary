using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace PAILibrary
{
  public partial class ViewFile : System.Web.UI.Page
  {
    //private string iniUsername = string.Empty;
    //private string iniDomain = string.Empty;
    //private string iniPassword = string.Empty;

    private string iniUsername = ConfigurationManager.AppSettings["Username"];
    private string iniDomain = ConfigurationManager.AppSettings["Domain"];
    private string iniPassword = ConfigurationManager.AppSettings["Password"];

    protected void Page_Load(object sender, EventArgs e)
    {
      IntPtr token = IntPtr.Zero;
      IntPtr tokenDuplicate = IntPtr.Zero;
      SqlConnection con = null;
      SqlCommand cmd = null;
      DataSet ds = new DataSet();
      string docid = Request.QueryString["docid"];
      string path = Helpers.LibPath;
      string fileName = string.Empty;
      string code = string.Empty;
      string tmpFile = string.Empty;
      string fullFile = string.Empty;

      try
      {
        // admins can still get here when offline
        if (Helpers.Offline == true)
          if (!Helpers.CheckAdminUser(User.Identity.Name))
            Response.Redirect("../Offline.aspx");

        // register a handler for this page
        //Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

        con = new SqlConnection(Helpers.GetSQLConString());
        con.Open();

        cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "usp_DL_GetFilenameAndCode";
        cmd.Parameters.AddWithValue("@DocID", docid);
        using (SqlDataReader dr = cmd.ExecuteReader())
        {
          if (dr.HasRows)
          {
            dr.Read();
            fileName = dr["FileName"].ToString();
            code = dr["Code"].ToString();
            string tmp = Path.Combine(Helpers.LibPath, code);
            fullFile = Path.Combine(tmp, fileName);
          }
          else
          {
            return;
          }
        }

        Response.Clear();

        //**************************************************** OLD CODE: CHANGED TO READ .webconfig FILE INSTEAD - START ***********************************************************
        //  if (!NativeMethods.LogonUser(
        //	Helpers.Username, Helpers.Domain, Helpers.Password,
        //	NativeMethods.LogonType.NewCredentials,
        //	NativeMethods.LogonProvider.Default,
        //	out token))
        //{
        //	Helpers.SendEmail("Unable to authenticate with network credentials - ." + Helpers.Username);
        //	Response.ContentType = "text/html";
        //	Helpers.WriteLogEntry("Unable to authenticate with network credentials.");
        //	Response.Write("Unable to authenticate with network credentials."
        //			+ "<br />Please email this error to <a href='mailto://swright@paisc.com'>Scott Wright</a>.");
        //	Response.End();
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


        //  if (!NativeMethods.LogonUser(
        //    Helpers.GetInfoFromINI("USERNAME", iniUsername), Helpers.GetInfoFromINI("DOMAIN", iniDomain), Helpers.GetInfoFromINI("PASSWORD", iniPassword),
        //    NativeMethods.LogonType.NewCredentials,
        //    NativeMethods.LogonProvider.Default,
        //    out token))
        {
          Helpers.SendEmail("Unable to authenticate with network credentials - ." + Helpers.Username);
          Response.ContentType = "text/html";
          Helpers.WriteLogEntry("Unable to authenticate with network credentials.");
          Response.Write("Unable to authenticate with network credentials."
                  + "<br />Please email this error to <a href='mailto://swright@paisc.com'>Scott Wright</a>.");
          Response.End();
        }
        //**************************************************** NEW CODE: CHANGED TO READ .webconfig FILE INSTEAD - END *************************************************************

        if (!NativeMethods.DuplicateToken(
        token,
        NativeMethods.SecurityImpersonationLevel.Impersonation,
        out tokenDuplicate))
        {
          Response.ContentType = "text/html";
          Helpers.WriteLogEntry("Unable to duplicate network credentials.");
          Response.Write("Unable to duplicate network credentials."
            + "<br />Please email this error to <a href='mailto://swright@paisc.com'>Scott Wright</a>.");
          Response.End();
        }

        using (WindowsImpersonationContext impersonationContext =
            new WindowsIdentity(tokenDuplicate).Impersonate())
        {
          // Here is were we can finally do our stuff

          // see if the file exists on the server
          FileInfo info = new FileInfo(fullFile);
          if (info.Exists)
          {
            // copy the file from the library to this server and send it
            tmpFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            info.CopyTo(tmpFile, true);
            FileInfo infoTmp = new FileInfo(tmpFile);

            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + info.Name + "\"");
            Response.AddHeader("Content-Length", infoTmp.Length.ToString());
            Response.ContentType = "application/octet-stream";

            // write file to browser
            //Response.WriteFile(infoTmp.FullName);
            byte[] bytes = File.ReadAllBytes(infoTmp.FullName);
            Response.OutputStream.Write(bytes, 0, bytes.Length);

            // create a background worker to remove the temp file
            Thread delProc = new Thread(DeleteTempFile);
            delProc.Start(tmpFile);

            //Response.End();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
          }
          else
          {
            Helpers.WriteLogEntry(string.Format("File doesn't exist - {0}.", info.FullName));
            Helpers.SendEmail("ViewFile.aspx - " + string.Format("File doesn't exist - {0}.", info.FullName));
            Response.ContentType = "text/html";
            Response.Write("File not found (" + info.Name + ")"
              + "<br />Please email this error to <a href='mailto://swright@paisc.com'>Scott Wright</a>.");
            Response.End();
          }

          // end our impersonation
          impersonationContext.Undo();
        }
      }
      catch (Exception ex)
      {
        Helpers.WriteLogEntry("ViewFile.aspx - " + ex.Message);
        Helpers.SendEmail(ex.Message);
      }
      finally
      {
        if (con != null && con.State == ConnectionState.Open)
          con.Close();
      }
    }

    void DeleteTempFile(object tmpFile)
    {
      object locker = new object();
      string file = tmpFile as string;

      try
      {
        lock (locker)
        {
          if (File.Exists(file))
          {
            Thread.Sleep(500);
            File.Delete(file);
#if DEBUG
            Helpers.WriteLogEntry("Temp file deleted: " + file);
#endif
          }
        }
      }
      catch { }
    }
  }
}
