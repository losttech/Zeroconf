/*
 3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */

namespace Heijden.DNS
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    sealed class RecordA : Record
    {
        public string Address;

        public RecordA(string address)
        {
            this.Address = address;
        }

        public RecordA(RecordReader rr)
        {
            Address = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}",
                rr.ReadByte(),
                rr.ReadByte(),
                rr.ReadByte(),
                rr.ReadByte());
        }

        public override ushort Length => 4;

        public override string ToString()
        {
            return Address;
        }

        public override void Write(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            var address = this.Address.Split('.').Select(byte.Parse).ToArray();
            writer.Write(address);
        }
    }
}
