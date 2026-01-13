using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;

namespace LibraryPublish
  {
  class Rtf2Pdf
    {
    /// <summary>
    /// Set to the last Exception.Message if one occured
    /// </summary>
    public static string LastError { get; private set; }

    /// <summary>
    /// Converts an RTF to a PDF using MS Word 2007
    /// </summary>
    /// <param name="rtf">Full path to the RTF to convert</param>
    /// <param name="pdf">Full path to the location to save PDF</param>
    /// <returns>true on success, else false and LastError is set</returns>
    public static bool SaveRtfToPdf(string rtf, string pdf, bool fixMargins = false)
      {
      //Application wApp = new Application();
      //Document wDoc = new Document();
      //WdExportFormat format = WdExportFormat.wdExportFormatPDF;
      //WdExportOptimizeFor optFor = WdExportOptimizeFor.wdExportOptimizeForOnScreen;
      //WdExportRange exportRange = WdExportRange.wdExportAllDocument;
      //WdExportItem exportItem = WdExportItem.wdExportDocumentContent;
      //WdExportCreateBookmarks createBookmarks = WdExportCreateBookmarks.wdExportCreateNoBookmarks;
      //object missing = Type.Missing;
      //object source = (object)rtf;
      //int startPage = 0;
      //int endPage = 0;
      //bool bOpenAfter = false;
      //bool bIncludeDocProps = false;
      //bool bKeepIRM = true;
      //bool bDocStrucTags = true;
      //bool bBitMapMissingFonts = true;
      //bool bUseISO19005_1 = true;
      bool bRet = true;
      Application wApp = new Application();
      try
        {
        // open our source document
        // the first param is a ref to an object so stick our string in an object

        // wDoc = wApp.Documents.Open(ref source, false, false,
        //    false, ref missing, ref missing, ref missing, ref missing,
        //    ref missing, WdOpenFormat.wdOpenFormatRTF, ref missing, ref missing,
        //    ref missing, ref missing, ref missing, ref missing);

        //// see if we got a document object, if not try once more to instantiate
        //if (wDoc == null)
        //  {
        //  wDoc = wApp.Documents.Open(ref source, false, false,
        //      false, ref missing, ref missing, ref missing, ref missing,
        //      ref missing, WdOpenFormat.wdOpenFormatRTF, ref missing, ref missing,
        //      ref missing, ref missing, ref missing, ref missing);
        //  }

        //// Export it to PDF.
        //if (wDoc != null)
        //  {
        //  wDoc.ExportAsFixedFormat(pdf, format, bOpenAfter, optFor, exportRange,
        //      startPage, endPage, exportItem, bIncludeDocProps, bKeepIRM,
        //      createBookmarks, bDocStrucTags, bBitMapMissingFonts,
        //      bUseISO19005_1, ref missing);
        //

        Document wDoc = wApp.Documents.Open(rtf,
                                            false,
                                            false,
                                            false,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            WdOpenFormat.wdOpenFormatRTF,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing);

        // see if we got a document object, if not try once more to instantiate
        if (wDoc == null)
          {
          wDoc = wApp.Documents.Open(rtf,
                                            false,
                                            false,
                                            false,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            WdOpenFormat.wdOpenFormatRTF,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing,
                                            Type.Missing);
          }

        // Export it to PDF.
        if (wDoc != null)
          {
          wDoc.ExportAsFixedFormat(pdf,
                        WdExportFormat.wdExportFormatPDF,
                        false,
                        WdExportOptimizeFor.wdExportOptimizeForOnScreen,
                        WdExportRange.wdExportAllDocument,
                        0,
                        0,
                        WdExportItem.wdExportDocumentContent,
                        false,
                        true,
                        WdExportCreateBookmarks.wdExportCreateNoBookmarks,
                        true,
                        true,
                        true,
                        Type.Missing);

          wDoc.Close(Type.Missing, Type.Missing, Type.Missing);
          wDoc = null;
          }
        else
          {
          FrmMain.Log(string.Format("wDoc == NULL; rtf={0}, pdf={1}, format={2}", rtf, pdf, WdExportFormat.wdExportFormatPDF), true, true);
          }
        }
      catch (Exception ex)
        {
        bRet = false;
        LastError = ex.Message;
        }
      finally
        {
        //// Close and release the Document object.
        //if (wDoc != null)
        //  {
        //  // the case to _Document is to avoid a compiler warning about
        //  // and ambiguous method
        //  ((_Document)wDoc).Close(ref missing, ref missing, ref missing);
        //  wDoc = null;
        //  }

        //// Quit Word and release the ApplicationClass object.
        if (wApp != null)
          {
          wApp.Quit(Type.Missing, Type.Missing, Type.Missing);
          System.Threading.Thread.Sleep(100);
          wApp = null;
          }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        }

      return bRet;
      }

    /// <summary>
    /// Converts an dictionary of rtf sources and pdf save paths to a PDF using MS Word
    /// </summary>
    /// <param name="rtfpdf">A <typeparamref name="Dictionary"/> of full paths to RTF's
    /// and the full path to the PDF's to save to.</param>
    /// <returns>true on success, else false and LastError is set</returns>
    public static bool ConvertAllRTF2PDF(Dictionary<string, string> rtfpdf, bool fixMargins = false)
      {
      //Application wApp = null;
      //Document wDoc = null;
      //WdExportFormat format = WdExportFormat.wdExportFormatPDF;
      //WdExportOptimizeFor optFor = WdExportOptimizeFor.wdExportOptimizeForOnScreen;
      //WdExportRange exportRange = WdExportRange.wdExportAllDocument;
      //WdExportItem exportItem = WdExportItem.wdExportDocumentContent;
      //WdExportCreateBookmarks createBookmarks = WdExportCreateBookmarks.wdExportCreateNoBookmarks;
      //object missing = Type.Missing;
      //int startPage = 0;
      //int endPage = 0;
      //bool bOpenAfter = false;
      //bool bIncludeDocProps = false;
      //bool bKeepIRM = true;
      //bool bDocStrucTags = true;
      //bool bBitMapMissingFonts = true;
      //bool bUseISO19005_1 = true;
      bool bRet = true;

      try
        {
        if (rtfpdf != null && rtfpdf.Count > 0)
          {
          //wApp = new Application();
          //wDoc = new Document();

          foreach (string key in rtfpdf.Keys)
            {

            // open our source document
            // the first param is a ref to an object so stick our string in an object
            //object rtf = key as object;
            //wDoc = wApp.Documents.Open(ref rtf, false, false,
            //    false, ref missing, ref missing, ref missing, ref missing,
            //    ref missing, WdOpenFormat.wdOpenFormatRTF, ref missing, ref missing,
            //    ref missing, ref missing, ref missing, ref missing);

            //// see if we got a document object, if not try once more to instantiate
            //if (wDoc == null)
            //  {
            //  wDoc = wApp.Documents.Open(ref rtf, false, false,
            //      false, ref missing, ref missing, ref missing, ref missing,
            //      ref missing, WdOpenFormat.wdOpenFormatRTF, ref missing, ref missing,
            //      ref missing, ref missing, ref missing, ref missing);
            //  }

            //// Export it to PDF
            //if (wDoc != null)
            //  {
            //  wDoc.ExportAsFixedFormat(rtfpdf[key], format, bOpenAfter, optFor, exportRange,
            //      startPage, endPage, exportItem, bIncludeDocProps, bKeepIRM,
            //      createBookmarks, bDocStrucTags, bBitMapMissingFonts,
            //      bUseISO19005_1, ref missing);
            //  ((_Document)wDoc).Close();
            //  }
            //else
            //  {
            //  FrmMain.Log(string.Format("wDoc == NULL; rtf={0}, pdf={1}, format={2}",
            //      key, rtfpdf[key], format), true, true);
            //  }
            bRet = SaveRtfToPdf(key, rtfpdf[key], fixMargins);
            if (bRet.Equals(false)) throw new Exception($"{LastError}");

            }
          }
        }
      catch (Exception ex)
        {
        bRet = false;
        LastError = ex.Message;
        }
      finally
        {
        // Close and release the Document object.
        //if (wDoc != null)
        //  {
        //  wDoc = null;
        //  }

        //// Quit Word and release the ApplicationClass object.
        //if (wApp != null)
        //  {
        //  wApp.Quit(ref missing, ref missing, ref missing);
        //  System.Threading.Thread.Sleep(100);
        //  wApp = null;
        //  }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        }

      return bRet;
      }
    }
  }
