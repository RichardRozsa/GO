using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using gma.Drawing.ImageInfo;

namespace Go
{
    public class Keywords
    {
        public Keywords()
        {
            keywords = new SortedList();
		    keywords["<<PixXDim>>"]         = new PhotoFileInfoDelegate(KeywordPixXDim);
            keywords["<<PixYDim>>"]         = new PhotoFileInfoDelegate(KeywordPixYDim);
            keywords["<<XResolution>>"]     = new PhotoFileInfoDelegate(KeywordXResolution);
            keywords["<<YResolution>>"]     = new PhotoFileInfoDelegate(KeywordYResolution);
            keywords["<<ResolutionUnit>>"]  = new PhotoFileInfoDelegate(KeywordResolutionUnit);
            keywords["<<Brightness>>"]      = new PhotoFileInfoDelegate(KeywordBrightness);
            keywords["<<EquipMake>>"]       = new PhotoFileInfoDelegate(KeywordEquipMake);
            keywords["<<EquipModel>>"]      = new PhotoFileInfoDelegate(KeywordEquipModel);
            keywords["<<Copyright>>"]       = new PhotoFileInfoDelegate(KeywordCopyright);
            keywords["<<DateTime>>"]        = new PhotoFileInfoDelegate(KeywordDateTime);
            keywords["<<DTOrig>>"]          = new PhotoFileInfoDelegate(KeywordDTOrig);
            keywords["<<DTDigitized>>"]     = new PhotoFileInfoDelegate(KeywordDTDigitized);
            keywords["<<ISOSpeed>>"]        = new PhotoFileInfoDelegate(KeywordISOSpeed);
            keywords["<<Orientation>>"]     = new PhotoFileInfoDelegate(KeywordOrientation);
            keywords["<<FocalLength>>"]     = new PhotoFileInfoDelegate(KeywordFocalLength);
            keywords["<<FNumber>>"]         = new PhotoFileInfoDelegate(KeywordFNumber);
            keywords["<<ExposureProg>>"]    = new PhotoFileInfoDelegate(KeywordExposureProg);
            keywords["<<MeteringMode>>"]    = new PhotoFileInfoDelegate(KeywordMeteringMode);
            keywords["<<FileName>>"]        = new PhotoFileInfoDelegate(KeywordFileName);
            keywords["<<CreationTime>>"]    = new PhotoFileInfoDelegate(KeywordCreationTime);
            keywords["<<FullName>>"]        = new PhotoFileInfoDelegate(KeywordFullName);
            keywords["<<LastAccessTime>>"]  = new PhotoFileInfoDelegate(KeywordLastAccessTime);
            keywords["<<LastWriteTime>>"]   = new PhotoFileInfoDelegate(KeywordLastWriteTime);
            keywords["<<FileSize>>"]        = new PhotoFileInfoDelegate(KeywordFileSize);
        }

        private delegate string PhotoFileInfoDelegate(FileInfo fileInfo, Info photoInfo);

        SortedList keywords = null;

        public string ReplaceKeywordsWithValues(ref string text, FileInfo fileInfo, Info photoInfo)
        {
            for (int i = 0; i < keywords.Count; i++)
            {
                string keyword = (string)keywords.GetKey(i);
                PhotoFileInfoDelegate photoFileDelete = (PhotoFileInfoDelegate)keywords.GetByIndex(i);

                int pos = text.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase);
                if (pos == 0)
                    text = photoFileDelete(fileInfo, photoInfo) + text.Substring(pos + keyword.Length);
                else if (pos > 0)
                    text = text.Substring(0, pos) + photoFileDelete(fileInfo, photoInfo) + text.Substring(pos + keyword.Length);
            }

            return text;
        }

        private string KeywordPixXDim(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.PixXDim.ToString();
        }

        private string KeywordPixYDim(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.PixYDim.ToString();
        }

        private string KeywordXResolution(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.XResolution.ToString();
        }

		private string KeywordYResolution(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.YResolution.ToString();
        }

		private string KeywordResolutionUnit(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.ResolutionUnit.ToString();
        }

        private string KeywordBrightness(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.Brightness.ToString();
        }

        private string KeywordEquipMake(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.EquipMake;
        }

        private string KeywordEquipModel(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.EquipModel;
        }

        private string KeywordCopyright(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.Copyright;
        }

        private string KeywordDateTime(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.DateTime;
        }

        private string KeywordDTOrig(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.DTOrig.ToString();
        }

        private string KeywordDTDigitized(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.DTDigitized.ToString();
        }

        private string KeywordISOSpeed(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.ISOSpeed.ToString();
        }

        private string KeywordOrientation(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.Orientation.ToString();
        }

        private string KeywordFocalLength(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.FocalLength.ToString();
        }

        private string KeywordFNumber(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.FNumber.ToString();
        }

        private string KeywordExposureProg(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.ExposureProg.ToString();
        }

        private string KeywordMeteringMode(FileInfo fileInfo, Info photoInfo)
        {
            return photoInfo.MeteringMode.ToString();
        }

        private string KeywordFileName(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.Name;
        }

        private string KeywordCreationTime(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.CreationTime.ToString();
        }

        private string KeywordDirectoryName(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.DirectoryName;
        }

        private string KeywordFullName(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.FullName;
        }

        private string KeywordLastAccessTime(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.LastAccessTime.ToString();
        }

        private string KeywordLastWriteTime(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.LastWriteTime.ToString();
        }

        private string KeywordFileSize(FileInfo fileInfo, Info photoInfo)
        {
            return fileInfo.Length.ToString();
        }
    }
}

