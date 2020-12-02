using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Payments
{
    /// <summary>
    /// Describes a payment provider capable of creating customers and payment sessions.
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// The <see cref="ICustomerFactory"/> instance which handles the creation of customer records.
        /// </summary>
        Jibberwock.Persistence.DataAccess.DataSources.Payments.ICustomerDataSource CustomerDataSource { get; }

        /// <summary>
        /// The <see cref="IPaymentSessionFactory"/> instance which handles the creation of customer records.
        /// </summary>
        IPaymentSessionFactory PaymentSessionFactory { get; }
    }
}
