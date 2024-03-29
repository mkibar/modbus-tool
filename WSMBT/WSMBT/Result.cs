﻿namespace WSMBT
{
    public enum Result
    {
        SUCCESS = 0,
        ILLEGAL_FUNCTION = 1,
        ILLEGAL_DATA_ADDRESS = 2,
        ILLEGAL_DATA_VALUE = 3,
        SLAVE_DEVICE_FAILURE = 4,
        ACKNOWLEDGE = 5,
        SLAVE_DEVICE_BUSY = 6,
        NEGATIVE_ACKNOWLEDGE = 7,
        MEMORY_PARITY_ERROR = 8,
        GATEWAY_PATH_UNAVAILABLE = 10, // 0x0000000A
        GATEWAY_DEVICE_FAILED = 11, // 0x0000000B
        CONNECT_ERROR = 200, // 0x000000C8
        CONNECT_TIMEOUT = 201, // 0x000000C9
        WRITE = 202, // 0x000000CA
        READ = 203, // 0x000000CB
        RESPONSE_TIMEOUT = 300, // 0x0000012C
        ISCLOSED = 301, // 0x0000012D
        CRC = 302, // 0x0000012E
        RESPONSE = 303, // 0x0000012F
        BYTECOUNT = 304, // 0x00000130
        QUANTITY = 305, // 0x00000131
        FUNCTION = 306, // 0x00000132
        TRANSACTIONID = 307, // 0x00000133
        DEMO_TIMEOUT = 1000, // 0x000003E8
    }

}