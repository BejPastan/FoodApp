using Dapper;
using System.Data;

namespace FoodApp.Utilities
{
    public class SQLDateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value)
        {
            return value switch
            {
                DateTime dateTime => DateOnly.FromDateTime(dateTime),
                DateOnly dateOnly => dateOnly,
                _ => throw new InvalidCastException($"Cannot convert {value?.GetType().Name} to DateOnly")
            };
        }

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }
    }
}
