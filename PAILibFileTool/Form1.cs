using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using PAILibFileTool.UI;
using PAILibFileTool.Properties;

namespace PAILibFileTool
{
	public partial class frmMain : Form
	{
		private string dir = "F:\\library\\";

		public frmMain()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string[] files = Directory.GetFiles(dir, "*.*");
			LibraryFile lf = null;
			textBox1.Text = string.Empty;

			using (new WaitCursor())
			{
				using (SqlConnection con = new SqlConnection())
				using (SqlCommand cmd = new SqlCommand())
				{
					con.ConnectionString = Settings.Default.conLib;
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 0;
					cmd.CommandText = "usp_DL_InsertDoc";

					foreach (string file in files)
					{
						lf = GetLibraryFile(file);

						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@GroupNo", lf.GroupNo);
						cmd.Parameters.AddWithValue("@Code", lf.Code);
						cmd.Parameters.AddWithValue("@DateStamp", lf.DateStamp);
						cmd.Parameters.AddWithValue("@Version", lf.Version);
						cmd.Parameters.AddWithValue("@FullDesc", lf.FullDesc);
						cmd.Parameters.AddWithValue("@Ext", lf.Ext);
						cmd.Parameters.AddWithValue("@FileName", lf.FileName);
						// create our output param
						SqlParameter pOut = new SqlParameter("@DocID", SqlDbType.Int);
						pOut.Direction = ParameterDirection.Output;
						cmd.Parameters.Add(pOut);
						try
						{
							cmd.ExecuteNonQuery();
						}
						catch
						{
							textBox1.Text += lf.GroupNo + "|"
								+ lf.Code + "|" + lf.DateStamp + "|" + lf.Version + "|"
								+ lf.FullDesc + "|" + lf.Ext + "|" + lf.FileName + Environment.NewLine;
							continue;
						}
					}
				}
			}
		}

		private LibraryFile GetLibraryFile(string file)
		{
			LibraryFile lf = new LibraryFile();

			try
			{
				FileInfo fi = new FileInfo(file);
				string[] tokens = null;

				if (fi.Exists)
				{
					//lf.DateStamp = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
					lf.FileName = fi.Name;
					lf.FileSize = fi.Length.ToString();
					lf.Ext = fi.Extension.Replace(".", "");
					lf.FileDate = fi.LastAccessTime.ToString("MM/dd/yyyy HH:mm:ss tt");

					tokens = lf.FileName.Split(new char[] { '-' }, 5);
					if (tokens.Length > 0) lf.GroupNo = tokens[0];
					if (tokens.Length > 1) lf.Code = tokens[1];
					if (tokens.Length > 2) lf.DateStamp = tokens[2];
					if (tokens.Length > 3) lf.Version = tokens[3];
					if (tokens.Length > 4) lf.FullDesc = tokens[4];
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			return lf;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			textBox1.Text = string.Empty;
			this.Cursor = Cursors.WaitCursor;

			try
			{
				using (SqlConnection con = new SqlConnection("Server=charwood;Database=PAILibraryCI;User ID=paiuser;Password=u84jKum;Persist Security Info=True;Async=true;Application Name=PAILIB_PUB"))
				using (SqlCommand cmd = new SqlCommand())
				{
					con.Open();

					cmd.Connection = con;
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = "SELECT GroupNo + '-' + FullDesc + '.pdf' As [FileName] FROM DL_Docs where Code = '063' and DateStamp = '" + textBox2.Text + "' ORDER BY RecordDate";
					//cmd.CommandText = "SELECT GroupNo + '-' + FullDesc + '.pdf' As [FileName] FROM DL_Docs where Code = '063' and CAST(RecordDate AS DATE) = '" + textBox2.Text + "' ORDER BY RecordDate";

					DataTable dt = new DataTable();
					SqlDataAdapter da = new SqlDataAdapter(cmd);
					da.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow row in dt.Rows)
						{
							FileInfo f = new FileInfo(@"\\pebble_beach\VFDocs\" + row["FileName"].ToString());
							if (f.Exists)
								textBox1.Text += f.Name + " exists" + Environment.NewLine;
							else
								textBox1.Text += f.Name + " DOES NOT exists" + Environment.NewLine;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}
	}
}
