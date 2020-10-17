using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Persistence.DataAccess.TableTypes.Emails
{
    /// <summary>
    /// Describes an email record to create in the database.
    /// </summary>
    public class Email : UserDefinedTableType
    {
        /// <summary>
        /// The unique external identifier for this email. Must be unique within the email batch.
        /// </summary>
        public string ExternalEmailId { get; set; }

        /// <summary>
        /// The salt for the To: address.
        /// </summary>
        public byte[] ToAddressSalt { get; set; }

        /// <summary>
        /// The hashed To: address.
        /// </summary>
        public string ToAddressHash { get; set; }

        /// <summary>
        /// Creates an <see cref="Email"/> record based upon an external email.
        /// </summary>
        /// <param name="externalEmailId">The unique external identifier for this email.</param>
        /// <param name="toAddressSalt">The salt for the To: address.</param>
        /// <param name="toAddressHash">The hashed To: address.</param>
        public Email(string externalEmailId, byte[] toAddressSalt, string toAddressHash)
            : base(
                    GetColumnMetadata<string>("External_Email_ID"),
                    GetColumnMetadata<byte[]>("To_Address_Salt"),
                    GetColumnMetadata<string>("To_Address_Hash")
                  )
        {
            base.SetValue<string>(0, externalEmailId);
            base.SetValue<byte[]>(1, toAddressSalt);
            base.SetValue<string>(2, toAddressHash);

            ExternalEmailId = externalEmailId;
            ToAddressSalt = toAddressSalt;
            ToAddressHash = toAddressHash;
        }
    }
}
