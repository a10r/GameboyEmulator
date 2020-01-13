using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GameboyEmulator.Core.Utils
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Sets a property of an object to some value. The property to set is selected by 
        /// a selection expression of the form "obj => obj.Property".
        /// </summary>
        public static void SetProperty<TObj, TProp>(TObj obj, TProp value, Expression<Func<TObj, TProp>> propertyExpression)
        {
            var expr = (MemberExpression) propertyExpression.Body;
            var prop = (PropertyInfo) expr.Member;
            prop.SetValue(obj, value, null);
        }
    }
}
