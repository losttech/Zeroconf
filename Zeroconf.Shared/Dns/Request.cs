using System;
using System.Collections.Generic;
using System.Text;

namespace Heijden.DNS
{
    public class Request
	{
		public Header Header { get; }

        List<Question> questions;

		public Request()
		{
			Header = new Header();
			Header.OPCODE = OPCode.Query;
			Header.QDCOUNT = 0;

			questions = new List<Question>();
		}

		public void AddQuestion(Question question)
		{
			questions.Add(question);
		}

		public byte[] Data
		{
			get
			{
				List<byte> data = new List<byte>();
				Header.QDCOUNT = (ushort)questions.Count;
				data.AddRange(Header.Data);
				foreach (Question q in questions)
					data.AddRange(q.Data);
				return data.ToArray();
			}
		}
	}
}
