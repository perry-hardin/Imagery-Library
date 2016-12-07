using System;
using System.IO;

namespace Carto
{ 
    public class IdrisiGrid
    {
        public DataArray Cell;
        public int NumCols { get; protected set; }
        public int NumRows { get; protected set; }
        public int Length { get { return NumCells; } }
        public double CellRes { get; protected set; }
        public double MinX { get; protected set; }
        public double MaxY { get; protected set; }
        public int NumCells { get { return NumCols * NumRows; } }

        public IdrisiGrid()
        {
            Cell = null;
            NumCols = Const.INTUNSPECIFIED;
            NumRows = Const.INTUNSPECIFIED;
            CellRes = Const.INTUNSPECIFIED;
            MinX = Const.DBLUNSPECIFIED;
            MaxY = Const.DBLUNSPECIFIED;
        }

        public void CloneFr(IdrisiGrid Source)
        {
            Init(Source.NumRows, Source.NumCols, Source.Cell.DataType, Source.CellRes, Source.MinX, Source.MaxY);
            for (int i = 0; i < Source.NumCells; i++)
            {
                double Value = Source.Cell.Get(i);
                this.Cell.Set(i, Value);
            }
        }

        public void Init(int NumRows, int NumCols, string DataType, double CellRes, double MinX, double MaxY)
        {
            Cell = null;
            this.NumRows = NumRows;
            this.NumCols = NumCols;
            this.CellRes = CellRes;
            this.MinX = MinX;
            this.MaxY = MaxY;
            if (DataType == "real") DataType = "float";
            switch (DataType)
            {
                case "integer":
                    Cell = new ShortDataArray(NumCells);
                    break;
                case "byte":
                    Cell = new ByteDataArray(NumCells);
                    break;
                case "float":
                    Cell = new FloatDataArray(NumCells);
                    break;
                default:
                    throw new Exception("Idrisi raster grid must contain byte, integer, or real (single precision) data.");
            }
        }

        public void ConvertType(string NewType)
        {
            if (!IdrisiOps.DataTypeLegal(NewType)) throw new Exception("Data type requested is not legal internal representation: " + NewType);
            DataArray Old = this.Cell;
            Init(NumRows, NumCols, NewType, CellRes, MinX, MaxY);
            for (int i = 0; i < Old.Length; i++)
            {
                double Value = Old.Get(i);
                this.Cell.Set(i, Value);
            }
            Old = null;
        }


        public bool GoodRC(int Row, int Col)
        {
            if ((Row < 0) || (Row > NumRows - 1)) return false;
            if ((Col < 0) || (Col > NumCols - 1)) return false;
            return true;
        }


        public void Set(int Index, double Value)
        {
            Cell.Set(Index, Value);
        }

        public void Set(int Row, int Col, double Value) 
        {
            if ((Row < 0) || (Row > NumRows - 1)) throw new Exception("Row index for SetX is out of range: " + Row.ToString());
            if ((Col < 0) || (Col > NumCols - 1)) throw new Exception("Column index for SetX is out of range: " + Col.ToString());
            Cell.Set(Row * NumCols + Col, Value);
        }

        public void RC2EN(double Row, double Col, out double Easting, out double Northing) 
        {
            Easting = Col * CellRes + MinX;
            Northing = MaxY - Row * CellRes;
        }

        public void EN2RC(double Easting, double Northing, out int Row, out int Col)
        {
            double East = Easting - MinX;
            double North = MaxY - Northing;
            East = East / CellRes;
            North = North / CellRes;
            Col = (int)(East);
            Row = (int)(North);
        }

        public void Set(double Easting, double Northing, double Value)
        {
            int Row, Col;
            EN2RC(Easting, Northing, out Row, out Col);
            Set(Row, Col, Value);
        }

        public double Get(int Row, int Col)
        {
            if ((Row < 0) || (Row > NumRows - 1)) throw new Exception("Row index for SetX is out of range: " + Row.ToString());
            if ((Col < 0) || (Col > NumCols - 1)) throw new Exception("Column index for SetX is out of range: " + Col.ToString());
            return Cell.Get(Row * NumCols + Col);
        }

        public double Get(int Index)
        {
            return Cell.Get(Index);
        }

        public double GetEN(double Easting, double Northing)
        {
            int Row, Col;
            EN2RC(Easting, Northing, out Row, out Col);
            return Get(Row, Col);
        }


        ~IdrisiGrid() 
        {
           Cell = null;
        }
    }

    public class IdrisiGridFile 
    {
        /// <summary>
        /// Checks to see if a file path exists.  Fixes a file path if no .rst extension
        /// </summary>
        /// <param name="FilePath">The complete path to the file</param>
        /// <param name="FileMustExist">true if checking for an existing file, false otherwise</param>
        public static void CheckFilePath(ref string FilePath, bool FileMustExist)
        {
            FilePath = FilePath.Trim().ToLower();
            string Extension = Path.GetExtension(FilePath);

            //If not a new file, and it has an extension
            if ((Extension != string.Empty) && (Extension != ".rst")) throw new Exception("Idrisi grid files must have an .RST extension.");

            //If not a new file and has no extension
            if (Extension == string.Empty) FilePath = FilePath + ".rst";

            //Check to make sure the path part is correct
            string Dir = Path.GetDirectoryName(FilePath);
            string FileName = Path.GetFileName(FilePath);
            if (!Directory.Exists(Dir)) throw new Exception("The directory " + Dir + "\\ for Idrisi grid file " + FileName + " does not exist.");

            //See if it is there
            if ((FileMustExist) && ((!File.Exists(FilePath)))) throw new Exception("Idrisi grid file " + FilePath + " cannot be found.");
        }

        public static void Read(string FullPath, ref IdrisiGrid Grid)
        {
            CheckFilePath(ref FullPath, true);
            Grid.Cell.ReadAll(FullPath);
        }

        public static void Write(string FullPath, IdrisiGrid Grid)
        {
            CheckFilePath(ref FullPath, false);
            Grid.Cell.WriteAll(FullPath);
        }
        
    }

   


}
