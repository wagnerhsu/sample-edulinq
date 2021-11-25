#region Copyright and license information
// Copyright 2010-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Edulinq.TestSupport;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Edulinq.Tests
{
    public class AggregateTest
    {
       [Fact]
        public void NullSourceUnseeded()
        {
            int[] source = null;
            Action action = (() => source.Aggregate((x, y) => x + y));
            action.Should().Throw<ArgumentNullException>();

        }

       [Fact]
        public void NullFuncUnseeded()
        {
            int[] source = { 1, 3 };
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(null));
        }

       [Fact]
        public void UnseededAggregation()
        {
            int[] source = { 1, 4, 5 };
            // First iteration: 0 * 2 + 1 = 1
            // Second iteration: 1 * 2 + 4 = 6
            // Third iteration: 6 * 2 + 5 = 17
            source.Aggregate((current, value) => current * 2 + value).Should().Be(17);
        }

       [Fact]
        public void NullSourceSeeded()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(3, (x, y) => x + y));
        }

       [Fact]
        public void NullFuncSeeded()
        {
            int[] source = { 1, 3 };
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, null));
        }

       [Fact]
        public void SeededAggregation()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;
            Func<int, int, int> func = (current, value) => current * 2 + value;
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            source.Aggregate(seed, func).Should().Be(57);
        }

       [Fact]
        public void NullSourceSeededWithResultSelector()
        {
            int[] source = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(3, (x, y) => x + y, result => result.ToInvariantString()));
        }

       [Fact]
        public void NullFuncSeededWithResultSelector()
        {
            int[] source = { 1, 3 };
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, null, result => result.ToInvariantString()));
        }

       [Fact]
        public void NullProjectionSeededWithResultSelector()
        {
            int[] source = { 1, 3 };
            Func<int, string> resultSelector = null;
            Assert.Throws<ArgumentNullException>(() => source.Aggregate(5, (x, y) => x + y, resultSelector));
        }

       [Fact]
        public void SeededAggregationWithResultSelector()
        {
            int[] source = { 1, 4, 5 };
            int seed = 5;
            Func<int, int, int> func = (current, value) => current * 2 + value;
            Func<int, string> resultSelector = result => result.ToInvariantString();
            // First iteration: 5 * 2 + 1 = 11
            // Second iteration: 11 * 2 + 4 = 26
            // Third iteration: 26 * 2 + 5 = 57
            // Result projection: 57.ToInvariantString() = "57"
            source.Aggregate(seed, func, resultSelector).Should().Be("57");
        }

       [Fact]
        public void DifferentSourceAndAccumulatorTypes()
        {
            int largeValue = 2000000000;
            int[] source = { largeValue, largeValue, largeValue };
            long sum = source.Aggregate(0L, (acc, value) => acc + value);
            sum.Should().Be(6000000000L);
            // Just to prove we haven't missed off a zero...
            (sum > int.MaxValue).Should().BeTrue();
        }

       [Fact]
        public void EmptySequenceUnseeded()
        {
            int[] source = { };
            Assert.Throws<InvalidOperationException>(() => source.Aggregate((x, y) => x + y));
        }

       [Fact]
        public void EmptySequenceSeeded()
        {
            int[] source = { };
            source.Aggregate(5, (x, y) => x + y).Should().Be(5);
        }

       [Fact]
        public void EmptySequenceSeededWithResultSelector()
        {
            int[] source = { };
            source.Aggregate(5, (x, y) => x + y, x => x.ToInvariantString()).Should().Be("5");
        }

        // Originally I'd thought it was the default value of TSource which was used as the seed...
       [Fact]
        public void FirstElementOfInputIsUsedAsSeedForUnseededOverload()
        {
            int[] source = { 5, 3, 2 };
            source.Aggregate((acc, value) => acc * value).Should().Be(30);
        }
    }
}
