using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Text;
using gma.Drawing.ImageInfo;
using Go.Tools;

namespace Go
{
    public class WebStuff : GoImplementation
    {
        #region Initialization
        static private WebStuff         webstuff        = null;
        static private ProgressEvent    progressEvent   = null;
        
        // Configuration settings.
        private readonly string settingDefaultUser                  = "DefaultUser";
        private readonly string settingInitials                     = "Initials";
        private readonly string settingPictureImportDir             = "PictureImportDir";
        private readonly string settingPictureNewDir                = "PictureNewDir";
        private readonly string settingPictureArchiveDir            = "PictureArchiveDir";
        private readonly string settingWebsitePhotoPageDir          = "WebsitePhotoPageDir";
        private readonly string settingArchiveSubDirectoryFormat    = "ArchiveSubDirectoryFormat";

        static public WebStuff GetWebStuff(ref SortedList options, ref SortedList directories, ref ProgressEvent pe)
        {
            progressEvent = pe;
            if (webstuff == null)
                webstuff = new WebStuff(ref options, ref directories);
            return webstuff;
        }

        static public WebStuff GetWebStuff()
        {
            if (webstuff == null)
                throw new Exception("Can't instantiate WebStuff class without parameters");
            return webstuff;
        }

        private WebStuff(ref SortedList options, ref SortedList directories)
            : base(ref options, ref directories)
        {
            var root = ConfigurationFile.GetLocalConfiguration(ref directories, "Go.WebStuff.xml");
            AddConfigurationOptions(root);

            string category;
            category = "WebStuff";
            AddOption(new Options(category, "ImportPictures", "", "", "Import pictures using date/time picture was taken and sort into dated directories in New directory.", new GoExecuteDelegate(ImportPictures), new GoSyntaxDelegate(ImportPicturesSyntax), null));
            AddOption(new Options(category, "ArchivePictures", "", "", "Move New contents to Archive optionally cleaning directories first of unwanted photos.", new GoExecuteDelegate(ArchivePictures), new GoSyntaxDelegate(ArchivePicturesSyntax), null));
            AddOption(new Options(category, "GeneratePhotoPages", "", "", "Generate HTML pages, generate thumbnails, and upload pages and pictures to website.", new GoExecuteDelegate(GeneratePhotoPages), new GoSyntaxDelegate(GeneratePhotoPagesSyntax), null));
        }
        #endregion Initialization

