public class Car
    {
        public long Id {get;set;}
        public string Name {get;set;}
    } 

    public static class CarExtensions
    {
        [ExpressionMethod(nameof(FilterBySpecialStringExpression))]
        public static bool FilterBySpecialString(this Car car, bool? isSpecial = false)
        {
            return !isSpecial.HasValue || isSpecial.Value && car.Name == "Special" || !isSpecial.Value && car.Name != "Special";
        }

        public static Expression<Func<Car, bool?, bool>> FilterBySpecialStringExpression()
        {
            return (x, isSpecial) => !isSpecial.HasValue || isSpecial.Value && x.Name == "Special" || !isSpecial.Value && x.Name != "Special";
        }
    }

    public class Linq2DbPlayground
    {
        public void Start()
        {
            var context = new DataConnection(new SQLiteDataProvider(ProviderName.SQLiteClassic), "Data Source=:memory:");
            context.CreateTable<Car>();
            var fluentMappingBuilder = context.MappingSchema.GetFluentMappingBuilder();
            var carBuilder = fluentMappingBuilder.Entity<Car>();
            carBuilder.Property(x => x.Id).IsPrimaryKey();
            var carTable = context.GetTable<Car>();

            var query = carTable.Where(x => x.FilterBySpecialString(null)).ToList();

            var lastQuery = context.LastQuery;
        }
    }
