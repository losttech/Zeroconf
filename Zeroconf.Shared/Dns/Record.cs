// Stuff records are made of
namespace Heijden.DNS
{
    using System;
    using System.IO;
    public abstract class Record
	{
		/// <summary>
		/// The Resource Record this RDATA record belongs to
		/// </summary>
		public RR RR;

        public virtual void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
	}
}
