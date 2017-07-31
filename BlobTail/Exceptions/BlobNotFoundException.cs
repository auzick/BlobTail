using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobTail.Exceptions
{
    public class BlobNotFoundException : Exception
    {
        public BlobNotFoundException()
        {
        }

        public BlobNotFoundException(string message)
            : base(message)
        {
        }

        public BlobNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
