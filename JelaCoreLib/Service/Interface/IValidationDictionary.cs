using System;
using System.Collections.Generic;
using System.Text;

namespace JelaCoreLib.Service.Interface
{
    /// <summary>
    /// Interface for validation dictionary.
    /// </summary>
    public interface IValidationDictionary
    {
        /// <summary>
        /// Add errorMessage to list of errors that is associated with specified key.
        /// </summary>
        /// <param name="key">The key that specifies given errorMessage</param>
        /// <param name="errorMessage">The error message that is added.</param>
        void AddError(string key, string errorMessage);

        /// <summary>
        /// Gets the value that indicates whether any values in the dictionary is invalid or not validated.
        /// </summary>
        bool IsValid { get; }
    }
}
