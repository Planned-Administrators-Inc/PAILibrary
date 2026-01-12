using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PaiLibFixDirs
{
	public partial class frmMain : Form
	{
		// ONE OFF APP so hard code is kinda OK
		private static string[] aCodes = {"001", "002", "003",  "004", "005", "006", "007", "008",
			"009", "010", "011", "012", "013", "014", "015", "016", "017", "018",
			"019", "020", "021", "022", "023", "024", "025", "026", "027", "028",
			"029", "030", "031", "032", "033", "034", "035", "036", "037", "038",
			"039", "040", "041", "042", "043", "044", "045", "046", "047", "048",
			"049", "050", "051", "052", "053", "054", "055", "056", "057", "059",
			"060", "061", "062", "063", "064", "065"};
		
		private string sDocs = @"\\pai_server\Public\library\";
		private string goodList = @"C:\logs\PAILibMove_Good.log";
		private string goodListNoDB = @"C:\logs\PAILibMove_Good_NoDB.log";
		private string badList = @"C:\logs\PAILibMove_Bad.log";
		private string badListNoDB = @"C:\logs\PAILibMove_Bad_NoDB.log";
		private string errLog = @"C:\logs\PAILibMove_Err.log";
		private int grpPos = 1;
		private List<string> codes = new List<string>(aCodes);

		public frmMain()
		{
			InitializeComponent();
		}

		private void btnSpeedRacer_Click(object sender, EventArgs e)
		{
			string[] sFiles = Directory.GetFiles(sDocs, "*.*", SearchOption.TopDirectoryOnly);
			string[] fileParts;
			string fNameNoExt = string.Empty;
			int i = 0;
			FileInfo fi;
			SqlConnection con = null;
			SqlCommand cmd = null;
			SqlDataReader dr = null;

			try
			{
				this.Cursor = Cursors.WaitCursor;
				pbStatus.Minimum = 0;
				pbStatus.Maximum = sFiles.GetLength(0);

				using (con = new SqlConnection("Server=charwood;Database=PAILibraryCI;User ID=paiuser;Password=u84jKum;Persist Security Info=True"))
				using (cmd = new SqlCommand())
				{
					con.Open();
					cmd.Connection = con;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = "usp_DL_GetCodeByFilename";

					foreach (string s in sFiles)
					{
						fi = new FileInfo(s);
						lblStatus.Text = fi.Name;
						pbStatus.Value = ++i;
						this.Refresh();
						Application.DoEvents();

						fNameNoExt = fi.Name.Replace(fi.Extension, "");
						fileParts = fNameNoExt.Split('-');
						
						// Do we have bad names?
						if (fileParts.Length < 4)
						{
							//badListNoDB
							cmd.Parameters.Clear();
							cmd.Parameters.AddWithValue("@FileName", fi.Name);
							using (dr = cmd.ExecuteReader())
							{
								if (dr.HasRows)
								{
									Log(fi.Name, badList);
								}
								else
								{
									Log(fi.Name, badListNoDB);
								}
							}
						}
						else if (codes.Contains(fileParts[grpPos]))
						{
							cmd.Parameters.Clear();
							cmd.Parameters.AddWithValue("@FileName", fi.Name);
							using (dr = cmd.ExecuteReader())
							{
								if (dr.HasRows)
								{
									Log("Dir:" + fileParts[grpPos] + " - " + fi.Name, goodList);
								}
								else
								{
									Log("Dir:" + fileParts[grpPos] + " - " + fi.Name, goodListNoDB);
								}
							}
						}
						else
						{
							Log(fi.Name, badList);
						}
					}
				}
				MessageBox.Show("We be done man!");
			}
			catch (Exception ex)
			{
				Log(ex.Message, errLog);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
		
		private void Log(string msg, string path)
		{
			//File.AppendAllText(path, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - " + msg + Environment.NewLine);
			File.AppendAllText(path, msg + Environment.NewLine);
		}
	}
}