        #region WebStuff
        public string ImportPicturesSyntax(bool syntaxOnly, bool switchesOnly)
        {
            string syntax = "[-U <User>] [-P <Path>] [-Q] [-I] [-A] [-F]";

            if (switchesOnly)
                return syntax;
            else
                return syntax + @"

User :: Initials associated with a user's directories and used to label pictures.
Path :: Path to directory to process.  If not provided, <User>_ImportPicturesDir configuration setting is used.
-Q   :: Specifies quiet mode - no files are processed that require user interaction. 
-I   :: Don't prompt but automatically move identical files (same date and size).
-A   :: Automatically keep larger (or identical) and older (or identical) copy when duplicates (and delete other copy).
-F   :: Use file date (if no EXIF date).
";
        }
        public void ImportPictures(Options option)
        {
            SortedList argOptions = new SortedList();
            argOptions["-U"] = "";		// -U <User>
            argOptions["-P"] = "";		// -P <Path>
            argOptions["-Q"] = false;   // -Q : Quiet mode
            argOptions["-I"] = false;   // -I : IgnoreDuplicates mode
            argOptions["-A"] = false;   // -A : Automatic mode
            argOptions["-F"] = false;   // -F : UseFileDate (if no EXIF date) mode
            CommandLine.EvalCommandLine(ref argOptions);
            var user            = (string)argOptions["-U"];
            var path            = (string)argOptions["-P"];
            var quietMode       = (bool)argOptions["-Q"];
            var ignoreDups      = (bool)argOptions["-I"];
            var automaticMode   = (bool)argOptions["-A"];
            var fileDateMode    = (bool)argOptions["-F"];

            // Validate arguments and configuration
            if (user.Length == 0)
                user = GetDirectory(settingDefaultUser);
            if (user.Length == 0)
                throw new Exception("No -U <user> specified and no DefaultUser configured");

            var defaultUser                  = GetDirectory(settingDefaultUser);
            var initials                     = GetDirectory(user + "/" + settingInitials);
            var pictureImportDir             = GetDirectory(user + "/" + settingPictureImportDir);
            var pictureNewDir                = GetDirectory(user + "/" + settingPictureNewDir);
            var pictureArchiveDir            = GetDirectory(user + "/" + settingPictureArchiveDir);
            var websitePhotoPageDir          = GetDirectory(user + "/" + settingWebsitePhotoPageDir);
            var archiveSubDirectoryFormat    = GetDirectory(user + "/" + settingArchiveSubDirectoryFormat);

            if (path.Length == 0)
                path = pictureImportDir;
            if (path.Length == 0)
                throw new Exception("No -P <path> specified and no <user>_ImportPicturesDir configured");
            if (!Directory.Exists(path))
                throw new Exception("Specified or configured directory [" + path + "] doesn't exist.");

            var cultureInfo = new CultureInfo("en-US");

            // Process files.
            var files = new SortedList();
            DirectoryTools.FindFiles(ref files, path, "*.jpg", true);

            //progressBar.EnableProgressBar("ImportPictures", "From:", "To:", files.Count);
            var pea = new ProgressEventArgs(files.Count, false, "From:", "To:");
            progressEvent.OnProgress(pea);

            if (MessageBox.Show("About to process " + files.Count + " files.", "GO ImportPictures", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                throw new Exception("Cancelled at user's request");

            for (var fileNr = 0; fileNr < files.Count; fileNr++)
            {
                var file = (string)files.GetByIndex(fileNr);

                progressEvent.OnProgress(pea.UpdateProgress(fileNr + 1, file));

                // Get EXIF DateTime from Jpeg file.
                var dt = DateTime.MinValue;
                Info inf = null;
                try
                {
                    inf = new Info(file);
                    dt = inf.DTOrig.ToLocalTime();
                }
                catch (Exception)
                {
                    if (!quietMode)
                    {
                        dt = File.GetLastWriteTime(file);
                        if (!fileDateMode)
                        {
                            switch (MessageBox.Show("File [" + file + "] doesn't contain an EXIF date/time (for the time the picture was taken). Use the file date [" + dt.ToString() + "]?", "GO ImportPictures", MessageBoxButtons.YesNoCancel))
                            {
                                case DialogResult.Yes:
                                    break;
                                case DialogResult.No:
                                    dt = DateTime.MinValue;
                                    break;
                                case DialogResult.Cancel:
                                    throw new Exception("Cancelled at user's request");
                            }
                        }
                    }
                }
                finally
                {
                    inf.Dispose();
                }

                // Move into New directory in Originals sub-folder.
                if (dt != DateTime.MinValue)
                {
                    // Create Archive directory (if necessary)

                    // Create Year directory if configured to do so.
                    var dir = pictureNewDir;
//                    if (archiveIntoYearFolder)
                    {
                        dir += "\\" + dt.Year.ToString();
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            if (!Directory.Exists(dir))
                                throw new Exception("Unable to create directory [" + dir + "]");
                        }
                    }

                    // Create month directory if configured to do so.
//                    if (archiveIntoMonthFolder)
                    {
                        dir += "\\" + string.Format(cultureInfo, "[{0:D2}] {1:MMMM}", dt.Month, dt);
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            if (!Directory.Exists(dir))
                                throw new Exception("Unable to create directory [" + dir + "]");
                        }
                    }

                    // Create day directories.
                    dir += "\\" + "Photos_" + string.Format(cultureInfo, "{0:u}", dt).Substring(0, 10);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        if (!Directory.Exists(dir))
                            throw new Exception("Unable to create directory [" + dir + "]");
                    }
                    string editedDir = dir + "\\Edited";
                    if (!Directory.Exists(editedDir))
                    {
                        Directory.CreateDirectory(editedDir);
                        if (!Directory.Exists(editedDir))
                            throw new Exception("Unable to create directory [" + editedDir + "]");
                    }
                    dir += "\\Originals";
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        if (!Directory.Exists(dir))
                            throw new Exception("Unable to create directory [" + dir + "]");

                        // Set the Hidden attribute for the Originals directory.
                        DirectoryInfo di = new DirectoryInfo(dir);
                        di.Attributes = di.Attributes | FileAttributes.Hidden;
                    }

                    // Move picture.
                    bool moveFile = false;
                    string newFile = dir + string.Format(cultureInfo, "\\{0:yyyyMMMdd-HHmmss}", dt) + ".jpg";
                    //progressBar.UpdateProgress(file, newFile, fileNr);
                    progressEvent.OnProgress(pea.UpdateProgress(fileNr + 1, file, newFile));

                    if (File.Exists(newFile))
                    {
                        FileInfo fi1 = new FileInfo(file);
                        FileInfo fi2 = new FileInfo(newFile);
                        long fileSize = fi1.Length;
                        long newFileSize = fi2.Length;
                        DateTime fileDate = fi1.LastWriteTime;
                        DateTime newFileDate = fi2.LastWriteTime;

                        string basedir = null;
                        string subdir = null;
                        string fn = null;

                        string newFileLarger = null;
                        string fileLarger = null;
                        string newFileNewer = null;
                        string fileNewer = null;

                        bool duplicate = true;
                        bool chooseImportCopy = true;
                        bool chooseNewCopy = true;

                        if (fileSize > newFileSize)
                        {
                            duplicate = false;
                            chooseImportCopy = true;
                            chooseNewCopy = false;
                            fileLarger = " (larger)";
                            newFileLarger = " (smaller)";
                        }
                        else if (fileSize < newFileSize)
                        {
                            duplicate = false;
                            chooseImportCopy = false;
                            chooseNewCopy = true;
                            newFileLarger = " (larger)";
                            fileLarger = " (smaller)";
                        }
                        else
                        {
                            chooseImportCopy = true;
                            chooseNewCopy = true;
                            fileLarger = " (identical)";
                            newFileLarger = " (identical)";
                        }

                        if (fileDate > newFileDate)
                        {
                            duplicate = false;
                            chooseImportCopy = false;
                            chooseNewCopy = (chooseNewCopy && true);
                            fileNewer = " (newer)";
                            newFileNewer = " (older)";
                        }
                        else if (fileDate < newFileDate)
                        {
                            duplicate = false;
                            chooseImportCopy = (chooseImportCopy && true);
                            chooseNewCopy = false;
                            newFileNewer = " (newer)";
                            fileNewer = " (older)";
                        }
                        else
                        {
                            chooseImportCopy = (chooseImportCopy && true);
                            chooseNewCopy = (chooseNewCopy && true);
                            fileNewer = " (identical)";
                            newFileNewer = " (identical)";
                        }

                        if (duplicate && ignoreDups)
                        {
                            moveFile = true;
                        }
                        else if (automaticMode && chooseImportCopy)
                        {
                            moveFile = true;
                        }
                        else if (automaticMode && chooseNewCopy)
                        {
                            File.Delete(file);
                        }
                        else if (!quietMode)
                        {
                            string msg = "There is already a file with the same name in this location.\n\n";

                            msg += "Yes: Move and Overwrite\nReplace the file in the New folder with the file from the Import directory:\n";
                            GetFileParts(file, out basedir, out subdir, out fn);
                            msg += fn + "\n" + subdir + " (" + basedir + ")\n" + "Size: " + fileSize.ToString("n0") + fileLarger + "\nOriginal date: " + fileDate.ToString("d") + " " + fileDate.ToString("T") + fileNewer + "\n\n";

                            msg += "No: Don't move\nNo files will be changed. Leave this file in the New folder:\n";
                            GetFileParts(newFile, out basedir, out subdir, out fn);
                            msg += fn + "\n" + subdir + " (" + basedir + ")\n" + "Size: " + newFileSize.ToString("n0") + newFileLarger + "\nOriginal date: " + newFileDate.ToString("d") + " " + newFileDate.ToString("T") + newFileNewer;

                            switch (MessageBox.Show(msg, "GO ImportPictures", MessageBoxButtons.YesNoCancel))
                            {
                                case DialogResult.Yes:
                                    moveFile = true;
                                    break;
                                case DialogResult.No:
                                    break;
                                case DialogResult.Cancel:
                                    throw new Exception("Cancelled at user's request");
                            }
                        }
                    }
                    else
                    {
                        moveFile = true;
                    }

                    if (moveFile)
                    {
                        if (File.Exists(newFile))
                        {
                            FileInfo fiTmp = new FileInfo(newFile);
                            fiTmp.Attributes = FileAttributes.Normal;
                            File.Delete(newFile);
                        }
                        File.Move(file, newFile);

                        // Copy picture to Edited directory.
                        string editedFile = editedDir + "\\" + newFile.Substring(newFile.LastIndexOf('\\') + 1);
                        if (File.Exists(editedFile))
                            File.Delete(editedFile);
                        File.Copy(newFile, editedFile);

                        // Make Originals copy read-only.
                        FileInfo fi = new FileInfo(newFile);
                        fi.Attributes = fi.Attributes | FileAttributes.ReadOnly;
                    }
                }
            }

