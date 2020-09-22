using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.DataModels.Tenants
{
    /// <summary>
    /// This class encapsulates the contact details used to communicate with a <see cref="Tenant"/>.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// The unique internal reference for this <see cref="Contact"/>.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the person in question.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// If applicable, the person's telephone number
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// If applicable, the person's email address
        /// </summary>
        public string EmailAddress { get; set; }
    }
}
