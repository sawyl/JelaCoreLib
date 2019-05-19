using JelaCoreLib.Service.Interface;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace JelaCoreLib.Service
{
    /// <summary>
    /// Wrap controllers' Modelstate into implementation of IValidationDictionary.
    /// </summary>
    public class MsValidationDictionary : IValidationDictionary
    {
        /// <summary>
        /// Handle for the modelstate dictionary.
        /// </summary>
        private ModelStateDictionary _modelState;

        /// <summary>
        /// ModelState as property.
        /// </summary>
        public ModelStateDictionary ModelState
        {
            get
            {
                return this._modelState;
            }
            set
            {
                this._modelState = value;
            }
        }

        /// <summary>
        /// Create new instance of the validation dictionary.
        /// </summary>
        /// <param name="modelState">The modelstate that is used by the dictionary.</param>
        public MsValidationDictionary(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        /// <summary>
        /// Add errorMessage to list of errors that is associated with specified key.
        /// </summary>
        /// <param name="key">The key that specifies given errorMessage</param>
        /// <param name="errorMessage">The error message that is added.</param>
        public void AddError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        /// <summary>
        /// Gets the value that indicates whether any values in the dictionary is invalid or not validated.
        /// </summary>
        public bool IsValid
        {
            get { return _modelState.IsValid; }
        }
    }
}
