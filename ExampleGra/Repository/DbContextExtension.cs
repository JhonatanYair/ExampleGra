using ExampleGra.Datos;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ExampleGra.Repository
{
    public class DbContextExtension
    {

        private readonly ExampleDBContext context;

        public DbContextExtension(ExampleDBContext _context)
        {
            context = _context;
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsyncInclude<T>() where T : class
        {
            var includePaths = GetIncludePaths(typeof(T));
            IQueryable<T> query = context.Set<T>(); // Cambio DbSet a IQueryable

            foreach (var includePath in includePaths)
            {
                query = query.Include(includePath);
            }

            return await query.ToListAsync();
        }

        private List<string> GetIncludePaths(Type type)
        {
            var properties = type.GetProperties();
            var includePaths = new List<string>();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    // Solo incluir la propiedad de navegación si es una colección (ICollection)
                    includePaths.Add(property.Name);
                }
            }

            return includePaths;
        }

        public async Task<T> AddAsync<T>(T entity) where T : class
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync<T>(T entity) where T : class
        {
            context.Entry(entity).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync<T>(int id) where T : class
        {
            var entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
            {
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<T>> GetEntitiesWithInclude<T>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> include = null)
            where T : class
        {
            IQueryable<T> query = context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public static Expression<Func<T, bool>> BuildDynamicFilterExpression<T>(T filterModel)
        {
            var parameter = Expression.Parameter(typeof(T), "p");
            var filterProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var conditions = new List<Expression>();
            foreach (var property in filterProperties)
            {

                //Ignora las propiedades virtuales.
                if (property.GetMethod.IsVirtual == true)
                {
                    continue;
                }

                //Si el campo es un llave primaria o foranea y esta en 0, se ignora.
                var value = property.GetValue(filterModel);
                if (property.Name.ToLower().Contains("id") == true && Equals(value, 0))
                {
                    continue;
                }

                if (value != null && (int)value != -999)
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    var propertyValue = Expression.Constant(value);

                    if (property.PropertyType == typeof(int?))
                    {
                        int? nullableValue = (int?)value;
                        propertyValue = Expression.Constant(nullableValue, typeof(int?));

                    }
                    var equality = Expression.Equal(propertyAccess, propertyValue);
                    conditions.Add(equality);
                }

            }

            if (conditions.Count == 0)
            {
                return null; // No hay condiciones para aplicar
            }

            var combinedConditions = conditions.Aggregate(Expression.And);
            var filterExpression = Expression.Lambda<Func<T, bool>>(combinedConditions, parameter);
            return filterExpression;
        }


    }
}
