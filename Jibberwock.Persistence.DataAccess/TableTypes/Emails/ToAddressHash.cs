using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Emails
{
    internal class ToAddressHash : UserDefinedTableType
    {
        public byte[] Salt { get; set; }

        public string Hash { get; set; }

        public ToAddressHash(byte[] salt, string hash)
            : base(GetColumnMetadata<byte[]>("Salt"),
                  GetColumnMetadata<string>("Hash"))
        {
            base.SetValue<byte[]>(0, salt);
            base.SetValue<string>(1, hash);

            Salt = salt;
            Hash = hash;
        }
    }
}
