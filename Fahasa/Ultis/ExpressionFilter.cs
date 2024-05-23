using Fahasa.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI.WebControls;

namespace Fahasa.Ultis
{
    public class ExpressionFilter
    {
        public static Expression<Func<T, bool>> Build<T>(ParameterExpression parameter, List<object> filter)
        {
            var propertyName = (string)filter[0];
            var operation = (string)filter[1];
            var value = filter[2];

            MemberExpression property = null;
            ConstantExpression constant = null;

            property = Expression.Property(parameter, propertyName);
            constant = Expression.Constant(value);

            Expression body = null;

            switch (operation)
            {
                case "=":
                    {
                        if (value is string)
                        {
                            var propertyToLower = Expression.Call(property, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            var constantToLower = Expression.Call(constant, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            body = Expression.Equal(propertyToLower, constantToLower);
                        }
                        else
                        {
                            body = Expression.Equal(property, constant);
                        }
                        break;
                    }
                case "contains":
                    {
                        if (value is string && property.Type == typeof(int))
                        {
                            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            var propertyToString = Expression.Call(property, typeof(int).GetMethod("ToString", System.Type.EmptyTypes));
                            var propertyToLower = Expression.Call(propertyToString, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            var constantToLower = Expression.Call(constant, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            body = Expression.Call(propertyToLower, method, constantToLower);
                        }
                        else {
                            var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            var propertyToLower = Expression.Call(property, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            var constantToLower = Expression.Call(constant, typeof(string).GetMethod("ToLower", System.Type.EmptyTypes));
                            body = Expression.Call(propertyToLower, method, constantToLower);
                        }
                        break;
                    }
                case ">=":
                    {
                        if (propertyName.ToLower().Contains("date") || value is DateTime)
                        {
                            if (value is string)
                            {
                                DateTime dateTime;
                                dateTime = DateTime.ParseExact((string)value, "MM/dd/yyyy HH:mm:sss", CultureInfo.InvariantCulture);
                                constant = Expression.Constant(dateTime, typeof(DateTime?));
                            }
                            else
                            {
                                constant = Expression.Constant(value, typeof(DateTime?));
                            }
                            body = Expression.GreaterThanOrEqual(property, constant);
                        }
                        else
                        {
                            body = Expression.GreaterThanOrEqual(property, constant);
                        }
                        break;
                    }
                case "<>":
                    {
                        body = Expression.NotEqual(property, constant);
                        break;
                    }
                case "<":
                    {
                        if (propertyName.ToLower().Contains("date") || value is DateTime)
                        {
                            if (value is string)
                            {
                                DateTime dateTime;
                                dateTime = DateTime.ParseExact((string)value, "MM/dd/yyyy HH:mm:sss", CultureInfo.InvariantCulture);
                                constant = Expression.Constant(dateTime, typeof(DateTime?));
                            }
                            else
                            {
                                constant = Expression.Constant(value, typeof(DateTime?));
                            }
                            body = Expression.LessThan(property, constant);
                        }
                        else
                        {
                            body = Expression.LessThan(property, constant);
                        }
                        break;
                    }

            }

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public static Expression<Func<T, bool>> createLamda<T>(ParameterExpression parameter, List<dynamic>filterObjects)
        {
            Expression<Func<T, bool>> expr = null;
            for (int i = 0; i < filterObjects.Count; i += 2)
            {
                var condition = ExpressionFilter.Build<Order>(parameter, filterObjects[i]);
                if (expr == null)
                    expr = condition;
                else if ((string)filterObjects[i - 1] == "or")
                    expr = System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(System.Linq.Expressions.Expression.Or(expr.Body, condition.Body), parameter);
                else
                    expr = System.Linq.Expressions.Expression.Lambda<Func<Order, bool>>(System.Linq.Expressions.Expression.And(expr.Body, condition.Body), parameter);
            }
            return expr;
        }
    }
}