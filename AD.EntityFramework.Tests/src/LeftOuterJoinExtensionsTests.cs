//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace AD.EntityFramework.Tests
//{
//    [TestClass]
//    public class LeftOuterJoinExtensionsTests
//    {
//        [TestMethod]
//        public void LeftOuterJoinTest()
//        {
//            // Arrange
//            ITradeContext context = MockContextFactory.CreateMockTradeContext();
//            var imports = context.DatawebImports(new string[] { "2015" });
//            var names = context.CountryCodeCrosswalk;

//            // Act
//            var test = from a in imports
//                       from b in names.LeftOuterJoin(x => x.CountryCodeDataweb == a.CountryCode)
//                       select new
//                       {
//                           a.CountryCode,
//                           b.Iso
//                       };

//            // Assert
//            Assert.IsTrue(test.ReduceExpression().ToList().Count == 4);
//        }
//    }
//}