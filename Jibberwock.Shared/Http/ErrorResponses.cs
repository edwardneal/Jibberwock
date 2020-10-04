using System;
using System.Collections.Generic;
using System.Text;

namespace Jibberwock.Shared.Http
{
    /// <summary>
    /// Contains all of the possible error responses from an API call.
    /// </summary>
    public static class ErrorResponses
    {
        /// <summary>
        /// Unable to locate a body for this API call.
        /// </summary>
        public const string MissingBody = "missing_body";
        /// <summary>
        /// EasyAuth redirection type is missing.
        /// </summary>
        public const string MissingRedirectionType = "missing_type";
        /// <summary>
        /// EasyAuth redirection type is invalid.
        /// </summary>
        public const string InvalidRedirectionType = "invalid_type";
        /// <summary>
        /// The filter provided is missing or invalid.
        /// </summary>
        public const string InvalidFilter = "invalid_filter";
        /// <summary>
        /// The ID provided is missing or invalid.
        /// </summary>
        public const string InvalidId = "invalid_id";
        /// <summary>
        /// The product characteristic is associated with a tier.
        /// </summary>
        public const string AssociatedWithTier = "associated_with_tier";
        /// <summary>
        /// The product characteristic is associated with a product.
        /// </summary>
        public const string AssociatedWithProduct = "associated_with_product";
        /// <summary>
        /// The product characteristic being referenced does not exist (or is not enabled.)
        /// </summary>
        public const string InvalidCharacteristic = "invalid_characteristic";
        /// <summary>
        /// The product characteristic value being specified is not valid according to the characteristic's data type.
        /// </summary>
        public const string InvalidCharacteristicValue = "invalid_characteristic_value";
        /// <summary>
        /// There is a server-side misconfiguration: the Application Insights AppID has not been specified.
        /// </summary>
        public const string MisconfiguredApplicationInsightsId = "invalid_appinsights_appid";
        /// <summary>
        /// There is a server-side misconfiguration: the Application Insights Tenant ID has not been specified.
        /// </summary>
        public const string MisconfiguredApplicationInsightsTenant = "invalid_appinsights_tenantid";
    }
}