            // Remove empty Import sub-directories.
            DirectoryTools.DeleteEmptySubDirectories(path);

            progressEvent.OnProgress(pea.Reset());
        }

        public string ArchivePicturesSyntax(bool syntaxOnly, bool switchesOnly)
        {
            string syntax = "[-U <User>] [-XC] [-XW]";

            if (switchesOnly)
                return syntax;
            else
                return syntax + @"

User :: Initials associated with a user's directories and used to label pictures.
-XC  :: Doesn't clean pictures (by deleting Edited pictures where they are identical to Original and deleting Original where it's deleted from Edited.
-XW  :: Doesn't additionally copy pictures to WebSite directory.
";
        }
        public void ArchivePictures(Options option)
        {
            SortedList argOptions = new SortedList();
            argOptions["-U"] = "";		// -U <User>
            argOptions["-XC"] = false;  // -XC : Doesn't clean mode
            argOptions["-XW"] = false;  // -XW : Doesn't copy to web site mode
            CommandLine.EvalCommandLine(ref argOptions);
            string user         = (string)argOptions["-U"];
            bool cleanMode      = !(bool)argOptions["-XC"];
            bool copyToWebSite  = !(bool)argOptions["-XW"];

            // Validate arguments and configuration
            if (user.Length == 0)
                user = GetDirectory(settingDefaultUser);
            if (user.Length == 0)
                throw new Exception("No -U user specified and no DefaultUser configured");

            var pictureNewDir = GetDirectory(user + settingPictureNewDir);
            var pictureArchiveDir = GetDirectory(user + settingPictureArchiveDir);
            var websitePhotoPageDir = GetDirectory(user + settingWebsitePhotoPageDir);

            ProgressEventArgs pea = null;
            int progressCnt = 0;
            int handledCnt = 0;
            int taskCount = 2;
            if (copyToWebSite)
                taskCount++;

            pea = new ProgressEventArgs(taskCount, false, "Task:");
            progressEvent.OnProgress(pea);

            if (cleanMode)
            {
                SortedList files = new SortedList();
                DirectoryTools.FindFiles(ref files, pictureNewDir, "*.jpg", true);
                taskCount += 2;
                pea.Maximum = files.Count * taskCount;

                bool userVerifiesCleanMode = true;
                switch (MessageBox.Show("Delete Original photos deleted from Edited directory?", "GO ArchivePictures", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        userVerifiesCleanMode = false;
                        progressCnt += files.Count;
                        break;
                    case DialogResult.Cancel:
                        throw new Exception("Cancelled at user's request");
                }

                if (userVerifiesCleanMode)
                {
                    // Delete Originals files that are missing from Edited directory.
                    for (int fileNr = 0; fileNr < files.Count; fileNr++)
                    {
                        progressEvent.OnProgress(pea.UpdateProgress(++progressCnt, "Delete Originals files that are missing from Edited directories..."));

                        string file = (string)files.GetByIndex(fileNr);
                        if (file.Contains("\\Originals\\"))
                        {
                            string path = file.Substring(0, file.LastIndexOf('\\'));
                            string basePath = path.Substring(0, path.LastIndexOf('\\'));
                            path = basePath + "\\Edited" + file.Substring(file.LastIndexOf('\\'));
                            if (!File.Exists(path))
                            {
                                DeletePicture delPict = new DeletePicture(file);
                                switch (delPict.ShowDialog())
                                {
                                    case DialogResult.Yes:
                                        File.SetAttributes(file, FileAttributes.Normal);
                                        File.Delete(file);
                                        break;
                                    case DialogResult.No:
                                        break;
                                    case DialogResult.Cancel:
                                        return;
                                }
                            }
                        }
                    }
                    handledCnt++;
                }

                userVerifiesCleanMode = true;
                switch (MessageBox.Show("Delete unedited photos from Edited directory?", "GO ArchivePictures", MessageBoxButtons.YesNoCancel))
                {
                    case DialogResult.Yes:
                        break;
                    case DialogResult.No:
                        userVerifiesCleanMode = false;
                        progressCnt += files.Count;
                        break;
                    case DialogResult.Cancel:
                        throw new Exception("Cancelled at user's request");
                }

                if (userVerifiesCleanMode)
                {
                    // Delete Edited file when it's identical to Originals file.
                    for (int fileNr = 0; fileNr < files.Count; fileNr++)
                    {
                        progressEvent.OnProgress(pea.UpdateProgress(++progressCnt, "Delete Edited files when they are identical to Originals files..."));

                        string file = (string)files.GetByIndex(fileNr);
                        if (file.Contains("\\Edited\\"))
                        {
                            string path = file.Substring(0, file.LastIndexOf('\\'));
                            string basePath = path.Substring(0, path.LastIndexOf('\\'));
                            path = basePath + "\\Originals" + file.Substring(file.LastIndexOf('\\'));
                            if (!File.Exists(path))
                            {
                                switch (MessageBox.Show("File [" + path + "] doesn't exist.  Continue processing remaining files?", "GO ArchivePictures", MessageBoxButtons.OKCancel))
                                {
                                    case DialogResult.OK:
                                        break;
                                    case DialogResult.Cancel:
                                        throw new Exception("Cancelled at user's request");
                                }
                            }
                            else
                            {
                                FileInfo fiEdited = new FileInfo(file);
                                FileInfo fiOriginal = new FileInfo(path);
                                long editedSize = fiEdited.Length;
                                long originalSize = fiOriginal.Length;
                                DateTime editedDate = fiEdited.LastWriteTime;
                                DateTime originalDate = fiOriginal.LastWriteTime;

                                if (editedSize == originalSize && editedDate == originalDate)
                                {
                                    File.SetAttributes(file, FileAttributes.Normal);
                                    File.Delete(file);
                                }
                            }
                        }
                    }
                    handledCnt++;
                }
            }

            // Remove Hidden attribute on Originals directories.
            var origDirs = new List<string>();
            DirectoryTools.FindDirectories(origDirs, pictureNewDir, null, null, true);
            pea.Maximum = origDirs.Count * taskCount;
            progressCnt = origDirs.Count * handledCnt;
            for (int dirNr = 0; dirNr < origDirs.Count; dirNr++)
            {
                progressEvent.OnProgress(pea.UpdateProgress(++progressCnt, "Remove Hidden attribute on Originals directories..."));

                string directory = origDirs[dirNr];
                if (directory.EndsWith("\\Originals"))
                {
                    DirectoryInfo di = new DirectoryInfo(directory);
                    di.Attributes = FileAttributes.Directory;
                }
            }
            handledCnt++;

            // Copy to website.
            if (copyToWebSite)
            {
                SortedList files = new SortedList();
                DirectoryTools.FindFiles(ref files, pictureNewDir, "*.jpg", true);
                pea.Maximum = files.Count * taskCount;
                progressCnt = files.Count * handledCnt;
                for (int fileNr = 0; fileNr < files.Count; fileNr++)
                {
                    progressEvent.OnProgress(pea.UpdateProgress(++progressCnt, "Copy photos to website..."));

                    string originalFile = (string)files.GetByIndex(fileNr);
                    if (originalFile.Contains("\\Originals\\"))
                    {
                        string srcFile = originalFile;
                        string filename = originalFile.Substring(originalFile.LastIndexOf('\\') + 1);
                        string editedFile = originalFile.Substring(0, originalFile.LastIndexOf("\\Originals\\")) 
                            + "\\Edited\\" + filename;
                        if (File.Exists(editedFile))
                            srcFile = editedFile;

                        string dstdir = srcFile.Substring(0, srcFile.LastIndexOf('\\'));
                        dstdir = dstdir.Substring(0, dstdir.LastIndexOf('\\'));
                        dstdir = websitePhotoPageDir + "\\" + dstdir.Substring(dstdir.LastIndexOf('\\') + 1);
                        if (!Directory.Exists(dstdir))
                            Directory.CreateDirectory(dstdir);
                        string dstfile = dstdir + "\\" + filename;
                        if (File.Exists(dstfile))
                            File.Delete(dstfile);
                        File.Copy(srcFile, dstfile);
                    }
                }
                handledCnt++;
            }

            // Move New directories to Archive directory.
            string[] directories = Directory.GetDirectories(pictureNewDir);
            pea.Maximum = directories.Length * taskCount;
            progressCnt = directories.Length * handledCnt;
            foreach (string directory in directories)
            {
                progressEvent.OnProgress(pea.UpdateProgress(++progressCnt, "Move New directories to Archive directory..."));

                string subdir = directory.Substring(directory.LastIndexOf('\\') + 1);
                string dstdir = pictureArchiveDir + "\\" + subdir;

                try {
                    Directory.Move(directory, dstdir);
                }
                catch (Exception ex) {
                    MessageBox.Show("Error encountered: " + ex.Message + "\nMove directory '" + directory + "' to '" + dstdir + "'!\n\nCAUTION: Do not run this command again until you do this!", "GO ArchivePictures");
                }
            }
            handledCnt++;
        }
        
        private void GetFileParts(string path, out string dir, out string subdir, out string filename)
        {
            int i = path.LastIndexOf('\\');
            filename = path.Substring(i + 1);
            dir = path.Substring(0, i);
            i = dir.LastIndexOf('\\');
            subdir = "";
            if (i >= 0)
            {
                subdir = dir.Substring(i + 1);
                dir = dir.Substring(0, i);
            }
        }

        public string GeneratePhotoPagesSyntax(bool syntaxOnly, bool switchesOnly)
        {
            string syntax = "[-U <User>] [-C] [-X]";

            if (switchesOnly)
                return syntax;
            else
                return syntax + @"

User :: Initials associated with a user's directories and used to label pictures.
-C   :: Cleans pictures (by deleting Edited pictures where they are identical to Original and deleting Original where it's deleted from Edited.
-X   :: Doesn't additionally copy pictures to WebSite directory.
";
        }
        public void GeneratePhotoPages(Options option)
        {
            SortedList argOptions = new SortedList();
            argOptions["-U"] = "";		// -U <User>
            CommandLine.EvalCommandLine(ref argOptions);
            string user = (string)argOptions["-U"];
 
            // Validate arguments and configuration
            if (user.Length == 0)
                user = GetDirectory(settingDefaultUser);
            if (user.Length == 0)
                throw new Exception("No -U user specified and no DefaultUser configured");

            string newPicturesDir = GetDirectory(user + settingPictureNewDir);
            string pictureArchiveDir = GetDirectory(user + settingPictureArchiveDir);
            string websitePhotoPageDir = GetDirectory(user + settingWebsitePhotoPageDir);

        }

        private bool GeneratePhotoPages(bool generateHtmlInBase, string baseDirectory, string currentDirectory)
        {
            SortedList items = new SortedList();

            // Add non-empty sub-directories to list.
            string[] directories = Directory.GetDirectories(currentDirectory);
            foreach (string directory in directories)
            {
                if (GeneratePhotoPages(generateHtmlInBase, baseDirectory, directory))
                {
                    string subDirectory = directory.Substring(directory.LastIndexOf('\\'));
                    items.Add(subDirectory + "\\", directory.Substring(baseDirectory.Length));
                }
            }

            // Add files to list.
            string[] files = Directory.GetFiles(currentDirectory);
            foreach (string file in files)
            {
                string filename = file.Substring(file.LastIndexOf('\\'));
                if (    !filename.Equals("desktop.ini", StringComparison.CurrentCultureIgnoreCase)
                    &&  !filename.Equals("Thumbs.db",   StringComparison.CurrentCultureIgnoreCase))
                {
                    items.Add(filename, file.Substring(baseDirectory.Length));
                }
            }

            // If list is empty, this directory shouldn't be processed - return false.
            if (items.Count == 0)
                return false;

            string htmlFilePath = null;
            if (generateHtmlInBase)
                htmlFilePath = baseDirectory + currentDirectory.Substring(baseDirectory.Length) + ".html";
            else
                htmlFilePath = currentDirectory + @"\index.html";

            TextFileContents tfc = new TextFileContents(htmlFilePath);
            tfc.Contents = FormatHtmlPage(ref items);
            return true;
        }

        private string FormatHtmlPage(ref SortedList items)
        {
            StringBuilder html = new StringBuilder(FormatPageHeader());

            // Process list.  Each directory starts new group.
            bool headerPrinted = false;
            int fileCount = 0;
            int filesPerColumn = 4;
            for (int i = 0; i < items.Count; i++)
            {
                string item = (string)items.GetKey(i);
                string path = (string)items.GetByIndex(i);

                if (item.EndsWith("\\"))
                {
                    if (headerPrinted)
                    {
                        if (fileCount != 0)
                            html.Append(FormatColumnFooter());
                        fileCount = 0;
                        html.Append(FormatFileFooter());
                        headerPrinted = false;
                    }

                    html.Append(FormatDirectoryHeader());
                    html.Append(FormatDirectoryBody());
                    html.Append(FormatDirectoryFooter());
                }
                else
                {
                    if (!headerPrinted)
                    {
                        html.Append(FormatFileHeader());
                        headerPrinted = true;
                    }

                    if (fileCount == filesPerColumn)
                    {
                        html.Append(FormatColumnFooter());
                        fileCount = 0;
                    }
                    if (fileCount == 0)
                        html.Append(FormatColumnHeader());
                    fileCount++;
                    html.Append(FormatFileBody());
                }
            }

            if (fileCount != 0)
                html.Append(FormatColumnFooter());
            if (headerPrinted)
                html.Append(FormatFileFooter());
            html.Append(FormatPageFooter());

            return html.ToString();
        }

        private string FormatPageHeader()
        {
            return @"<html>
<head>
<title>#SITE# -> Family -> #USER# -> Photos #GROUP_MONTH_LONG# #GROUP_YEAR#</title>
</head>
<body>
<form method='post' name='PhotoRequest' action='http://#SITE#/cgi-bin/FormMail.pl'>

<center>

#SITE# -> Family -> #USER# -> Photos #GROUP_MONTH_LONG# #GROUP_YEAR#

<BR>

<H4>
These are thumbnails of the actual pictures.
Click on the picture to see it in full size.
<BR>
To have print quality pictures e-mailed to you,
check the one's you like and press the 'Request checked photos'
button at the bottom of this page.
</H4>
";
        }

        private string FormatPageFooter()
        {
            return @"
<BR><BR>
<input type='hidden' name='recipient' value='#USER_EMAIL#'>
<input type='hidden' name='subject' value='Photo request from http://#SITE#/family/#USER# (Photos_#GROUP_YEAR##GROUP_MONTH_SHORT#.html)'>
<input type='hidden' name='missing_fields_redirect' value=''>
<input type='hidden' name='env_report' value='REMOTE_ADDR,HTTP_USER_AGENT'>
<input type='hidden' name='print_blank_fields' value='0'>
<input type='hidden' name='required' value='email,realname'>
<input type='hidden' name='redirect' value='http://#SITE#/family/#USER#/Photos_#GROUP_YEAR##GROUP_MONTH_SHORT#.html'>
<table width=30%>
	<tr>
		<td>
			Your name:
		</td>
		<td>
			<input type='TEXT' name='realname'>
		</td>
	</tr>
	<tr>
		<td>
			Your e-mail address:
		</td>
		<td>
			<input type='TEXT' name='email'>
		</td>
	</tr>
	<tr>
		<td>
			Your phone number:
		</td>
		<td>
			<input type='Text' name='phone'>
		</td>
	</tr>
</table>
Comments:
<textarea rows='5' name='COMMENTS' cols='62'></textarea><BR><BR>
<input type='SUBMIT' value='Request checked photos'>
<input type='reset' value='Reset'>

<BR><BR>
<img src='/cgi-bin/Count.cgi?df=rozsa/counter-family-#USER_LOWER#-Photos#GROUP_YEAR##GROUP_MONTH_LOWER#.dat'><BR>
<font face='Comic Sans MS'>This page was last updated on #TODAY_DAY# #TODAY_MONTH_LONG#, #TODAY_YEAR#.</font>

</center>
</body>
</html>
";
        }

        private string FormatDirectoryHeader()
        {
            return "<BR>";
        }

        private string FormatDirectoryBody()
        {
            return "<a href=''><H2>#DIRECTORY_TITLE#</H2></a><BR>";
        }

        private string FormatDirectoryFooter()
        {
            return "<BR>";
        }

        private string FormatFileHeader()
        {
            return @"<H2>#GROUP_DAY# #GROUP_MONTH_LONG#, #GROUP_YEAR#</H2>
<table>
";
        }

        private string FormatFileBody()
        {
            return @"        <td width='170'>
			<input type='checkbox' name='PHOTO#PAGE_FILE_COUNT#' value='#FILE_PATH_WINDOWS#'>
			<font size='1'>#USER_PREFIX#-#FILENAME#
			<a href='#FILE_PATH_HTML#'><img src='#FILE_PATH_HTML#'></a>
			</font>
		</td>
";
        }

        private string FormatFileFooter()
        {
            return @"</table>
";
        }

        private string FormatColumnHeader()
        {
            return @"	<tr align='center' valign='top'>
";
        }

        private string FormatColumnFooter()
        {
            return @"	</tr>
";
//            WebBrowser wb = new WebBrowser();
        }
//             FileInfo fi = new FileInfo(@"C:\Users\Richard Rozsa\Pictures\Archive\2004\[04] April\Photos_2004-04-22\Originals\2004Apr22_125825.jpg");
//             Info pi = new Info(@"C:\Users\Richard Rozsa\Pictures\Archive\2004\[04] April\Photos_2004-04-22\Originals\2004Apr22_125825.jpg");
// 
//             Keywords k = new Keywords();
//             string s = "<<DTOrig>>testing";
//             k.ReplaceKeywordsWithValues(ref s, fi, pi);
//             s = "test<<DTOrig>>ing";
//             k.ReplaceKeywordsWithValues(ref s, fi, pi);
//             s = "testing<<DTOrig>>";
//             k.ReplaceKeywordsWithValues(ref s, fi, pi);

#endregion WebStuff
    }
}
