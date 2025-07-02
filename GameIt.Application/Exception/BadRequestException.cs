using FluentValidation.Results;

namespace GameIt.Application.Exeptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string massage) : base(massage)
        {

        }

        public BadRequestException(string message, ValidationResult validationResult)
            : base(message)
        {
            ValidationErrors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();
        }

        public List<string> ValidationErrors { get; set; }
    }
}
