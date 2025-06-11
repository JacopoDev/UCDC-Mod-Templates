using System;
using System.Text;

namespace GoogleCloudVoiceMod.Utility
{
    public class ByteReader
    {
        private readonly byte[] buffer;
        private int position;

        public ByteReader(byte[] data)
        {
            buffer = data;
            position = 0;
        }

        public int Position => position;

        public string ReadString(int length)
        {
            var str = Encoding.ASCII.GetString(buffer, position, length);
            position += length;
            return str;
        }

        public byte ReadByte()
        {
            return buffer[position++];
        }

        public short ReadInt16()
        {
            short value = (short)(buffer[position] | (buffer[position + 1] << 8));
            position += 2;
            return value;
        }

        public int ReadInt32()
        {
            int value = buffer[position]
                        | (buffer[position + 1] << 8)
                        | (buffer[position + 2] << 16)
                        | (buffer[position + 3] << 24);
            position += 4;
            return value;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];
            Array.Copy(buffer, position, result, 0, count);
            position += count;
            return result;
        }

        public void Skip(int count)
        {
            position += count;
        }
    }
}