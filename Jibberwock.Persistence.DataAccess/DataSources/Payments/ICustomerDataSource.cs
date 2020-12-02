using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jibberwock.Persistence.DataAccess.DataSources.Payments
{
    /// <summary>
    /// Interface describing a payment provider's ability to create and update customer records.
    /// </summary>
    public interface ICustomerDataSource
    {
        /// <summary>
        /// Creates a named customer.
        /// </summary>
        /// <param name="name">The customer name.</param>
        /// <param name="emailAddress">The customer's billing email address.</param>
        /// <param name="phoneNumber">The customer's billing phone number.</param>
        /// <param name="metadata">Any additional metadata attached to the customer.</param>
        /// <returns>The ID of the created customer.</returns>
        Task<string> CreateCustomer(string name, string emailAddress, string phoneNumber, Dictionary<string, string> metadata);

        /// <summary>
        /// Updates a specified customer.
        /// </summary>
        /// <param name="id">The ID of the customer to update.</param>
        /// <param name="name">The customer name.</param>
        /// <param name="emailAddress">The customer's billing email address.</param>
        /// <param name="phoneNumber">The customer's billing phone number.</param>
        /// <param name="metadata">Any additional metadata attached to the customer.</param>
        Task UpdateCustomer(string id, string name, string emailAddress, string phoneNumber, Dictionary<string, string> metadata);
    }
}
