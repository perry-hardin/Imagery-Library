using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    public class IdrisiImage
    {
        public IdrisiGrid Grid;
        public IdrisiHeader Header;

        public void Init(string Title, int NumRows, int NumCols, string DataType, double CellRes, double MinX, double MaxY)
        {
            Grid.Init(NumRows, NumCols, DataType, CellRes, MinX, MaxY);
            Header.Init(Title, NumRows, NumCols, DataType, CellRes, MinX, MaxY);
        }
        
        public IdrisiImage() 
        {
            Grid = new IdrisiGrid();
            Header = new IdrisiHeader();
        }

        public void CloneFr(IdrisiImage Source)
        {
            Grid.CloneFr(Source.Grid);
            Header.CloneFr(Source.Header);
        }

        public void ConvertType(string NewType)
        {
            NewType = NewType.Trim().ToLower();
            if (!IdrisiOps.DataTypeLegal(NewType)) throw new Exception("Data type requested is not legal internal representation: " + NewType);
            Header.DataType = NewType;
            Grid.ConvertType(NewType);
        }

        ~IdrisiImage()
        {
            Grid = null;
            Header = null;
        }

       
    }

    public class IdrisiImageFile
    {
        
        public static void Read(string FullPath, ref IdrisiImage Image)
        {
            string HeaderPath = FullPath;
            IdrisiHeaderFile.Read(HeaderPath, ref Image.Header);

            string GridPath = FullPath;
            Image.Grid.Init(Image.Header.NumRows, Image.Header.NumCols, Image.Header.DataType,
                            Image.Header.CellRes, Image.Header.MinX, Image.Header.MaxY);
            IdrisiGridFile.Read(GridPath, ref Image.Grid);
        }

        public static void Write(string FullPath, IdrisiImage Image)
        {
            IdrisiOps.UpdateMinMax(ref Image);
            string HeaderPath = FullPath;
            IdrisiHeaderFile.Write(HeaderPath, Image.Header);
            string GridPath = FullPath;
            IdrisiGridFile.Write(GridPath, Image.Grid);
        }

    }
}
