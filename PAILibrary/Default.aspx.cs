using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PAILibrary
{
	public partial class _Default : System.Web.UI.Page
	{
		private string userName = string.Empty;

		protected void Page_Load(object sender, EventArgs e)
		{
			 if (Helpers.Offline == true)
				Response.Redirect("Offline.aspx");

			//NEW CODE START
			if (User.Identity.Name == "")
                userName = Helpers.RemoveDomainFromUserName(HttpContext.Current.Request.LogonUserIdentity.Name);
            else
                userName = Helpers.RemoveDomainFromUserName(User.Identity.Name);
            //NEW CODE END

            litFooter.Text = string.Empty;

			if (Page.IsPostBack == false)
			{
				DateStamp.Value = "yyyymmdd";
				DateStamp.Style.Add("color", "#A9A9A9");
			}

		}

		protected void btnSearch_Click(object sender, EventArgs e)
		{
			string group = Group.Value.Trim();
			string code = Code.Value.Trim();
			string dateStamp = DateStamp.Value.Trim();
			string version = Version.Value.Trim();
			string desc = Desc.Value.Trim();
			string dateSearchType = DateSearchType.Value.Trim();
			string sql = dateSearchType;

			if (group == string.Empty && code == string.Empty && dateStamp == string.Empty
				&& version == string.Empty && desc == string.Empty)
			{
				litFooter.Text = "Plese enter at least one search criteria.";
				return;
			}

			// build our sql string
			sql = "SELECT DocID, GroupNo, t1.Code, DateStamp, [VERSION], FullDesc, Ext AS 'Type', Notes "
				+ " FROM DL_Docs t1 INNER JOIN DL_Security t2 ON t1.Code = t2.Code  WHERE 1=1";

			if (group != string.Empty)
				sql += string.Format(" AND GroupNo = '{0}'", group);
			if (code != string.Empty)
				sql += string.Format(" AND t1.Code = '{0}'", code);

			if (dateStamp != string.Empty && dateStamp != "yyyymmdd")
			{
				switch (dateSearchType)
				{
					case "=":
						sql += string.Format(" AND DateStamp LIKE '{0}%'", dateStamp);
						break;
					case "<":
						sql += string.Format(" AND DateStamp < '{0}'", dateStamp);
						break;
					case ">":
						sql += string.Format(" AND DateStamp > '{0}'", dateStamp);
						break;
					case "<=":
						sql += string.Format(" AND DateStamp <= '{0}'", dateStamp);
						break;
					case ">=":
						sql += string.Format(" AND DateStamp >= '{0}'", dateStamp);
						break;
					default:
						break;
				}
			}
			
			if (version != string.Empty)
				sql += string.Format(" AND VERSION = '{0}'", version);
			if (desc != string.Empty)
				sql += string.Format(" AND FullDesc LIKE '%{0}%'", desc);
			sql += " AND (UserID = '" + userName + "' OR UserID = 'ALL')";
			sql += " " + Helpers.SortOrder;  // add the ORDER BY clause
			
#if DEBUG
			litSql.Text = sql;
#endif

			BuildResultTable(sql);
		}

		private void BuildResultTable(string sql)
		{
			TableRow tRow;
			TableCell tCell;
			SqlConnection con = new SqlConnection();
			SqlDataAdapter da;
			DataSet ds = new DataSet();
			
			try
			{
				// run our query to see if we get any results
				con = new SqlConnection(Helpers.GetSQLConString());
				da = new SqlDataAdapter(sql, con);
				da.Fill(ds);

				if (ds.Tables[0].Rows.Count > 0)
				{
					// add header row
					tblResults.Rows.Add(BuildTableHeaderRow());

					// now loop through dataset and add rows
					foreach (DataRow row in ds.Tables[0].Rows)
					{
						tRow = new TableRow();
						tRow.TableSection = TableRowSection.TableBody;
						foreach (DataColumn dc in ds.Tables[0].Columns)
						{
							tCell = new TableCell();
							tCell.VerticalAlign = VerticalAlign.Top;

							if (dc.ColumnName.ToUpper() == "DOCID")
							{
								tCell.Text = "<a target=\"_blank\" href=\"ViewFile.aspx?docid=" + row[dc].ToString() + "\">Select</a>";
							}
							else if (dc.ColumnName.ToUpper() == "NOTES")
							{
								string note = Helpers.GetNote(row["DocID"].ToString());

								if (note != null && note != string.Empty)
									tCell.Text = "<a target=\"_blank\" href=\"ViewNote.aspx?nid=" + row["DocID"].ToString() + "\">View</a>";
								else
									tCell.Text = "&nbsp;";
							}
							else
							{
								tCell.Text = row[dc].ToString();
							}
							tRow.Cells.Add(tCell);
						}
						tblResults.Rows.Add(tRow);
					}
				}
				else
				{
					// remove any previous rows
					tblResults.Rows.Clear();
					litFooter.Text = "No results found.";
				}
			}
			catch (Exception ex)
			{
				litFooter.Text = "Error: " + ex.Message;
				Helpers.SendEmail("Default.aspx - " + ex.Message);
			}
		}

		private TableHeaderRow BuildTableHeaderRow()
		{
			TableHeaderRow header;
			TableHeaderCell th;
			
			header = new TableHeaderRow();
			header.TableSection = TableRowSection.TableHeader;

			try
			{
				th = new TableHeaderCell();
				th.Text = "";  // this will be for the 'Select' items
				th.Style.Add("width", "50px");
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Group";
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Code";
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Date Stamp";
				th.Style.Add("width", "85px");
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Ver";
				th.Style.Add("width", "50px");
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Desc";
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Type";
				header.Cells.Add(th);

				th = new TableHeaderCell();
				th.Text = "Notes";
				th.Style.Add("width", "45px");
				header.Cells.Add(th);
			}
			catch (Exception ex)
			{
				Helpers.SendEmail("Default.aspx - " + ex.Message);
			}

			return header;
		}
	}
}
