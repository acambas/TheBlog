using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
    [Serializable]
    public class ValueObjectIsInvalidException : Exception
    {
        public ValueObjectIsInvalidException() { }
        public ValueObjectIsInvalidException(string message) : base(message) { }
        public ValueObjectIsInvalidException(string message, Exception inner) : base(message, inner) { }
        protected ValueObjectIsInvalidException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
