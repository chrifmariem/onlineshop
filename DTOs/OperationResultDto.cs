namespace onlineShop.DTOs
{
    public class OperationResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public object Data { get; set; }

        public static OperationResultDto SuccessResult(string message = "Opération réussie", object data = null)
        {
            return new OperationResultDto
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResultDto ErrorResult(string message, List<string> errors = null)
        {
            return new OperationResultDto
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}