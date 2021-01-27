 public class MyProjection
    {
        public Car Car { get; set; }
        public Tyre Tyre { get; set; }
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
            context.CreateTable<Tyre>();
            context.CreateTable<Group>();
            context.CreateTable<CarTyreMapping>();
            context.CreateTable<Brand>();

            var fluentMappingBuilder = context.MappingSchema.GetFluentMappingBuilder();

            var brandBuilder = fluentMappingBuilder.Entity<Brand>();
            brandBuilder.Property(x => x.Id).IsPrimaryKey();
            brandBuilder.Association(x => x.Groups, x => x.Id, x => x.BrandId);

            var groupBuilder = fluentMappingBuilder.Entity<Group>();
            groupBuilder.Property(x => x.Id).IsPrimaryKey();
            groupBuilder.Association(x => x.Cars, x => x.Id, x => x.GroupId);
            groupBuilder.Association(x => x.Brand, x => x.BrandId, x => x.Id);

            var carBuilder = fluentMappingBuilder.Entity<Car>();
            carBuilder.Property(x => x.Id).IsPrimaryKey();
            carBuilder.Association(x => x.Group, x => x.GroupId, x => x.Id);

            var tyreHistoryMappingBuilder = fluentMappingBuilder.Entity<CarTyreMapping>();

            var tyreBuilder = fluentMappingBuilder.Entity<Tyre>();
            tyreBuilder.Property(x => x.Id).IsPrimaryKey();

            context.Insert(new Car { Id = 1, Name = "MyCar", IsSet = true });
            context.Insert(new Car { Id = 2, Name = "Bla" });
            context.Insert(new Group { Id = 1, Name = "group 1", IsSet = false });
            context.Insert(new Tyre { Id = 1, Name = "MyTyre", CarId = 1 });
            context.Insert(new CarTyreMapping { CarId = 1, TyreId = 1, Time = DateTime.UtcNow.AddDays(-2) });

            var carTable = context.GetTable<Car>();
            var tyreTable = context.GetTable<Tyre>();


            var query = carTable.Where(x => x.FilterBySpecialString(null)).ToList();

            var lastQuery = context.LastQuery;





        }
    }
