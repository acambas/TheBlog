using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class AppTaskExtensions
    {
        public static T RunAsyncAsSync<T>(Func<Task<T>> asyncFunc)
        {
            return Task.Run<T>(() => asyncFunc.Invoke().Result).Result;
        }
    }
}
