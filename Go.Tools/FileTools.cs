using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;

namespace Go.Tools
{
	public class FileTools
	{
		/// <summary>
		/// Writes string to temporary filename
		/// </summary>
		/// <param name="filename">Filename (without path)</param>
		/// <param name="text">string to write to file</param>
		/// <returns>Just generated filename</returns>
		static public string WriteTempFile(string filename, string textContents)
		{
			string path = Environment.ExpandEnvironmentVariables(@"%TEMP%\") + filename;
            WriteFile(path, textContents);
			return path;
		}

        static public void WriteFile(string filePath, string textContents)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(textContents);
            }
        }
        
        static public void TouchFilename(string filename, DateTime touchDateTime) 
		{
			bool clearReadOnlyAttribute = false;
			FileAttributes oldAttribute = File.GetAttributes(filename);
			if ((oldAttribute & FileAttributes.Directory) == FileAttributes.Directory)
			{
				DirectoryTools.TouchDirectory(filename, touchDateTime);
			}
			else
			{
				if ((oldAttribute & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					clearReadOnlyAttribute = true;
					File.SetAttributes(filename, FileAttributes.Normal);
				}
				File.SetCreationTime(filename, touchDateTime);
				File.SetLastWriteTime(filename, touchDateTime);
				// File.SetLastAccessTime(filename, touchDateTime);
				if (clearReadOnlyAttribute)
					File.SetAttributes(filename, oldAttribute);
			}
		}
	}

	public class TextFileContents
	{
		private string filename;
		private string contents;
		private bool isRead;
		private string[] lines;
		private int linePos;
		private string tablename;
		private Match sqlInsertMatch;
		private ArrayList fields;
		private SortedList values;

		public TextFileContents(string filename)
		{
			this.filename = filename;
			this.contents = "";
			this.isRead = false;
			this.lines = null;
			this.linePos = 0;
			this.tablename = null;
			this.sqlInsertMatch = null;
			this.fields	= null;
			this.values = null;
		}
			
		public TextFileContents(string filename, string contents)
		{
			this.filename = filename;
			this.contents = contents;
			this.isRead = false;
			this.lines = null;
			this.linePos = 0;
			this.tablename = null;
			this.sqlInsertMatch = null;
			this.fields	= null;
			this.values = null;
		}

		public bool Exists
		{
			get { return File.Exists(filename); }
		}

		public void Copy(string dstFilename, bool overwrite)
		{
			if (this.Filename != dstFilename)
				File.Copy(this.Filename, dstFilename, overwrite);
		}

		public void Copy(TextFileContents dstFile, bool overwrite)
		{
			if (this.Filename != dstFile.Filename)
			{
				File.Copy(this.Filename, dstFile.Filename, overwrite);
				dstFile.Contents = this.Contents;
			}
		}

		public void CopyIfExists(string dstFilename, bool overwrite)
		{
			if (this.Exists)
				this.Copy(dstFilename, overwrite);
		}

		public void CopyIfExists(TextFileContents dstFile, bool overwrite)
		{
			if (this.Exists)
				this.Copy(dstFile, overwrite);
		}

		public string Filename
		{
			get { return filename; }
		}

		public string GetFirstLine()
		{
			lines = Contents.Split(new char[] {'\n'});
			linePos = 0;
			return GetNextLine();
		}

		public string GetNextLine()
		{
			if (lines.Length > linePos) {
				string line = lines[linePos++];
				if (line.EndsWith("\r"))
					return line.Substring(0, line.Length - 1);
				else
					return line;
			}
			else
				return null;
		}

		public void GetSqlInsertStatements(ref DataTable table) {
			ArrayList allFields = new ArrayList();
			string tableName = table.TableName.Substring(table.TableName.LastIndexOf('.') + 1);

			// Build field list first.
			bool ret = GetFirstSqlInsertStatement(tableName);
			while (ret) {
				for (int i = 0; i < fields.Count; i++) {
					if (!table.Columns.Contains((string)fields[i]))
						table.Columns.Add((string)fields[i]);
				}
				ret = GetNextSqlInsertStatement();
			}

			// Add values.
			ret = GetFirstSqlInsertStatement(tableName);
			while (ret) {
				DataRow row = table.NewRow();
				for (int i = 0; i < fields.Count; i++) {
					row[(string)fields[i]] = GetSqlFieldValue((string)fields[i]);
				}
				table.Rows.Add(row);

				ret = GetNextSqlInsertStatement();
			}
		}

		public bool GetFirstSqlInsertStatement(string tablename) {
			return GetFirstSqlInsertStatement("", "", tablename);
		}
		
		public bool GetFirstSqlInsertStatement(string owner, string tablename) {
			return GetFirstSqlInsertStatement("", owner, tablename);
		}

