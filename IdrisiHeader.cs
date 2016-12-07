using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Carto
{
    /// <summary>
    /// The IDRISI header file object, contains the data in the header file
    /// </summary>
    public class IdrisiHeader
    {
        public struct KeyValuePair
        {
            public int Key;
            public string Value;
        }

        public List<string> Lineage;
        public List<string> Comments;
        public List<string> Completeness;
        public List<string> Consistency;
        public List<KeyValuePair> LegendCodes;
        public string FileFormat;
        public string FileTitle;
        public string DataType;
        public string FileType;
        public int NumCols;
        public int NumRows;
        public string RefSystem;
        public string RefUnits;
        public double UnitDist;
        public double MinX;
        public double MinY;
        public double MaxX;
        public double MaxY;
        public string PosnError;
        public string Resolution;
        public double MinValue;
        public double MaxValue;
        public double DisplayMin;
        public double DisplayMax;
        public string ValueUnits;
        public string ValueError;
        public string FlagValue;
        public string FlagDefn;
        public int LegendCats;
        public double CellRes;

        /// <summary>
        /// Default constructor
        /// </summary>
        public IdrisiHeader()
        {
            BlankHeader();
        }

        ~IdrisiHeader()
        {
            Comments = null;
            Lineage = null;
            Consistency = null;
            LegendCodes = null;
            Completeness = null;
        }

        public void Init(string Title, int NumRows, int NumCols, string DataType, double CellRes, double MinX, double MaxY)
        {
            BlankHeader();
            this.FileTitle = Title;
            this.NumRows = NumRows;
            this.NumCols = NumCols;
            this.DataType = DataType.Trim().ToLower();
            this.CellRes = CellRes;
            this.MinX = MinX;
            this.MaxX = MinX + CellRes * NumCols;
            this.MaxY = MaxY;
            this.MinY = MaxY - CellRes * NumRows;
        }
       
        ///// <summary>
        ///// Create a new (blank) header file
        ///// </summary>
        ///// <param name="FilePath">The name of the header file to create</param>
        //public void CreateNew(string FilePath)
        //{
        //    CheckFilePath(ref FilePath,false);
        //    BlankHeader();
        //}

        /// <summary>
        /// Erase a header structure.  Returns all information to an unspecified state.
        /// </summary>
        public void EraseHeader()
        {
            BlankHeader();
        }


        /// <summary>
        /// Copy header info from an existing header
        /// </summary>
        /// <param name="Source">The Idrisi header from which to copy information</param>
        public void CloneFr(IdrisiHeader Source)
        {
            FileFormat = Source.FileFormat;
            FileTitle = Source.FileTitle;
            DataType = Source.DataType;
            FileType = Source.FileType;
            RefSystem = Source.RefSystem;
            RefUnits = Source.RefUnits;
            PosnError = Source.PosnError;
            Resolution = Source.Resolution;
            ValueUnits = Source.ValueUnits;
            ValueError = Source.ValueError;
            FlagValue = Source.FlagValue;
            FlagDefn = Source.FlagDefn;
            PosnError = Source.PosnError;
            Resolution = Source.Resolution;
            NumCols = Source.NumCols;
            NumRows = Source.NumRows;
            LegendCats = Source.LegendCats;
            UnitDist = Source.UnitDist;
            MinX = Source.MinX;
            MinY = Source.MinY;
            MaxX = Source.MaxX;
            MaxY = Source.MaxY;
            MinValue = Source.MinValue;
            MaxValue = Source.MaxValue;
            DisplayMin = Source.DisplayMin;
            DisplayMax = Source.DisplayMax;
            CellRes = Source.CellRes;

            foreach (string Item in Source.Comments) Comments.Add(Item);
            foreach (string Item in Source.Lineage) Lineage.Add(Item);
            foreach (string Item in Source.Consistency) Consistency.Add(Item);
            foreach (string Item in Source.Completeness) Completeness.Add(Item);
            foreach (KeyValuePair Item in Source.LegendCodes) LegendCodes.Add(Item);
        }

        /// <summary>
        /// Create a blank header structure (has no specified information)
        /// </summary>
        public void BlankHeader()
        {
            Lineage = null;
            Comments = null;
            Completeness = null;
            Consistency = null;
            LegendCodes = null;

            Lineage = new List<string>();
            Comments = new List<string>();
            Completeness = new List<string>();
            Consistency = new List<string>();
            LegendCodes = new List<KeyValuePair>();

            FileFormat = "IDRISI Raster A.1";
            FileTitle = Const.STRUNSPEC;
            DataType = "Real";
            FileType = "Binary";
            RefSystem = Const.STRUNSPEC;
            RefUnits = "Meters";
            PosnError = Const.STRUNSPEC;
            Resolution = Const.STRUNSPEC;
            ValueUnits = Const.STRUNSPEC;
            ValueError = Const.STRUNSPEC;
            FlagValue = Const.STRUNSPEC;
            FlagDefn = Const.STRUNSPEC;
            PosnError = Const.STRUNSPEC;
            Resolution = Const.STRUNSPEC;
            NumCols = Const.INTUNSPECIFIED;
            NumRows = Const.INTUNSPECIFIED;
            LegendCats = 0;
            UnitDist = 1.0;
            MinX = Const.DBLUNSPECIFIED;
            MinY = Const.DBLUNSPECIFIED;
            MaxX = Const.DBLUNSPECIFIED;
            MaxY = Const.DBLUNSPECIFIED;
            MinValue = Const.DBLUNSPECIFIED;
            MaxValue = Const.DBLUNSPECIFIED;
            DisplayMin = Const.DBLUNSPECIFIED;
            DisplayMax = Const.DBLUNSPECIFIED;
            CellRes = Const.DBLUNSPECIFIED;
        }
    }

    public class IdrisiHeaderFile
    {
        /// <summary>
        /// Checks to see if a file path exists.  Fixes a file path if no .rdc extension
        /// </summary>
        /// <param name="FilePath">The complete path to the file</param>
        /// <param name="FileMustExist">true if checking for an existing file, false otherwise</param>
        public static void CheckFilePath(ref string FilePath, bool FileMustExist)
        {
            FilePath = FilePath.Trim().ToLower();
            string Extension = Path.GetExtension(FilePath);

            //If not a new file, and it has an extension
            if ((Extension != string.Empty) && (Extension != ".rdc")) throw new Exception("Idrisi header files must have an .RDC extension.");

            //If not a new file and has no extension
            if (Extension == string.Empty) FilePath = FilePath + ".rdc";

            //Check to make sure the path part is correct
            string Dir = Path.GetDirectoryName(FilePath);
            string FileName = Path.GetFileName(FilePath);
            if (!Directory.Exists(Dir)) throw new Exception("The directory " + Dir + "\\ for Idrisi header file " + FileName + " does not exist.");

            //See if it is there
            if ((FileMustExist) && ((!File.Exists(FilePath)))) throw new Exception("Idrisi header file " + FilePath + " cannot be found.");
        }

        /// <summary>
        /// Helper function to get header items from data dictionary containing key/value pairs in idrisi header
        /// </summary>
        /// <param name="Dict">The data dictionary containing the key/value pairs</param>
        /// <param name="DataType">A constant indicating what the data type is, i.e. string, integer, or double</param>
        /// <param name="InKey">The key whose value we seek</param>
        /// <param name="OutValueStr">Returns the string value if data type is string</param>
        /// <param name="OutValueInt">Returns the integer value if data type is integer</param>
        /// <param name="OutValueDbl">Returns the double value if the data type is double</param>
        private static void Find(Dictionary<string, string> Dict, int DataType, string InKey, out string OutValueStr, out int OutValueInt, out double OutValueDbl)
        {
            OutValueStr = Const.STRUNSPEC;
            OutValueInt = Const.INTUNSPECIFIED;
            OutValueDbl = Const.DBLUNSPECIFIED;
            string FoundStr = null;
            Dict.TryGetValue(InKey, out FoundStr);
            if (FoundStr == null) return;
            switch (DataType)
            {
                case 0: //String
                    OutValueStr = FoundStr.Trim();
                    break;
                case 1: //Double
                    OutValueDbl = double.Parse(FoundStr);
                    break;
                case 2: //Integer
                    OutValueInt = int.Parse(FoundStr);
                    break;
            }
        }

        /// <summary>
        /// Read the Idrisi header file indicated by the FileName attribute
        /// </summary>
        /// <param name="FilePath">The full path to the file</param>
        public static void Read(string FilePath, ref IdrisiHeader Header)
        {
            Header.BlankHeader();
            CheckFilePath(ref FilePath, true);
            const int STR = 0;
            const int DBL = 1;
            const int INT = 2;
            string CodeStr = null;
            string KeyTag = null;
            string KeyValue = null;
            Dictionary<string, string> DataDict = null;
            DataDict = new Dictionary<string, string>();
            StreamReader Rdr = new StreamReader(FilePath);
            while (!Rdr.EndOfStream)
            {
                string OneLine = Rdr.ReadLine();
                KeyTag = OneLine.Substring(0, 12);
                KeyTag = KeyTag.Trim().ToLower();
                KeyValue = OneLine.Substring(13);
                KeyValue = KeyValue.Trim();
                if (KeyValue.Length == 0) KeyValue = "Unspecified";
                if (KeyTag.StartsWith("code") == true)
                {
                    CodeStr = KeyTag;
                    KeyTag = "code";
                }

                switch (KeyTag)
                {
                    case "lineage":
                        Header.Lineage.Add(KeyValue);
                        break;
                    case "comment":
                        Header.Comments.Add(KeyValue);
                        break;
                    case "completeness":
                        Header.Completeness.Add(KeyValue);
                        break;
                    case "consistency":
                        Header.Consistency.Add(KeyValue);
                        break;
                    case "code":
                        char[] Params = new char[1] { ' ' };
                        string[] Pieces = CodeStr.Split(Params, StringSplitOptions.RemoveEmptyEntries);
                        if (Pieces.Length != 2) throw new Exception("Could not parse code line: " + CodeStr);
                        Pieces[1].Trim();
                        int CodeNumber;
                        bool ParseResult = int.TryParse(Pieces[1], out CodeNumber);
                        if (ParseResult == false) throw new Exception("Could not parse code line: " + CodeStr);
                        IdrisiHeader.KeyValuePair NewData = new IdrisiHeader.KeyValuePair();
                        NewData.Key = CodeNumber;
                        NewData.Value = KeyValue;
                        Header.LegendCodes.Add(NewData);
                        break;
                    default:
                        DataDict.Add(KeyTag, KeyValue);
                        break;
                }
            }
            Rdr.Close();
            Rdr = null;

            //Now get the pieces we need for simplified use of the header
            string StrDummy;
            int IntDummy;
            double DblDummy;
            Find(DataDict, STR, "file format", out Header.FileFormat, out IntDummy, out DblDummy);
            Find(DataDict, STR, "file title", out Header.FileTitle, out IntDummy, out DblDummy);
            Find(DataDict, STR, "data type", out Header.DataType, out IntDummy, out DblDummy);
            Find(DataDict, STR, "file type", out Header.FileType, out IntDummy, out DblDummy);
            Find(DataDict, STR, "ref. system", out Header.RefSystem, out IntDummy, out DblDummy);
            Find(DataDict, STR, "ref. units", out Header.RefUnits, out IntDummy, out DblDummy);
            Find(DataDict, STR, "pos'n error", out Header.PosnError, out IntDummy, out DblDummy);
            Find(DataDict, STR, "resolution", out Header.Resolution, out IntDummy, out DblDummy);
            Find(DataDict, STR, "value units", out Header.ValueUnits, out IntDummy, out DblDummy);
            Find(DataDict, STR, "value error", out Header.ValueError, out IntDummy, out DblDummy);
            Find(DataDict, STR, "flag value", out Header.FlagValue, out IntDummy, out DblDummy);
            Find(DataDict, STR, "flag def'n", out Header.FlagDefn, out IntDummy, out DblDummy);
            Find(DataDict, INT, "columns", out StrDummy, out Header.NumCols, out DblDummy);
            Find(DataDict, INT, "rows", out StrDummy, out Header.NumRows, out DblDummy);
            Find(DataDict, INT, "legend cats", out StrDummy, out Header.LegendCats, out DblDummy);
            Find(DataDict, DBL, "min. x", out StrDummy, out IntDummy, out Header.MinX);
            Find(DataDict, DBL, "min. y", out StrDummy, out IntDummy, out Header.MinY);
            Find(DataDict, DBL, "max. x", out StrDummy, out IntDummy, out Header.MaxX);
            Find(DataDict, DBL, "max. y", out StrDummy, out IntDummy, out Header.MaxY);
            Find(DataDict, DBL, "unit dist.", out StrDummy, out IntDummy, out Header.UnitDist);
            Find(DataDict, DBL, "min. value", out StrDummy, out IntDummy, out Header.MinValue);
            Find(DataDict, DBL, "max. value", out StrDummy, out IntDummy, out Header.MaxValue);
            Find(DataDict, DBL, "display min", out StrDummy, out IntDummy, out Header.DisplayMin);
            Find(DataDict, DBL, "display max", out StrDummy, out IntDummy, out Header.DisplayMax);
            Header.CellRes = (Header.MaxX - Header.MinX) / (Header.NumCols);
            if (Header.DataType == "real") Header.DataType = "float";
        }

        /// <summary>
        /// A helper delegate for sorting the list of category codes (i.e. code /value pairs)
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns></returns>
        private static int CompareKeyValuePairs(IdrisiHeader.KeyValuePair P1, IdrisiHeader.KeyValuePair P2)
        {
            if (P1.Key > P2.Key)
                return 1;
            else if (P2.Key > P1.Key)
                return -1;
            else return 0;
        }

        /// <summary>
        /// A helper function for Write method.  Actually writes the header record to the header file
        /// </summary>
        /// <param name="S">The streamwriter pointing to the header file</param>
        /// <param name="Key">The left-hand side of the colon in the header file</param>
        /// <param name="Value">The right-hand side of the colon in the header file</param>
        private static void W(StreamWriter S, string Key, object Value)
        {
            const int KEYLEN = 12;
            int NumSpacesNeeded = KEYLEN - Key.Length;
            string OutLine = String.Format("{0,-12}: {1}", Key, Value);
            S.WriteLine(OutLine);
        }

        /// <summary>
        /// Write the header information to a header file
        /// </summary>
        /// <param name="FilePath">The full path to the header file</param>
        public static void Write(string FilePath, IdrisiHeader Header)
        {
            CheckFilePath(ref FilePath, false);
            StreamWriter S = new StreamWriter(FilePath);
            if (Header.DataType == "float") Header.DataType = "real";
            W(S, "file format", Header.FileFormat);
            W(S, "file title", Header.FileTitle);
            W(S, "data type", Header.DataType);
            W(S, "file type", Header.FileType);
            W(S, "columns", Header.NumCols);
            W(S, "rows", Header.NumRows);
            W(S, "ref. system", Header.RefSystem);
            W(S, "ref. units", Header.RefUnits);
            W(S, "unit dist.", Header.UnitDist);
            W(S, "min. X", Header.MinX);
            W(S, "max. X", Header.MaxX);
            W(S, "min. Y", Header.MinY);
            W(S, "max. Y", Header.MaxY);
            W(S, "pos'n error", Header.PosnError);
            W(S, "resolution", Header.Resolution);
            W(S, "min. value", Header.MinValue);
            W(S, "max. value", Header.MaxValue);
            W(S, "display min", Header.DisplayMin);
            W(S, "display max", Header.DisplayMax);
            W(S, "value units", Header.ValueUnits);
            W(S, "value error", Header.ValueError);
            W(S, "flag value", Header.FlagValue);
            W(S, "flag def'n", Header.FlagDefn);
            W(S, "legend cats", Header.LegendCats);

            Header.LegendCodes.Sort(CompareKeyValuePairs);
            foreach (IdrisiHeader.KeyValuePair Item in Header.LegendCodes)
            {
                string CatStr = Item.Key.ToString();
                CatStr = String.Format("{0,7}", Item.Key);
                W(S, "code" + CatStr, Item.Value);
            }

            foreach (string Item in Header.Lineage) W(S, "lineage", Item);
            foreach (string Item in Header.Completeness) W(S, "completeness", Item);
            foreach (string Item in Header.Consistency) W(S, "consistency", Item);
            foreach (string Item in Header.Comments) W(S, "comment", Item);
            S.Flush();
            S.Close();
            S = null;
        }


        /// <summary>
        /// Copy header info from an existing header
        /// </summary>
        /// <param name="Source">The Idrisi header from which to copy information</param>
        public static void Copy(string SourcePath, string TargetPath)
        {
            IdrisiHeader Header = new IdrisiHeader();
            IdrisiHeaderFile.Read(SourcePath, ref Header);
            Write(TargetPath, Header);
        }




    }
}
