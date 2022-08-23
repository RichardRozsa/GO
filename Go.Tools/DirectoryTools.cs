using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.SqlServer.Server;

namespace Go.Tools
{
	public class DirectoryTools
	{
		static public string FindDirectory(string baseDir, string directoryPrefix, bool recursive)
		{
			directoryPrefix = directoryPrefix.ToUpper();
			string[] directories = Directory.GetDirectories(baseDir);
			foreach (string directory in directories) {
				string dir = directory.Substring(directory.LastIndexOf('\\') + 1).ToUpper();
				if (dir.StartsWith(directoryPrefix))
					return directory;
			}

			if (recursive) {
				foreach (string directory in directories) {
					string dir = FindDirectory(directory, directoryPrefix, recursive);
					if (dir != null)
						return dir;
				}
			}

			return null;
		}

		static public void FindDirectories(List<string> directories, string baseDir, string directoryPrefix, string directoryName, bool recursive)
		{
			string[] baseDirectories = Directory.GetDirectories(baseDir);
			foreach (string directory in baseDirectories)
			{
				if (!string.IsNullOrEmpty(directoryPrefix))
				{
					if (directory.StartsWith(directoryPrefix, StringComparison.InvariantCultureIgnoreCase))
						directories.Add(directory);
				}
				else if (!string.IsNullOrEmpty(directoryName))
				{
					if (directory.Equals(directoryName, StringComparison.InvariantCultureIgnoreCase))
					{
						directories.Add(directory);
					}
				}
				else
				{
					directories.Add(directory);
				}
			}

			if (recursive) {
				foreach (string directory in baseDirectories) {
					FindDirectories(directories, directory, directoryPrefix, directoryName, true);
				}
			}
		}

		static public void FindFiles(ref SortedList files, string baseDir, string searchPattern, bool recursive)
		{
			if (searchPattern.Length == 0)
				searchPattern = "*";
			string[] list = Directory.GetFiles(baseDir, searchPattern);
			foreach (string file in list) {
				files[file] = file;
			}
			if (recursive) {
				list = Directory.GetDirectories(baseDir);
				foreach (string directory in list) {
					FindFiles(ref files, directory, searchPattern, recursive);
				}
			}
		}

        static public bool DeleteEmptySubDirectories(string directory)
        {
            bool treeEmpty = true;

            string[] subdirectoryEntries = Directory.GetDirectories(directory);
            foreach (string subdirectory in subdirectoryEntries)
            {
                if (DeleteEmptySubDirectories(subdirectory))
                    DirectoryTools.DeleteTree(subdirectory);
                else
                    treeEmpty = false;
            }

            if (treeEmpty)
            {
                string[] fileEntries = Directory.GetFiles(directory);
                return (fileEntries.LongLength == 0
                        || (fileEntries.LongLength == 1 && fileEntries[0].ToLower().EndsWith("thumbs.db")));
            }
            else
                return false;
        }

		static public void DeleteTree(string directory)
		{
			if (Directory.Exists(directory))
			{
				DeleteTreeRecurse(directory);
				Directory.Delete(directory, true);
			}
		}

		static private void DeleteTreeRecurse(string directory)
		{
			string [] fileEntries = Directory.GetFiles(directory);
			foreach(string filename in fileEntries)
			{
				File.SetAttributes(filename, FileAttributes.Normal);
				File.Delete(filename);
			}

			string [] subdirectoryEntries = Directory.GetDirectories(directory);
			foreach(string subdirectory in subdirectoryEntries)
				DeleteTreeRecurse(subdirectory);
		}

		static public void CopyTree(
			string sourceDirectory, 
			string targetDirectory)
		{
			CopyTree(sourceDirectory, targetDirectory, true, false, FileAttributes.Normal);
		}

		static public void CopyTree(
			string sourceDirectory, 
			string targetDirectory,
			bool truncateLongFilenames) {
			CopyTree(sourceDirectory, targetDirectory, truncateLongFilenames, false, FileAttributes.Normal);
		}

		static public void CopyTree(
			string sourceDirectory, 
			string targetDirectory, 
			bool truncateLongFilenames,
			bool setAttribute,
			FileAttributes newAttribute)
		{
			CopyTree(sourceDirectory, targetDirectory, truncateLongFilenames, setAttribute, newAttribute, false, DateTime.Now);
		}

		static public void CopyTree(
			string sourceDirectory, 
			string targetDirectory, 
			bool truncateLongFilenames,
			bool setAttribute,
			FileAttributes newAttribute,
			bool touchFiles,
			DateTime touchDateTime)
		{
			CopyTree(sourceDirectory, targetDirectory, truncateLongFilenames, setAttribute, newAttribute, touchFiles, touchDateTime, new string[0]);
		}

		static public void CopyTree(
			string sourceDirectory, 
			string targetDirectory, 
			bool truncateLongFilenames,
			bool setAttribute,
			FileAttributes newAttribute,
			bool touchFiles,
			DateTime touchDateTime,
			string[] excludeFilePattern)
		{
			if (!Directory.Exists(sourceDirectory))
				throw new Exception(string.Format("Error: Directory [{0}] doesn't exist", sourceDirectory));

			Directory.CreateDirectory(targetDirectory);
			if (touchFiles)
				TouchDirectory(targetDirectory, touchDateTime);

			CopyTreeRecurse(sourceDirectory, targetDirectory, 
				truncateLongFilenames,
				setAttribute, newAttribute, 
				touchFiles, touchDateTime, 
				excludeFilePattern);
		}

