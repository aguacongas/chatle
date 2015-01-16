using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ChatLe.Hosting.FastCGI
{
    public static class NameValuePairsSerializer
    {
        public static IEnumerable<KeyValuePair<string, string>> Parse(byte[] body)
        {
            var dataLength = body.Length;
            var offset = 0;
            var encoding = Encoding.UTF8;

            while (offset < dataLength)
            {
                var nlen = ParseLen(body, ref offset);
                var vlen = ParseLen(body, ref offset);

                if (offset + nlen + vlen > dataLength)
                    throw new InvalidOperationException("Cannot parse name value pairs");

                var name = encoding.GetString(body, offset, nlen);
                offset += nlen;
                var value = encoding.GetString(body, offset, vlen);
                offset += vlen;

                yield return new KeyValuePair<string, string>(name, value);
            }
        }

        private static int ParseLen(byte[] body, ref int offset)
        {
            int len = body[offset++];
            if (len >= 0x80)
                len = ((0x7F & len) * 0x1000000)
                + ((int)body[offset++]) * 0x10000
                + ((int)body[offset++]) * 0x100
                + ((int)body[offset++]);

            return len;
        }

        public static void Write(Stream stream, string key, string value)
        {
            var encoding = Encoding.UTF8;
            var bkey = encoding.GetBytes(key);
            var bvalue = encoding.GetBytes(value);
            stream.WriteByte((byte)bkey.Length);
            stream.WriteByte((byte)bvalue.Length);
            stream.Write(bkey, 0, bkey.Length);
            stream.Write(bvalue, 0, bvalue.Length);
        }
    }
}