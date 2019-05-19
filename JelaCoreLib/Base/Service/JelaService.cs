using JelaCoreLib.Base.Data.Interface;
using JelaCoreLib.Model.Interface;
using JelaCoreLib.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace JelaCoreLib.Base.Service
{
    /// <summary>
    /// Base for service layer implementation, in the special way by Jela.
    /// </summary>
    public abstract class JelaService<TContext, T, TKey>
        where TContext : IJelaDbContext
        where T : class, IJelaModel<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Context that is used to handle data.
        /// </summary>
        protected readonly TContext _context;

        /// <summary>
        /// Dictionary that is populated by validation.
        /// </summary>
        public IValidationDictionary _validatonDictionary;

        /// <summary>
        /// Logger for the events.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Currently active DbSet that is used for the queries.
        /// </summary>
        private DbSet<T> _activeDbSet;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JelaService(IValidationDictionary validatonDictionary, TContext context, ILogger logger)
        {
            _validatonDictionary = validatonDictionary;
            _context = context;
            _logger = logger;

            //Attempt to set the active dbset.
            if (!SetDbSet())
            {
                _logger.LogError("Unable to set active DbSet for service.");
            }
        }

        /// <summary>
        /// Read list of objects of type T.
        /// </summary>
        /// <returns>List of Objects of type T.</returns>
        public async Task<List<T>> ReadAll()
        {
            CheckPermission();

            return await ReadQuery()
                .ToListAsync();
        }

        /// <summary>
        /// Read list of objects of type T.
        /// </summary>
        /// <param name="projection">Projection that is used to select subset of data.</param>
        /// <returns>List of Objects of type T. Subset of selected data is specified by projection.</returns>
        public async Task<List<TRet>> ReadAll<TRet>(Expression<Func<T, TRet>> projection)
            where TRet : class
        {
            CheckPermission();

            var retList = ReadQuery()
                .Select(projection)
                .ToList();
            return await Task.FromResult(retList);
        }

        /// <summary>
        /// Read list of objects of type T.
        /// </summary>
        /// <param name="projection">Projection that is used to select subset of data.</param>
        /// <returns>List of Objects of type T. Subset of selected data is specified by projection.</returns>
        public async Task<List<object>> ReadAll(Expression<Func<T, object>> projection)
        {
            return await ReadAll<object>(projection);
        }

        /// <summary>
        /// Read object of type T.
        /// </summary>
        /// <param name="Id">Id of the object that is selected.</param>
        /// <returns>Object of type T.</returns>
        public async Task<T> Read(TKey Id)
        {
            return await ReadQuery(Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Read object of type T.
        /// </summary>
        /// <param name="Id">Id of the object that is selected.</param>
        /// <param name="projection">Projection that is used to select subset of data.</param>
        /// <returns>Object of type T with data specified by projection.</returns>
        public async Task<TRet> Read<TRet>(TKey Id, Expression<Func<T, TRet>> projection)
            where TRet : class
        {
            var retObj = ReadQuery(Id)
                .Select(projection)
                .FirstOrDefault();
            return await Task.FromResult(retObj);
        }

        /// <summary>
        /// Read object of type T.
        /// </summary>
        /// <param name="Id">Id of the object that is selected.</param>
        /// <param name="projection">Projection that is used to select subset of data.</param>
        /// <returns>Object of type T with data specified by projection.</returns>
        public async Task<object> Read(TKey Id, Expression<Func<T, object>> projection)
        {
            return await Read<object>(Id, projection);
        }
        
        /// <summary>
        /// Create new instance of an object of type T with the given data.
        /// </summary>
        /// <typeparam name="TDto">DTO model class.</typeparam>
        /// <param name="dtoData">Data that will be used for creation of new instance.</param>
        /// <returns>True if adding was successful, false otherwise.</returns>
        public async Task<bool> Create<TDto>(TDto dtoData)
            where TDto : class
        {
            //Start by checking if we have the required permission.
            CheckPermission();

            //Parse the details from the DTO data.
            T added = ParseFromDto(dtoData);

            //Force ID to be default on create so it won't clash.
            added.ID = default(TKey);

            //Set static data changes
            added = StandardizeData(added);

            //Validate the received data.
            if (!Validate(added))
                return false;

            try
            {
                _activeDbSet.Add(added);
                _context.Entry(added).State = EntityState.Added;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(String.Format("Unable to create new instance of {0}. Message: {1}", typeof(T).Name, e.Message));
                return false;
            }
        }

        /// <summary>
        /// Update existing instance of an object of type T with the given data.
        /// </summary>
        /// <typeparam name="TDto">DTO model class.</typeparam>
        /// <param name="dtoData">Data that will be used for creation of new instance.</param>
        /// <returns>True if adding was successful, false otherwise.</returns>
        public async Task<bool> Update<TDto>(TDto dtoData)
            where TDto : class
        {
            CheckPermission();

            //Parse the details from the DTO data.
            T added = ParseFromDto(dtoData);

            //Set static data changes
            added = StandardizeData(added);

            if (!Validate(added))
                return false;

            try
            {
                _context.Add(added);
                _context.Entry(added).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(String.Format("Unable to modify existing instance of {0}. Message: {1}", typeof(T).Name, e.Message));
                return false;
            }
        }

        /// <summary>
        /// Delete instance of an object of type T.
        /// </summary>
        /// <param name="Id">Id of the object that is deleted.</param>
        /// <returns>True if delete was successful, false otherwise.</returns>
        public async Task<bool> Delete(TKey Id)
        {
            CheckPermission();

            try
            {
                //Check if the specified object exists.
                var element = await ReadQuery(Id).FirstOrDefaultAsync();
                //If object wasn't found, we don't delete anything.
                if (element == null)
                    return false;

                //Remove the object.
                _activeDbSet.Remove(element);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                _logger.LogWarning(String.Format("Unable to delete instance of {0}. Message: {1}", typeof(T).Name, e.Message));
                return false;
            }
        }

        /// <summary>
        /// Provide the query for read operation.
        /// </summary>
        /// <returns>IQueryable for the read operation.</returns>
        protected virtual IQueryable<T> ReadQuery()
        {
            CheckPermission();

            var ret = QueryExtras(_activeDbSet);
            return ret.AsQueryable();
        }

        /// <summary>
        /// Provide the query for read operation.
        /// </summary>
        /// <param name="Id">Id of the object that is selected.</param>
        /// <returns>IQueryable for the read operation.</returns>
        protected virtual IQueryable<T> ReadQuery(TKey Id)
        {
            CheckPermission();

            var ret = QueryExtras(_activeDbSet);

            return ret.Where(e => e.ID.Equals(Id)).AsQueryable<T>();
        }

        /// <summary>
        /// Adding static additional parts into queries.
        /// </summary>
        /// <param name="source">The source IQueryable that the queries are done to.</param>
        /// <returns>Original IQueryable after the extras.</returns>
        protected virtual IQueryable<T> QueryExtras(IQueryable<T> source)
        {
            return source;
        }

        /// <summary>
        /// Parse contents from DTO into the object of type T.
        /// </summary>
        /// <typeparam name="TDto">DTO model class.</typeparam>
        /// <param name="dtoData">Data that will be used for parsing the data for object of type T.</param>
        /// <returns>Object of type T with data parsed from dtoData.</returns>
        protected virtual T ParseFromDto<TDto>(TDto dtoData)
            where TDto : class
        {
            return default(T);
        }

        /// <summary>
        /// Allows standardizing data.
        /// </summary>
        /// <param name="data">The original data that will be standardized.</param>
        /// <returns>standardized data.</returns>
        protected virtual T StandardizeData(T data)
        {
            return data;
        }

        /// <summary>
        /// Set the active DbSet.
        /// </summary>
        /// <returns>True if DbSet was successfully set, false otherwise.</returns>
        public virtual bool SetDbSet()
        {
            try
            {
                _activeDbSet = _context.Set<T>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <param name="data">The data that will be validated.</param>
        protected virtual bool Validate(T data)
        {
            //Base validate.
            if (!ValidateModel(data, out List<ValidationResult> validationResults))
            {
                foreach (var vRes in validationResults)
                {
                    //_validatonDictionary.AddError(item.);
                }
                // results will contain all the failed validation errors.
            }

            return _validatonDictionary.IsValid;
        }

        /// <summary>
        /// Base model validation.
        /// </summary>
        /// <typeparam name="TMod">Type of the model that will be validated.</typeparam>
        /// <param name="data">The data that will be validated.</param>
        /// <param name="results">List that will hold the results of validation.</param>
        /// <param name="contextItems">Key/Value pairs that are passed to validation.</param>
        /// <returns>True if data was valid, false otherwise.</returns>
        public bool ValidateModel<TMod>(TMod data, out List<ValidationResult> results, Dictionary<object, object> contextItems = null)
            where TMod : class
        {
            results = new List<ValidationResult>();
            var validationContext = new ValidationContext(data, null, contextItems);

            return Validator.TryValidateObject(data, validationContext, results);
        }

        /// <summary>
        /// Check permission to perform wanted action.
        /// </summary>
        public virtual void CheckPermission()
        {
            //
        }
    }
}
