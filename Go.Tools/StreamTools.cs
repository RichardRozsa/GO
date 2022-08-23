using System.IO;
using System.Text;

namespace Go.Tools
{
	public class StreamTools
	{
		public static string GetStringFromStream(Stream stream, Encoding encoding)
		{
			const int bufferSize = 1024;

			var sb = new StringBuilder();
			stream.Seek(0, SeekOrigin.Begin);
			var bytes = new byte[bufferSize];
			do
			{
				var bytesRead = stream.Read(bytes, 0, bufferSize);
				if (bytesRead == 0)
					break;
				sb.Append(encoding.GetString(bytes, 0, bytesRead));
			} while (true);
			return sb.ToString();
		}
	}
}
