using Microsoft.AspNetCore.Routing;
using System;

namespace JelaCoreLib.Helper.Collection
{
    /// <summary>
    /// The viewmodel for pagination display relevant data.
    /// </summary>
    public class PaginationViewModel
    {
        /// <summary>
        /// Tell if there is previous page or not.
        /// </summary>
        public bool HasPreviousPage { get; set; }
        /// <summary>
        /// Tell if there is next page or not.
        /// </summary>
        public bool HasNextPage { get; set; }
        /// <summary>
        /// Tell number of the first row that belongs to current page.
        /// </summary>
        public int FirstRowOnPage { get; set; }
        /// <summary>
        /// Tell number of the last row that belongs to current page.
        /// </summary>
        public int LastRowOnPage { get; set; }
        /// <summary>
        /// Tell the first visible page for pagination.
        /// </summary>
        public int FirstVisiblePage { get; set; }
        /// <summary>
        /// Tell the last visible page for pagination.
        /// </summary>
        public int LastVisiblePage { get; set; }
        /// <summary>
        /// Number of the currently selected page.
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// Size of pages. The amount of rows per page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Amount of pages in the original dataset.
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Amount of rows in the original dataset.
        /// </summary>
        public int RowCount { get; set; }
        /// <summary>
        /// Amount of pages that should be displayed in pagination. Prefer uneven numbers
        /// </summary>
        public int VisiblePages { get; set; }
        
        /// <summary>
        /// The routevalues that are used to create links within pagination.
        /// </summary>
        public RouteValueDictionary UrlVars { get; set; }

        /// <summary>
        /// Name of the view that will be used on display.
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Destination element of Ajax queries on ajax
        /// </summary>
        public string AjaxDest { get; set; }

        /// <summary>
        /// Get the viewmodel info with page being set into the object.
        /// </summary>
        /// <param name="page">The page that should be presented in the UrlVars.</param>
        /// <returns></returns>
        public Object GetUrlVarsWithPage(int page)
        {
            UrlVars["Page"] = page;
            return this.UrlVars;
        }
    }
}
