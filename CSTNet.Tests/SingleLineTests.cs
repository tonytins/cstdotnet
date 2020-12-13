// This project is licensed under the MIT license.

using Xunit;

namespace Sixam.CST.Tests
{
    public class SingleLineTests
    {
        [Theory]
        [InlineData(1, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin ac dictum orci, at tincidunt nulla. Donec aliquet, eros non interdum posuere, ipsum sapien molestie nunc, nec facilisis libero ipsum et risus. In sed lorem vel ipsum placerat viverra.")]
        [InlineData(3, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam venenatis ac odio ut pretium. Interdum et malesuada fames ac ante ipsum primis in faucibus. Donec semper turpis tempor, bibendum sapien at, blandit neque. Vivamus hendrerit imperdiet elit, vel sollicitudin nulla luctus vel. Vivamus nisl quam, feugiat a diam aliquam, iaculis vestibulum nunc. Maecenas euismod leo enim, faucibus ultrices ipsum semper eu. Praesent ullamcorper justo at maximus ultricies.")]
        public void V1Test(int key, string expected)
        {
            var actual = CSTHelper.CSTFile("v1.cst", key.ToString());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Singleline", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ultricies nulla eu tortor mattis, dictum posuere lacus ornare. Maecenas a massa in ligula finibus luctus eu vitae nibh. Proin imperdiet dapibus mauris quis placerat.")]
        public void V2Test(string key, string expected)
        {
            var actual = CSTHelper.CSTFile("v2.cst", key);
            Assert.Equal(expected, actual);
        }
    }
}
