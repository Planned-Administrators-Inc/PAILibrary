using LibraryPublish.Properties;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace LibraryPublish
  {
  public enum ReportTypes { TODAYSDOCS_REPORT, PUBLISH_REPORT, MISSED_LOOKUPS_REPORT }

  class Reports
    {
    public ReportTypes ReportType { get; set; }
    public string LastError { get; private set; }

    public bool CreateReport(string rptDate)
      {
      SqlConnection con = null;
      SqlCommand cmd = null;
      SqlDataAdapter da = null;
      DataSet ds = null;
      DataTable dt = null;
      Excel.Application eXL = null;
      Excel.Workbook eWBook = null;
      Excel.Worksheet eSheet = null;
      Excel.Range eRange = null;
      Excel.Font eFont = null;
      Excel.Border eBorder = null;
      System.Reflection.Missing missing = System.Reflection.Missing.Value;
      object[,] rptData = null;
      string sRange = string.Empty;
      string sRangeData = string.Empty;
      string sProc = string.Empty;
      string sSaveFile = string.Empty;
      bool bGood = true;

      try
        {
        // get our date of report
        if (DateTime.TryParse(rptDate, out DateTime theDate) == false)
          {
          return false;
          }

        //Start Excel and get Application object.
        eXL = new Excel.Application()
          {
          DisplayAlerts = false
          };
        eWBook = eXL.Workbooks.Add(missing);
        eSheet = (Excel.Worksheet)eWBook.Sheets["Sheet1"];

        // setup tab name and header row
        switch (ReportType)
          {
          case ReportTypes.MISSED_LOOKUPS_REPORT:
            eSheet.Name = "Missed Lookups";
            SetCellText((Excel.Range)eSheet.Cells[1, "A"], "MemID");
            SetCellText((Excel.Range)eSheet.Cells[1, "B"], "Last4");
            SetCellText((Excel.Range)eSheet.Cells[1, "C"], "BDay");
            SetCellText((Excel.Range)eSheet.Cells[1, "D"], "Group");
            SetCellText((Excel.Range)eSheet.Cells[1, "E"], "Plan");
            sRange = "A1:E1";
            sRangeData = "A2:E{0}";
            sProc = Settings.Default.rptMissed;
            sSaveFile = Path.Combine(Settings.Default.rptPath, Settings.Default.rptNameMissed);
            break;
          case ReportTypes.PUBLISH_REPORT:
            eSheet.Name = "Published";
            SetCellText((Excel.Range)eSheet.Cells[1, "A"], "Group");
            SetCellText((Excel.Range)eSheet.Cells[1, "B"], "DateStamp");
            SetCellText((Excel.Range)eSheet.Cells[1, "C"], "Desc");
            SetCellText((Excel.Range)eSheet.Cells[1, "D"], "FileName");
            SetCellText((Excel.Range)eSheet.Cells[1, "E"], "PublishPath");
            sRange = "A1:E1";
            sRangeData = "A2:E{0}";
            sProc = Settings.Default.rptPub;
            sSaveFile = Path.Combine(Settings.Default.rptPath, Settings.Default.rptNamePub);
            break;
          case ReportTypes.TODAYSDOCS_REPORT:
            eSheet.Name = "Changes";
            SetCellText((Excel.Range)eSheet.Cells[1, "A"], "Document");
            SetCellText((Excel.Range)eSheet.Cells[1, "B"], "User");
            SetCellText((Excel.Range)eSheet.Cells[1, "C"], "Date");
            SetCellText((Excel.Range)eSheet.Cells[1, "D"], "Action");
            sRange = "A1:D1";
            sRangeData = "A2:D{0}";
            sProc = Settings.Default.rptAdded;
            sSaveFile = Path.Combine(Settings.Default.rptPath, Settings.Default.rptNameAdded);
            break;
          }

        // add date to save file name
        sSaveFile = string.Format(sSaveFile, theDate.ToString("yyyyMMdd"));

        // set bold and bottom border
        eRange = eSheet.get_Range(sRange, missing);
        eFont = eRange.Font;
        eFont.Bold = true;
        // set a bottom border
        eBorder = eRange.Borders[Excel.XlBordersIndex.xlEdgeBottom];
        eBorder.LineStyle = Excel.XlLineStyle.xlContinuous;
        eBorder.Color = ColorTranslator.ToOle(Color.Black);

        // get our report data
        using (con = new SqlConnection(Settings.Default.conDBServer1))
        using (cmd = new SqlCommand())
          {
          SqlParameter parm = null;
          parm = new SqlParameter("@theDate", rptDate);

          con.Open();
          cmd.Connection = con;
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.CommandText = sProc;
          if (parm != null)
            cmd.Parameters.Add(parm);

          da = new SqlDataAdapter(cmd);
          ds = new DataSet();
          da.Fill(ds);
          dt = ds.Tables[0];

          if (dt.Rows.Count > 0)
            {
            // make our array big enough
            rptData = new object[dt.Rows.Count, dt.Columns.Count];

            // Copy the values to the object array
            for (int col = 0; col < dt.Columns.Count; col++)
              {
              for (int row = 0; row < dt.Rows.Count; row++)
                {
                rptData[row, col] = dt.Rows[row].ItemArray[col];
                }
              }

            // copy the whole array in one go instead of looping over each row
            string excelRange = string.Format(sRangeData, dt.Rows.Count + 1);
            eRange = eSheet.get_Range(excelRange, Type.Missing);
            eRange.NumberFormat = "@";
            eRange.Value2 = rptData;

            // make sure columns fit content
            eRange.Columns.AutoFit();

            // make sure our file doesn't exist
            if (File.Exists(sSaveFile) == true)
              {
              try
                {
                File.Delete(sSaveFile);
                }
              catch { }
              }

            if (!File.Exists(sSaveFile))
              {
              // save and close
              eWBook.SaveAs(sSaveFile, Excel.XlFileFormat.xlWorkbookNormal, missing, missing, missing, missing,
                  Excel.XlSaveAsAccessMode.xlShared, missing, missing, missing, missing, missing);
              }
            }
          }
        }
      catch (Exception ex)
        {
        LastError = ex.Message;
        bGood = false;
        FrmMain.Log(ex.Message, true, true);
        }
      finally
        {
        // release all COM object references
        if (eFont != null) Marshal.ReleaseComObject(eFont);
        eFont = null;
        if (eBorder != null) Marshal.ReleaseComObject(eBorder);
        eBorder = null;
        if (eRange != null) Marshal.ReleaseComObject(eRange);
        eRange = null;
        if (eSheet != null) Marshal.ReleaseComObject(eSheet);
        eSheet = null;
        if (eWBook != null)
          {
          Marshal.ReleaseComObject(eWBook);
          eWBook.Close(true, missing, missing);
          }
        eWBook = null;
        if (eXL != null)
          {
          eXL.Quit();
          Marshal.ReleaseComObject(eXL);
          eXL = null;
          }
        }

      return bGood;
      }

    private void SetCellText(Excel.Range rng, string txt)
      {
      rng.Value2 = txt;
      Marshal.ReleaseComObject(rng);
      }
    }
  }
