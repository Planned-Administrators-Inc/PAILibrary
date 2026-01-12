using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using LibraryPublish.Properties;
using PAI.Email;

namespace LibraryPublish
{
	public class Program
	{
		static private string cmdLineDate = string.Empty;

		#region Public Methods
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
      
			Application.Run(new FrmMain());
		}

//		public static void Main_OLD(string[] args)
//		{
//			try
//			{
//				Program p = new Program();

//				if (args.GetLength(0) > 0)
//				{
//					DateTime d;
//					if (DateTime.TryParse(args[0], out d) == true)
//					{
//						cmdLineDate = d.ToString("MM/dd/yyyy");
//					}
//					else
//					{
//						cmdLineDate = DateTime.Now.ToString("MM/dd/yyyy");
//					}
//				}
//				else
//				{
//					cmdLineDate = DateTime.Now.ToString("MM/dd/yyyy");
//				}

//				try
//				{
//#if !DEBUG
//					p.PublilshReports();
//#endif
//				}
//				catch (Exception ex)
//				{
//					SendEmail("Exception Main::PublishReports() - " + ex.Message);
//				}

//				if (p.GetRecordCount() > 0)
//				{
//					try
//					{
//						p.PublishDocuments();
//					}
//					catch (Exception ex)
//					{
//						SendEmail("Exception Main::PublishDocuments() - " + ex.Message);
//					}
//				}
//				else
//				{
//					Log("".PadLeft(80, '#'), false, true);
//					Log("No records to publish for " + cmdLineDate, true, true);
//					Log("".PadLeft(80, '#'), false, true);
//				}
//			}
//			catch (Exception ex)
//			{
//				Log(ex.Message, true, true);
//			}
//			finally
//			{
//				// makre sure Excel gets killed
//				foreach (var process in Process.GetProcessesByName("WINWORD"))
//				{
//					process.Kill();
//				}
//			}
//		}
		#endregion public Methods
	}
}
