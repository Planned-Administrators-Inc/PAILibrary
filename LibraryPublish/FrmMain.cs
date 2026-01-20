using LibraryPublish.Properties;
using PAI.Email;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LibraryPublish
  {
  public partial class FrmMain : Form
    {
    private DateTime publishDate;

    public FrmMain()
      {
      InitializeComponent();
      var pathName = Path.GetDirectoryName(Settings.Default.logPath);

      if (!Directory.Exists(pathName))
        {
        Directory.CreateDirectory(pathName);
        }
      pathName = Path.GetDirectoryName(Settings.Default.pubPath);
      if (!Directory.Exists(pathName))
        {
        Directory.CreateDirectory(pathName);
        }
      pathName = Path.GetDirectoryName(Settings.Default.libPath);
      if (!Directory.Exists(pathName))
        {
        Directory.CreateDirectory(pathName);
        }
      pathName = Path.GetDirectoryName(Settings.Default.pubPathHealthX);
      if (!Directory.Exists(pathName))
        {
        Directory.CreateDirectory(pathName);
        }
      pathName = Path.GetDirectoryName(Settings.Default.pubPathQBL);
      if (!Directory.Exists(pathName))
        {
        Directory.CreateDirectory(pathName);
        }

      }


    private void FrmMain_Load(object sender, EventArgs e)
      {
      dtpStartDate.CustomFormat = "MM/dd/yyyy";
      dtpStartDate.Value = DateTime.Now.AddDays(-3.00);

      dtpEndDate.CustomFormat = "MM/dd/yyyy";
      dtpEndDate.Value = DateTime.Now;
      }

    private void DisableControls()
      {
      dtpStartDate.Enabled = false;
      dtpEndDate.Enabled = false;
      button1.Enabled = false;
      this.Cursor = Cursors.WaitCursor;
      }

    private void EnableControls()
      {
      dtpStartDate.Enabled = true;
      dtpEndDate.Enabled = true;
      button1.Enabled = true;
      this.Cursor = Cursors.Default;
      }

    private void Button1_Click(object sender, EventArgs e)
      {
      bool bErr = false;
      try
        {
        DisableControls();
        try
          {
          for (publishDate = dtpStartDate.Value; publishDate <= dtpEndDate.Value; publishDate = publishDate.AddDays(1))
            {
            if (GetRecordCount() > 0)
              {
              if (publishDate == DateTime.Now)
                {
                PublilshReports();
                }
              PublishDocumentsHealthX();
              PublishDocumentsQBL();
              }
            else
              {
              Log("".PadLeft(80, '#'), false, true);
              Log("No records to publish for RTF files " + publishDate.ToString("MM/dd/yyyy"), true, true);
              Log("".PadLeft(80, '#'), false, true);
              }
            }

          //NEW CODE: xlsx files - START
          for (publishDate = dtpStartDate.Value; publishDate <= dtpEndDate.Value; publishDate = publishDate.AddDays(1))
            {
            if (GetRecordCountForXLSX() > 0)
              {
              if (publishDate == DateTime.Now)
                {
                PublilshReports();
                }
              PublishDocumentsHealthXForXLSX();
              PublishDocumentsQBLForXLSX();
              }
            else
              {
              Log("".PadLeft(80, '#'), false, true);
              Log("No records to publish for XLSX files " + publishDate.ToString("MM/dd/yyyy"), true, true);
              Log("".PadLeft(80, '#'), false, true);
              }
            }
          //NEW CODE: xlsx files - END
          }
        catch (Exception ex)
          {
          Log("Exception Main::PublishDocuments() - " + ex.Message, true, true);
          SendEmail("Exception Main::PublishDocuments() - " + ex.Message);
          bErr = true;
          }
        }
      catch (Exception ex)
        {
        Log("Exception: button1_Click() - " + ex.Message, true, true);
        SendEmail("Exception: button1_Click() - " + ex.Message);
        bErr = true;
        }
      finally
        {
        EnableControls();
        }

      if (bErr == false)
        {
        MessageBox.Show("Library Publish completed successfully");
        }
      else
        {
        MessageBox.Show("Library Publish completed with ERRORS", "Error");
        }
      }

    public static void Log(string msg, bool bAppendTime, bool newLine)
      {
      string NL = (newLine ? Environment.NewLine : "");
      string logfile = string.Format(Settings.Default.logPath, DateTime.Now.ToString("yyyyMMdd"));

      logfile = string.Format(Settings.Default.logPath, DateTime.Now.ToString("yyyy-MM-dd"));

      if (bAppendTime)
        {
        Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + msg + NL);
        File.AppendAllText(logfile, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + msg + NL);
        }
      else
        {
        Console.WriteLine(msg + Environment.NewLine);
        File.AppendAllText(logfile, msg + NL);
        }
      }

    public static bool IsPlanOnlyGroup(string group)
      {
      bool bSpecial = false;
      long lGpNo = 0;

      try
        {
        // see if the group number is in the ranges for certain groups
        if (long.TryParse(group, out lGpNo))
          {
          if (lGpNo >= 300100 && lGpNo <= 349999)  // McDonald's
            bSpecial = true;
          else if (lGpNo.ToString().Length >= 4)
            {
            if (lGpNo.ToString().Substring(0, 4) == "1200")  // Dunkin
              bSpecial = true;
            else if (lGpNo.ToString().Substring(0, 4) == "1201")  // Choice
              bSpecial = true;
            else if (lGpNo.ToString().Substring(0, 4) == "1203")  // Dine Equity
              bSpecial = true;
            else if (lGpNo.ToString().Substring(0, 4) == "1204")  // Carlson
              bSpecial = true;
            }
          else
            {
            bSpecial = false;
            }
          }
        }
      catch (Exception ex)
        {
        bSpecial = false;
        Log("IsPlanOnlyGroup - " + ex.Message, true, true);
        }

      return bSpecial;
      }

    public static void SendEmail(string msg)
      {
      string NL = Environment.NewLine;
      PAIEmail email = new PAIEmail()
        {
        From = "LibraryPublish@paisc.com",
        IsHTML = false,
        Message = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + msg + NL
        };
      email.Recipients.Add(Settings.Default.DevEmail);
      email.Subject = "IS Systems: Error in PAILibrary Publish";
      email.Send();
      }

    public bool CheckIfFilePublished(string file)
      {
      bool status = true;

      try
        {
        FileInfo fi = new FileInfo(file);
        if (!fi.Exists)
          {
          status = false;
          }
        }
      catch (Exception ex)
        {
        SendEmail("CheckIfFilePublished(" + file + ") - " + Environment.NewLine + ex.Message);
        status = false;
        }

      return status;
      }

    public int GetRecordCount()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataTable dt = new DataTable();
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProcCount;

          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(dt);
          if (int.TryParse(dt.Rows[0]["Count"].ToString(), out int c) == true)
            count = c;
          }
        }
      catch (Exception ex)
        {
        Log("Exception: GetRecordCount() - " + ex.Message, true, true);
        SendEmail("Exception: GetRecordCount() - " + ex.Message);
        count = 0;
        }

      return count;
      }

    //NEW CODE: xlsx files - START
    public int GetRecordCountForXLSX()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataTable dt = new DataTable();
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProcCountForXLSX;

          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(dt);
          if (int.TryParse(dt.Rows[0]["Count"].ToString(), out int c) == true)
            count = c;
          }
        }
      catch (Exception ex)
        {
        Log("Exception: GetRecordCountForXLSX() - " + ex.Message, true, true);
        SendEmail("Exception: GetRecordCountForXLSX() - " + ex.Message);
        count = 0;
        }

      return count;
      }
    //NEW CODE: xlsx files - END

    public void LogPublishedDocument(string docID, string libFile, string vfdocFile)
      {
      SqlConnection con = null;
      SqlCommand cmd = null;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = "usp_DL_InsertPublishLog";

          cmd.Parameters.AddWithValue("@DocID", docID);
          cmd.Parameters.AddWithValue("@LibraryFile", libFile);
          cmd.Parameters.AddWithValue("@VFDocFile", vfdocFile);

          cmd.ExecuteNonQuery();
          }
        }
      catch (Exception ex)
        {
        Log("Exception in LogPublishedDocument() - " + ex.Message, true, true);
        SendEmail("Exception in LogPublishedDocument() - " + ex.Message);
        }
      }

    public void PublilshReports()
      {
      try
        {
        if (GetRecordCount() > 0 || GetRecordCountForXLSX() > 0)
          {
#pragma warning disable IDE0017 // Simplify object initialization
          Reports rpt = new Reports();
#pragma warning restore IDE0017 // Simplify object initialization

          rpt.ReportType = ReportTypes.MISSED_LOOKUPS_REPORT;
          rpt.CreateReport(publishDate.ToString("MM/dd/yyyy"));

          rpt.ReportType = ReportTypes.PUBLISH_REPORT;
          rpt.CreateReport(publishDate.ToString("MM/dd/yyyy"));

          rpt.ReportType = ReportTypes.TODAYSDOCS_REPORT;
          rpt.CreateReport(publishDate.ToString("MM/dd/yyyy"));
          }
        }
      catch (Exception ex)
        {
        SendEmail("Exception in PublilshReports() - " + ex.Message + " - " + publishDate);
        }
      }

    public void PublishDocumentsHealthX()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataSet ds = new DataSet();
      Dictionary<string, string> rtfpdf = new Dictionary<string, string>();
      string rtf = string.Empty;
      string code = string.Empty;
      string pdf = string.Empty;
      string libPath = string.Empty;
      string docID = string.Empty;
      int loop = 0;
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;

          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProc;
          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(ds);

          Log("".PadLeft(80, '#'), false, true);
          Log("Publishing started for " + ds.Tables[0].Rows.Count.ToString() + " file(s) for " + publishDate, true, true);

          // loop over each record, if any, convert to PDF and publish to
          // \\pai_server\Public\library\
          count = ds.Tables[0].Rows.Count;
          if (count > 0)
            {
            foreach (DataRow row in ds.Tables[0].Rows)
              {
              docID = row["DocID"].ToString();
              code = row["Code"].ToString();
              libPath = Path.Combine(Settings.Default.libPath, code);
              rtf = Path.Combine(libPath, row["FileName"].ToString());

              if (IsPlanOnlyGroup(row["GroupNo"].ToString()))
                {
                //if (GetFileExtension(rtf) == "rtf")
                pdf = row["FullDesc"].ToString() + ".pdf";
                }
              else
                {
                //if (GetFileExtension(rtf) == "rtf")
                //pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";
                string[] strFullDescPart = row["FullDesc"].ToString().Split('-');
                pdf = row["GroupNo"].ToString() + "_" + strFullDescPart[0] + "_063_" + row["FullDesc"].ToString() + ".pdf";

                }
              pdf = Path.Combine(Settings.Default.pubPathHealthX, pdf);

              try
                {
                // delete this PDF
                //if (!TryToDeleteFile(pdf))
                //{
                //	Log("Couldn't delete PDF file: " + pdf, true, true);
                //	SendEmail("Couldn't delete PDF file: " + pdf);
                //	continue;
                //}

                // save rtfs and pdfs to be converted in dictionary
                // or publish pdf files
                if (GetFileExtension(rtf) == "rtf")
                  {
                  rtfpdf.Add(rtf, pdf);
                  }
                else
                  {
                  File.Copy(rtf, pdf, true);
                  }
                }
              catch (Exception ex)
                {
                Log("Exception: In publish loop - " + ex.Message, true, true);
                SendEmail("Exception: In publish loop - " + ex.Message);
                }

              // sleep because of crappy servers/moon/venus/Xenu!!!
              //System.Threading.Thread.Sleep(150);
              loop++;
              }
            // publish all documents at once
            if (rtfpdf != null && rtfpdf.Count > 0)
              {
              bool good = Rtf2Pdf.ConvertAllRTF2PDF(rtfpdf);
              // convert RTF to PDF and save
              for (int index = 0; index < rtfpdf.Count; index++)
                {
                var item = rtfpdf.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                //NEW CODE if (!CheckIfFilePublished(pdf))
                if (!CheckIfFilePublished(itemValue))
                  {
                  //NEW CODE Log("Couldn't publish PDF file: " + pdf, true, true);
                  //NEW CODE SendEmail("Couldn't publish PDF file: " + pdf);
                  Log("Couldn't publish PDF file: " + itemValue, true, true);
                  SendEmail("Couldn't publish PDF file: " + itemValue);
                  continue;
                  }
                else
                  {
                  //NEW CODE Log(string.Format("Record {0} of {1}: Published {2} to {3}", loop + 1, count, xlsx, pdf), true, true);
                  //NEW CODE LogPublishedDocument(docID, xlsx, pdf);
                  Log(string.Format("Record {0} of {1}: Published {2} to {3}", index + 1, count, itemKey, itemValue), true, true);
                  LogPublishedDocument(docID, itemKey, itemValue);
                  }
                }
              }
            }
          }
        }
      catch (Exception ex)
        {
        Log("Exception - " + ex.Message, true, true);
        SendEmail("Exception - " + ex.Message);
        }
      finally
        {
        Log("".PadLeft(80, '#'), false, true);
        }
      }

    public void PublishDocumentsQBL()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataSet ds = new DataSet();
      Dictionary<string, string> rtfpdf = new Dictionary<string, string>();
      string rtf = string.Empty;
      string code = string.Empty;
      string pdf = string.Empty;
      string libPath = string.Empty;
      string docID = string.Empty;
      int loop = 0;
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;

          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProc;
          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(ds);

          Log("".PadLeft(80, '#'), false, true);
          Log("Publishing started for " + ds.Tables[0].Rows.Count.ToString() + " file(s) for " + publishDate, true, true);

          // loop over each record, if any, convert to PDF and publish to
          // \\pai_server\Public\library\
          count = ds.Tables[0].Rows.Count;
          if (count > 0)
            {
            foreach (DataRow row in ds.Tables[0].Rows)
              {
              docID = row["DocID"].ToString();
              code = row["Code"].ToString();
              libPath = Path.Combine(Settings.Default.libPath, code);
              rtf = Path.Combine(libPath, row["FileName"].ToString());

              if (IsPlanOnlyGroup(row["GroupNo"].ToString()))
                {
                //if (GetFileExtension(rtf) == "rtf")
                pdf = row["FullDesc"].ToString() + ".pdf";
                }
              else
                {
                //if (GetFileExtension(rtf) == "rtf")
                //pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";
                //string[] strFullDescPart = row["FullDesc"].ToString().Split('-');
                pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";

                }
              pdf = Path.Combine(Settings.Default.pubPathQBL, pdf);

              try
                {
                // delete this PDF
                //if (!TryToDeleteFile(pdf))
                //{
                //	Log("Couldn't delete PDF file: " + pdf, true, true);
                //	SendEmail("Couldn't delete PDF file: " + pdf);
                //	continue;
                //}

                // save rtfs and pdfs to be converted in dictionary
                // or publish pdf files
                if (GetFileExtension(rtf) == "rtf")
                  {
                  rtfpdf.Add(rtf, pdf);
                  }
                else
                  {
                  File.Copy(rtf, pdf, true);
                  }
                }
              catch (Exception ex)
                {
                Log("Exception: In publish loop - " + ex.Message, true, true);
                SendEmail("Exception: In publish loop - " + ex.Message);
                }

              // sleep because of crappy servers/moon/venus/Xenu!!!
              //System.Threading.Thread.Sleep(150);
              loop++;
              }
            // publish all documents at once
            if (rtfpdf != null && rtfpdf.Count > 0)
              {
              bool good = Rtf2Pdf.ConvertAllRTF2PDF(rtfpdf);
              // convert RTF to PDF and save
              for (int index = 0; index < rtfpdf.Count; index++)
                {
                var item = rtfpdf.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                //NEW CODE if (!CheckIfFilePublished(pdf))
                if (!CheckIfFilePublished(itemValue))
                  {
                  //NEW CODE Log("Couldn't publish PDF file: " + pdf, true, true);
                  //NEW CODE SendEmail("Couldn't publish PDF file: " + pdf);
                  Log("Couldn't publish PDF file: " + itemValue, true, true);
                  SendEmail("Couldn't publish PDF file: " + itemValue);
                  continue;
                  }
                else
                  {
                  //NEW CODE Log(string.Format("Record {0} of {1}: Published {2} to {3}", loop + 1, count, xlsx, pdf), true, true);
                  //NEW CODE LogPublishedDocument(docID, xlsx, pdf);
                  Log(string.Format("Record {0} of {1}: Published {2} to {3}", index + 1, count, itemKey, itemValue), true, true);
                  LogPublishedDocument(docID, itemKey, itemValue);
                  }
                }
              }
            }
          }
        }
      catch (Exception ex)
        {
        Log("Exception - " + ex.Message, true, true);
        SendEmail("Exception - " + ex.Message);
        }
      finally
        {
        Log("".PadLeft(80, '#'), false, true);
        }
      }

    //NEW CODE: xlsx files - START
    public void PublishDocumentsHealthXForXLSX()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataSet ds = new DataSet();
      Dictionary<string, string> xlsxpdf = new Dictionary<string, string>();
      string xlsx = string.Empty;
      string code = string.Empty;
      string pdf = string.Empty;
      string libPath = string.Empty;
      string docID = string.Empty;
      int loop = 0;
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;

          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProcForXLSX;
          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(ds);

          Log("".PadLeft(80, '#'), false, true);
          Log("Publishing started for " + ds.Tables[0].Rows.Count.ToString() + " file(s) for " + publishDate, true, true);

          // loop over each record, if any, convert to PDF and publish to
          // \\pai_server\Public\library\
          count = ds.Tables[0].Rows.Count;
          if (count > 0)
            {
            foreach (DataRow row in ds.Tables[0].Rows)
              {
              docID = row["DocID"].ToString();
              code = row["Code"].ToString();
              libPath = Path.Combine(Settings.Default.libPath, code);
              xlsx = Path.Combine(libPath, row["FileName"].ToString());

              if (IsPlanOnlyGroup(row["GroupNo"].ToString()))
                {
                //if (GetFileExtension(rtf) == "xlsx")
                pdf = row["FullDesc"].ToString() + ".pdf";
                }
              else
                {
                //if (GetFileExtension(rtf) == "xlsx")
                //pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";
                string[] strFullDescPart = row["FullDesc"].ToString().Split('-');
                pdf = row["GroupNo"].ToString() + "_" + strFullDescPart[0] + "_063_" + row["FullDesc"].ToString() + ".pdf";

                }
              pdf = Path.Combine(Settings.Default.pubPathHealthX, pdf);

              try
                {
                // delete this PDF
                //if (!TryToDeleteFile(pdf))
                //{
                //	Log("Couldn't delete PDF file: " + pdf, true, true);
                //	SendEmail("Couldn't delete PDF file: " + pdf);
                //	continue;
                //}

                // save rtfs and pdfs to be converted in dictionary
                // or publish pdf files
                if (GetFileExtension(xlsx) == "xlsx")
                  {
                  xlsxpdf.Add(xlsx, pdf);
                  }
                else
                  {
                  File.Copy(xlsx, pdf, true);
                  }
                }
              catch (Exception ex)
                {
                Log("Exception: In publish loop - " + ex.Message, true, true);
                SendEmail("Exception: In publish loop - " + ex.Message);
                }

              // sleep because of crappy servers/moon/venus/Xenu!!!
              //System.Threading.Thread.Sleep(150);
              loop++;
              }
            // publish all documents at once
            if (xlsxpdf != null && xlsxpdf.Count > 0)
              {
              bool good = Xlsx2Pdf.ConvertAllXLSX2PDF(xlsxpdf);
              // convert XLSX to PDF and save
              for (int index = 0; index < xlsxpdf.Count; index++)
                {
                var item = xlsxpdf.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                //NEW CODE if (!CheckIfFilePublished(pdf))
                if (!CheckIfFilePublished(itemValue))
                  {
                  //NEW CODE Log("Couldn't publish PDF file: " + pdf, true, true);
                  //NEW CODE SendEmail("Couldn't publish PDF file: " + pdf);
                  Log("Couldn't publish PDF file: " + itemValue, true, true);
                  SendEmail("Couldn't publish PDF file: " + itemValue);
                  continue;
                  }
                else
                  {
                  //NEW CODE Log(string.Format("Record {0} of {1}: Published {2} to {3}", loop + 1, count, xlsx, pdf), true, true);
                  //NEW CODE LogPublishedDocument(docID, xlsx, pdf);
                  Log(string.Format("Record {0} of {1}: Published {2} to {3}", index + 1, count, itemKey, itemValue), true, true);
                  LogPublishedDocument(docID, itemKey, itemValue);
                  }
                }
              }
            }
          }
        }
      catch (Exception ex)
        {
        Log("Exception - " + ex.Message, true, true);
        SendEmail("Exception - " + ex.Message);
        }
      finally
        {
        Log("".PadLeft(80, '#'), false, true);
        }
      }

    public void PublishDocumentsQBLForXLSX()
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataSet ds = new DataSet();
      Dictionary<string, string> xlsxpdf = new Dictionary<string, string>();
      string xlsx = string.Empty;
      string code = string.Empty;
      string pdf = string.Empty;
      string libPath = string.Empty;
      string docID = string.Empty;
      int loop = 0;
      int count = 0;

      try
        {
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          con.Open();

          cmd.Connection = con;

          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = Settings.Default.sProcForXLSX;
          cmd.Parameters.AddWithValue("@theDate", publishDate);

          da = new SqlDataAdapter(cmd);
          da.Fill(ds);

          Log("".PadLeft(80, '#'), false, true);
          Log("Publishing started for " + ds.Tables[0].Rows.Count.ToString() + " file(s) for " + publishDate, true, true);

          // loop over each record, if any, convert to PDF and publish to
          // \\pai_server\Public\library\
          count = ds.Tables[0].Rows.Count;
          if (count > 0)
            {
            foreach (DataRow row in ds.Tables[0].Rows)
              {
              docID = row["DocID"].ToString();
              code = row["Code"].ToString();
              libPath = Path.Combine(Settings.Default.libPath, code);
              xlsx = Path.Combine(libPath, row["FileName"].ToString());

              if (IsPlanOnlyGroup(row["GroupNo"].ToString()))
                {
                //if (GetFileExtension(rtf) == "xlsx")
                pdf = row["FullDesc"].ToString() + ".pdf";
                }
              else
                {
                //if (GetFileExtension(rtf) == "xlsx")
                //pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";
                //string[] strFullDescPart = row["FullDesc"].ToString().Split('-');
                pdf = row["GroupNo"].ToString() + "-" + row["FullDesc"].ToString() + ".pdf";

                }
              pdf = Path.Combine(Settings.Default.pubPathQBL, pdf);

              try
                {
                // delete this PDF
                //if (!TryToDeleteFile(pdf))
                //{
                //	Log("Couldn't delete PDF file: " + pdf, true, true);
                //	SendEmail("Couldn't delete PDF file: " + pdf);
                //	continue;
                //}

                // save rtfs and pdfs to be converted in dictionary
                // or publish pdf files
                if (GetFileExtension(xlsx) == "xlsx")
                  {
                  xlsxpdf.Add(xlsx, pdf);
                  }
                else
                  {
                  File.Copy(xlsx, pdf, true);
                  }
                }
              catch (Exception ex)
                {
                Log("Exception: In publish loop - " + ex.Message, true, true);
                SendEmail("Exception: In publish loop - " + ex.Message);
                }

              // sleep because of crappy servers/moon/venus/Xenu!!!
              //System.Threading.Thread.Sleep(150);
              loop++;
              }
            // publish all documents at once
            if (xlsxpdf != null && xlsxpdf.Count > 0)
              {
              bool good = Xlsx2Pdf.ConvertAllXLSX2PDF(xlsxpdf);
              // convert XLSX to PDF and save
              for (int index = 0; index < xlsxpdf.Count; index++)
                {
                var item = xlsxpdf.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;

                //NEW CODE if (!CheckIfFilePublished(pdf))
                if (!CheckIfFilePublished(itemValue))
                  {
                  //NEW CODE Log("Couldn't publish PDF file: " + pdf, true, true);
                  //NEW CODE SendEmail("Couldn't publish PDF file: " + pdf);
                  Log("Couldn't publish PDF file: " + itemValue, true, true);
                  SendEmail("Couldn't publish PDF file: " + itemValue);
                  continue;
                  }
                else
                  {
                  //NEW CODE Log(string.Format("Record {0} of {1}: Published {2} to {3}", loop + 1, count, xlsx, pdf), true, true);
                  //NEW CODE LogPublishedDocument(docID, xlsx, pdf);
                  Log(string.Format("Record {0} of {1}: Published {2} to {3}", index + 1, count, itemKey, itemValue), true, true);
                  LogPublishedDocument(docID, itemKey, itemValue);
                  }
                }
              }
            }
          }
        }
      catch (Exception ex)
        {
        Log("Exception - " + ex.Message, true, true);
        SendEmail("Exception - " + ex.Message);
        }
      finally
        {
        Log("".PadLeft(80, '#'), false, true);
        }
      }
    //NEW CODE: xlsx files - END

    public bool TryToDeleteFile(string file)
      {
      bool status = true;

      try
        {
        // try up to 3 times to delete this PDF
        for (int i = 0; i < 3; i++)
          {
          if (File.Exists(file))
            {
            File.Delete(file);
            }
          else
            {
            break;
            }
          }
        }
      catch (Exception ex)
        {
        SendEmail("TryToDeleteFile(" + file + ") - " + Environment.NewLine + ex.Message);
        status = false;
        }

      return status;
      }

    public string GetFileExtension(string file)
      {
      string ext = string.Empty;

      try
        {
        ext = file.Substring(file.LastIndexOf('.') + 1);
        }
      catch (Exception ex)
        {
        ext = string.Empty;
        Log("GetFileExtension::Exception - " + ex.Message, true, true);
        SendEmail("GetFileExtension::Exception - " + ex.Message);
        }

      return ext;
      }
    }
  }
