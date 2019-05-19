using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JelaCoreLib.Helper.Collection
{
    /// <summary>
    /// List helper for paginating results from EntityFramework.
    /// </summary>
    /// <typeparam name="T">Type of the stored data.</typeparam>
    public class PaginatedList<T> : List<T>
        where T : class
    {
        /// <summary>
        /// The currently selected page.
        /// </summary>
        public readonly int _currentPage;

        /// <summary>
        /// The amount of rows per page.
        /// </summary>
        public readonly int _pageSize;

        /// <summary>
        /// Amount of pages in the original dataset.
        /// </summary>
        public readonly int _pageCount;

        /// <summary>
        /// Amount of rows in the original dataset.
        /// </summary>
        public readonly int _rowCount;

        /// <summary>
        /// Create new instance of the paginated list.
        /// </summary>
        /// <param name="items">List of filtered items.</param>
        /// <param name="rowCount">Amount of rows in the original dataset.</param>
        /// <param name="currentPage">The currently selected page.</param>
        /// <param name="pageSize">The amount of rows per page.</param>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        public PaginatedList(List<T> items, int rowCount, int currentPage, int pageSize)
        {
            //Store variables.
            _rowCount = rowCount;
            _currentPage = Math.Max(currentPage, 1);     //Page cant be below 1.
            _pageSize = Math.Max(pageSize, 1);           //Page size cant be below 1.

            //Calculate page count.
            _pageCount = (int)Math.Ceiling(_rowCount / (double)_pageSize);
            //Add items to list.
            this.AddRange(items);
        }

        /// <summary>
        /// Create PaginatedList from IQueryable.
        /// </summary>
        /// <param name="source">The source where list is created from.</param>
        /// <param name="currentPage">The currently selected page.</param>
        /// <param name="pageSize">The amount of rows per page.</param>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        /// <returns>PaginatedList generated from source.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int currentPage, int pageSize = 50)
        {
            currentPage = Math.Max(currentPage, 1); //Current page can't be below 1.
            pageSize = Math.Max(pageSize, 1); //Page must contain atleast one row.

            var count = await source.CountAsync();
            var items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, currentPage, pageSize);
        }

        /// <summary>
        /// Tell if there is previous page or not.
        /// </summary>
        public bool HasPreviousPage
        {
            get
            {
                return (_currentPage > 1);
            }
        }

        /// <summary>
        /// Tell if there is next page or not.
        /// </summary>
        public bool HasNextPage
        {
            get
            {
                return (_currentPage < _pageCount);
            }
        }

        /// <summary>
        /// Tell number of the first row that belongs to current page.
        /// </summary>
        public int FirstRowOnPage
        {
            get
            {
                return Math.Min((_currentPage - 1) * _pageSize + 1, LastRowOnPage);
            }
        }

        /// <summary>
        /// Tell number of the last row that belongs to current page.
        /// </summary>
        public int LastRowOnPage
        {
            get
            {
                return Math.Min(_currentPage * _pageSize, _rowCount);
            }
        }
    }
}
