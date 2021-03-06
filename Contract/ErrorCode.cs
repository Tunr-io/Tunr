﻿// Copyright (c) Microsoft Corporation
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the ""License""); you may
// not use this file except in compliance with the License. You may obtain a
// copy of the License at http://www.apache.org/licenses/LICENSE-2.0 
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABLITY OR NON-INFRINGEMENT. 
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Net;

namespace Microsoft.Xbox.Music.Platform.Contract
{
    public enum ErrorCode
    {
        // ReSharper disable InconsistentNaming
        #region Catalog errors
        [Description("No response from Catalog")]
        [StatusCode(HttpStatusCode.BadGateway)]
        CATALOG_UNAVAILABLE,

        [Description("Item does not exist")]
        [StatusCode(HttpStatusCode.NotFound)]
        CATALOG_NO_RESULT,

        [Description("Error while reading catalog data, some results may be missing or incomplete")]
        // No status code for this one because none makes sense, and this error code shouldn't be used in an exception (non-blocking error)
        CATALOG_INVALID_DATA,
        #endregion

        #region Collection errors
        [Description("No result from Collection")]
        [StatusCode(HttpStatusCode.NotFound)]
        COLLECTION_NO_RESULT,

        [Description("Invalid response from Cloud Collection")]
        [StatusCode(HttpStatusCode.BadGateway)]
        COLLECTION_INVALID_RESPONSE,

        [Description("Error while reading collection data, some results may be missing or incomplete")]
        // No status code for this one because none makes sense, and this error code shouldn't be used in an exception (non-blocking error)
        COLLECTION_INVALID_DATA,

        [Description("Your collection is full")]
        [StatusCode(HttpStatusCode.BadRequest)]
        COLLECTION_FULL,

        [Description("This playlist is full")]
        [StatusCode(HttpStatusCode.BadRequest)]
        COLLECTION_PLAYLIST_FULL,

        [Description("This operation is not supported")]
        [StatusCode(HttpStatusCode.BadRequest)]
        COLLECTION_INVALID_OPERATION,

        [Description("Invalid collection id for this operation")]
        [StatusCode(HttpStatusCode.BadRequest)]
        COLLECTION_INVALID_ID,

        [Description("Required playlist information is missing")]
        [StatusCode(HttpStatusCode.BadRequest)]
        COLLECTION_INVALID_PLAYLIST_INFO,

        [Description("The operation failed")]
        [StatusCode(HttpStatusCode.BadGateway)]
        COLLECTION_OPERATION_UNKNOWN_ERROR,

        [Description("Some of the operations failed")]
        // No status code, it's a non-blocking error
        COLLECTION_SOME_OPERATIONS_FAILED,
        #endregion

        #region Discovery errors
        [Description("No result from Discovery")]
        [StatusCode(HttpStatusCode.NotFound)]
        DISCOVERY_NO_RESULT,

        [Description("No result from Discovery, you might want to check if genre is well formatted")]
        [StatusCode(HttpStatusCode.NotFound)]
        DISCOVERY_INVALID_GENRE,

        [Description("Invalid response from Discovery")]
        [StatusCode(HttpStatusCode.BadGateway)]
        DISCOVERY_INVALID_RESPONSE,

        [Description("Error while reading discovery data, some results may be missing or incomplete")]
        // No status code for this one because none makes sense, and this error code shouldn't be used in an exception (non-blocking error)
        DISCOVERY_INVALID_DATA,
        #endregion

        #region Delivery errors
        [Description("Invalid response from the Music Delivery Service")]
        [StatusCode(HttpStatusCode.BadGateway)]
        DELIVERY_INVALID_RESPONSE,

        [Description("Content is not available for streaming")]
        [StatusCode(HttpStatusCode.NotFound)]
        DELIVERY_UNAVAILABLE_CONTENT,

        [Description("The user is already streaming on another device")]
        [StatusCode(HttpStatusCode.Conflict)]
        DELIVERY_CONCURRENT_STREAMING,

        [Description("Unknown delivery error")]
        [StatusCode(HttpStatusCode.NotFound)]
        DELIVERY_ERROR,
        #endregion

        #region AccessToken errors
        [Description("Azure Marketplace access token required")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_MISSING,

        [Description("Invalid Azure Marketplace token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_INVALID,

        [Description("Expired Azure Marketplace token")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_EXPIRED,

        [Description("Unexpected error while validating Azure Marketplace token")]
        [StatusCode(HttpStatusCode.InternalServerError)]
        ACCESS_TOKEN_VALIDATION_ERROR,

        [Description("Azure Marketplace client id is not a subscriber to the Xbox Music data offer")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        ACCESS_TOKEN_INVALID_SUBSCRIPTION,

        [Description("Unexpected error while validating Azure Marketplace subscription status")]
        [StatusCode(HttpStatusCode.InternalServerError)]
        ACCESS_TOKEN_SUBSCRIPTION_VALIDATION_ERROR,
        #endregion

        #region ContinuationToken errors
        [Description("The continuation token provided is incorrect")]
        [StatusCode(HttpStatusCode.BadRequest)]
        CONTINUATION_TOKEN_INVALID_ERROR,
        #endregion

        #region General input validation errors
        [Description("Missing or invalid authorization header")]
        [StatusCode(HttpStatusCode.Unauthorized)]
        INVALID_AUTHORIZATION_HEADER,

        [Description("The user does not have an Xbox Music subscription")]
        [StatusCode(HttpStatusCode.Forbidden)]
        NO_MUSIC_PASS_SUBSCRIPTION,

        [Description("Missing or empty mandatory parameter")]
        [StatusCode(HttpStatusCode.BadRequest)]
        MISSING_INPUT_PARAMETER,

        [Description("Invalid parameter value")]
        [StatusCode(HttpStatusCode.BadRequest)]
        INVALID_INPUT_PARAMETER,

        [Description("Incompatible parameters")]
        [StatusCode(HttpStatusCode.BadRequest)]
        INCOMPATIBLE_INPUT_PARAMETERS,

        [Description("Unauthorized input parameter")]
        UNAUTHORIZED_INPUT_PARAMETER,

        [Description("Unauthorized API method")]
        [StatusCode(HttpStatusCode.Forbidden)]
        UNAUTHORIZED_API_METHOD,

        [Description("The requested functionality is not available in this region")]
        [StatusCode(HttpStatusCode.BadRequest)]
        INVALID_COUNTRY,      
        #endregion

        [Description("Oops, something went seriously wrong")]
        [StatusCode(HttpStatusCode.InternalServerError)]
        INTERNAL_SERVER_ERROR,

        [Description("Too Many Requests")]
        [StatusCode((HttpStatusCode)429)] // http://tools.ietf.org/html/rfc6585
        TOO_MANY_REQUESTS,
        // ReSharper restore InconsistentNaming
    }

    public class StatusCodeAttribute : Attribute
    {
        public HttpStatusCode StatusCode { get; private set; }

        public StatusCodeAttribute(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }

#if PORTABLE
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
#endif
}
