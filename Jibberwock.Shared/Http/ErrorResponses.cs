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
    }
}
