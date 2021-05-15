using NUnit.Framework;
using DataProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor.Tests {
    [TestFixture()]
    public class DataProcessorTests {
        public class MockData {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        List<MockData> source = new List<MockData> {
            new MockData { Id = 1, Name = "Mock A" },
            new MockData { Id = 2, Name = "Test B" }
        };

        [Test()]
        public void TotalCount() {
            var options = new TestLoadOption {
                Filter = new object[] { "Id", "=", 2 },
            };
            Assert.AreEqual(1, DataProcessor.Load(source, options).TotalCount);
        }

        [Test()]
        public void Select() {
            var options = new TestLoadOption {
                Select = new[] { "Id" },
            };

            var count = DataProcessor.Load(source, options).Data.Cast<MockData>().Count(e => e.Name != null);
            Assert.AreEqual(0, count);
        }

        [Test()]
        public void Sort() {
            var options = new TestLoadOption {
                Sort = "Id",
                SortDescending = true
            };

            var firstId = DataProcessor.Load(source, options).Data.Cast<MockData>().First().Id;
            Assert.AreEqual(2, firstId);
        }
    }
}