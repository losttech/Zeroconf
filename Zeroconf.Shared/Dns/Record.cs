// Stuff records are made of
namespace Heijden.DNS
{
    using System;
    using System.IO;
    public abstract class Record
    {
        public virtual ushort Length { get { throw new NotImplementedException(); } }
        public virtual void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
