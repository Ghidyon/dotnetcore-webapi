using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            // split query string to get individual fields
            var orderParams = orderByQueryString.Trim().Split(',');

            // using reflection to represent properties of our Employee class
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                // run through all parameters and check for their existence
                if (string.IsNullOrWhiteSpace(param))
                    continue;

                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                // when no property is found, skip to the next parameter in the list
                if (objectProperty is null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";

                // use stringbuilder to build our query with each loop
                orderQueryBuilder.Append($"{objectProperty.Name} {direction},");
            }

            return orderQueryBuilder.ToString().TrimEnd(',', ' ');
        }
    }
}
