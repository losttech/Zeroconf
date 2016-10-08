using System;
using System.Diagnostics;
using System.IO;

namespace Heijden.DNS
{
	#region RFC info
	/*
	3.2. ResourceRecord definitions

	3.2.1. Format

	All RRs have the same top level format shown below:

										1  1  1  1  1  1
		  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                                               |
		/                                               /
		/                      NAME                     /
		|                                               |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                      TYPE                     |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                     CLASS                     |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                      TTL                      |
		|                                               |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                   RDLENGTH                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
		/                     RDATA                     /
		/                                               /
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+


	where:

	NAME            an owner name, i.e., the name of the node to which this
					resource record pertains.

	TYPE            two octets containing one of the ResourceRecord TYPE codes.

	CLASS           two octets containing one of the ResourceRecord CLASS codes.

	TTL             a 32 bit signed integer that specifies the time interval
					that the resource record may be cached before the source
					of the information should again be consulted.  Zero
					values are interpreted to mean that the ResourceRecord can only be
					used for the transaction in progress, and should not be
					cached.  For example, SOA records are always distributed
					with a zero TTL to prohibit caching.  Zero values can
					also be used for extremely volatile data.

	RDLENGTH        an unsigned 16 bit integer that specifies the length in
					octets of the RDATA field.

	RDATA           a variable length string of octets that describes the
					resource.  The format of this information varies
					according to the TYPE and CLASS of the resource record.
	*/
	#endregion

	/// <summary>
	/// Resource Record (rfc1034 3.6.)
	/// </summary>
    [DebuggerDisplay("Name = {NAME} TTL={TTL} Class={Class} Type = {Type} Record={RECORD}")]
	public class ResourceRecord
	{
		/// <summary>
		/// The name of the node to which this resource record pertains
		/// </summary>
		public string NAME;

		/// <summary>
		/// Specifies type of resource record
		/// </summary>
		public Type Type;

		/// <summary>
		/// Specifies type class of resource record, mostly Internet but can be CS, Chaos or Hesiod 
		/// </summary>
		public Class Class;

		/// <summary>
		/// Time to live in seconds, the time interval that the resource record may be cached
		/// </summary>
		public uint TTL
		{
			get
			{
				return (uint)Math.Max(0, m_TTL - TimeLived);
			}
			set
			{
				m_TTL = value;
			}
		}

	    uint m_TTL;

		/// <summary>
		/// One of the Record* classes
		/// </summary>
		public Record RECORD { get; set; }

		public int TimeLived;

        public ResourceRecord() { }

		public ResourceRecord(RecordReader rr)
		{
			TimeLived = 0;
			NAME = rr.ReadDomainName();
			Type = (Type)rr.ReadUInt16();
			Class = (Class)rr.ReadUInt16();
			TTL = rr.ReadUInt32();
		    var length = rr.ReadUInt16();
			RECORD = rr.ReadRecord(Type, length);
		}

        public void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write(Question.WriteName(this.NAME));
            writer.Write(Question.WriteShort((ushort)this.Type));
            writer.Write(Question.WriteShort((ushort)this.Class));
            NetworkWrite(writer, BitConverter.GetBytes(TTL));
            NetworkWrite(writer, BitConverter.GetBytes(this.RECORD.Length));
            this.RECORD.Write(writer);
        }

        internal static void NetworkWrite(BinaryWriter writer, byte[] bytes)
        {
            if (BitConverter.IsLittleEndian) {
                bytes = new byte[bytes.Length];
                Array.Reverse(bytes);
            }
            writer.Write(bytes);
        }

		public override string ToString()
		{
			return $"{NAME,-32} {TTL}\t{Class}\t{Type}\t{RECORD}";
		}
	}

    public sealed class AnswerResourceRecord : ResourceRecord
	{
        public AnswerResourceRecord() { }
		public AnswerResourceRecord(RecordReader br)
			: base(br)
		{
		}
	}

    class AuthorityResourceRecord : ResourceRecord
	{
		public AuthorityResourceRecord(RecordReader br)
			: base(br)
		{
		}
	}

    class AdditionalResourceRecord : ResourceRecord
	{
		public AdditionalResourceRecord(RecordReader br)
			: base(br)
		{
		}
	}
}
