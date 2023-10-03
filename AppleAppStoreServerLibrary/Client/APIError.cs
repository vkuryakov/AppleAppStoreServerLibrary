﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppleAppStoreServerLibrary.Client
{
    public enum APIError
    {
        GENERAL_BAD_REQUEST = 4000000,
        INVALID_APP_IDENTIFIER = 4000002,
        INVALID_REQUEST_REVISION = 4000005,
        INVALID_TRANSACTION_ID = 4000006,
        INVALID_ORIGINAL_TRANSACTION_ID = 4000008,
        INVALID_EXTEND_BY_DAYS = 4000009,
        INVALID_EXTEND_REASON_CODE = 4000010,
        INVALID_IDENTIFIER = 4000011,
        START_DATE_TOO_FAR_IN_PAST = 4000012,
        START_DATE_AFTER_END_DATE = 4000013,
        INVALID_PAGINATION_TOKEN = 4000014,
        INVALID_START_DATE = 4000015,
        INVALID_END_DATE = 4000016,
        PAGINATION_TOKEN_EXPIRED = 4000017,
        INVALID_NOTIFICATION_TYPE = 4000018,
        MULTIPLE_FILTERS_SUPPLIED = 4000019,
        INVALID_TEST_NOTIFICATION_TOKEN = 4000020,
        INVALID_SORT = 4000021,
        INVALID_PRODUCT_TYPE = 4000022,
        INVALID_PRODUCT_ID = 4000023,
        INVALID_SUBSCRIPTION_GROUP_IDENTIFIER = 4000024,
        INVALID_EXCLUDE_REVOKED = 4000025,
        INVALID_IN_APP_OWNERSHIP_TYPE = 4000026,
        INVALID_EMPTY_STOREFRONT_COUNTRY_CODE_LIST = 4000027,
        INVALID_STOREFRONT_COUNTRY_CODE = 4000028,
        INVALID_REVOKED = 4000030,
        INVALID_STATUS = 4000031,
        SUBSCRIPTION_EXTENSION_INELIGIBLE = 4030004,
        SUBSCRIPTION_MAX_EXTENSION = 4030005,
        FAMILY_SHARED_SUBSCRIPTION_EXTENSION_INELIGIBLE = 4030007,
        ACCOUNT_NOT_FOUND = 4040001,
        ACCOUNT_NOT_FOUND_RETRYABLE = 4040002,
        APP_NOT_FOUND = 4040003,
        APP_NOT_FOUND_RETRYABLE = 4040004,
        ORIGINAL_TRANSACTION_ID_NOT_FOUND = 4040005,
        ORIGINAL_TRANSACTION_ID_NOT_FOUND_RETRYABLE = 4040006,
        SERVER_NOTIFICATION_URL_NOT_FOUND = 4040007,
        TEST_NOTIFICATION_NOT_FOUND = 4040008,
        STATUS_REQUEST_NOT_FOUND = 4040009,
        TRANSACTION_ID_NOT_FOUND = 4040010,
        RATE_LIMIT_EXCEEDED = 4290000,
        GENERAL_INTERNAL = 5000000,
        GENERAL_INTERNAL_RETRYABLE = 5000001
    }
}
