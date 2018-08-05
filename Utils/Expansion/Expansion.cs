using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Utils.Expansion
{
    public static class Expansion
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> obj, Int32 pageIndex, Int32 pageSize)
        {
            return obj.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Pagination<T>(this DbSet<T> obj, Int32 pageIndex, Int32 pageSize) where T : class
        {
            return obj.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
    }
}
