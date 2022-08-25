using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Go.Tools
{
	public class GZipTools
	{
		public static bool IsGZip(byte[] buf)
		{
			return buf.Length >= 3 && buf[0] == 31 && buf[1] == 139 && buf[2] == 8;
		}

		public static bool UnGZip(byte[] bytes, int count, Encoding encoding, out string unzippedString)
		{
			using (var compressedStream = new MemoryStream(bytes, 0, count))
			{
				return UnGZip(compressedStream, encoding, out unzippedString);
			}
		}

		public static bool UnGZip(Stream compressedStream, Encoding encoding, out string decompressedString)
		{
			using (var decompressedStream = new MemoryStream())
			{
				using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
				{
					try
					{
						gzipStream.CopyTo(decompressedStream);
					}
					catch (Exception)
					{
						decompressedString = string.Empty;
						return false;
					}
				}

				decompressedString = StreamTools.GetStringFromStream(decompressedStream, encoding);
			}

			return decompressedString.Length > 0;
		}
	}
}