		public bool GetFirstSqlInsertStatement(string database, string owner, string tablename) {
			this.tablename = tablename;
			this.fields	= null;
			this.values = null;

			// TODO: Filter properly on database.owner.table rather than just table.
			Regex regexInsertStatement = new Regex(@"^(?<BeginStatement>\s*INSERT\s+(INTO\s+)*(?<Database>\[*.*\]*\.)*(?<Owner>\[*(dbo)*\]*\.)*(?<Table>\[*" + tablename + @"\]*)\s*\()",
//			Regex regexInsertStatement = new Regex(@"^(?<BeginStatement>\s*INSERT\s+INTO\s+\[*(?<Table>" + tablename + @")\]*\s*\()",
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

			sqlInsertMatch = regexInsertStatement.Match(Contents);

			return ParseSqlInsertStatement();
		}

		public bool GetNextSqlInsertStatement() {
			if (sqlInsertMatch == null)
				return false;
			sqlInsertMatch = sqlInsertMatch.NextMatch();
			return ParseSqlInsertStatement();
		}

		private bool ParseSqlInsertStatement() {
			fields = new ArrayList();
			values = new SortedList();

			if (!sqlInsertMatch.Success)
				return false;

			// Get previous line if it's a comment.
			Regex regexInsertStatement1 = new Regex(@"\/\*\s*(?<Comment>.*)\s*\*\/");
			Regex regexInsertStatement2 = new Regex(@"\/\/\s*(?<Comment>.*)\s*$");
			Regex regexInsertStatement3 = new Regex(@"--s*(?<Comment>.*)\s*$");

			int commentEndPos = this.Contents.LastIndexOf('\n', sqlInsertMatch.Index);
			int commentStartPos = this.Contents.LastIndexOf('\n', commentEndPos - 1);
			string comment = this.Contents.Substring(commentStartPos + 1, commentEndPos - commentStartPos - 2);
			Match match = regexInsertStatement1.Match(comment);
			if (!match.Success)
				match = regexInsertStatement2.Match(comment);
			if (!match.Success)
				match = regexInsertStatement3.Match(comment);
			if (!match.Success)
				comment = null;
			else
				comment = match.Result("${Comment}").Trim();
			fields.Add("INSERT_COMMENT");
			values.Add("INSERT_COMMENT", comment);

			// Get regex results.
			string qualifiedTableName = sqlInsertMatch.Result("${Table}");
			string beginStatement = sqlInsertMatch.Result("${BeginStatement}");
			int startPos = sqlInsertMatch.Index + beginStatement.Length;
			
			// Parse field names.
			while (this.Contents[startPos] != ')') {
				if (this.Contents[startPos] == ',')
					startPos++;
				int endPos = this.Contents.IndexOfAny(new char[] {',', ')'}, startPos);
				if (endPos == -1)
					return false;
				string fieldname = this.Contents.Substring(startPos, endPos - startPos).Trim(new char[] {' ', '\t', '\r', '\n'});
				if (fieldname[0] == '[')
					fieldname = fieldname.Substring(1, fieldname.Length - 2);
				fields.Add(fieldname.ToUpper());
				startPos = endPos;
			}

			// Skip past ") values (" string.
			startPos++;
			for ( ; startPos < this.Contents.Length && this.Contents.IndexOfAny(new char[] {' ', '\t', '\r', '\n'}, startPos, 1) == startPos; startPos++);
			string valuesStr = "values";
			if (startPos + valuesStr.Length >= this.Contents.Length || this.Contents.Substring(startPos, valuesStr.Length).ToLower() != valuesStr)
				return false;
			startPos += valuesStr.Length;
			for ( ; startPos < this.Contents.Length && this.Contents.IndexOfAny(new char[] {' ', '\t', '\r', '\n'}, startPos, 1) == startPos; startPos++);
			if (this.Contents[startPos] != '(')
				return false;
			startPos++;

			// Parse field values.
			int fieldNumber = 1;
			while (this.Contents[startPos] != ')') {
				if (fieldNumber == fields.Count)
					return false;
				if (this.Contents[startPos] == ',')
					startPos++;

				int endPos = FindClosingCharPos(startPos);
				string fieldvalue = this.Contents.Substring(startPos, endPos - startPos + 1);
				values.Add(fields[fieldNumber++], fieldvalue);

				startPos = endPos + 1;
			}

			/*
				for ( ; startPos < this.Contents.Length && this.Contents.IndexOfAny(new char[] {' ', '\t', '\r', '\n'}, startPos, 1) == startPos; startPos++);

				// If string value, find closing quote.
				int endPos = 0;
				string fieldvalue = "";
				if (this.Contents[startPos] == '\'') {
					startPos++;
					endPos = startPos;
					int quoteCount = 1;
					while (quoteCount % 2 != 0) {
						endPos = this.Contents.IndexOf('\'', endPos);
						if (endPos < 0)
							return false;
						quoteCount++;
						endPos++;
						for ( ; endPos < this.Contents.Length && this.Contents[endPos] == '\''; endPos++, quoteCount++);
					}
					fieldvalue = this.Contents.Substring(startPos, endPos - startPos - 1);

					for ( ; endPos < this.Contents.Length && this.Contents.IndexOfAny(new char[] {' ', '\t', '\r', '\n'}, endPos, 1) == endPos; endPos++);
				}
				else {
					endPos = startPos;
					// Handle cases:
					//	123
					//	getdate()
					//	datediff(d, getdate(), field)
					//	'value'+char(9)+'value'
					while (true) {
						endPos = this.Contents.IndexOfAny(new char[] {',', ')'}, endPos);
						if (endPos == -1)
							return false;
						if (	CountChar(this.Contents.Substring(startPos, endPos - startPos), '(')
							==	CountChar(this.Contents.Substring(startPos, endPos - startPos), ')'))
							break;
						endPos++;
					}
					fieldvalue = this.Contents.Substring(startPos, endPos - startPos).Trim(new char[] {' ', '\t', '\r', '\n'});
				}
				values.Add(fields[fieldNumber++], fieldvalue);

				startPos = endPos;
			}
			*/

			return true;
		}

		private int	FindClosingCharPos(int startPos) {
			string whiteSpace = " \t\r\n";
			string numbers = "0123456789";
			string upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string numberChars = numbers + ".";
			string sqlIdentifierChars = upperLetters + upperLetters.ToLower() + numbers + "@$#_";

			int endPos = startPos;
			char ch = this.Contents[startPos];

			if		(ch == ',' || ch == ')') {		// Next value or end value
				return endPos - 1;
			}
			if		(whiteSpace.IndexOf(ch) >= 0) {	// Whitespace
				endPos = LastIndexOfAnyConsecutive(whiteSpace, endPos);
				endPos = FindClosingCharPos(endPos + 1);
			}
			else if ('\'' == ch) {					// Quoted string
				startPos++;
				endPos = startPos;
				int quoteCount = 1;
				while (quoteCount % 2 != 0) {
					endPos = this.Contents.IndexOf('\'', endPos);
					if (endPos < 0)
						throw new Exception("No closing quote (') found in string beginning [" + this.Contents.Substring(startPos, 100));
					for ( ; endPos < this.Contents.Length && this.Contents[endPos] == '\''; endPos++, quoteCount++);
				}
				if (this.Contents[endPos] != '\'')
					endPos--;
			}
			else if ('(' == ch) {					// Function parameter list
				endPos++;
				while (this.Contents[endPos] != ')') {
					if (this.Contents[endPos] == ',')
						endPos++;
					endPos = FindClosingCharPos(endPos);
					endPos++;
				}
			}
			else if (ch >= '0' && ch <= '9') {		// Number
				endPos = LastIndexOfAnyConsecutive(numberChars, endPos);
			}
			else if (ch == '+') {					// Concatenate operator
				endPos++;
			}
			else {
				if (ch == '@')						// SQL variable
					endPos++;

				// Identifier: variable or function
				endPos = LastIndexOfAnyConsecutive(sqlIdentifierChars, endPos);
			}

			return FindClosingCharPos(endPos + 1);
		}

		private int LastIndexOfAnyConsecutive(string searchChars, int pos) {
			for ( ; pos < this.Contents.Length && searchChars.IndexOf(this.Contents[pos]) >= 0; pos++);
			if (searchChars.IndexOf(this.Contents[pos]) < 0)
				pos--;
			return pos;
		}

		public string GetSqlFieldValue(string fieldname) {
			if (values != null && values.Contains(fieldname.ToUpper()))
				return (string)values[fieldname.ToUpper()];
			else
				return null;
		}

		public string GetSqlFieldValue(int fieldNumber) {
			if (fields != null && fieldNumber >= 0 && fieldNumber < fields.Count)
				return GetSqlFieldValue((string)fields[fieldNumber]);
			else
				return null;
		}

		private int CountChar(string s, char ch) {
			int chCount = 0;

			for (int pos = 0; pos < s.Length; pos++, chCount++) {
				pos = s.IndexOf(ch, pos);
				if (pos < 0)
					break;
			}

			return chCount;
		}

		public string Contents 
		{
			get { Read(); return contents; }
			set
			{
				contents = value;
				using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Default))
				{
					sw.Write(contents);
				}
			}
		}

		public void Read()
		{
			if (!isRead)
			{
				if (File.Exists(filename))
				{
					using (StreamReader sr = new StreamReader(filename, System.Text.Encoding.Default))
					{
						contents = sr.ReadToEnd();
						isRead = true;
					}
				}
			}
		}

		public bool Compare(string filename)
		{
			return this.Compare(new TextFileContents(filename));
		}

		public bool Compare(TextFileContents textFileContents)
		{
			return Contents.CompareTo(textFileContents.Contents) == 0;
		}
	}
}
