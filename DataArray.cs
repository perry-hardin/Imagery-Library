using System;
using System.IO;

namespace Carto
{
    public abstract class DataArray
    {
        protected BinaryReader Reader;
        protected BinaryWriter Writer;
        public int Length;
        public string DataType;
        public double MinValue;
        public double MaxValue;

        public abstract void Set(int Index, double Value);
        public abstract double Get(int Index);
        public abstract void Read(int Index);
        public abstract void Write(int Index);

        public void CheckIndex(int Index)
        {
            if (Index < 0) throw new Exception("Index for DataArray is less than zero.");
            if (Index >= Length) throw new Exception("Index is out of range (too large) for DataArray.");
        }

        public void CheckValue(double Value)
        {
            if (Value < MinValue) throw new Exception("Value being assigned to a " + DataType + " grid is too small for range of " + DataType);
            if (Value > MaxValue) throw new Exception("Value being assigned to a " + DataType + " grid is too large for range of " + DataType);
        }

        public void WriteAll(string FullPath)
        {
            Writer = new BinaryWriter(File.Open(FullPath, FileMode.Create));
            for (int i = 0; i < Length; i++) Write(i);
            Writer.Close();
            Writer = null;
        }

        public void ReadAll(string FullPath)
        {
            if (!File.Exists(FullPath)) throw new Exception("Trying to read a DataArray file that does not exist:  " + FullPath);
            Reader = new BinaryReader(File.Open(FullPath, FileMode.Open));
            for (int i = 0; i < Length; i++) Read(i);
            Reader.Close();
            Reader = null;
        }
    }

    public class ShortDataArray : DataArray
    {
        public short[] Matrix;

        public ShortDataArray(int NumElements)
        {
            DataType = "short";
            MinValue = short.MinValue;
            MaxValue = short.MaxValue;
            Matrix = new short[NumElements];
            Length = NumElements;
        }

        ~ShortDataArray() { Matrix = null; }

        public override double Get(int Index)
        {
            CheckIndex(Index);
            return Matrix[Index];
        }

        public override void Set(int Index, double Value)
        {
            CheckIndex(Index);
            CheckValue(Value);
            Matrix[Index] = (short)Value;
        }

        public override void Read(int Index)
        {
            CheckIndex(Index);
            Matrix[Index] = Reader.ReadInt16();
        }

        public override void Write(int Index)
        {
            CheckIndex(Index);
            Writer.Write(Matrix[Index]);
        }
    }

    public class FloatDataArray : DataArray
    {
        public float[] Matrix;

        public FloatDataArray(int NumElements)
        {
            DataType = "float";
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
            Matrix = new float[NumElements];
            Length = NumElements;
        }

        ~FloatDataArray() { Matrix = null; }

        public override double Get(int Index)
        {
            CheckIndex(Index);
            return Matrix[Index];
        }

        public override void Set(int Index, double Value)
        {
            CheckIndex(Index);
            CheckValue(Value);
            Matrix[Index] = (float)Value;
        }

        public override void Read(int Index)
        {
            CheckIndex(Index);
            Matrix[Index] = Reader.ReadSingle();
        }

        public override void Write(int Index)
        {
            CheckIndex(Index);
            Writer.Write(Matrix[Index]);
        }
    }

    public class ByteDataArray : DataArray
    {
        public byte[] Matrix;

        public ByteDataArray(int NumElements)
        {
            DataType = "byte";
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
            Matrix = new byte[NumElements];
            Length = NumElements;
        }

        ~ByteDataArray() { Matrix = null; }

        public override double Get(int Index)
        {
            CheckIndex(Index);
            return Matrix[Index];
        }

        public override void Set(int Index, double Value)
        {
            CheckIndex(Index);
            CheckValue(Value);
            Matrix[Index] = (byte)Value;
        }

        public override void Read(int Index)
        {
            CheckIndex(Index);
            Matrix[Index] = Reader.ReadByte();
        }

        public override void Write(int Index)
        {
            CheckIndex(Index);
            Writer.Write(Matrix[Index]);
        }
    }

}