		// Process all files in the directory passed in, recurse on any directories 
		// that are found, and process the files they contain.
		static private void CopyTreeRecurse(
			string sourceDirectory, 
			string targetDirectory, 
			bool truncateLongFilenames,
			bool setAttribute,
			FileAttributes newAttribute,
			bool touchFiles,
			DateTime touchDateTime,
			string[] excludeFilePattern) 
		{
			// Build list of exclude files
			ArrayList excludeList = new ArrayList();
			foreach (string excludePattern in excludeFilePattern)
			{
				string [] excludeEntries = Directory.GetFiles(sourceDirectory, excludePattern);
				foreach (string excludeFile in excludeEntries)
					excludeList.Add(excludeFile);
			}

			// Process the list of files found in the directory.
			string [] fileEntries = Directory.GetFiles(sourceDirectory);
			foreach(string filename in fileEntries)
			{
				string file = filename.Substring(sourceDirectory.Length);
				string targetFilename = targetDirectory + file;
				bool copy = true;
				foreach (string excludeFile in excludeList)
				{
					if (filename == excludeFile)
						copy = false;
				}
				if (copy)
				{
					string drivePart;
					string dirPart;
					string filePart;
					string extPart;
					string truncFilename = filename;
					string truncTargetFilename = targetFilename;
					if (truncFilename.Length >= 260) {
						if (truncateLongFilenames) {
							StringTools.SplitPath(truncFilename, out drivePart, out dirPart, out filePart, out extPart);
							truncFilename = truncFilename.Substring(0, 259 - extPart.Length) + extPart;
						}
						else
							MessageBox.Show("Src file too long: " + filename);
					}
					if (truncTargetFilename.Length >= 260) {
						if (truncateLongFilenames) {
							StringTools.SplitPath(truncTargetFilename, out drivePart, out dirPart, out filePart, out extPart);
							truncTargetFilename = truncTargetFilename.Substring(0, 259 - extPart.Length) + extPart;
						}
						else
							MessageBox.Show("Dst file too long: " + targetFilename);
					}
					File.Copy(truncFilename, truncTargetFilename, true);
					if (touchFiles)
						FileTools.TouchFilename(truncTargetFilename, touchDateTime);
					if (setAttribute)
						File.SetAttributes(truncTargetFilename, newAttribute);
				}
			}

			// Recurse into subdirectories of this directory.
			string [] subdirectoryEntries = Directory.GetDirectories(sourceDirectory);
			foreach(string subdirectory in subdirectoryEntries)
			{
				string targetSubdirectory = targetDirectory +  
					subdirectory.Substring(sourceDirectory.Length);
				Directory.CreateDirectory(targetSubdirectory);
				if (touchFiles)
					TouchDirectory(targetSubdirectory, touchDateTime);
				CopyTreeRecurse(subdirectory, targetSubdirectory, 
					truncateLongFilenames,
					setAttribute, newAttribute, 
					touchFiles, touchDateTime,
					excludeFilePattern);
			}
		}

		static public void TouchTree(string directory, DateTime touchDateTime)
		{
			if (!Directory.Exists(directory))
				throw new Exception(string.Format("Error: Directory [{0}] doesn't exist", directory));

			TouchTreeRecurse(directory, touchDateTime);
		}

		static public void TouchDirectory(string filename, DateTime touchDateTime)
		{
			Directory.SetCreationTime(filename, touchDateTime);
			Directory.SetLastWriteTime(filename, touchDateTime);
			// Directory.SetLastAccessTime(filename, touchDateTime);
		}

		static private void TouchTreeRecurse(string directory, DateTime touchDateTime) 
		{
			// Process the list of files found in the directory.
			string [] fileEntries = Directory.GetFiles(directory);
			foreach(string filename in fileEntries)
			{
				FileTools.TouchFilename(filename, touchDateTime);
			}

			// Recurse into subdirectories of this directory.
			string [] subdirectoryEntries = Directory.GetDirectories(directory);
			foreach(string subdirectory in subdirectoryEntries)
			{
				TouchDirectory(subdirectory, touchDateTime);
				TouchTreeRecurse(subdirectory, touchDateTime);
			}
		}

        static public void SynchronizeTree(string srcDirectory, string dstDirectory, ref int copiedFiles, ref int replacedFiles, ProgressEvent progressEvent, ProgressEventArgs pea)
        {
            if (!Directory.Exists(dstDirectory))
            {
                Directory.CreateDirectory(dstDirectory);
            }

            string[] srcFiles = Directory.GetFiles(srcDirectory);
            foreach (string srcFile in srcFiles)
            {
                string dstFile = dstDirectory + srcFile.Substring(srcFile.LastIndexOf('\\'));
                FileInfo srcFi = new FileInfo(srcFile);
                FileInfo dstFi = new FileInfo(dstFile);
                if (!dstFi.Exists)
                {
                    pea.Maximum++;
                    copiedFiles++;
                    progressEvent.OnProgress(pea.UpdateProgress(copiedFiles + replacedFiles, srcFile, dstFile));
                    File.Copy(srcFile, dstFile);
                }
                else if (srcFi.LastWriteTime != dstFi.LastWriteTime || srcFi.Length != dstFi.Length)
                {
                    pea.Maximum++;
                    replacedFiles++;
                    progressEvent.OnProgress(pea.UpdateProgress(copiedFiles + replacedFiles, srcFile, dstFile));
                    File.SetAttributes(dstFile, FileAttributes.Normal);
                    File.Delete(dstFile);
                    File.Copy(srcFile, dstFile);
                }
            }

            string[] srcDirectories = Directory.GetDirectories(srcDirectory);
            foreach (string srcSubDirectory in srcDirectories)
            {
                string dstSubDirectory = dstDirectory + srcSubDirectory.Substring(srcSubDirectory.LastIndexOf('\\'));
                SynchronizeTree(srcSubDirectory, dstSubDirectory, ref copiedFiles, ref replacedFiles, progressEvent, pea);
            }
        }
	}
}
