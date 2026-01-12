using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;

namespace LibraryPublish
{
    class Xlsx2Pdf
    {
        /// <summary>
        /// Set to the last Exception.Message if one occured
        /// </summary>
        public static string LastError { get; private set; }

        /// <summary>
        /// Converts an dictionary of xlsx sources and pdf save paths to a PDF using MS Word
        /// </summary>
        /// <param name="xlsxpdf">A <typeparamref name="Dictionary"/> of full paths to RTF's
        /// and the full path to the PDF's to save to.</param>
        /// <returns>true on success, else false and LastError is set</returns>
        public static bool ConvertAllXLSX2PDF(Dictionary<string, string> xlsxpdf, bool fixMargins = false)
        {
            bool bRet = true;

            //Excel Spreadsheet - START
            Microsoft.Office.Interop.Excel.Application excelApplication = new Microsoft.Office.Interop.Excel.Application();
            //Application excelApplication = new Application();
            Workbook excelWorkBook = null;

            object paramMissing = Type.Missing;

            XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
            XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
            bool paramOpenAfterPublish = false;
            bool paramIncludeDocProps = true;
            bool paramIgnorePrintAreas = true;
            object paramFromPage = Type.Missing;
            object paramToPage = Type.Missing;

            try
            {
                if (xlsxpdf != null && xlsxpdf.Count > 0)
                {
                    foreach (string key in xlsxpdf.Keys)
                    {
                        // open our source document
                        // the first param is a ref to an object so stick our string in an object
                        object xlsx = key as object;

                        // Open the source workbook.
                        excelWorkBook = excelApplication.Workbooks.Open(key,
                        paramMissing, false, paramMissing, paramMissing,
                        paramMissing, false, paramMissing, paramMissing,
                        true, paramMissing, paramMissing, paramMissing,
                        paramMissing, paramMissing);

                        // Save it in the target format.
                        if (excelWorkBook != null)
                            excelWorkBook.ExportAsFixedFormat(paramExportFormat,
                            xlsxpdf[key], paramExportQuality,
                            paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                            paramToPage, paramOpenAfterPublish,
                            paramMissing);
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
                // Close the workbook object.
                if (excelWorkBook != null)
                {
                    excelWorkBook.RefreshAll();
                    excelWorkBook.Close(false, paramMissing, paramMissing);
                    excelWorkBook = null;
                }

                // Quit Excel and release the ApplicationClass object.
                if (excelApplication != null)
                {
                    excelApplication.Quit();
                    excelApplication = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            //Excel Spreadsheet - END

            return bRet;
        }
    }
}
