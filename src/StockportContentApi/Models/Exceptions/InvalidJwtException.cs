﻿namespace StockportContentApi.Models.Exceptions;

public class InvalidJwtException : Exception
{
    public InvalidJwtException(string message) : base(message) { }
}
