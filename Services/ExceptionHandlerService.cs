namespace FcmsPortalUI.Services
{
    public class ExceptionHandlerService
    {
        private readonly ILogger<ExceptionHandlerService> _logger;

        public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
        {
            _logger = logger;
        }

        public void HandleException(Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An unhandled exception occurred");

            // You could add additional logic here, like sending to a monitoring service
        }

        public string GetUserFriendlyMessage(Exception ex)
        {
            // Convert technical exceptions to user-friendly messages
            return ex switch
            {
                ArgumentException => "Invalid input. Please check your data and try again.",
                InvalidOperationException => "This operation cannot be performed right now.",
                // Add more exception types as needed
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }
}
