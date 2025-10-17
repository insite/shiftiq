using Xunit.Abstractions;
using Xunit.Sdk;

namespace Shift.Test
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        private class PriorityItem
        {
            public int Priority { get; }
            public ITestCase TestCase { get; }

            public PriorityItem(int priority, ITestCase testCase)
            {
                Priority = priority;
                TestCase = testCase;
            }
        }

        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var result = new List<PriorityItem>();

            foreach (var testCase in testCases)
            {
                var priority = 0;

                foreach (var attr in testCase.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute)))
                    priority = attr.GetNamedArgument<int>("Priority");

                result.Add(new PriorityItem(priority, testCase));
            }

            return result
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.TestCase.TestMethod.Method.Name)
                .Select(x => (TTestCase)x.TestCase);
        }
    }
}
