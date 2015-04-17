using System;

namespace Easy.NET.Tools
{
    public class Retry<T>
    {
        public Func<T> Function { get; private set; }

        public Retry(Func<T> func)
        {
            Function = func;
        }

        public T RetryIfException(int times = 3)
        {
            var res = default(T);
            while (times-- > 0)
            {
                try
                {
                    res = Function.Invoke();
                    return res;
                }
                catch (Exception)
                {
                    if (times <= 0)
                        throw;
                    continue;
                }
            }
            return res;
        }
    }
}