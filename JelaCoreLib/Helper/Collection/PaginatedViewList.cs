using Microsoft.AspNetCore.Routing;
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
    public class PaginatedViewList<T> : PaginatedList<T>
        where T : class
    {
        /// <summary>
        /// Amount of pages that should be displayed in pagination view. Prefer uneven numbers!
        /// </summary>
        public readonly int _visiblePages;

        /// <summary>
        /// Create new instance of the paginated list with view component.
        /// </summary>
        /// <param name="items">List of filtered items.</param>
        /// <param name="rowCount">Amount of rows in the original dataset.</param>
        /// <param name="currentPage">The currently selected page.</param>
        /// <param name="pageSize">The amount of rows per page.</param>
        /// <param name="visiblePages">Amount of pages that are visible at pagination view.</param>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        public PaginatedViewList(List<T> items, int rowCount, int currentPage, int pageSize, int visiblePages)
            :base(items,rowCount,currentPage,pageSize)
        {
            _visiblePages = Math.Max(visiblePages, 1);   //Display atleast 1 page.
        }

        /// <summary>
        /// Create PaginatedList from IQueryable.
        /// </summary>
        /// <param name="source">The source where list is created from.</param>
        /// <param name="currentPage">The currently selected page.</param>
        /// <param name="pageSize">The amount of rows per page.</param>
        /// <param name="visiblePages">Amount of pages that are visible at pagination view.</param>
        /// <exception cref="ArgumentNullException">Thrown if required parameter is null value.</exception>
        /// <returns>PaginatedViewList generated from source.</returns>
        public static async Task<PaginatedViewList<T>> CreateAsync(IQueryable<T> source, int currentPage, int pageSize = 50, int visiblePages = 5)
        {
            currentPage = Math.Max(currentPage, 1); //Current page can't be below 1.
            pageSize = Math.Max(pageSize, 1); //Page must contain atleast one row.

            var count = await source.CountAsync();
            var items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedViewList<T>(items, count, currentPage, pageSize, visiblePages);
        }

        /// <summary>
        /// Get the relevant information of paginated list for displaying the list.
        /// </summary>
        /// <returns>Model with the relevant information for displaying the PaginatedList.</returns>
        public PaginationViewModel GetPaginationViewModel(Object urlVars, string view = null, string ajaxDest = null)
        {
            var urlVarDict = new RouteValueDictionary(urlVars);
            var model = new PaginationViewModel
            {
                HasPreviousPage = this.HasPreviousPage,
                HasNextPage = this.HasNextPage,
                FirstRowOnPage = this.FirstRowOnPage,
                LastRowOnPage = this.LastRowOnPage,
                FirstVisiblePage = this.FirstVisiblePage,
                LastVisiblePage = this.LastVisiblePage,

                CurrentPage = _currentPage,
                RowCount = _rowCount,
                PageSize = _pageSize,
                VisiblePages = _visiblePages,
                PageCount = _pageCount,

                ViewName = view,
                AjaxDest = ajaxDest,

                UrlVars = urlVarDict
            };

            return model;
        }

        /// <summary>
        /// Tell the first visible page for pagination.
        /// </summary>
        public int FirstVisiblePage
        {
            get
            {
                //Go half of visible backwards
                int first = _currentPage - (int)(Math.Floor(_visiblePages / 2.0));
                //IF we're close to end we need to go back even more.
                first = Math.Min((_pageCount + 1 - _visiblePages), first);
                //And we need to make sure we dont go negative in the start.
                first = Math.Max(1, first);
                return first;
            }
        }

        /// <summary>
        /// Tell the last visible page for pagination.
        /// </summary>
        public int LastVisiblePage
        {
            get
            {
                //Go amount of wanted pages from start page, or until the end of amount of pages.
                return Math.Min(FirstVisiblePage + _visiblePages - 1, _pageCount);
            }
        }
    }
}
