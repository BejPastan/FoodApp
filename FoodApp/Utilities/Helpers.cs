public class Helpers
{
    
        /// <summary>
        /// Validates that required parameters are not null or empty
        /// </summary>
        /// <param name="parameters">Dictionary of parameter names and values</param>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static void ValidateRequiredParameters(Dictionary<string, object?> parameters)
        {
            var validationErrors = new Dictionary<string, string[]>();

            foreach (var parameter in parameters)
            {
                if (parameter.Value == null || 
                    (parameter.Value is string stringValue && string.IsNullOrWhiteSpace(stringValue)) ||
                    (parameter.Value is int intValue && intValue == 0))
                {
                    validationErrors[parameter.Key] = new[] { $"{parameter.Key} is required" };
                }
            }

            if (validationErrors.Count > 0)
            {
                throw new ArgumentException("Validation failed");
            }
        }
}