﻿using FluentValidation.Results;

namespace GameIt.Application.Exeptions;

public class BadRequestException : Exception
{
    public BadRequestException(string massage) : base(massage)
    {

    }

    public BadRequestException(string message, ValidationResult validationResult)
        : base(message)
    {
        ValidationErrors = validationResult.ToDictionary();
    }

    public IDictionary<string, string[]> ValidationErrors { get; set; }
}
