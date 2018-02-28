using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ChatLe.Cryptography.Test
{
    public class UtilityTest
    {
        [Fact]
        public async Task ShouldEncryptDecryptFile()
        {
            var sut = new Utility("a test key");

            await sut.EncryptFile(@"..\..\..\UtilityTest.cs");

            using (var stream = await sut.DecryptFile(@"..\..\..\UtilityTest.cs.enc"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var content = await reader.ReadToEndAsync();

                    using (var expectStream = File.OpenRead(@"..\..\..\UtilityTest.cs"))
                    {
                        using (var expectReader = new StreamReader(expectStream))
                        {
                            Assert.Equal(await expectReader.ReadToEndAsync(), content);
                        }
                    }
                }
            }
        }
    }
}
