using System;

namespace ChatLe.Hosting.FastCGI.Payloads
{
    public enum RecordType : byte
    {
        None,
        BeginRequest,
        AbortRequest,
        EndRequest,
        Params,
        StdInput,
        StdOutput,
        StdError,
        Data,
        GetValues,
        GetValuesResult,
        Unknown
    }

    public class Record
    {
        public byte Version { get; set; }
        public byte Type { get; set; }
        public ushort RequestId { get; set; }
        public ushort Length { get; set; }
        public byte Padding { get; set; }
        public int Offset { get; set; }

        public const int HeaderSize = 8;
        public const int MaxBodySize = 0xffff;
        //SuggestedBodySize+HeaderSize=0x10000 - good size for whole packet
        public const int SuggestedBodySize = 0xfff8;
        public const int ProtocolVersion = 1;            
        
           
    }
}