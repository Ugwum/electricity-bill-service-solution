namespace ElectricityBillService.Core.Models
{
    public class APIResult
    {
        public bool succeeded { get; private set; }
        public string message { get; private set; }
        public dynamic  data { get; private set; }

        // Private constructor to enforce the use of static factory methods
        private APIResult(bool isSuccess, object data = null, string message = null)
        {
            succeeded = isSuccess;
            this.data = data;
            this.message = message;
        }

        // Static factory method for a successful payment result
        public static APIResult Success(object data, string message) => new APIResult(true, data, message);

        // Static factory method for a failed payment result
        public static APIResult Failure(string errorMessage) => new APIResult(false, message: errorMessage);
    }

    public class GenericErrorResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }
}
