using System;
using System.Windows.Forms;

namespace PAILibFileTool.UI
{
	/// <summary>
	/// Utility class used to display a wait cursor
	/// while a long operation takes place and
	/// guarantee that it will be removed on exit.
	/// </summary>
	/// <example>
	/// using(new WaitCursor())
	/// {
	///     // long running operation goes here...
	/// }
	/// </example>
	internal class WaitCursor : IDisposable
	{
		private readonly Cursor _cursor;
		
		public WaitCursor()
		{
			_cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		
		public void Dispose()
		{
			Cursor.Current = _cursor;
		}
	}
}
