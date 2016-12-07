using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carto
{
    public class TwoVarDelegates
    {
        public delegate double TwoVarFormula(double CellValue, double Number);

        public static double DivideBy(double CellValue, double Divisor)
        {
            return CellValue / Divisor;
        }
    }


    public class OneVarDelegates
    {
        public delegate double OneVarFormula(double Number);
        

        public static double SetConstant(double TheConstant)
        {
            return TheConstant;
        }

        public static double KelvinToCelsius(double Kelvin)
        {
            return Kelvin - 273.0;
        }

        public static double KelvinToFahrenheit(double Kelvin)
        {
            double C = Kelvin - 273;
            return 1.8 * C + 32.0;
        }

        public static double CelsiusToFahrenheit(double Celsius)
        {
            return 1.8 * Celsius + 32.0;
        }
    }


    

    public class IdrisiOps
    {
        
        public static void Calculate(ref IdrisiImage Img, 
                                     OneVarDelegates.OneVarFormula Formula,
                                     double NoDataIndicator,
                                     double ValueToAssignWhenNoData) 
        {
            for (int i = 0; i < Img.Grid.Cell.Length; i++)
            {
                double Value = Img.Grid.Get(i);
                if (Value == NoDataIndicator)
                    Value = ValueToAssignWhenNoData;
                else
                     Value = Formula(Value);
                Img.Grid.Set(i, Value);
            }
        }

        public static void Calculate(ref IdrisiImage Img,
                                     TwoVarDelegates.TwoVarFormula Formula,
                                     double Number,
                                     double NoDataIndicator,
                                     double ValueToAssignWhenNoData)
        {
            for (int i = 0; i < Img.Grid.Cell.Length; i++)
            {
                double Value = Img.Grid.Get(i);
                if (Value == NoDataIndicator)
                    Value = ValueToAssignWhenNoData;
                else
                    Value = Formula(Value, Number);
                Img.Grid.Set(i, Value);
            }
        }




        public static void ChangeCellSize(IdrisiImage InImage, ref IdrisiImage OutImage, double MissingData)
        {
            if (InImage.Grid.Cell.DataType != OutImage.Grid.Cell.DataType)
                throw new Exception("Input and Output images must be same type");
            SetConstant(ref OutImage, 0.0);
            double Value;
            int InRow, InCol;
            for(int OutRow = 0; OutRow < OutImage.Grid.NumRows; OutRow++)
                for (int OutCol = 0; OutCol < OutImage.Grid.NumCols; OutCol++)
                {
                    double Easting, Northing;
                    OutImage.Grid.RC2EN(OutRow, OutCol, out Easting, out Northing);
                    InImage.Grid.EN2RC(Easting, Northing, out InRow, out InCol);
                    if (!InImage.Grid.GoodRC(InRow, InCol))
                        OutImage.Grid.Set(OutRow, OutCol, MissingData);
                    else
                    {
                        Value = InImage.Grid.Get(InRow, InCol);
                        OutImage.Grid.Set(OutRow, OutCol, Value);
                    }
                }
        }

        public static void GetRandomRowsCols(IdrisiImage Img, int NumWanted, out int[] RowIdx, out int[] ColIdx, double MissingValueFlag)
        {
            Random Gen = new Random();
            double Value = 0.0;
            int Row, Col;
            RowIdx = new int[NumWanted];
            ColIdx = new int[NumWanted];
            for (int i = 0; i < NumWanted; i++)
            {
                do
                {
                    Row = Gen.Next(0, Img.Header.NumRows);
                    Col = Gen.Next(0, Img.Header.NumCols);
                    Value = Img.Grid.Get(Row, Col);
                } while (Value == MissingValueFlag);
                RowIdx[i] = Row;
                ColIdx[i] = Col;
            }
        }



        //public static void DivideBy(ref IdrisiImage Img, double TheDivisor)
        //{
        //    for (int i = 0; i < Img.Grid.Cell.Length; i++)
        //    {
        //        double Value = Img.Grid.Get(i);
        //        Value /= TheDivisor;
        //        Img.Grid.Set(i, Value);
        //    }
        //}

        public static void SetConstant(ref IdrisiImage Img, double TheConstant) 
        {
            for (int i = 0; i < Img.Grid.Cell.Length; i++) Img.Grid.Set(i, TheConstant);
        }

        public static bool DataTypeLegal(string InType)
        {
            InType = InType.ToLower().Trim();
            switch (InType)
            {
                case "byte": return true;
                case "integer": return true;
                case "float": return true;
                default: return false;
            }
        }

        public static void UpdateMinMax(ref IdrisiImage Img)
        {
            double[] X = new double[Img.Grid.Cell.Length];
            TransferData(Img, double.NaN, out X);
            double Min = Descriptives.Minimum(X);
            double Max = Descriptives.Maximum(X);
            Img.Header.MinValue = Min;
            Img.Header.MaxValue = Max;
            Img.Header.DisplayMin = Min;
            Img.Header.DisplayMax = Max;
        }

        public static int CountGoodCells(IdrisiImage Img, double IgnoreValue)
        {
            int Count = 0;
            for (int i = 0; i < Img.Grid.Length; i++)
            {
                if (Img.Grid.Get(i) != IgnoreValue) Count++;
            }
            return Count;
        }

        public static int CountGoodCells(IdrisiImage Img1, double IgnoreValue1,
                                         IdrisiImage Img2, double IgnoreValue2)
        {
            if (Img1.Grid.NumCells != Img2.Grid.NumCells) throw new Exception("Count good cells requires the same number of cells for the two images.");
            int Count = 0;
            for (int i = 0; i < Img1.Grid.Length; i++)
            {
                if (Img1.Grid.Get(i) == IgnoreValue1) continue;
                if (Img2.Grid.Get(i) == IgnoreValue2) continue;
                Count++;
            }
            return Count;
        }

        public static void TransferData(IdrisiImage Img1, double IgnoreValue1,
                                        IdrisiImage Img2, double IgnoreValue2,
                                        out double[] X1,
                                        out double[] X2)
        {
            X1 = null;
            X2 = null;
            if (Img1.Grid.NumCells != Img2.Grid.NumCells) throw new Exception("Transfer data requires the same number of cells for the two images.");
            int Count = CountGoodCells(Img1, IgnoreValue1, Img2, IgnoreValue2);
            X1 = new double[Count];
            X2 = new double[Count];
            int Idx = -1;
            for (int i = 0; i < Img1.Grid.Length; i++)
            {
                double Value1 = Img1.Grid.Get(i);
                double Value2 = Img2.Grid.Get(i);
                if (Value1 == IgnoreValue1) continue;
                if (Value2 == IgnoreValue2) continue;
                Idx++;
                X1[Idx] = Value1;
                X2[Idx] = Value2;
            }
        }
        
        public static void TransferData(IdrisiImage Img, double IgnoreValue, out double[] X)
        {
            X = null;
            int Count = CountGoodCells(Img, IgnoreValue);
            X = new double[Count];
            int Idx = -1;
            for (int i = 0; i < Img.Grid.Length; i++)
            {
                double Value = Img.Grid.Get(i);
                if (Value == IgnoreValue) continue;
                Idx++;
                X[Idx] = Value;
            }
          }

        public static double Mean(IdrisiImage Img, double IgnoreValue)
        {
            double[] X;
            TransferData(Img, IgnoreValue, out X);
            double Mean = Descriptives.Average(X);
            X = null;
            return Mean;
        }

        public static void Regress(IdrisiImage X, double X_IgnoreValue, IdrisiImage Y, double Y_IgnoreValue, out double Slope, out double Intercept, out double R2)
        {
            double[] XX;
            double[] YY;
            IdrisiImage Img1 = X;
            IdrisiImage Img2 = Y;
            double IgnoreValue1 = X_IgnoreValue;
            double IgnoreValue2 = Y_IgnoreValue;
            TransferData(Img1, IgnoreValue1, Img2, IgnoreValue2, out XX, out YY);
            SimpleRegression.Calculate(XX, YY, out Intercept, out Slope, out R2);
        }


        public static double Pearsons(IdrisiImage X, double X_IgnoreValue, IdrisiImage Y, double Y_IgnoreValue)
        {
            double Intercept, Slope, R2;
            Regress(X, X_IgnoreValue, Y, Y_IgnoreValue, out Intercept, out Slope, out R2);
            return Math.Sqrt(R2);
        }


        public static double RectMean(IdrisiImage Img, double ULX, double ULY, double LRX, double LRY, double IgnoreValue)
        {
            int ULR, ULC;
            int LRR, LRC;
            int NumCols = Img.Header.NumCols;
            Img.Grid.EN2RC(ULX, ULY, out ULR, out ULC);
            Img.Grid.EN2RC(LRX, LRY, out LRR, out LRC);
            int Count = 0;
            double Sum = 0.0;
            for (int r = ULR; r <= LRR; r++)
                for (int c = ULC; c <= LRC; c++)
                {
                    double Val = Img.Grid.Cell.Get(r * NumCols + c);
                    if (Val == IgnoreValue) continue;
                    Count++;
                    Sum += Val;
                }
            if (Count == 0) return IgnoreValue;
            return Sum / Count;
        }


    }
}
