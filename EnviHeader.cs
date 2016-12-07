using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Carto
{
    /*  Header description from http://geol.hu/data/online_help/ENVI_Header_Format.html
    description - a character string describing the image or processing performed.
    samples - number of samples (pixels) per image line for each band.

    lines - number of lines per image for each band.

    bands - number of bands per image file.

    header offset - refers to the number of bytes of imbedded header information present in the file (for example 128 bytes for ERDAS 7.5 .lan files). These bytes are skipped when the ENVI file is read.

    file type - refers to specific ENVI defined file types such as certain data formats and processing results. The available file types are listed in the filetype.txt file described below. The file type ASCII string must   
    match an entry in the filetype.txt file verbatim, including case.

    data type - parameter identifying the type of data representation, where 
    *  1=8 bit byte; 
    *  2=16-bit signed integer; 
    *  3=32-bit signed uint integer; 
    *  4=32-bit floating point; 
    *  5=64-bit double precision floating point; 
    *  6=2x32-bit complex, real-imaginary pair of double precision; 
    *  9=2x64-bit double precision complex, real-imaginary pair of double precision; 
    * 12=16-bit unsigned integer; 
    * 13=32-bit unsigned uint integer; 
    * 14=64-bit signed uint integer;           
    * 15=64-bit unsigned uint integer.

    interleave - refers to whether the data are band sequential (BSQ), band interleaved by pixel (BIP), or band interleaved by line (BIL).

    sensor type - refers to specific instruments such as Landsat TM, SPOT, RadarSat, etc. The available sensor types are listed in the sensor.txt file described below. The sensor type ASCII string defined here must match one 
    of the entries in the sensor.txt file verbatim., including case.

    byte order -describes the order of the bytes in integer, uint integer, 64-bit integer, unsigned 64-bit integer, floating point, double precision, and complex data types; Byte order=0 is Least Significant Byte First (LSF) 
    data (DEC and MS-DOS systems) and byte order=1 is Most Significant Byte First (MSF) data (all others - SUN, SGI, IBM, HP, DG).

    x start and y start - parameters define the image coordinates for the upper-left hand pixel in the image. The values in the header file are specified in "file coordinates," which is a zero-based number.

    map info - lists geographic coordinates information in the order of projection name (UTM), reference pixel x location in file coordinates, pixel y, pixel easting, pixel northing, x pixel size, y pixel size, Projection 
    Zone, North or South for UTM only. 
    */

    /* Binary data  http://geol.hu/data/online_help/ENVI_File_Formats.html
    ENVI uses a generalized raster data format consisting of a simple flat binary file and a small associated ASCII (text) header file. This approach permits ENVI's flexible use of nearly any image format, including those with embedded header information. All data types are supported in their native formats (byte, signed and unsigned integer, uint integer, floating point, double precision, 64-bit integer, unsigned 64-bit integer, complex, or double complex). The generalized raster data is stored as a binary stream of bytes either in BSQ, BIP, or BIL formats (see ENVI File Formats).

    BSQ (Band Sequential Format)

    In its simplest form, the data is in BSQ format, with each line of the data followed immediately by the next line in the same spectral band. This format is optimal for spatial (X, Y) access of any part of a single spectral band.

    BIP (Band Interleaved by Pixel Format)

    Images stored in BIP format have the first pixel for all bands in sequential order, followed by the second pixel for all bands, followed by the third pixel for all bands, etc., interleaved up to the number of pixels. This format provides optimum performance for spectral (Z) access of the image data.

    BIL (Band Interleaved by Line Format)

    Images stored in BIL format have the first line of the first band followed by the first line of the second band, followed by the first line of the third band, interleaved up to the number of bands. Subsequent lines for each band are interleaved in similar fashion. This format provides a compromise in performance between spatial and spectral processing and is the recommended file format for most ENVI processing tasks.
    */

    public class EnviHeader
    {
        public string FileType = Const.STRUNKNOWN;
        public string Description = Const.STRUNKNOWN;
        public string SensorType = Const.STRUNKNOWN;
        public string WavelengthUnits = Const.STRUNKNOWN;
        public int NumRows = Const.INTUNSPEC;
        public int NumCols = Const.INTUNSPEC;
        public int NumBands = Const.INTUNSPEC;
        public int HeaderOffset = Const.INTUNSPEC;
        public int XStart = Const.INTUNSPEC;
        public int YStart = Const.INTUNSPEC;
        public TypeCode DataType = TypeCode.Empty;
        public ByteOrder DataByteOrder = ByteOrder.UNKNOWN;
        public BandInterleave Interleave = BandInterleave.UNKNOWN;
        public List <string> BandNames = null;
        public List <double> Wavelength = null;
        public List <double> FWHM = null;
        public List<string> CoordSysInfo = null;
        public List<string> MapInfo = null;

        public static void WriteHeader(string BasePathWithExt, uint NumRows, uint NumCols, uint NumBands, ValueType DataType, string FileFormat)
        {
            //throw new Exception("This was commented out and I never checked it.");
            //string FullPath = BasePathWithExt;
            //StreamWriter Wtr = new StreamWriter(FullPath);
            //Wtr.WriteLine("ENVI");
            //Wtr.WriteLine("lines = " + NumRows.ToString());
            //Wtr.WriteLine("samples = " + NumCols.ToString());
            //Wtr.WriteLine("data type = " + ((int)DataType).ToString());
            //Wtr.WriteLine("interleave = " + FileFormat);
            //Wtr.WriteLine("bands = " + NumBands.ToString());
            //Wtr.Flush();
            //Wtr.Close();
            //Wtr.Dispose();
            //Wtr = null;
        }

        //Required by an earlier version of Envi Image
        //public static void Read(string BasePath, out int NumBands, out long NumRows, out long NumCols, out int DataType, out string FileFormat)
        //{
        //    string FullPath = BasePath + ".hdr";
        //    EnviHeader T = new EnviHeader();
        //    T.Read(FullPath);
        //    NumBands = T.NumBands;
        //    NumRows = T.NumRows;
        //    NumCols = T.NumCols;
        //    DataType = T.IntDataType;
        //    FileFormat = T.FileFormat;
        //}

        public void Read(string BasePath)
        {
            //char [] Delimiters = {':',','};
            string FullPath = BasePath + ".hdr";
            StreamReader Rdr = new StreamReader(FullPath,Encoding.UTF8);

            //The first line should read ENVI
            string OneLine = Rdr.ReadLine().Trim().ToUpper();
            if (OneLine != "ENVI")
                throw new Exception("First line of ENVI header file should read ENVI: " + FullPath);

            while (!Rdr.EndOfStream)
            {
                OneLine = Rdr.ReadLine();
                if (string.IsNullOrEmpty(OneLine)) continue;
                string[] Piece = OneLine.Split('=');
                string Key = Piece[0].Trim();
                string Value = Piece[1].Trim();
                switch (Key.ToUpper())
                {
                    case "DESCRIPTION":
                        char[] D1 = { '}', '{' };
                        Description = null;
                        do
                        {
                            OneLine = Rdr.ReadLine();
                            Piece = OneLine.Split(D1, StringSplitOptions.RemoveEmptyEntries);
                            Description = Description + " " + Piece[0].Trim();
                        } while (!OneLine.EndsWith("}"));
                        break;
                    case "SAMPLES":
                        NumCols = Convert.ToInt32(Value);
                        break;
                    case "LINES":
                        NumRows = Convert.ToInt32(Value);
                        break;
                    case "BANDS":
                        NumBands = Convert.ToInt32(Value);
                        break;
                    case "HEADER OFFSET":
                        HeaderOffset = Convert.ToInt32(Value);
                        break;
                    case "FILE TYPE":
                        FileType = Value;
                        break;
                    case "DATA TYPE":
                        int IntDataType = Convert.ToInt32(Value);
                        switch (IntDataType)
                        {
                            case 1: DataType = TypeCode.Byte; break;
                            case 2: DataType = TypeCode.Int16; break;
                            case 3: DataType = TypeCode.Int32; break;
                            case 4: DataType = TypeCode.Single; break;
                            case 5: DataType = TypeCode.Double; break;
                            case 12: DataType = TypeCode.UInt16; break;
                            case 13: DataType = TypeCode.UInt32; break;
                            case 14: DataType = TypeCode.Int64; break;
                            case 15: DataType = TypeCode.UInt64; break;
                            default: throw new Exception("EnviHeader.Read reports that DataType code is unrecognized.");
                        }
                        break;
                    case "INTERLEAVE":
                        Value = Value.ToUpper();
                        switch (Value)
                        {
                            case "BSQ": Interleave = BandInterleave.BS; break;
                            case "BIP": Interleave = BandInterleave.BIP; break;
                            case "BIL": Interleave = BandInterleave.BIL; break;
                            default: throw new Exception("EnviHeader.Read reports that Interleave code is unrecognized."); 
                        }
                        break;
                    case "BYTE ORDER":
                        int iByteOrder = Convert.ToInt32(Value);
                        switch (iByteOrder)
                        {
                            case 0:DataByteOrder = ByteOrder.LSB_FIRST; break;
                            case 1: DataByteOrder = ByteOrder.MSB_FIRST; break;
                            default: throw new Exception("EnviHeader.Read reports that ByteOrder code is unrecognized."); 
                        }
                        break;
                    case "SENSOR TYPE":
                        SensorType = Value;
                        break;
                    case "BAND NAMES" :
                        char[] D2 = new char[] {','};
                        BandNames = new List<string>();
                        do
                        {
                            OneLine = Rdr.ReadLine().Trim();
                            BandNames.Add(OneLine.Substring(0, OneLine.Length - 1));
                        } while (!OneLine.EndsWith("}"));
                        break;
                    case "WAVELENGTH" :
                        char[] D3 = new char[] { '{', '}', ',', ' ' };
                        Wavelength = new List<double>();
                        OneLine = Rdr.ReadLine().Trim();
                        Piece = OneLine.Split(D3, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Val in Piece) Wavelength.Add(Convert.ToDouble(Val.Trim()));
                        break;
                    case "WAVELENGTH UNITS":
                        WavelengthUnits = Value.Trim();
                        break;
                    case "FWHM":
                        char[] D4 = new char [] { '{', '}', ',', ' ' };
                        FWHM = new List<double>();
                        OneLine = Rdr.ReadLine().Trim();
                        Piece = OneLine.Split(D4, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Val in Piece) FWHM.Add(Convert.ToDouble(Val.Trim()));
                        break;
                    case "X START":
                        XStart = Convert.ToInt32(Value);
                        break;
                    case "Y START":
                        YStart = Convert.ToInt32(Value);
                        break;
                    case "MAP INFO":
                        char[] D5 = new char[] { '{', '}', ',', ' ' };
                        MapInfo = new List<string>();
                        Piece = OneLine.Split(D5, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 3; i < Piece.Length; i++)
                            MapInfo.Add(Piece[i].Trim());
                        string MapUnits = MapInfo[MapInfo.Count - 1];
                        MapInfo.RemoveAt(MapInfo.Count - 1);
                        Piece = MapUnits.Split('=');
                        MapInfo.Add(Piece[1].Trim());
                        break;
                    case "COORDINATE SYSTEM STRING":
                        char[] D6 = new char[] { '{', '}', ',', ' ', '\"', '[', ']' };
                        CoordSysInfo = new List<string>();
                        //OneLine = Rdr.ReadLine().Trim();
                        Piece = Value.Split(D6, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string Val in Piece) CoordSysInfo.Add(Val.Trim());
                        break;
                    default: Console.WriteLine("Envi header had an unrecognized key = " + Key); break;
                }
            }

            //Clean up the file
            Rdr.Close();
            Rdr.Dispose();
            Rdr = null;
        }
    }


    public class EnviImage
    {
        public EnviHeader Header = null;
        private IdrisiHeader IdrHeader = null;
        private List<IdrisiGrid> IdrStack = null;
        public int NumRows { get { return Header.NumRows; } }
        public int NumCols { get { return Header.NumCols; } }
        public int NumBands { get { return Header.NumBands; } }
        public int NumElements { get { return Header.NumRows * Header.NumCols * Header.NumBands; } }

        public double this[int BandIndex, int RowIndex, int ColIndex]
        {
            get { return IdrStack[BandIndex].Get(RowIndex, ColIndex); }
            set { IdrStack[BandIndex].Set(RowIndex, ColIndex, value); }
        }

        ~EnviImage()
        {
            IdrHeader = null;
            Header = null;
            IdrStack = null;
        }

        private void Setup()
        {
            X = new DataCube(Header.DataType, Geometry.ArrayBase.ZERO, Header.NumRows - 1, Header.NumCols - 1, Header.NumBands);
        }

        public EnviImage(string BasePath, string BinaryExt, string HeaderExt)
        {
            Header = new EnviHeader();
            Header.Read(BasePath + HeaderExt);
            Setup();
            ReadBinary(BasePath + BinaryExt);
        }

        public void ReadBinary(string BasePathWithExt)
        {
            X.Allocate();
            X.ReadFile(BasePathWithExt, Header.Interleave, Header.ByteOrder);
        }







        //public void Write(string BasePath, string BinaryExtension, string HeaderExtension)
        //{
        //    EnviImage.WriteHeader(BasePath + HeaderExtension, NumRows, NumCols, NumBands, DataType, FileFormat);
        //    X.WriteFile(BasePath + BinaryExtension);
        //}

    }

}