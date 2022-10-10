using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }

        // true if the currentpage property is grater than 1
        public bool HasPrevious => CurrentPage > 1;

        // true if value of current page is less than the number of total pages
        public bool HasNext => CurrentPage < TotalPages;


        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // to add the items to the colletion
            AddRange(items);
        }

        
        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            // in this method, we count the elements inside the source collection
            var count = source.Count();

            // take the items by applying the skip 
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // return the result 
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

    }
}
