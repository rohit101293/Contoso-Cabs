using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContosoCabs.Utils
{
    public class ResponseCode
    {
        public const int SUCCESS = 0;
        public const int PARTIAL_SUCCESS = 1;
        public const int OLA_ERROR = 9;
        public const int UBER_ERROR = 8;
        public const int NULL_VALUES = 7;
        public const int NO_CABS_AVAILABALE = 3;
        public const int SESSION_EXPIRED = 2;
        public const int PERMISSIONS_REQUIRED = 4;
        public const int SECURITY_ERROR = 777;
        public const int DB_ERROR = 888;
        public const int MYSQL_DUPLICATES = 100;
        public const int MYSQL_NULL_VALUES = 101;
        public const int MYSQL_INVALID_QUERY_PARAMETERS = 104;
        public const int MYSQL_NO_SUCH_VALUE = 102;
        public const int MYSQL_FIELDS_MISMATCH = 103;
        public const int INTERNAL_SERVER_ERROR = 500;
    }
}
