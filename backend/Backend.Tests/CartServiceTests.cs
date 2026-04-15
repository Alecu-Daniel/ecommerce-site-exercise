using backend.Services;

namespace Backend.Tests
{
    public class CartServiceTests
    {
        [Fact]
        public void CalculateTotal_MultipleItems()
        {
            var service = new CartService();
            var items = new List<(int Quantity, decimal Price)>
            {
                (2, 10.00m),
                (1, 5.00m),
                (3, 20.00m)
            };

            var result = service.CalculateTotal(items);

            Assert.Equal(85.00m, result);
        }

        [Fact]
        public void CalculateTotal_CartIsEmpty()
        {
            var service = new CartService();
            var items = new List<(int Quantity, decimal Price)> { };

            var result = service.CalculateTotal(items);

            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateTotal_DecimalPrecision()
        {
            var service = new CartService();
            var items = new List<(int Quantity, decimal Price)>
            {
                (3, 9.99m)
            };

            var result = service.CalculateTotal(items);

            Assert.Equal(29.97m, result);
        }
    }
}
