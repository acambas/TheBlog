using Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Image
{
    public class AppImage : EntityBase<string>
    {
        public byte[] ImageBinaryData { get; set; }

        public string FullName { get; set; }

        public override bool Validate()
        {
            return true;
        }
    }
}
