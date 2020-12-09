// This project is licensed under the MIT license.
using System;
using System.IO;
using Xunit;

namespace CSTNet.Tests
{
    public class BasicTests
    {
        string CSTFile(string cst, string key)
        {
            var path = Path.Combine(AppContext.BaseDirectory, cst);
            var file = File.ReadAllText(path);

            return CaretSeparatedText.Parse(file, key);
        }

        [Theory]
        [InlineData(1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin ac dictum orci, at tincidunt nulla. Donec aliquet, eros non interdum posuere, ipsum sapien molestie nunc, nec facilisis libero ipsum et risus. In sed lorem vel ipsum placerat viverra.")]
        [InlineData(4, @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce justo dui, rhoncus a pulvinar sit amet, fermentum vitae lorem. Maecenas nec nisi sit amet eros rutrum congue. In sagittis suscipit arcu, ac vestibulum nunc feugiat volutpat.

Vivamus consequat velit dui, sit amet rhoncus dui malesuada a. Maecenas hendrerit commodo mi et scelerisque. Cras pharetra ultrices aliquam. Praesent ac efficitur magna, vitae scelerisque metus.")]
        public void V1Test(int key, string expected)
        {
            var actual = CSTFile("v1.cst", key.ToString());
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Singleline", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ultricies nulla eu tortor mattis, dictum posuere lacus ornare. Maecenas a massa in ligula finibus luctus eu vitae nibh. Proin imperdiet dapibus mauris quis placerat.")]
        [InlineData("Multiline", @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc gravida nunc non justo pretium consectetur. Sed tempus libero ac ligula aliquam elementum. Duis vitae interdum leo. Sed semper nulla %1 a lectus dictum dictum.

Quisque vehicula, nisi ut scelerisque sodales, nisi ipsum sodales ipsum, in rutrum tellus lacus sed nibh. Etiam mauris velit, elementum sed placerat et, elementum et tellus. Duis vitae elit fermentum, viverra lorem in, lobortis elit.")]
        public void V2Test(string key, string expected)
        {
            var actual = CSTFile("v2.cst", key);
            Assert.Equal(expected, actual);
        }
    }
}
