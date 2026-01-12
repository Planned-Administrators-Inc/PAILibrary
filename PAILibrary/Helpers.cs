using PAI.Email;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace PAILibrary
  {
  public class Helpers
    {
    //NEW CODE public static readonly string Domain = "pai";
    //NEW CODE public static readonly string Username = "pailibrary";
    //NEW CODE public static readonly string Password = "!Tenace3Picinae";
    //NEW CODE Return Domain, Username and Password values from .ini file
    //NEW CODE - START
    public static readonly string Domain;
    public static readonly string Username;
    public static readonly string Password;
    public static readonly string tmpPath = ConfigurationManager.AppSettings["LibUserLogs"];
    public static object locker = new object();
    //NEW CODE - END

    public static void SendEmail(string msg)
      {
      string NL = Environment.NewLine;
      PAIEmail email = new PAIEmail();

      email.From = "PAILibrary@paisc.com";
      email.IsHTML = false;
      email.Message = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + msg + NL;
      email.Recipients.Add("schedjob@paisc.com");
      email.Subject = "IS Systems: - Error in PAILibrary";
      email.Send();
      }

    public static void WriteLogEntry(string msg)
      {
      try
        {
        File.AppendAllText(Path.Combine(tmpPath, "pailib.log"),
            DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + " - " + msg + Environment.NewLine);
        }
      catch { }
      }

    public static string ConString { get { return ConfigurationManager.ConnectionStrings["conLib"].ConnectionString; } }
    public static string GetSQLConString()
      {
      return ConfigurationManager.ConnectionStrings["conLib"].ConnectionString;

      }

    public static bool Offline
      {
      get { if (ConfigurationManager.AppSettings["OffLine"] == "1") return true; else return false; }
      }

    public static string LibPath
      {
      get { return ConfigurationManager.AppSettings["LibPath"]; }
      }

    public static string LibPathTemp
      {
      get { return ConfigurationManager.AppSettings["LibPathTemp"]; }
      }

    public static string SortOrder
      {
      get { return ConfigurationManager.AppSettings["SortOrder"]; }
      }

    //NEW CODE - START
    //Return Domain, Username and Password values from .ini file
    public static string GetInfoFromINI(string strField, string strReturn)
      {
      string line;
      // Read the file and display it line by line.

      using (System.IO.StreamReader file = new System.IO.StreamReader("C:\\code\\PAILibrary\\trunk\\PAILibrary\\ini\\DomainUsernamePassword.ini"))
        {
        switch (strField)
          {
          case "USERNAME":
            while ((line = file.ReadLine()) != null)
              {
              if (line.ToLower().StartsWith("username"))
                {
                string[] fullName = line.Split('=');
                strReturn = fullName[1];
                break;
                }
              }
            break;

          case "DOMAIN":
            while ((line = file.ReadLine()) != null)
              {
              if (line.ToLower().StartsWith("domain"))
                {
                string[] fullName = line.Split('=');
                strReturn = fullName[1];
                break;
                }
              }
            break;

          case "PASSWORD":
            while ((line = file.ReadLine()) != null)
              {
              if (line.ToLower().StartsWith("password"))
                {
                string[] fullName = line.Split('=');
                strReturn = fullName[1];
                break;
                }
              }
            break;

          default:
            break;

          }
        }
      return strReturn;
      }
    //NEW CODE - END

    public static string RemoveDomainFromUserName(string user)
      {
      string tmp = string.Empty;

      if (user.IndexOf('\\') != -1)
        tmp = user.Split('\\')[1];
      else
        tmp = user;

      return tmp;
      }

    public static bool InsertAudit(string externID, string externName, string oldValue,
                                   string newValue, string userID, string auditNote)
      {
      bool ret = false;

      try
        {
        using (SqlConnection con = new SqlConnection(GetSQLConString()))
        using (SqlCommand cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandTimeout = 0;
          cmd.CommandText = "usp_DL_InsertAudit";
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@externID", externID);
          cmd.Parameters.AddWithValue("@externName", externName);
          cmd.Parameters.AddWithValue("@oldValue", oldValue);
          cmd.Parameters.AddWithValue("@newValue", newValue);
          cmd.Parameters.AddWithValue("@userID", userID);
          cmd.Parameters.AddWithValue("@auditNote", auditNote);

          if (cmd.ExecuteScalar().ToString() == "1")
            ret = true;
          else
            ret = false;
          }
        }
      catch (Exception ex)
        {
        HttpContext.Current.Session["Error"] = ex.Message;
        ret = false;
        }

      return ret;
      }

    public static bool CheckAdminUser(string user)
      {
      bool ret = false;
      if (user == string.Empty)
        {
        user = Environment.UserName;
        }

      try
        {
        using (SqlConnection con = new SqlConnection(GetSQLConString()))
        using (SqlCommand cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandTimeout = 0;
          cmd.CommandText = "usp_DL_CheckAdminUser";
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@UserID", RemoveDomainFromUserName(user));

          if (cmd.ExecuteScalar().ToString() == "1")
            ret = true;
          else
            ret = false;
          }
        }
      catch (Exception ex)
        {
        HttpContext.Current.Session["Error"] = ex.Message;
        ret = false;
        }

      return ret;
      }

    public static string GetNote(string docid)
      {
      string note = string.Empty;

      try
        {
        using (SqlConnection con = new SqlConnection(GetSQLConString()))
        using (SqlCommand cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandTimeout = 0;
          cmd.CommandText = "usp_DL_GetNotes";
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("@DocID", docid);

          using (SqlDataReader r = cmd.ExecuteReader())
            {
            if (r.HasRows)
              {
              r.Read();
              note = r.GetString(0);
              }
            }
          }
        }
      catch (Exception ex)
        {
        HttpContext.Current.Session["Error"] = ex.Message;
        note = string.Empty;
        }

      return note;
      }
    }
  }
