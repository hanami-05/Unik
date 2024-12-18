using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Linq
{
    internal class ParallelCount
    {
        private static int[] nums;
        private static bool IsUnique(int n) 
        {
            int s = 1;
            for (int i = 2; i < (int)Math.Pow(n, 0.5)+1; i++) 
            {
                if (n % i == 0) 
                {
                    if (i != n / i)
                    {
                        s += i;
                        s += n / i;
                    }
                    else 
                    {
                        s += i;
                    }
                }
            }

            return s == n;
        }
        public static IEnumerable<int> GetParallelSum(int n) 
        {
            nums = new int[10];

            for (int i = 0; i < 10; i++) nums[i] = i;

            var query = from num in nums.AsParallel() select num*num;

            return query;
        }
    }
}
