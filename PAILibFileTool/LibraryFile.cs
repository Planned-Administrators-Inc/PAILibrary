using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAILibFileTool
{
	class LibraryFile
	{
		public string GroupNo { get; set; }
		public string Code { get; set; }
		public string DateStamp { get; set; }
		public string Version { get; set; }
		public string FullDesc { get; set; }
		public string Ext { get; set; }
		public string FileName { get; set; }
		public string FileDate { get; set; }
		public string FileSize { get; set; }

		public LibraryFile(string groupNo, string code, string dateStamp, string version,
			string fullDesc, string ext, string fileName, string fileDate, string fileSize)
		{
			GroupNo = groupNo;
			Code = code;
			DateStamp = dateStamp;
			Version = version;
			FullDesc = fullDesc;
			Ext = ext;
			FileName = fileName;
			FileDate = fileDate;
			FileSize = fileSize;
		}

		public LibraryFile()
		{
			GroupNo = string.Empty;
			Code = string.Empty;
			DateStamp = string.Empty;
			Version = string.Empty;
			FullDesc = string.Empty;
			Ext = string.Empty;
			FileName = string.Empty;
			FileDate = string.Empty;
			FileSize = string.Empty;
		}
	}
}
