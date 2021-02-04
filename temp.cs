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
            var connectionString = "";
            var context = new DataConnection(new OracleDataProvider(ProviderName.OracleManaged), connectionString);
            try
            {
                context.CreateTable<Car>();
            }
            catch{}
            
            var fluentMappingBuilder = context.MappingSchema.GetFluentMappingBuilder();
            var carBuilder = fluentMappingBuilder.Entity<Car>();
            carBuilder.Property(x => x.Id).IsPrimaryKey();
            var carTable = context.GetTable<Car>();

            // Start: This works perfectly
            var values = new bool?[]{ null, false, true };
            var sqlResults = new List<string>();
            foreach(var value in values)
            {
                var query = carTable.Where(x => x.FilterBySpecialString(value)).ToList();
                sqlResults.Add(context.LastQuery);
            }
            // End of first example
            
            // Start: This does not work meaning that there is not difference in the resulting SQL statement
            sqlResults.Clear();
            
            carTable.Where(x => x.FilterBySpecialString(null)).ToList();
            sqlResults.Add(context.LastQuery);
            
            carTable.Where(x => x.FilterBySpecialString(true)).ToList();
            sqlResults.Add(context.LastQuery);
            
            carTable.Where(x => x.FilterBySpecialString(false)).ToList();
            sqlResults.Add(context.LastQuery);
            
            // End of failing code
        }
    }
